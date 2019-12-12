using System;
using System.Linq;
using Acryl.Engine.Graphics.Font;
using Acryl.Engine.Stores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpFont;

namespace Acryl.Engine.Graphics.Core
{
    public class Text2D : Drawable
    {
        [DependencyResolved]
        private FontFaceStore FaceStore { get; set; }        
        
        private FontService _service;

        public string Text;
        private bool _isLoaded;
        
        public Face FontFace 
        {
            get => _service.FontFace;
            set => _service.FontFace = value;
        }

        public float Size
        {
            get;
            set;
        }

        [LoadAsync]
        private void Load(Library lib, DependencyContainer container)
        {
            _service = new FontService(lib);
            DependencyInjector.InjectIntoObject(_service, container);
            
            if (FontFace == null)
                FontFace = FaceStore.Get("Resources/Fonts/Exo-Regular");
            
            _isLoaded = true;
        }

        public Text2D(string text)
        {
            Text = text;
        }

        private bool invalidated = true;
        private Texture2D[] cachedTextures;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        { 
            if (!Visible || !_isLoaded)
                return;

            if (_service.Size > Size || _service.Size < Size )
                _service.Size = Size;

            var s = Text.Split("\n");

            if (invalidated)
                cachedTextures = s.Select(t => _service.RenderString(t, System.Drawing.Color.White, System.Drawing.Color.Transparent)).ToArray();
            invalidated = false;
            
            float lastHeight = 0;
            foreach (var t in cachedTextures)
            {
                var (color, destRect, rotation, origin) = CalculateFrame(t.Width, t.Height);

                destRect.Y += (int) MathF.Round(lastHeight, MidpointRounding.AwayFromZero);
                
                spriteBatch.Draw(t,
                    destRect,
                    null,
                    color,
                    rotation,
                    origin,
                    SpriteEffects.None,
                    0);
                
                lastHeight += t.Height * 1.1f;
            }
        }

        public override void Dispose(bool isDisposing)
        {
            _service.Dispose();
            
            base.Dispose(isDisposing);
        }
    }
}