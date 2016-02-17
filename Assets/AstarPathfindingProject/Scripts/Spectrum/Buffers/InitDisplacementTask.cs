using UnityEngine;
using System.Collections;
using System;

using Ceto.Common.Threading.Tasks;

#pragma warning disable 414

namespace Ceto
{

	public class InitDisplacementTask : ThreadedTask
	{

		Color[] m_spectrum01;
		
		Color[] m_spectrum23;

		DisplacementBufferCPU m_buffer;

		float m_time;

		public InitDisplacementTask(DisplacementBufferCPU buffer, WaveSpectrumCondition condition, float time) : base(true)
		{

			m_buffer = buffer;
			m_time = time;

			int size = condition.Size;

			m_spectrum01 = new Color[size*size];
			m_spectrum23 = new Color[size*size];

			System.Array.Copy(condition.SpectrumData01, m_spectrum01, size*size);
			System.Array.Copy(condition.SpectrumData23, m_spectrum23, size*size);

		}

		public void Reset(WaveSpectrumCondition condition, float time)
		{

			base.Reset();

			m_time = time;

			int size = condition.Size;

			System.Array.Copy(condition.SpectrumData01, m_spectrum01, size*size);
			System.Array.Copy(condition.SpectrumData23, m_spectrum23, size*size);

		}
		
		public override IEnumerator Run()
		{
		
			Initilize();

			FinishedRunning();
			return null;
		}

		Vector2 GetSpectrum(float t, float w, float s0x, float s0y, float s0cx, float s0cy)
		{
			float c = Mathf.Cos(w * t);
			float s = Mathf.Sin(w * t);
			return new Vector2((s0x + s0cx) * c - (s0y + s0cy) * s, (s0x - s0cx) * s + (s0y - s0cy) * c);
		}
		
		Vector2 COMPLEX(Vector2 z)
		{
			// returns i times z (complex number)
			return new Vector2(-z.y, z.x); 
		}

		public void Initilize()
		{
			
			Vector2 uv, st, k1, k2, k3, k4;
			Vector2 h1, h2, h3, h4, h12, h34;
			Vector2 n1, n2, n3, n4;
			Vector4 s12, s34, s12c, s34c;
			int i, j;
			Color w;
			float c, s;
			float K1, K2, K3, K4, IK1, IK2, IK3, IK4;

			int size = m_buffer.Size;
			float ifsize = 1.0f / (float)size;
			
			Vector4 inverseGridSizes = m_buffer.InverseGridSizes(size);
			
			Vector4[] data0 = m_buffer.GetReadBuffer(0);
			Vector4[] data1 = m_buffer.GetReadBuffer(1);
			Vector4[] data2 = m_buffer.GetReadBuffer(2);
			
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					uv.x = x * ifsize;
					uv.y = y * ifsize;
					
					st.x = uv.x > 0.5f ? uv.x - 1.0f : uv.x;
					st.y = uv.y > 0.5f ? uv.y - 1.0f : uv.y;
					
					i = x + y * size;
					j = ((size - x) % size) + ((size - y) % size) * size;
					
					s12 = m_spectrum01[i];
					s34 = m_spectrum23[i];

					s12c = m_spectrum01[j];
					s34c = m_spectrum23[j];
					
					k1.x = st.x * inverseGridSizes.x;
					k1.y = st.y * inverseGridSizes.x;
					
					k2.x = st.x * inverseGridSizes.y;
					k2.y = st.y * inverseGridSizes.y;
					
					k3.x = st.x * inverseGridSizes.z;
					k3.y = st.y * inverseGridSizes.z;
					
					k4.x = st.x * inverseGridSizes.w;
					k4.y = st.y * inverseGridSizes.w;
					
					w = m_buffer.WTable[i];
					
					//h1 = GetSpectrum(time, w.r, s12.x, s12.y, s12c.x, s12c.y);
					//h2 = GetSpectrum(time, w.g, s12.z, s12.w, s12c.z, s12c.w);
					//h3 = GetSpectrum(time, w.b, s34.x, s34.y, s34c.x, s34c.y);
					//h4 = GetSpectrum(time, w.a, s34.z, s34.w, s34c.z, s34c.w);
					
					c = Mathf.Cos(w.r * m_time);
					s = Mathf.Sin(w.r * m_time);
					
					h1.x = (s12.x + s12c.x) * c - (s12.y + s12c.y) * s;
					h1.y = (s12.x - s12c.x) * s + (s12.y - s12c.y) * c;
					
					c = Mathf.Cos(w.g * m_time);
					s = Mathf.Sin(w.g * m_time);
					
					h2.x = (s12.z + s12c.z) * c - (s12.w + s12c.w) * s;
					h2.y = (s12.z - s12c.z) * s + (s12.w - s12c.w) * c;
					
					c = Mathf.Cos(w.b * m_time);
					s = Mathf.Sin(w.b * m_time);
					
					h3.x = (s34.x + s34c.x) * c - (s34.y + s34c.y) * s;
					h3.y = (s34.x - s34c.x) * s + (s34.y - s34c.y) * c;
					
					c = Mathf.Cos(w.a * m_time);
					s = Mathf.Sin(w.a * m_time);
					
					h4.x = (s34.z + s34c.z) * c - (s34.w + s34c.w) * s;
					h4.y = (s34.z - s34c.z) * s + (s34.w - s34c.w) * c;
					
					//heights
					//h12 = h1 + COMPLEX(h2);
					//h34 = h3 + COMPLEX(h4);
					
					h12.x = h1.x + -h2.y;
					h12.y = h1.y + h2.x;
					
					h34.x = h3.x + -h4.y;
					h34.y = h3.y + h4.x;
					
					//slopes (normals)
					//n1 = COMPLEX(k1.x * h1) - k1.y * h1;
					//n2 = COMPLEX(k2.x * h2) - k2.y * h2;
					//n3 = COMPLEX(k3.x * h3) - k3.y * h3;
					//n4 = COMPLEX(k4.x * h4) - k4.y * h4;
					
					n1.x = -(k1.x * h1.y) - k1.y * h1.x;
					n1.y = k1.x * h1.x - k1.y * h1.y;
					
					n2.x = -(k2.x * h2.y) - k2.y * h2.x;
					n2.y = k2.x * h2.x - k2.y * h2.y;
					
					n3.x = -(k3.x * h3.y) - k3.y * h3.x;
					n3.y = k3.x * h3.x - k3.y * h3.y;
					
					n4.x = -(k4.x * h4.y) - k4.y * h4.x;
					n4.y = k4.x * h4.x - k4.y * h4.y;
					
					K1 = Mathf.Sqrt(k1.x * k1.x + k1.y * k1.y);
					K2 = Mathf.Sqrt(k2.x * k2.x + k2.y * k2.y);
					K3 = Mathf.Sqrt(k3.x * k3.x + k3.y * k3.y);
					K4 = Mathf.Sqrt(k4.x * k4.x + k4.y * k4.y);
					
					IK1 = K1 == 0.0f ? 0.0f : 1.0f / K1;
					IK2 = K2 == 0.0f ? 0.0f : 1.0f / K2;
					IK3 = K3 == 0.0f ? 0.0f : 1.0f / K3;
					IK4 = K4 == 0.0f ? 0.0f : 1.0f / K4;
					
					if(data0 != null) data0[i] = new Vector4(h12.x, h12.y, h34.x, h34.y);
					if(data1 != null) data1[i] = new Vector4(n1.x * IK1, n1.y * IK1, n2.x * IK2, n2.y * IK2);
					if(data2 != null) data2[i] = new Vector4(n3.x * IK3, n3.y * IK3, n4.x * IK4, n4.y * IK4);
					
				}
			}
			
		}


	}

}
