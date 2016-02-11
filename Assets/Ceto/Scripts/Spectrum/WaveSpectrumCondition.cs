using UnityEngine;
using System.Collections;

namespace Ceto
{

	public class WaveSpectrumCondition  
	{


		public bool Done { get; set; }

        public int Size { get { return Key.Size; } }

		public float WindSpeed { get { return Key.WindSpeed; } }

		public float WindDir { get { return Key.WindDir; } }

		public float WaveAge { get { return Key.WaveAge; } }

        public WaveSpectrumConditionKey Key { get; private set; }

		public Texture2D Spectrum01 { get; private set; }

		public Color[] SpectrumData01 { get; private set; }

		public Texture2D Spectrum23 { get; private set; }

		public Color[] SpectrumData23 { get; private set; }

        public WaveSpectrumCondition(int size, float windSpeed, float windDir, float waveAge)
		{

			Key = new WaveSpectrumConditionKey(size, windSpeed, windDir, waveAge);

			SpectrumData01 = new Color[size*size];
			SpectrumData23 = new Color[size*size];

		}

        public void Release()
        {
            Object.Destroy(Spectrum01);
            Spectrum01 = null;

            Object.Destroy(Spectrum23);
            Spectrum23 = null;
        }

		public void CreateTextures()
		{

            if (Spectrum01 == null)
            {
				Spectrum01 = new Texture2D(Size, Size, TextureFormat.RGBAFloat, false, true);
                Spectrum01.filterMode = FilterMode.Point;
                Spectrum01.wrapMode = TextureWrapMode.Repeat;
				Spectrum01.hideFlags = HideFlags.HideAndDontSave;
                Spectrum01.name = Ocean.InstanceName + " Spectrum01 Condition";
            }

            if (Spectrum23 == null)
            {
				Spectrum23 = new Texture2D(Size, Size, TextureFormat.RGBAFloat, false, true);
                Spectrum23.filterMode = FilterMode.Point;
                Spectrum23.wrapMode = TextureWrapMode.Repeat;
				Spectrum23.hideFlags = HideFlags.HideAndDontSave;
                Spectrum23.name = Ocean.InstanceName + " Spectrum23 Condition";
            }

		}

        public void Apply(Color[] spectrum01, Color[] spectrum23)
        {

            if (Spectrum01 == null || Spectrum23 == null) return;

			Spectrum01.SetPixels(spectrum01);
			Spectrum01.Apply();
	
			Spectrum23.SetPixels(spectrum23);
			Spectrum23.Apply();

			System.Array.Copy(spectrum01, SpectrumData01, spectrum01.Length);
			System.Array.Copy(spectrum23, SpectrumData23, spectrum23.Length);
		}

	}

}












