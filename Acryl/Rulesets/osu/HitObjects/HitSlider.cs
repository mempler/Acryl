using System;
using System.Collections.Generic;
using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Skin;
using Acryl.osu;
using Acryl.Rulesets.osu.Beatmap.HitObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Acryl.Rulesets.osu.HitObjects
{
    public enum HitSliderType
    {
        Catmull,
        Bezier,
        Linear,
        Perfect,
    }

    public class HitSlider : HitObject, IHasCurve
    {
        private readonly float _scale;
        private readonly double _pixelLength;
        private readonly int _repeats;
        public SliderPath Path { get; }
        public List<Sprite> PathSprites { get; } = new List<Sprite>();
        private HitCircle _sliderBegHc;
        private Sprite _sliderFollower;
        
        public double EndTime => Timing + ((_repeats * Path.Distance) / TimingPoint.Velocity);
        public double Duration => (EndTime - Timing);
        
        public static Texture2D PathTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/slider_inner");

        public static Texture2D FollowerTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/approachcircle");

        public static Effect BorderShader
            => SkinManager.GetEffect("Effects/Slider");
        
        public HitSlider(
            HitSliderType type, List<Vector2> sliderCurvePoints,
            Color col, float scale, double pixelLength, int repeats)
        {
            _scale = scale;
            _pixelLength = pixelLength;
            _repeats = repeats;
            
            Path = new SliderPath((PathType) (int) type, sliderCurvePoints.ToArray())
            {
                ExpectedDistance = pixelLength
            };

            _sliderBegHc = new HitCircle(Path.PositionAt(0), col, scale) {Visible = true, Alpha = 1f};

            _sliderFollower = new Sprite(FollowerTexture) {Visible = true};
        }

        private bool _forceHide;
        protected override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_sliderBegHc.Timing == 0)
                _sliderBegHc.Timing = Timing;
            
            if (_forceHide)
                Visible = false;

            if (!Visible)
                return;

            if (PathSprites.Count < Path.Distance)
            {
                for (var i = 0; i < Path.Distance; i++)
                {
                    var pos = BeatmapManager.MapPosition(Path.PositionAt(i / Path.Distance));

                    var sprite = new Sprite(PathTexture);

                    sprite.Position += pos;
                    var aspectRatio = PathTexture.Width / PathTexture.Height;
                    var aspectHcRatio = HitCircle.HitCircleTexture.Width / HitCircle.HitCircleTexture.Height;
                    
                    sprite.Scale *= new Vector2(_scale * aspectHcRatio / aspectRatio) * .25f;
                    sprite.Origin = Origin.Center;
                    
                    PathSprites.Add(sprite);
                }
            }
            
            foreach (var s in PathSprites)
            {
                s.DrawFrame(spriteBatch, gameTime);
            }
            //_sliderBegHc.DrawFrame(spriteBatch, gameTime);
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed > Timing - 450f) {
                Visible = true;
            }

            if (BeatmapManager.ActiveBeatmap.CurrentElapsed > EndTime)
            {
                //_forceHide = true;
                Visible = false;
            }

            if (_forceHide)
            {
                return;
            }
            
            //_sliderBegHc.UpdateFrame(gameTime);
        }
    }
}