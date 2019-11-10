using System.IO;
using Acryl.Audio;
using Acryl.Extension.Discord;
using Acryl.Graphics;
using Acryl.Graphics.Scenes;
using Acryl.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Acryl
{
    public class AcrylGame : Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public SpriteBatch SpriteBatch { get; private set; }

        public static GameTime UpdateGameTime { get; private set; }
        public static AcrylGame Game { get; private set; }
        public static Scene ActiveScene { get; private set; }
        public static AudioEngine AudioEngine { get; private set; }
        public static Discord Discord { get; private set; }
        public static VirtualField Field { get; private set; }

        public static string AcrylDirectory = Path.Combine(
            //Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FullName,
            ".acryl"
            );
        
        public static SpriteFont DefaultFont { get; set; }
        
        protected override void Dispose(bool disposing)
        {
            AudioEngine?.Dispose();
            Discord?.Dispose();
            base.Dispose(disposing);
        }

        public AcrylGame()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                SynchronizeWithVerticalRetrace = false,
                PreferMultiSampling = true,
            };

            GraphicsDeviceManager.PreparingDeviceSettings += (sender, args) =>
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage =
                    RenderTargetUsage.PlatformContents;

            GraphicsDeviceManager.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = false;

            Field = new VirtualField(1280, 720);
            
            Game = this;
        }
        
        protected override void Initialize()
        {
            if (!Directory.Exists(Path.Combine(AcrylDirectory, "Skins")))
                Directory.CreateDirectory(Path.Combine(AcrylDirectory, "Skins"));
            if (!Directory.Exists(Path.Combine(AcrylDirectory, "BeatMaps")))
                Directory.CreateDirectory(Path.Combine(AcrylDirectory, "BeatMaps"));
            
            GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None, MultiSampleAntiAlias = true };
            GraphicsDevice.BlendState = new BlendState() { AlphaSourceBlend = Blend.SourceAlpha, AlphaDestinationBlend = Blend.InverseSourceColor, ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.InverseSourceAlpha };
            
            
            AudioEngine = new AudioEngine();
            
            Discord = new Discord(641308731367489536, (ulong) CreateFlags.NoRequireDiscord);
            
            //GraphicsDeviceManager.ApplyChanges();

            Window.Title = "Acryl";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            DefaultFont = Content.Load<SpriteFont>("Fonts/Exo-Regular");
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            //ActiveScene = new GamePlayScene();
            ActiveScene = new GamePlayScene();
            ActiveScene.SwitchTo(ActiveScene);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                
            /* TODO: Swap to Fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) &&
                Keyboard.GetState().IsKeyDown(Keys.Enter))
                Exit();
            */

            UpdateGameTime = gameTime;

            // TODO: Add your update logic here
            Easing.Update(gameTime);

            ActiveScene.UpdateFrame(gameTime);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.DarkSlateGray);
            GraphicsDevice.Clear(Color.Transparent);
            
            ActiveScene.DrawFrame(SpriteBatch, gameTime);

            base.Draw(gameTime);
                        
            Discord.RunCallbacks();
        }
    }
}