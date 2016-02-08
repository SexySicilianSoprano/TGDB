using UnityEngine;
using System;
using System.Collections.Generic;

using Ceto.Common.Containers.Interpolation;

namespace Ceto
{
	public static class QueryDisplacements
	{

		public readonly static int CHANNELS = 3;
		public readonly static int GRIDS = 4;

		public static void QueryWaves(WaveQuery query, int enabled, IList<InterpolatedArray2f> displacements, QueryGridScaling scaling)
		{

			if(displacements.Count != GRIDS)
				throw new InvalidOperationException("Query Displacements requires a displacement buffer for each of the " + GRIDS + " grids.");

			if(displacements[0].Channels != CHANNELS)
				throw new InvalidOperationException("Query Displacements requires displacement buffers have " + CHANNELS + " channels.");

            //Only these modes are relevant to this code.
            if (query.mode != QUERY_MODE.DISPLACEMENT && query.mode != QUERY_MODE.POSITION) return;

			float x = query.posX + scaling.offset.x;
            float z = query.posZ + scaling.offset.z;
			
			if(enabled  == 0)
			{
				return;
			}
			else if(enabled  == 1 || query.mode == QUERY_MODE.DISPLACEMENT)
			{
				float displacement;
				Sample(out displacement, displacements, query.sampleSpectrum, x, z, scaling);
				
				query.result.height = displacement;
				query.result.displacementX = 0.0f;
				query.result.displacementZ = 0.0f;
				query.result.iterations = 0;
				query.result.error = 0.0f;
			}
			else if(enabled  == 3)
			{
				
				float dx = x;
				float dz = z;
				float u = x;
				float v = z;
				Vector4 displacement;
				
				float minError2 = query.minError;
				if(minError2 < 0.1f) minError2 = 0.1f;
				minError2 = minError2*minError2;
				
				float error;
				int i = 0;
				
				do
				{
					u += x - dx;
					v += z - dz;
					
					Sample(out displacement, displacements, query.sampleSpectrum, u, v, scaling);
					
					dx = u + displacement.x;
					dz = v + displacement.z;
					
					i++;
					
					float lx = x-dx;
					float lz = z-dz;
					
					error = lx*lx + lz*lz;
				}
				while (error > minError2 && i <= WaveQuery.MAX_ITERATIONS);
				
				query.result.height = displacement.y;
				query.result.displacementX = displacement.x;
				query.result.displacementZ = displacement.z;
				query.result.iterations = i;
				query.result.error = error;
				
			}
			else
			{
				throw new InvalidOperationException("Invalid number of displacement buffers enabled, " + enabled);
			}
			
		}

		static void Sample(out float d, IList<InterpolatedArray2f> displacements, bool[] sample, float x, float z, QueryGridScaling scaling)
		{
			
			d = 0.0f;
			
			float u, v;
			
			float[] result = new float[CHANNELS];
			
			if(sample[0])
			{
				
				u = x / (scaling.gridSizes.x * scaling.gridScale.x);
				v = z / (scaling.gridSizes.x * scaling.gridScale.x);
				
				displacements[0].Get(u, v, result);
				d += result[1] * scaling.gridScale.y;
			}
			
			if(sample[1])
			{
				u = x / (scaling.gridSizes.y * scaling.gridScale.x);
				v = z / (scaling.gridSizes.y * scaling.gridScale.x);
				
				displacements[1].Get(u, v, result);
				d += result[1] * scaling.gridScale.y;
			}
			
			if(sample[2])
			{
				u = x / (scaling.gridSizes.z * scaling.gridScale.x);
				v = z / (scaling.gridSizes.z * scaling.gridScale.x);
				
				displacements[2].Get(u, v, result);
				d += result[1] * scaling.gridScale.y;
			}
			
			if(sample[3])
			{
				u = x / (scaling.gridSizes.w * scaling.gridScale.x);
				v = z / (scaling.gridSizes.w * scaling.gridScale.x);
				
				displacements[3].Get(u, v, result);
				d += result[1] * scaling.gridScale.y;
			}
			
		}
		
		static void Sample(out Vector4 d, IList<InterpolatedArray2f> displacements, bool[] sample, float x, float z, QueryGridScaling scaling)
		{
			
			d.x = 0.0f;
			d.y = 0.0f;
			d.z = 0.0f;
			d.w = 0.0f;
			
			float u, v;

			float[] result = new float[CHANNELS];

			if(sample[0])
			{
				
				u = x / (scaling.gridSizes.x * scaling.gridScale.x);
				v = z / (scaling.gridSizes.x * scaling.gridScale.x);
				
				displacements[0].Get(u, v, result);
				
				d.x += result[0] * scaling.choppyness.x * scaling.gridScale.x;
				d.y += result[1] * scaling.gridScale.y;
				d.z += result[2] * scaling.choppyness.x * scaling.gridScale.x;
			}
			
			if(sample[1])
			{
				u = x / (scaling.gridSizes.y * scaling.gridScale.x);
				v = z / (scaling.gridSizes.y * scaling.gridScale.x);
				
				displacements[1].Get(u, v, result);
				d.x += result[0] * scaling.choppyness.y * scaling.gridScale.x;
				d.y += result[1] * scaling.gridScale.y;
				d.z += result[2] * scaling.choppyness.y * scaling.gridScale.x;
			}
			
			if(sample[2])
			{
				u = x / (scaling.gridSizes.z * scaling.gridScale.x);
				v = z / (scaling.gridSizes.z * scaling.gridScale.x);
				
				displacements[2].Get(u, v, result);
				d.x += result[0] * scaling.choppyness.z * scaling.gridScale.x;
				d.y += result[1] * scaling.gridScale.y;
				d.z += result[2] * scaling.choppyness.z * scaling.gridScale.x;
			}
			
			if(sample[3])
			{
				u = x / (scaling.gridSizes.w * scaling.gridScale.x);
				v = z / (scaling.gridSizes.w * scaling.gridScale.x);
				
				displacements[3].Get(u, v, result);
				d.x += result[0] * scaling.choppyness.w * scaling.gridScale.x;
				d.y += result[1] * scaling.gridScale.y;
				d.z += result[2] * scaling.choppyness.w * scaling.gridScale.x;
			}
			
		}
		
		public static Vector4 MaxRange(IList<InterpolatedArray2f> displacements, Vector4 choppyness, Vector2 gridScale)
		{

			if(displacements.Count != GRIDS)
				throw new InvalidOperationException("Query Displacements requires a displacement buffer for each of the " + GRIDS + " grids.");
			
			if(displacements[0].Channels != CHANNELS)
				throw new InvalidOperationException("Query Displacements requires displacement buffers have " + CHANNELS + " channels.");

			int size = displacements[0].SX;

			Vector3 ninf = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
			Vector3 pinf = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

			Vector3[] max = new Vector3[] { ninf, ninf, ninf, ninf };
			Vector3[] min = new Vector3[] { pinf, pinf, pinf, pinf };

			float[] h = new float[CHANNELS];

			int grids = GRIDS;
			//grids = 3;

			for(int i = 0; i < grids; i++)
			{

				float[] data = displacements[i].Data;

				for(int x = 0; x < size; x++)
				{
					for(int y = 0; y < size; y++)
					{
						int idx = (x+y*size)*CHANNELS;

						h[0] = data[idx + 0];
						h[1] = data[idx + 1];
						h[2] = data[idx + 2];
						
						if(h[0] < min[i].x) min[i].x = h[0];
						if(h[0] > max[i].x) max[i].x = h[0];

						if(h[1] < min[i].y) min[i].y = h[1];
						if(h[1] > max[i].y) max[i].y = h[1];

						if(h[2] < min[i].z) min[i].z = h[2];
						if(h[2] > max[i].z) max[i].z = h[2];

					}
				}
			}

			Vector4 result = Vector4.zero;

			for(int i = 0; i < grids; i++)
			{
				result.x += Mathf.Max(max[i].x, Mathf.Abs(min[i].x)) * choppyness[i];
				result.y += Mathf.Max(max[i].y, Mathf.Abs(min[i].y));
				result.z += Mathf.Max(max[i].z, Mathf.Abs(min[i].z)) * choppyness[i];
			}

			result.x *= gridScale.x;
			result.y *= gridScale.y;
			result.z *= gridScale.x;
			
			return result;
			
		}


		public static void CopyAndCreateDisplacements(IList<InterpolatedArray2f> source, out IList<InterpolatedArray2f> des)
		{

			if(source.Count != GRIDS)
				throw new InvalidOperationException("Query Displacements requires a displacement buffer for each of the " + GRIDS + " grids.");
			
			if(source[0].Channels != CHANNELS)
				throw new InvalidOperationException("Query Displacements requires displacement buffers have " + CHANNELS + " channels.");

			int size = source[0].SX;

			des = new InterpolatedArray2f[GRIDS];

			des[0] = new InterpolatedArray2f(source[0].Data, size, size, CHANNELS, true);
			des[1] = new InterpolatedArray2f(source[1].Data, size, size, CHANNELS, true);
			des[2] = new InterpolatedArray2f(source[2].Data, size, size, CHANNELS, true);
			des[3] = new InterpolatedArray2f(source[3].Data, size, size, CHANNELS, true);
			
		}

		public static void CopyDisplacements(IList<InterpolatedArray2f> source, IList<InterpolatedArray2f> des)
		{
			
			if(source.Count != GRIDS)
				throw new InvalidOperationException("Query Displacements requires a displacement buffer for each of the " + GRIDS + " grids.");
			
			if(source[0].Channels != CHANNELS)
				throw new InvalidOperationException("Query Displacements requires displacement buffers have " + CHANNELS + " channels.");

			des[0].Copy(source[0].Data);
			des[1].Copy(source[1].Data);
			des[2].Copy(source[2].Data);
			des[3].Copy(source[3].Data);
			
		}

	}
}
























