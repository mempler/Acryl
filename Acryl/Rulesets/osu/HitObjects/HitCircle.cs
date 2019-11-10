using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Skin;
using Acryl.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Rulesets.osu.Beatmap.HitObjects
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

        private float _circleScale;
        public HitCircle(Vector2 pos, Color col, float scale)
        {
            Alpha = 0f;
            Scale *= scale;
            _circleScale = scale;
            Position = BeatmapManager.MapPosition(pos);

            
            
            _hitCircleSprite = new Sprite(HitCircleTexture);
            _hitCircleSprite.Color = col;
            _hitCircleSprite.Origin = Origin.Center;
            Add(_hitCircleSprite);


            _hitCircleOverlaySprite = new Sprite(HitCircleOverlayTexture);
            _hitCircleOverlaySprite.Origin = Origin.Center;
            Add(_hitCircleOverlaySprite);

            
            _approachCircleSprite = new Sprite(ApproachCircleTexture);
            _approachCircleSprite.Scale *= scale * 1.3f;
            _approachCircleSprite.Origin = Origin.Center;
            Add(_approachCircleSprite);
        }
        
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
        
        private bool HasBeginScaling;
        private bool isFading;

        private const int HideTime = 250; // TODO: This should be based on AR OD

        protected override void Update(GameTime gameTime)
        {
            Freeze = BeatmapManager.ActiveBeatmap.FreezeBeatmap;
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed > (Timing - 250) &&
                BeatmapManager.ActiveBeatmap.CurrentElapsed < (Timing) &&
                !isFading)
            {
                isFading = true;
                FadeTo(1f, 250);
                Visible = true;
            }

            if (Alpha >= 1f)
                isFading = false;
            
            if (isFading)
                return;
            
            if (!Visible)
                return;
            
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed >= HideTime + Timing)
            {
                isFading = true;
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