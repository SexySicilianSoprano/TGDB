using UnityEngine;
using System;
using System.Collections;

namespace Ceto
{

    /// <summary>
    /// Base class for a spectrum buffer.
    /// Spectrum buffers are responsible for transforming
    /// the spectrum using FFT and then managing the data
    /// created. The buffer superclass can implement the 
    /// FFT using what ever method they want and generate 
    /// what ever type of data they want from the spectrum,
    /// is the displacements, slope, etc.
    /// </summary>
	public abstract class WaveSpectrumBuffer
	{

        /// <summary>
        /// Has the buffer finished creating the requested data.
        /// </summary>
		public abstract bool Done { get; }

        /// <summary>
        /// The fourier size of the buffer.
        /// </summary>
		public abstract int Size { get; }

        /// <summary>
        /// Does this buffer run on the GPU.
        /// </summary>
		public abstract bool IsGPU { get; }

        /// <summary>
        /// Get the texture at idx created by this buffer.
        /// </summary>
		public abstract Texture GetTexture(int idx);

		/// <summary>
		/// The time value used to create the data.
		/// </summary>
		public float TimeValue { get; protected set; }

		/// <summary>
		/// False until the first time run is called.
		/// </summary>
		public bool HasRun { get; protected set; }

		/// <summary>
		/// Has the data been sampled.
		/// </summary>
		public bool BeenSampled { get; set; }

        /// <summary>
        /// Initialized the buffer with the spectrum for these conditions at this time.
        /// </summary>
		protected abstract void Initilize(WaveSpectrumCondition condition, float time);

        /// <summary>
        /// Run the buffer with the spectrum for these conditions at this time.
        /// </summary>
		public abstract void Run(WaveSpectrumCondition condition, float time);

        /// <summary>
        /// The buffer may generate multiple sets of data. 
        /// Enable the data in buffer idx.
        /// </summary>
		public abstract void EnableBuffer(int idx);

        /// <summary>
        /// The buffer may generate multiple sets of data. 
        /// Disable the data in buffer idx.
        /// </summary>
		public abstract void DisableBuffer(int idx);

        /// <summary>
        /// How many of the buffers are enabled.
        /// </summary>
		public abstract int EnabledBuffers();

        /// <summary>
        /// Is this buffer enabled.
        /// </summary>
        public abstract bool IsEnabledBuffer(int i);

        /// <summary>
        /// Release any resources held.
        /// </summary>
		public virtual void Release()
		{

		}

        /// <summary>
        /// Call before sampling from any of the buffer textures.
        /// </summary>
		public virtual void EnableSampling()
		{

		}

        /// <summary>
        /// Call after sampling from any of the buffer textures.
        /// </summary>
		public virtual void DisableSampling()
		{

		}

		/// <summary>
		/// Some of the values needed in the InitWaveSpectrum function can be precomputed.
		/// If the grid sizes change this function must called again.
		/// </summary>
		protected Color[] CreateWTable(int size, Vector4 inverseGridSizes)
		{
			
			float fsize = (float)size;
			
			Color[] table = new Color[size*size];
			
			float WAVE_KM_2 = WaveSpectrum.WAVE_KM * WaveSpectrum.WAVE_KM;
			
			Vector2 uv, st;
			float k1, k2, k3, k4, w1, w2, w3, w4;
			
			for (int x = 0; x < size; x++) 
			{
				for (int y = 0; y < size; y++) 
				{
					uv = new Vector2(x,y) / fsize;
					
					st.x = uv.x > 0.5f ? uv.x - 1.0f : uv.x;
					st.y = uv.y > 0.5f ? uv.y - 1.0f : uv.y;
					
					k1 = (st * inverseGridSizes.x).magnitude;
					k2 = (st * inverseGridSizes.y).magnitude;
					k3 = (st * inverseGridSizes.z).magnitude;
					k4 = (st * inverseGridSizes.w).magnitude;
					
					w1 = Mathf.Sqrt(9.81f * k1 * (1.0f + k1 * k1 / WAVE_KM_2));
					w2 = Mathf.Sqrt(9.81f * k2 * (1.0f + k2 * k2 / WAVE_KM_2));
					w3 = Mathf.Sqrt(9.81f * k3 * (1.0f + k3 * k3 / WAVE_KM_2));
					w4 = Mathf.Sqrt(9.81f * k4 * (1.0f + k4 * k4 / WAVE_KM_2));
					
					table[x+y*size].r = w1;
					table[x+y*size].g = w2;
					table[x+y*size].b = w3;
					table[x+y*size].a = w4;
					
				}
			}

			return table;
		}

        /// <summary>
        /// The inverse grids size for this fourier size.
        /// </summary>
		public Vector4 InverseGridSizes(float size)
		{
			
			float factor = 2.0f * Mathf.PI * size;
			return new Vector4(factor / WaveSpectrum.GRID_SIZES.x, 
			                   factor / WaveSpectrum.GRID_SIZES.y, 
			                   factor / WaveSpectrum.GRID_SIZES.z, 
			                   factor / WaveSpectrum.GRID_SIZES.w);
			
		}
		
        /// <summary>
        /// The offset for this fourier size.
        /// </summary>
		public Vector4 Offset(float size)
		{
			return new Vector4(1.0f + 0.5f / size, 1.0f + 0.5f / size, 0, 0);
		}

	}

}
