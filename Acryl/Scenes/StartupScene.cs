using System.Drawing;
using System.Numerics;
using Acryl.Engine;
using Acryl.Engine.Graphics;
using Acryl.Engine.Graphics.Core;
using Acryl.Engine.Stores;
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
            
            WarningSprite.Colour = Color.Yellow;
            WarningSprite.Scale = new Vector2(.1f, .1f);
            WarningSprite.Origin = Origin.Center;
            WarningSprite.PositionOrigin = Origin.Center;
            WarningSprite.PositionOffset = new Vector2(-8f, -80);
            WarningSprite
                .FadeTo(1, 1, 0)
                .Easing(EasingFunctions.SineInOut)
                .OnEnd(t =>
                {
                    WarningSprite
                        .FadeTo(0, 2, 3f)
                        .Easing(EasingFunctions.SineInOut);

                    WarningSprite
                        .MoveTo(new Vector2(-5000, -25), 32, 3f)
                        .Easing(EasingFunctions.SineInOut);
                });;

            WarningHeader.Colour = Color.Yellow;
            WarningHeader.Origin = Origin.Center;
            WarningHeader.PositionOrigin = Origin.Center;
            WarningHeader.PositionOffset += new Vector2(0, -25);
            WarningHeader
                .FadeTo(1, 2, 0)
                .Easing(EasingFunctions.SineInOut)
                .OnEnd(t =>
                {
                    WarningHeader
                        .FadeTo(0, 2, 2.3f)
                        .Easing(EasingFunctions.SineInOut);

                    WarningHeader
                        .MoveTo(new Vector2(-5000, -25), 32, 2.3f)
                        .Easing(EasingFunctions.SineInOut);
                });
            
            WarningBody.Origin = Origin.Center;
            WarningBody.PositionOrigin = Origin.Center;
            WarningBody
                .FadeTo(1, 2, .5f)
                .Easing(EasingFunctions.SineInOut)
                .OnEnd(t =>
                {
                    WarningBody
                        .FadeTo(0, 2, 2)
                        .Easing(EasingFunctions.SineInOut);

                    WarningBody
                        .MoveTo(new Vector2(-5000, 0), 32, 2)
                        .Easing(EasingFunctions.SineInOut);
                });
        }
    }
}