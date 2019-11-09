using System.Collections.Generic;
using Acryl.Graphics;
using Acryl.Graphics.Elements;
using Acryl.Graphics.Skin;
using Acryl.osu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Acryl.Rulesets.osu.Beatmap.HitObjects
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
        
        public double EndTime => Timing + (_repeats) * Path.Distance / TimingPoint.Velocity;
        public double Duration => EndTime - Timing;
        
        public static Texture2D PathTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/slider_outer");

        public static Texture2D FollowerTexture =>
            SkinManager.GetSkinElement("Rulesets/osu/HitObject/approachcircle");
        
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

            _sliderBegHc = new HitCircle(Path.PositionAt(0), col, scale);
            _sliderBegHc.Visible = true;
            _sliderBegHc.Alpha = 1f;
            
            _sliderFollower = new Sprite(FollowerTexture);
            _sliderFollower.Visible = true;
        }

        private bool _forceHide;
        private double _i;
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
                _i += .8f;
                if (_i % 1.7f >= 1f)
                {
                    var pos = BeatmapManager.MapPosition(Path.PositionAt(_i / Path.Distance));

                    var sprite = new Sprite(PathTexture);

                    sprite.Position += pos;
                    var aspectRatio = PathTexture.Width / PathTexture.Height;
                    var aspectHcRatio = HitCircle.HitCircleTexture.Width / HitCircle.HitCircleTexture.Height;
                    
                    sprite.Scale *= new Vector2(_scale * aspectHcRatio / aspectRatio) * .25f;
                    sprite.Origin = Origin.Center;
                    
                    PathSprites.Add(sprite);
                }

            }

            /* // Old slider code.
            if (PathSprite == null) {

                var bmp = new Bitmap(1280, 720);
                
                using (var gfx = System.Drawing.Graphics.FromImage(bmp))
                using (var pth = new GraphicsPath(FillMode.Winding))
                {
                    gfx.InterpolationMode = InterpolationMode.Bicubic;
                    gfx.Clear(System.Drawing.Color.Transparent);

                    for (var i = 0; i < Path.Distance; i++)
                    {
                        var (x, y) = BeatmapManager.MapPosition(Path.PositionAt(i / Path.Distance));

                        pth.AddEllipse(x, y,
                            HitCircle.HitCircleTexture.Width * _scale,
                            HitCircle.HitCircleTexture.Height * _scale);
                    }
                  
                    gfx.FillPath(Brushes.White, pth);
                    //gfx.DrawPath(Pens.Black, pth);
                }


                PathSprite =
                    new Sprite(Texture2DExtension.TexFromBitmap(AcrylGame.Game.GraphicsDevice, bmp))
                    {
                        Visible = true, Parent = this,
                        Alpha = 1f
                    };
                
                //PathSprite.FadeTo(1, 100f, EasingType.In);
            }
            */

            /*
            for (var i = 0; i < 100; i++)
            {
                var pos = new Line(Path.PositionAt(i / 100f), Path.PositionAt((i+1) / 100f));

                spriteBatch.DrawLine(pos, Color.White);
            }
            */

            foreach (var s in PathSprites)
            {
                s.DrawFrame(spriteBatch, gameTime);
            }
            _sliderBegHc.DrawFrame(spriteBatch, gameTime);
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed > EndTime - 250f &&
                BeatmapManager.ActiveBeatmap.CurrentElapsed < EndTime)
                Visible = true;
            
            if (BeatmapManager.ActiveBeatmap.CurrentElapsed > EndTime)
                _forceHide = true;
            
            if (_forceHide)
            {
                return;
            }
            
            _sliderBegHc.UpdateFrame(gameTime);
        }
    }
}