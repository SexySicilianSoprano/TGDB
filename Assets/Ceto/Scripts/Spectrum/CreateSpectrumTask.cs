using UnityEngine;
using System;
using System.Collections;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

    /// <summary>
    /// 
    /// Generates a wave spectrum using the formula in the follow research paper.
    /// 
    /// WAVES SPECTRUM
    /// using "A unified directional spectrum for long and short wind-driven waves"
    /// T. Elfouhaily, B. Chapron, K. Katsaros, D. Vandemark
    /// Journal of Geophysical Research vol 102, p781-796, 1997
    /// 
    /// </summary>
	public class CreateSpectrumTask : ThreadedTask
	{

		Color[] spectrum01;

		Color[] spectrum23;

		WaveSpectrumCondition m_condition;

		int m_size;

		float m_windSpeed;

		float m_windDir;

		float m_omega;

		Vector4 m_gridSizes;

		System.Random m_rnd;

		readonly float U10;
		readonly float PI_2;
		readonly float SQRT_10;
		readonly float G_SQ_OMEGA_U10;
		readonly float Z_SQ_U10_G;
		readonly float LOG_OMEGA_6;
		readonly float SIGMA;
		readonly float SQ_SIGMA_2;
		readonly float ALPHA_P;
		readonly float LOG_2_4;
		readonly float WAVE_CM = WaveSpectrum.WAVE_CM;
		readonly float WAVE_KM = WaveSpectrum.WAVE_KM;

		public CreateSpectrumTask(WaveSpectrumCondition condition) : base(true)
		{

			m_condition = condition;
			m_size = condition.Size;
			m_windSpeed = condition.WindSpeed;
			m_omega = condition.WaveAge;
			m_windDir = condition.WindDir;
			m_gridSizes = WaveSpectrum.GRID_SIZES;

			spectrum01 = new Color[m_size*m_size];
			spectrum23 = new Color[m_size*m_size];

			m_rnd = new System.Random(0);

			U10 = m_windSpeed;

			PI_2 = Mathf.PI * 2.0f;

			SQRT_10 = Mathf.Sqrt(10);

			G_SQ_OMEGA_U10 = 9.81f * sqr(m_omega / U10);

			Z_SQ_U10_G = 3.7e-5f * sqr(U10) / 9.81f;

			LOG_OMEGA_6 = Mathf.Log(m_omega) * 6f;

			SIGMA = 0.08f * (1.0f + 4.0f / Mathf.Pow(m_omega, 3.0f));

			SQ_SIGMA_2 = sqr(SIGMA) * 2.0f;

			ALPHA_P = 0.006f * Mathf.Sqrt(m_omega);

			LOG_2_4 = Mathf.Log(2.0f) / 4.0f;

		}

        public override void Start()
        {

            base.Start();

            m_condition.CreateTextures();

			m_condition.Done = false;

        }

		public override IEnumerator Run()
		{

			GenerateWavesSpectrum();

            FinishedRunning();

			return null;
		}

		public override void End()
		{

			base.End();

			m_condition.Apply(spectrum01, spectrum23);

			m_condition.Done = true;

		}

		float sqr(float x) { return x*x; }
		
		float omega(float k) { return Mathf.Sqrt(9.81f * k * (1.0f + sqr(k / WaveSpectrum.WAVE_KM))); } // Eq 24

		
		/// <summary>
		/// I know this is a big chunk of ugly math but dont worry to much about what it all means
		/// It recreates a statistically representative model of a wave spectrum in the frequency domain.
		/// </summary>
		float Spectrum(float kx, float ky)
		{

			// phase speed
			float k = Mathf.Sqrt(kx * kx + ky * ky);
			float c = omega(k) / k;
			
			// spectral peak
			float kp = G_SQ_OMEGA_U10; // after Eq 3
			float cp = omega(kp) / kp;
			
			// friction velocity
			float z0 = Z_SQ_U10_G * Mathf.Pow(U10 / cp, 0.9f); // Eq 66
			float u_star = 0.41f * U10 / Mathf.Log(10.0f / z0); // Eq 60
			
			float Lpm = Mathf.Exp(- 5.0f / 4.0f * sqr(kp / k)); // after Eq 3
			float gamma = (m_omega < 1.0f) ? 1.7f : 1.7f + LOG_OMEGA_6; // after Eq 3 // log10 or log?
			float Gamma = Mathf.Exp(-1.0f / SQ_SIGMA_2 * sqr(Mathf.Sqrt(k / kp) - 1.0f));
			float Jp = Mathf.Pow(gamma, Gamma); // Eq 3
			float Fp = Lpm * Jp * Mathf.Exp(-m_omega / SQRT_10 * (Mathf.Sqrt(k / kp) - 1.0f)); // Eq 32
			float alphap = ALPHA_P; // Eq 34
			float Bl = 0.5f * alphap * cp / c * Fp; // Eq 31
			
			float alpham = 0.01f * (u_star < WAVE_CM ? 1.0f + Mathf.Log(u_star / WAVE_CM) : 1.0f + 3.0f * Mathf.Log(u_star / WAVE_CM)); // Eq 44
			float Fm = Mathf.Exp(-0.25f * sqr(k / WAVE_KM - 1.0f)); // Eq 41
			float Bh = 0.5f * alpham * WAVE_CM / c * Fm * Lpm; // Eq 40 (fixed)
			
			float a0 = LOG_2_4; 
			float ap = 4.0f; 
			float am = 0.13f * u_star / WAVE_CM; // Eq 59
			float Delta = (float)Math.Tanh(a0 + ap * Mathf.Pow(c / cp, 2.5f) + am * Mathf.Pow(WAVE_CM / c, 2.5f)); // Eq 57
			
			float phi = Mathf.Atan2(ky, kx);
			
			if (kx < 0.0f) return 0.0f;
			
			Bl *= 2.0f;
			Bh *= 2.0f;
			
			// remove waves perpendicular to wind dir
			float tweak = Mathf.Sqrt(Mathf.Max(kx/Mathf.Sqrt(kx*kx+ky*ky), 0.0f ));
			//tweak = 1.0;
	
			return (Bl + Bh) * (1.0f + Delta * Mathf.Cos(2.0f * phi)) / (PI_2 * sqr(sqr(k))) * tweak; // Eq 677
		}
		
		float GetSpectrumSample(float i, float j, float lengthScale, float kMin)
		{
			float dk = PI_2 / lengthScale;
			float kx = i * dk;
			float ky = j * dk;
			float result = 0.0f;

			float theta = m_windDir * Mathf.PI / 180.0f;
			float ct = Mathf.Cos(theta);
			float st = Mathf.Sin(theta);
			
			float u = kx * ct - ky * st;
			float v = kx * st + ky * ct;

			kx = u;
			ky = v;
		
			if(Math.Abs(kx) >= kMin || Math.Abs(ky) >= kMin)
			{
				float S = Spectrum(kx, ky);
				float h = Mathf.Sqrt(S / 2.0f) * dk;

				if(float.IsNaN(h) || float.IsInfinity(h)) h = 0.0f;

				result = h;
			}
			
			return result;
		}
		
		/// <summary>
		/// Generates the wave spectrum based on the 
		/// settings wind speed, wave amp and wave age.
		/// If these values change this function must be called again.
		/// </summary>
		void GenerateWavesSpectrum()
		{
			
			int size = m_size;
			float fsize = (float)size;

			int idx;
			float i, j;

			Vector4 sample;

			float phi;
			const float sqrt2 = 1.414213562f;

			for (int x = 0; x < size; x++) 
			{
				for (int y = 0; y < size; y++) 
				{
					idx = x+y*size;
					i = (x >= size / 2) ? (float)(x - size) : (float)x;
					j = (y >= size / 2) ? (float)(y - size) : (float)y;

					sample.x = (float)GetSpectrumSample(i, j, m_gridSizes.x, Mathf.PI / m_gridSizes.x);
					sample.y = (float)GetSpectrumSample(i, j, m_gridSizes.y, Mathf.PI * fsize / m_gridSizes.x);
					sample.z = (float)GetSpectrumSample(i, j, m_gridSizes.z, Mathf.PI * fsize / m_gridSizes.y);
					sample.w = (float)GetSpectrumSample(i, j, m_gridSizes.w, Mathf.PI * fsize / m_gridSizes.z);

					phi = (float)m_rnd.NextDouble() * PI_2;
					
					spectrum01[idx].r = sample.x * (Mathf.Cos(phi) * sqrt2);
					spectrum01[idx].g = sample.x * (Mathf.Sin(phi) * sqrt2);

					phi = (float)m_rnd.NextDouble() * PI_2;
					
					spectrum01[idx].b = sample.y * (Mathf.Cos(phi) * sqrt2);
					spectrum01[idx].a = sample.y * (Mathf.Sin(phi) * sqrt2);

					phi = (float)m_rnd.NextDouble() * PI_2;
					
					spectrum23[idx].r = sample.z * (Mathf.Cos(phi) * sqrt2);
					spectrum23[idx].g = sample.z * (Mathf.Sin(phi) * sqrt2);

					phi = (float)m_rnd.NextDouble() * PI_2;
					
					spectrum23[idx].b = sample.w * (Mathf.Cos(phi) * sqrt2);
					spectrum23[idx].a = sample.w * (Mathf.Sin(phi) * sqrt2);

				}
			}

		}


	}

}
