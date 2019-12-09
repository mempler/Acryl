using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acryl.Engine.Audio;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Stores;
using Acryl.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpFont;

namespace Acryl.Engine
{
    public class GameBase : Game, IChildrenContainer<Drawable>
    {
        protected SpriteBatch SpriteBatch;
        protected DependencyContainer Dependencies { get; } = new DependencyContainer();
        protected AudioEngine AudioEngine { get; private set; }

        protected GraphicsDeviceManager GraphicsDeviceManager { get; }
        
        public GameBase()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = false,
                PreferMultiSampling = true
            };
            
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Initialize();
        }
        protected override async void LoadContent()
        {
            Dependencies.Add(SpriteBatch = new SpriteBatch(GraphicsDevice));
            Dependencies.Add(AudioEngine = new AudioEngine());
            Dependencies.Add(GraphicsDeviceManager);
            Dependencies.Add(GraphicsDevice);
            Dependencies.Add(this, "game");

            Dependencies.Add(new DllResourceStore(
                new []{
                    Assembly.GetAssembly(typeof(GameBase)),
                    Assembly.GetEntryAssembly()
                }
            ), "dllResourceStore");
            Dependencies.Add(new FileResourceStore(), "fileResourceStore");

            Dependencies.Add(new VirtualField(1280, 720)); // 720p field
            Dependencies.Add(new Library());
            
            Dependencies.Add(new FontFaceStore());
            Dependencies.Add(new TextureStore());
            Dependencies.Add(Dependencies); // Add root DepContainer
            
            await AsyncLoadingPipeline.LoadForObject(GetType(), this, Dependencies);
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            lock (Children)
                foreach (var child in Children)
                {
                    child.UpdateFrame(gameTime);
                }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            lock (Children)
                foreach (var child in Children)
                {
                    child.DrawFrame(SpriteBatch, gameTime);
                }
            
            SpriteBatch.End();
            
            base.Draw(gameTime);
        }

        private readonly List<Drawable> _children = new List<Drawable>();
        public List<Drawable> Children => _children;
        public Drawable Child
        {
            get
            {
                lock(Children)
                    return Children.Count == 1 ?
                        Children.FirstOrDefault() :
                        null;
            }
            set
            {
                lock(Children)
                    if (Children.Count <= 0)
                        Children.Add(value);
            }
        }

        public Drawable Parent => null;
        
        public async void Add(Drawable child)
        {
            await AsyncLoadingPipeline.LoadForObject(child, Dependencies); // Lets load for Drawable first.
            await AsyncLoadingPipeline.LoadForObject(child.GetType(), child, Dependencies);

            lock (Children)
                Children.Add(child);
        }

        public void Remove(Drawable child)
        {
            lock (Children)
                Children.Remove(child);
        }

        protected override void Dispose(bool disposing)
        {
            lock (Children)
            {
                foreach (var child in Children)
                {
                    child?.Dispose(disposing);
                }
            }
            
            base.Dispose(disposing);
        }
    }
}