using UnityEngine;

namespace Minipede.Utility
{
    public static class Tweens
    {
        public enum Function
        {
            Linear,
            ExpoEaseInOut,
            ExpoEaseOut,
            ExpoEaseIn,
            ExpoEaseOutIn,
            CircEaseOut,
            CircEaseIn,
            CircEaseInOut,
            CircEaseOutIn,
            QuadEaseOut,
            QuadEaseIn,
            QuadEaseInOut,
            QuadEaseOutIn,
            SineEaseOut,
            SineEaseIn,
            SineEaseInOut,
            SineEaseOutIn,
            CubicEaseOut,
            CubicEaseIn,
            CubicEaseInOut,
            CubicEaseOutIn,
            QuartEaseIn,
            QuartEaseOut,
            QuartEaseInOut,
            QuartEaseOutIn,
            QuintEaseIn,
            QuintEaseOut,
            QuintEaseInOut,
            QuintEaseOutIn,
            ElasticEaseIn,
            ElasticEaseOut,
            ElasticEaseInOut,
            ElasticEaseOutIn,
            BounceEaseIn,
            BounceEaseOut,
            BounceEaseInOut,
            BounceEaseOutIn,
            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,
            BackEaseOutIn
        };

        public static float Ease( Function function, float t, float duration, float start = 0, float end = 1 )
        {
            switch ( function )
            {
                case Function.ExpoEaseInOut:
                    return ExpoEaseInOut( t, duration, start, end );
                case Function.ExpoEaseOut:
                    return ExpoEaseOut( t, duration, start, end );
                case Function.ExpoEaseIn:
                    return ExpoEaseIn( t, duration, start, end );
                case Function.ExpoEaseOutIn:
                    return ExpoEaseOutIn( t, duration, start, end );
                case Function.CircEaseOut:
                    return CircEaseOut( t, duration, start, end );
                case Function.CircEaseIn:
                    return CircEaseIn( t, duration, start, end );
                case Function.CircEaseInOut:
                    return CircEaseInOut( t, duration, start, end );
                case Function.CircEaseOutIn:
                    return CircEaseOutIn( t, duration, start, end );
                case Function.QuadEaseOut:
                    return QuadEaseOut( t, duration, start, end );
                case Function.QuadEaseIn:
                    return QuadEaseIn( t, duration, start, end );
                case Function.QuadEaseInOut:
                    return QuadEaseInOut( t, duration, start, end );
                case Function.QuadEaseOutIn:
                    return QuadEaseOutIn( t, duration, start, end );
                case Function.SineEaseOut:
                    return SineEaseOut( t, duration, start, end );
                case Function.SineEaseIn:
                    return SineEaseIn( t, duration, start, end );
                case Function.SineEaseInOut:
                    return SineEaseInOut( t, duration, start, end );
                case Function.SineEaseOutIn:
                    return SineEaseOutIn( t, duration, start, end );
                case Function.CubicEaseOut:
                    return CubicEaseOut( t, duration, start, end );
                case Function.CubicEaseIn:
                    return CubicEaseIn( t, duration, start, end );
                case Function.CubicEaseInOut:
                    return CubicEaseInOut( t, duration, start, end );
                case Function.CubicEaseOutIn:
                    return CubicEaseOutIn( t, duration, start, end );
                case Function.QuartEaseIn:
                    return QuartEaseIn( t, duration, start, end );
                case Function.QuartEaseOut:
                    return QuartEaseOut( t, duration, start, end );
                case Function.QuartEaseInOut:
                    return QuartEaseInOut( t, duration, start, end );
                case Function.QuartEaseOutIn:
                    return QuartEaseOutIn( t, duration, start, end );
                case Function.QuintEaseIn:
                    return QuintEaseIn( t, duration, start, end );
                case Function.QuintEaseOut:
                    return QuintEaseOut( t, duration, start, end );
                case Function.QuintEaseInOut:
                    return QuintEaseInOut( t, duration, start, end );
                case Function.QuintEaseOutIn:
                    return QuintEaseOutIn( t, duration, start, end );
                case Function.ElasticEaseIn:
                    return ElasticEaseIn( t, duration, start, end );
                case Function.ElasticEaseOut:
                    return ElasticEaseOut( t, duration, start, end );
                case Function.ElasticEaseInOut:
                    return ElasticEaseInOut( t, duration, start, end );
                case Function.ElasticEaseOutIn:
                    return ElasticEaseOutIn( t, duration, start, end );
                case Function.BounceEaseIn:
                    return BounceEaseIn( t, duration, start, end );
                case Function.BounceEaseOut:
                    return BounceEaseOut( t, duration, start, end );
                case Function.BounceEaseInOut:
                    return BounceEaseInOut( t, duration, start, end );
                case Function.BounceEaseOutIn:
                    return BounceEaseOutIn( t, duration, start, end );
                case Function.BackEaseIn:
                    return BackEaseIn( t, duration, start, end );
                case Function.BackEaseOut:
                    return BackEaseOut( t, duration, start, end );
                case Function.BackEaseInOut:
                    return BackEaseInOut( t, duration, start, end );
                case Function.BackEaseOutIn:
                    return BackEaseOutIn( t, duration, start, end );
                case Function.Linear:
                    return Linear( t, duration, start, end );
                default:
                    Debug.LogError( $"<b>{(int)function}</b> is an invalid function. Falling back to linear function." );
                    return Linear( t, duration, start, end );
            }
        }
        /// <summary>
        /// Easing equation function for a simple linear tweening, with no easing.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float Linear( float t, float duration, float start = 0, float end = 1 )
        {
            return end * t / duration + start;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ExpoEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return (t == duration) ? start + end : end * (-Mathf.Pow( 2, -10 * t / duration ) + 1) + start;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ExpoEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return (t == 0) ? start : end * Mathf.Pow( 2, 10 * (t / duration - 1) ) + start;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ExpoEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t == 0 )
                return start;

            if ( t == duration )
                return start + end;

            if ( (t /= duration / 2) < 1 )
                return end / 2 * Mathf.Pow( 2, 10 * (t - 1) ) + start;

            return end / 2 * (-Mathf.Pow( 2, -10 * --t ) + 2) + start;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ExpoEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return ExpoEaseOut( t * 2, duration , start, end / 2 );

            return ExpoEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CircEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return end * Mathf.Sqrt( 1 - (t = t / duration - 1) * t ) + start;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CircEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return -end * (Mathf.Sqrt( 1 - (t /= duration) * t ) - 1) + start;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CircEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return -end / 2 * (Mathf.Sqrt( 1 - t * t ) - 1) + start;

            return end / 2 * (Mathf.Sqrt( 1 - (t -= 2) * t ) + 1) + start;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CircEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return CircEaseOut( t * 2, duration, start, end / 2 );

            return CircEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuadEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return -end * (t /= duration) * (t - 2) + start;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuadEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end * (t /= duration) * t + start;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuadEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return end / 2 * t * t + start;

            return -end / 2 * ((--t) * (t - 2) - 1) + start;
        }

        /// <summary>
        /// Easing equation function for a quadratic (t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuadEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return QuadEaseOut( t * 2, duration, start, end / 2 );

            return QuadEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float SineEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return end * Mathf.Sin( t / duration * (Mathf.PI / 2) ) + start;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float SineEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return -end * Mathf.Cos( t / duration * (Mathf.PI / 2) ) + end + start;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float SineEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return end / 2 * (Mathf.Sin( Mathf.PI * t / 2 )) + start;

            return -end / 2 * (Mathf.Cos( Mathf.PI * --t / 2 ) - 2) + start;
        }

        /// <summary>
        /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float SineEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return SineEaseOut( t * 2, duration, start, end / 2 );

            return SineEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CubicEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return end * ((t = t / duration - 1) * t * t + 1) + start;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CubicEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end * (t /= duration) * t * t + start;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CubicEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return end / 2 * t * t * t + start;

            return end / 2 * ((t -= 2) * t * t + 2) + start;
        }

        /// <summary>
        /// Easing equation function for a cubic (t^3) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float CubicEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return CubicEaseOut( t * 2, duration, start, end / 2 );

            return CubicEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuartEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return -end * ((t = t / duration - 1) * t * t * t - 1) + start;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuartEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end * (t /= duration) * t * t * t + start;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuartEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return end / 2 * t * t * t * t + start;

            return -end / 2 * ((t -= 2) * t * t * t - 2) + start;
        }

        /// <summary>
        /// Easing equation function for a quartic (t^4) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuartEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return QuartEaseOut( t * 2, duration, start, end / 2 );

            return QuartEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuintEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return end * ((t = t / duration - 1) * t * t * t * t + 1) + start;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuintEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end * (t /= duration) * t * t * t * t + start;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuintEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) < 1 )
                return end / 2 * t * t * t * t * t + start;
            return end / 2 * ((t -= 2) * t * t * t * t + 2) + start;
        }

        /// <summary>
        /// Easing equation function for a quintic (t^5) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float QuintEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return QuintEaseOut( t * 2, duration, start, end / 2 );
            return QuintEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration) == 1 )
                return start + end;

            float p = duration * .3f;
            float s = p / 4;

            return (end * Mathf.Pow( 2, -10 * t ) * Mathf.Sin( (t * duration - s) * (2 * Mathf.PI) / p ) + end + start);
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration) == 1 )
                return start + end;

            float p = duration * .3f;
            float s = p / 4;

            return -(end * Mathf.Pow( 2, 10 * (t -= 1) ) * Mathf.Sin( (t * duration - s) * (2 * Mathf.PI) / p )) + start;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration / 2) == 2 )
                return start + end;

            float p = duration * (.3f * 1.5f);
            float s = p / 4;

            if ( t < 1 )
                return -.5f * (end * Mathf.Pow( 2, 10 * (t -= 1) ) * Mathf.Sin( (t * duration - s) * (2 * Mathf.PI) / p )) + start;
            return end * Mathf.Pow( 2, -10 * (t -= 1) ) * Mathf.Sin( (t * duration - s) * (2 * Mathf.PI) / p ) * .5f + end + start;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return ElasticEaseOut( t * 2, duration, start, end / 2 );
            return ElasticEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BounceEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( (t /= duration) < (1f / 2.75f) )
                return end * (7.5625f * t * t) + start;
            else if ( t < (2f / 2.75f) )
                return end * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + start;
            else if ( t < (2.5f / 2.75f) )
                return end * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + start;
            else
                return end * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + start;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BounceEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end - BounceEaseOut( duration - t, duration, 0, end ) + start;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BounceEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return BounceEaseIn( t * 2, duration, 0, end ) * .5f + start;
            else
                return BounceEaseOut( t * 2 - duration, duration, 0, end ) * .5f + end * .5f + start;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BounceEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return BounceEaseOut( t * 2, duration, start, end / 2 );
            return BounceEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BackEaseOut( float t, float duration, float start = 0, float end = 1 )
        {
            return end * ((t = t / duration - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1) + start;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BackEaseIn( float t, float duration, float start = 0, float end = 1 )
        {
            return end * (t /= duration) * t * ((1.70158f + 1) * t - 1.70158f) + start;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BackEaseInOut( float t, float duration, float start = 0, float end = 1 )
        {
            float s = 1.70158f;
            if ( (t /= duration / 2) < 1 )
                return end / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + start;
            return end / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + start;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Final value.</param>
        /// <param name="duration">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float BackEaseOutIn( float t, float duration, float start = 0, float end = 1 )
        {
            if ( t < duration / 2 )
                return BackEaseOut( t * 2, duration, start, end / 2 );
            return BackEaseIn( (t * 2) - duration, duration, start + end / 2, end / 2 );
        }

    }
}