using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acryl.Engine.Audio;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Stores;
using Acryl.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tweening;
using SharpFont;
using Vector2 = System.Numerics.Vector2;

namespace Acryl.Engine
{
    public class GameBase : Game, IChildrenContainer<Drawable>, IDependencyContainer
    {
        protected SpriteBatch SpriteBatch;
        protected DependencyContainer Dependencies { get; } = new DependencyContainer();
        protected AudioEngine AudioEngine { get; private set; }
        protected Discord.Discord Discord { get; set; }
        protected Scene ActiveScene { get; set; } = new Scene(); // Empty Scene
        protected GraphicsDeviceManager GraphicsDeviceManager { get; }
        
        protected PostProcessor PostProcessor { get; set; }
        protected bool PostProcessing { get; set; } = true;
        
        private Tweener sceneTweener = new Tweener();

        public GameBase()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = false,
                PreferMultiSampling = true
            };
            IsFixedTimeStep = false;
            
            IsMouseVisible = true;
        }

        public void SwitchScene(Scene scene, float duration, float delay = 0)
        {
            ActiveScene
                .FadeTo(0, duration, delay)
                .OnEnd(x =>
                {
                    Remove(ActiveScene);
                    
                    scene.Alpha = 0;
                    ActiveScene = scene;

                    ActiveScene
                        .FadeTo(1, duration, 0);
                    
                    Add(ActiveScene);
                })
                .Easing(EasingFunctions.SineInOut);
        }

        protected override void Initialize()
        {
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.Initialize();
        }
        
        protected override async void LoadContent()
        {
            Dependencies.Add(new VirtualField(1280, 720)); // 720p field
            Dependencies.Add(SpriteBatch = new SpriteBatch(GraphicsDevice));
            Dependencies.Add(AudioEngine = new AudioEngine());
            Dependencies.Add(GraphicsDeviceManager);
            Dependencies.Add(GraphicsDevice);
            Dependencies.Add(this);

            Dependencies.Add(new DllResourceStore(
                new []{
                    Assembly.GetAssembly(typeof(GameBase)),
                    Assembly.GetEntryAssembly()
                }
            ));
            Dependencies.Add(new FileResourceStore());
            Dependencies.Add(new OnlineStore());

            Dependencies.Add(new Library());
            
            Dependencies.Add(new FontFaceStore());
            Dependencies.Add(new TextureStore());
            Dependencies.Add(new EffectStore());
            Dependencies.Add(Dependencies);

            Dependencies.Add(PostProcessor = new PostProcessor());
            
            Add(ActiveScene);
            Add(PostProcessor);
            Remove(PostProcessor);
            
            PostProcessor.Size = new Vector2(
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            await AsyncLoadingPipeline.LoadForObject(GetType(), this, Dependencies);
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            sceneTweener.Update((float) gameTime.ElapsedGameTime.TotalSeconds);
            
            
            lock (Children)
                foreach (var child in Children.ToList())
                {
                    child.UpdateFrame(gameTime);
                }
            
            Discord?.RunCallbacks();
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            
            if (PostProcessing)
                PostProcessor.DrawFrame(SpriteBatch, gameTime);
            else
            {
                SpriteBatch.Begin();
            
                lock (Children)
                    foreach (var child in Children.ToList())
                        child.DrawFrame(SpriteBatch, gameTime);
                   
                SpriteBatch.End(); 
            }

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

        public Drawable Parent { get; internal set; }
        public object ResolveDependency(Type t, string hint = "", bool skipRootCheck = false)
            => Dependencies.ResolveDependency(t, hint, skipRootCheck);

        public async void Add(Drawable child)
        {
            if (child is DependencyContainer container)
            {
                container.Parent = this;
            }
            
            await AsyncLoadingPipeline.LoadForObject(child, Dependencies); // Lets load for Drawable first.
            await AsyncLoadingPipeline.LoadForObject(child.GetType(), child, Dependencies);

            lock (Children)
                Children.Add(child);
            
            Dependencies.Add(child);
        }

        public void Remove(Drawable child)
        {
            lock (Children)
                Children.Remove(child);
        }

        protected override void Dispose(bool disposing)
        {
            lock (Children)
                foreach (var child in Children)
                    child?.Dispose(disposing);

            base.Dispose(disposing);
        }

        IDependencyContainer IDependencyContainer.Parent => Dependencies;
    }
}