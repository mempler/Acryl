using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Stores;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tweening;

namespace Acryl.Scenes
{
    public class StartupScene : Scene
    {
        private Sprite WarningSprite;
        private Text2D WarningHeader;
        private Text2D WarningBody;
        
        [LoadAsync]
        private void Load(TextureStore store)
        {
            WarningSprite = new Sprite(store.Get("Resources/Textures/exclamation-triangle-solid.svg"));
            WarningSprite.Alpha = 0;

            WarningHeader = new Text2D("Warning!") {Size = 24, Alpha = 0};

            WarningBody = new Text2D("This Game is in a Very early alpha stage! which means " +
                                     "a lot of bugs are expected!\n" +
                                     "Please consider reporting them all! no matter how small they are!");
            WarningBody.Size = 16;
            WarningBody.Alpha = 0;
            
            Add(WarningSprite);
            Add(WarningHeader);
            Add(WarningBody);
            
            WarningSprite.Color = Color.Yellow;
            WarningSprite.Scale = new Vector2(.2f, .2f);
            WarningSprite.Origin = Origin.Center;
            WarningSprite.PositionOrigin = Origin.Center;
            WarningSprite.PositionOffset = new Vector2(-10, -110);
            WarningSprite
                .FadeTo(1, 1, 0)
                .Easing(EasingFunctions.SineInOut);

            WarningHeader.Color = Color.Yellow;
            WarningHeader.Origin = Origin.Center;
            WarningHeader.PositionOrigin = Origin.Center;
            WarningHeader.PositionOffset += new Vector2(0, -25);
            WarningHeader
                .FadeTo(1, 2, 0)
                .Easing(EasingFunctions.SineInOut);
            
            WarningBody.Origin = Origin.Center;
            WarningBody.PositionOrigin = Origin.Center;
            WarningBody
                .FadeTo(1, 2, .5f)
                .Easing(EasingFunctions.SineInOut);
        }
    }
}