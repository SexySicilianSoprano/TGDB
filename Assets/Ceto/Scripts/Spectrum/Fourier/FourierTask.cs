using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

	public class FourierTask : ThreadedTask
	{

		FourierCPU m_fourier;

		WaveSpectrumBufferCPU m_buffer;

		int m_index;

		public FourierTask(WaveSpectrumBufferCPU buffer, FourierCPU fourier, int index) : base(true)
		{

			m_buffer = buffer;

			m_fourier = fourier;

			m_index = index;

		}

		public override void Start()
		{
			base.Start();
		}
		
		public override IEnumerator Run()
		{

			PerformFourier();

			FinishedRunning();
			return null;
		}
		
		public override void End()
		{

			base.End();

			m_buffer.PackData(m_index);

		}

		void PerformFourier()
		{

			IList<IList<Vector4[]>> data = m_buffer.GetData(m_index);
			IList<Color[]> results = m_buffer.GetResults(m_index);

			if(data.Count != results.Count)
				throw new InvalidOperationException("data and result count is not the same, " + data.Count + "/" + results.Count);

			//Always start writing at buffer index 0 and the end read buffer should always end up at index 1.
			const int write = 0;
			int read = -1;

			if(data.Count == 1)
			{
				read = m_fourier.PeformFFT(write, data[0]);
			}
			else if(data.Count == 2)
			{
				read = m_fourier.PeformFFT(write, data[0], data[1]);
			}
			else if(data.Count == 3)
			{
				read = m_fourier.PeformFFT(write, data[0], data[1], data[2]);
			}
			else if(data.Count == 4)
			{
				read = m_fourier.PeformFFT(write, data[0], data[1], data[2], data[3]);
			}

			if(read != 1)
				throw new InvalidOperationException("Fourier transform did not result in the read buffer at index 1");

			int size = m_buffer.Size;

			for(int i = 0; i < results.Count; i++)
			{

				Vector4[] datum = data[i][read];
				Color[] result = results[i];

				for (int j = 0; j < size * size; j++)
				{
					result[j] = datum[j];
				}

			}


		}

	}

}














