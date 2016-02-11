using UnityEngine;
using System;
using System.Collections;

namespace Ceto
{

	public class WaveSpectrumConditionKey : IEquatable<WaveSpectrumConditionKey>
    {

        public int Size { get; private set; }

        public float WindSpeed { get; private set; }

        public float WindDir { get; private set; }

        public float WaveAge { get; private set; }

        public WaveSpectrumConditionKey(int size, float windSpeed, float windDir, float waveAge)
        {

            Size = size;
            WindSpeed = windSpeed;
            WaveAge = waveAge;
            WindDir = windDir;

        }

        /// <summary>
        /// Are these keys equal.
        /// </summary>
        public static bool operator ==(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
        {

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(k1, k2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)k1 == null) || ((object)k2 == null))
            {
                return false;
            }

            if (k1.Size != k2.Size) return false;
            if (k1.WindSpeed != k2.WindSpeed) return false;
            if (k1.WindDir != k2.WindDir) return false;
            if (k1.WaveAge != k2.WaveAge) return false;

            return true;
        }

        /// <summary>
        /// Are these keys not equal.
        /// </summary>
        public static bool operator !=(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
        {
            return !(k1 == k2);
        }

        /// <summary>
        /// Is the key equal to another key.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is WaveSpectrumConditionKey)) return false;

            WaveSpectrumConditionKey k = (WaveSpectrumConditionKey)o;
            return k == this;
        }

        /// <summary>
        /// Is the key equal to another key.
        /// </summary>
        public bool Equals(WaveSpectrumConditionKey k)
        {
            return k == this;
        }

        /// <summary>
        /// The keys hash code.
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 23;

            hashcode = (hashcode * 37) + Size.GetHashCode();
            hashcode = (hashcode * 37) + WindSpeed.GetHashCode();
            hashcode = (hashcode * 37) + WindDir.GetHashCode();
            hashcode = (hashcode * 37) + WaveAge.GetHashCode();

            return hashcode;
        }
    }

}






