using System.Collections.Generic;
using System.IO;
using Acryl.Graphics.Skin;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics.Elements
{
    public class SpriteAnimation : Drawable
    {
        private int currentFrame = 0;
        public List<Sprite> Sprites { get; }
        
        public double DelayPerFrame = 1000f;

        public void Add(Sprite sprite)
        {
            Sprites.Add(sprite);
        }

        public SpriteAnimation()
        {
            Sprites = new List<Sprite>();
        }

        private double elapsed;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            
            var frame = Sprites[currentFrame];
            
            frame.DrawFrame(spriteBatch, gameTime);
            
            if (elapsed > DelayPerFrame) {
                currentFrame++;
            }
            if (Sprites.Count >= currentFrame)
                currentFrame = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var sprite in Sprites)
                sprite.UpdateFrame(gameTime);
        }


        public static SpriteAnimation FromImageArray(string path)
        {
            var anim = new SpriteAnimation();

            var rootFile = Path.GetFileNameWithoutExtension(path);
            var pathF = Path.Join(Path.GetDirectoryName(path), $"Skins/{SkinManager.SkinName}/");
            
            var dirInfo = new DirectoryInfo(pathF);
            if (dirInfo.Exists)
                foreach (var f in dirInfo.GetFiles(rootFile + "_*.*"))
                {
                    if (!f.Exists)
                        continue;
                    
                    using var fileStream = File.OpenRead(f.Name);
                    var tex = Texture2D.FromStream(AcrylGame.Game.GraphicsDevice, fileStream);
                    anim.Sprites.Add(new Sprite(tex));
                }
            
            if (anim.Sprites.Count <= 0)
                anim.Add(new Sprite(SkinManager.GetSkinElement(path)));
            
            return anim;
        }
    }
}