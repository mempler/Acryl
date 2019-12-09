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

        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        { 
            if (!Visible || !_isLoaded)
                return;

            if (_service.Size > Size || _service.Size < Size )
                _service.Size = Size;
            
            var tex = _service.RenderString(Text, System.Drawing.Color.White, System.Drawing.Color.Transparent);
            
            var (color, pos, rotation, scale, origin) = CalculateFrame(tex.Width, tex.Height);
            
            spriteBatch.Draw(
                tex,
                pos,
                null,
                color,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
        }
    }
}