using UnityEngine;

namespace Minipede.Utility
{
    [System.Serializable]
    public struct WaveDatum
    {
        public float Amplitude;
        public float Frequency;
        public float Phase;

        public float Evaluate( float time )
        {
            float radianFrequency = 2 * Mathf.PI * Frequency;
            return Amplitude * Mathf.Sin( time * radianFrequency + Phase );
        }
    }
}
