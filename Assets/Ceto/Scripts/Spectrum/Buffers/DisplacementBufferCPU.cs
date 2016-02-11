using UnityEngine;
using System;
using System.Collections.Generic;

using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

	public class DisplacementBufferCPU : WaveSpectrumBufferCPU, IDisplacementBuffer
	{

		readonly static int READ = 1;
		readonly static int WRITE = 0;
		readonly static int NUM_BUFFERS = 3;

		IList<InterpolatedArray2f[]> m_displacements;

		public DisplacementBufferCPU(int size, Scheduler scheduler) : base(size, NUM_BUFFERS, scheduler)
		{

			int GRIDS = QueryDisplacements.GRIDS;
			int CHANNELS = QueryDisplacements.CHANNELS;

			m_displacements = new List<InterpolatedArray2f[]>(2);

			m_displacements.Add( new InterpolatedArray2f[GRIDS] );
			m_displacements.Add( new InterpolatedArray2f[GRIDS] );

			for (int i = 0; i < GRIDS; i++)
			{
				m_displacements[0][i] = new InterpolatedArray2f(size, size, CHANNELS, true);
				m_displacements[1][i] = new InterpolatedArray2f(size, size, CHANNELS, true);
			}

		}

		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{

			InterpolatedArray2f[] displacements = GetWriteDisplacements();

			displacements[0].Clear();
			displacements[1].Clear();
			displacements[2].Clear();
			displacements[3].Clear();

			if(m_initTask == null)
				m_initTask = new InitDisplacementTask(this, condition, time);
			else
				(m_initTask as InitDisplacementTask).Reset(condition, time);
			
		}

		public InterpolatedArray2f[] GetWriteDisplacements()
		{
			return m_displacements[WRITE];
		}

		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return m_displacements[READ];
		}

		public override void Run(WaveSpectrumCondition condition, float time)
		{
			SwapDisplacements();
			base.Run(condition, time);
		}

		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] source = GetReadDisplacements();
			QueryDisplacements.CopyAndCreateDisplacements(source, out displacements);
		}

		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] source = GetReadDisplacements();
			QueryDisplacements.CopyDisplacements(source, displacements);
		}

		void SwapDisplacements()
		{

			InterpolatedArray2f[] tmp = m_displacements[0];
			m_displacements[0] = m_displacements[1];
			m_displacements[1] = tmp;

		}

		public override void PackData(int index)
		{

			base.PackData(index);

			int CHANNELS = QueryDisplacements.CHANNELS;

			IList<Color[]> results = GetResults(index);

			int size = Size;
			Color c;

			InterpolatedArray2f[] displacements = GetWriteDisplacements();
			
			for(int i = 0; i < results.Count; i++)
			{
				Color[] result = results[i];

				int INDEX = (index == -1) ? i : index;
		
				for (int j = 0; j < size * size; j++)
				{
					c = result[j];

					int IDX = j*CHANNELS;

					if(INDEX == 0)
					{
						displacements[0].Data[IDX + 1] = c.r;
						displacements[1].Data[IDX + 1] = c.g;
						displacements[2].Data[IDX + 1] = c.b;
						displacements[3].Data[IDX + 1] = c.a;
					}
					else if(INDEX == 1)
					{
						displacements[0].Data[IDX + 0] += c.r;
						displacements[0].Data[IDX + 2] += c.g;
						displacements[1].Data[IDX + 0] += c.b;
						displacements[1].Data[IDX + 2] += c.a;
					}
					else if(INDEX == 2)
					{
						displacements[2].Data[IDX + 0] += c.r;
						displacements[2].Data[IDX + 2] += c.g;
						displacements[3].Data[IDX + 0] += c.b;
						displacements[3].Data[IDX + 2] += c.a;
					}
					else if(INDEX == 3)
					{
						throw new InvalidOperationException("Invalid buffer Index");
					}
				
				}
				
			}
			
		}

		public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{

			InterpolatedArray2f[] displacements = GetReadDisplacements();

			return QueryDisplacements.MaxRange(displacements, choppyness, gridScale);

		}

		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{

			int enabled = EnabledBuffers();

			//If no buffers are enabled there is nothing to sample.
			if(enabled == 0) return;

			InterpolatedArray2f[] displacements = GetReadDisplacements();
			
			QueryDisplacements.QueryWaves(query, enabled, displacements, scaling);
			
		}

	}

}











