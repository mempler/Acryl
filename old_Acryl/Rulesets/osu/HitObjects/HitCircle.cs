using System;
using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Skin;
using Acryl.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu.HitObjects
{
    public sealed class HitCircle : HitObject
    {
        private readonly Sprite _hitCircleSprite;
        private readonly Sprite _hitCircleOverlaySprite;
        private readonly Sprite _approachCircleSprite;
        
        public static Texture2D HitCircleTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/hitcircle");
        public static Texture2D HitCircleOverlayTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/hitcircleoverlay");
        public static Texture2D ApproachCircleTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/approachcircle");

        private double _circleScale;
        public HitCircle(Vector2 pos, Color col, double scale)
        {
            Alpha = 0f;
            Scale *= (float) Math.Round(scale, 4);
            _circleScale = scale;
            Position = BeatmapManager.MapPosition(pos);
            
            _hitCircleSprite = new Sprite(HitCircleTexture);
            _hitCircleSprite.Color = col;
            _hitCircleSprite.Origin = Origin.Center;
            Add(_hitCircleSprite);


            _hitCircleOverlaySprite = new Sprite(HitCircleOverlayTexture) {Origin = Origin.Center};
            Add(_hitCircleOverlaySprite);

            
            _approachCircleSprite = new Sprite(ApproachCircleTexture);
            _approachCircleSprite.Scale *= (float) Math.Round(scale, 4) * 1.3f;
            _approachCircleSprite.Origin = Origin.Center;
            Add(_approachCircleSprite);
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
        
        private bool HasBeginScaling;
        private bool _isFading;

        private const int HideTime = 250; // TODO: This should be based on AR OD

        protected override void Update(GameTime gameTime)
        {
            var elapsed = BeatmapManager.ActiveBeatmap.CurrentElapsed;
            
            Freeze = BeatmapManager.ActiveBeatmap.FreezeBeatmap;
            if (elapsed > (Timing - 250) &&
                elapsed < (Timing) &&
                !_isFading)
            {
                _isFading = true;
                FadeTo(1f, 250);
                Visible = true;
            }

            if (Alpha >= 1f)
                _isFading = false;
            
            if (_isFading)
                return;
            
            if (!Visible)
                return;
            
            if (elapsed >= HideTime + Timing)
            {
                _isFading = true;
                Alpha = 1f;
                FadeTo(0, 100);
                ScaleTo(Vector2.One, 200);
            }

            if (HasBeginScaling) return;
            var beginScale = _approachCircleSprite.Scale;

            _approachCircleSprite.Scale = beginScale * 2.5f;
            _approachCircleSprite.ScaleTo(beginScale, HideTime, EasingType.In);
            HasBeginScaling = true;
        }
    }
}