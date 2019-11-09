using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Acryl.Helpers
{
    public enum EasingType {
        None,
        Out, In, InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InSine, OutSine, InOutSine,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc, 
        InElastic, OutElastic, OutElasticHalf, OutElasticQuarter, InOutElastic,
        InBack, OutBack, InOutBack,
        InBounce, OutBounce, InOutBounce,
        OutPow10
    }
    
    public delegate void UpdateCallback<T>(T current);
    
    public class EasingRequest<T>
    {
        public T From;
        public T To;
        public T Current;
        
        public double StartTime;
        public double EndTime;
        public EasingType Type;
        
        public UpdateCallback<EasingRequest<T>> Callback;

        public bool Freeze = false;
    }
    
    public static class Easing
    {
        private static readonly List<EasingRequest<Vector2>> EasingPipelineVector
            = new List<EasingRequest<Vector2>>();
        private static readonly List<EasingRequest<double>> EasingPipelineDouble
            = new List<EasingRequest<double>>();
        
        // Always handle our Tweening on our Update Loop
        public static void Update(GameTime gameTime)
        {
            // Vector2 Pipeline
            {
                var toRemove = new List<EasingRequest<Vector2>>();
            
                lock (EasingPipelineVector)
                    foreach (var easing in EasingPipelineVector)
                    {
                        if (easing.Freeze)
                            continue;
                        
                        easing.Current = ValueAt(
                            gameTime.TotalGameTime.TotalMilliseconds,
                            easing.From, easing.To,
                            easing.StartTime, easing.EndTime,
                            easing.Type);

                        if (easing.Current == easing.To)
                            toRemove.Add(easing);
                    
                        easing.Callback(easing);
                    
                        if (easing.Current == easing.To)
                            toRemove.Add(easing);
                    }

                foreach(var easing in toRemove)
                    lock (EasingPipelineVector)
                        EasingPipelineVector.Remove(easing);
            }

            // Double Pipeline
            {
                var toRemove = new List<EasingRequest<double>>();
            
                lock (EasingPipelineDouble)
                    foreach (var easing in EasingPipelineDouble)
                    {
                        if (easing.Freeze)
                            continue;
                        
                        if (easing.Freeze) {
                            easing.StartTime += gameTime.TotalGameTime.TotalMilliseconds - easing.StartTime;
                            easing.EndTime += gameTime.TotalGameTime.TotalMilliseconds - easing.EndTime;
                        }
                        
                        easing.Current = ValueAt(
                            gameTime.TotalGameTime.TotalMilliseconds,
                            easing.From, easing.To,
                            easing.StartTime, easing.EndTime,
                            easing.Type);
                        
                        if (easing.Current == easing.To)
                            toRemove.Add(easing);

                        easing.Callback(easing);
                    }

                foreach(var easing in toRemove)
                    lock (EasingPipelineDouble)
                        EasingPipelineDouble.Remove(easing);
            }
        }

        public static EasingRequest<Vector2> ApplyEasing(UpdateCallback<EasingRequest<Vector2>> callback,
            Vector2 startValue, Vector2 endValue, float duration,
            EasingType type)
        {
            var startTime = AcrylGame.UpdateGameTime?.TotalGameTime.TotalMilliseconds ?? 0;
            var endTime = startTime + duration;
            
            var easing = new EasingRequest<Vector2>
            {
                From = startValue,
                To = endValue,
                Type = type,
                EndTime = endTime,
                StartTime = startTime,
                Callback = callback
            };

            lock (EasingPipelineVector)
                EasingPipelineVector.Add(easing);

            return easing;
        }
        
        public static EasingRequest<double> ApplyEasing(UpdateCallback<EasingRequest<double>> callback,
            double startValue, double endValue, double duration,
            EasingType type)
        {
            var startTime = AcrylGame.UpdateGameTime?.TotalGameTime.TotalMilliseconds ?? 0;
            var endTime = startTime + duration;
            
            var easing = new EasingRequest<double>
            {
                From = startValue,
                To = endValue,
                Type = type,
                StartTime = startTime,
                EndTime = endTime,
                Callback = callback
            };

            lock (EasingPipelineDouble)
                EasingPipelineDouble.Add(easing);

            return easing;
        }
        
        public static double ValueAt(double time,
            double val1, double val2,
            double startTime, double endTime,
            EasingType easing = EasingType.None)
        {
            if (val1 == val2)
                return val1;

            var current = time - startTime;
            var duration = endTime - startTime;

            if (current == 0)
                return val1;
            if (duration == 0)
                return val2;
            
            if (current >= duration)
                return val2;

            var t = ApplyEasing(easing, current / duration);
            return val1 + t * (val2 - val1);
        }
        
        public static Vector2 ValueAt(double time, Vector2 val1, Vector2 val2, double startTime, double endTime, EasingType easing = EasingType.None)
        {
            var current = (float)(time - startTime);
            var duration = (float)(endTime - startTime);

            if (duration == 0 || current == 0)
                return val1;

            if (current >= duration)
                return val2;
            
            var t = (float)ApplyEasing(easing, current / duration);
            return val1 + t * (val2 - val1);
        }
        
        public static double ApplyEasing(EasingType easing, double time)
        {
            const double elastic_const = 2 * Math.PI / .3;
            const double elastic_const2 = .3 / 4;

            const double back_const = 1.70158;
            const double back_const2 = back_const * 1.525;

            const double bounce_const = 1 / 2.75;

            switch (easing)
            {
                default:
                    return time;

                case EasingType.In:
                case EasingType.InQuad:
                    return time * time;

                case EasingType.Out:
                case EasingType.OutQuad:
                    return time * (2 - time);

                case EasingType.InOutQuad:
                    if (time < .5) return time * time * 2;

                    return --time * time * -2 + 1;

                case EasingType.InCubic:
                    return time * time * time;

                case EasingType.OutCubic:
                    return --time * time * time + 1;

                case EasingType.InOutCubic:
                    if (time < .5) return time * time * time * 4;

                    return --time * time * time * 4 + 1;

                case EasingType.InQuart:
                    return time * time * time * time;

                case EasingType.OutQuart:
                    return 1 - --time * time * time * time;

                case EasingType.InOutQuart:
                    if (time < .5) return time * time * time * time * 8;

                    return --time * time * time * time * -8 + 1;

                case EasingType.InQuint:
                    return time * time * time * time * time;

                case EasingType.OutQuint:
                    return --time * time * time * time * time + 1;

                case EasingType.InOutQuint:
                    if (time < .5) return time * time * time * time * time * 16;

                    return --time * time * time * time * time * 16 + 1;

                case EasingType.InSine:
                    return 1 - Math.Cos(time * Math.PI * .5);

                case EasingType.OutSine:
                    return Math.Sin(time * Math.PI * .5);

                case EasingType.InOutSine:
                    return .5 - .5 * Math.Cos(Math.PI * time);

                case EasingType.InExpo:
                    return Math.Pow(2, 10 * (time - 1));

                case EasingType.OutExpo:
                    return -Math.Pow(2, -10 * time) + 1;

                case EasingType.InOutExpo:
                    if (time < .5) return .5 * Math.Pow(2, 20 * time - 10);

                    return 1 - .5 * Math.Pow(2, -20 * time + 10);

                case EasingType.InCirc:
                    return 1 - Math.Sqrt(1 - time * time);

                case EasingType.OutCirc:
                    return Math.Sqrt(1 - --time * time);

                case EasingType.InOutCirc:
                    if ((time *= 2) < 1) return .5 - .5 * Math.Sqrt(1 - time * time);

                    return .5 * Math.Sqrt(1 - (time -= 2) * time) + .5;

                case EasingType.InElastic:
                    return -Math.Pow(2, -10 + 10 * time) * Math.Sin((1 - elastic_const2 - time) * elastic_const);

                case EasingType.OutElastic:
                    return Math.Pow(2, -10 * time) * Math.Sin((time - elastic_const2) * elastic_const) + 1;

                case EasingType.OutElasticHalf:
                    return Math.Pow(2, -10 * time) * Math.Sin((.5 * time - elastic_const2) * elastic_const) + 1;

                case EasingType.OutElasticQuarter:
                    return Math.Pow(2, -10 * time) * Math.Sin((.25 * time - elastic_const2) * elastic_const) + 1;

                case EasingType.InOutElastic:
                    if ((time *= 2) < 1)
                        return -.5 * Math.Pow(2, -10 + 10 * time) * Math.Sin((1 - elastic_const2 * 1.5 - time) * elastic_const / 1.5);

                    return .5 * Math.Pow(2, -10 * --time) * Math.Sin((time - elastic_const2 * 1.5) * elastic_const / 1.5) + 1;

                case EasingType.InBack:
                    return time * time * ((back_const + 1) * time - back_const);

                case EasingType.OutBack:
                    return --time * time * ((back_const + 1) * time + back_const) + 1;

                case EasingType.InOutBack:
                    if ((time *= 2) < 1) return .5 * time * time * ((back_const2 + 1) * time - back_const2);

                    return .5 * ((time -= 2) * time * ((back_const2 + 1) * time + back_const2) + 2);

                case EasingType.InBounce:
                    time = 1 - time;
                    if (time < bounce_const)
                        return 1 - 7.5625 * time * time;
                    if (time < 2 * bounce_const)
                        return 1 - (7.5625 * (time -= 1.5 * bounce_const) * time + .75);
                    if (time < 2.5 * bounce_const)
                        return 1 - (7.5625 * (time -= 2.25 * bounce_const) * time + .9375);

                    return 1 - (7.5625 * (time -= 2.625 * bounce_const) * time + .984375);

                case EasingType.OutBounce:
                    if (time < bounce_const)
                        return 7.5625 * time * time;
                    if (time < 2 * bounce_const)
                        return 7.5625 * (time -= 1.5 * bounce_const) * time + .75;
                    if (time < 2.5 * bounce_const)
                        return 7.5625 * (time -= 2.25 * bounce_const) * time + .9375;

                    return 7.5625 * (time -= 2.625 * bounce_const) * time + .984375;

                case EasingType.InOutBounce:
                    if (time < .5) return .5 - .5 * ApplyEasing(EasingType.OutBounce, 1 - time * 2);

                    return ApplyEasing(EasingType.OutBounce, (time - .5) * 2) * .5 + .5;

                case EasingType.OutPow10:
                    return --time * Math.Pow(time, 10) + 1;
            }
        }
    }
}