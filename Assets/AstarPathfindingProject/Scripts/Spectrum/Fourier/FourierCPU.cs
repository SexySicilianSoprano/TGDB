using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ceto
{
	
    public class FourierCPU
    {

		public int size { get { return m_size; } }
		int m_size;
		float m_fsize;
		
		public int passes { get { return m_passes; } }
		int m_passes;

		float[] m_butterflyLookupTable = null;

        public FourierCPU(int size)
        {

			if (!Mathf.IsPowerOfTwo(size))
				throw new ArgumentException("Fourier grid size must be pow2 number");

            m_size = size;
            m_fsize = (float)m_size;
            m_passes = (int)(Mathf.Log(m_fsize) / Mathf.Log(2.0f));
            ComputeButterflyLookupTable();
        }


        int BitReverse(int i)
        {
            int j = i;
            int Sum = 0;
            int W = 1;
            int M = m_size / 2;
            while (M != 0)
            {
                j = ((i & M) > M - 1) ? 1 : 0;
                Sum += j * W;
                W *= 2;
                M /= 2;
            }
            return Sum;
        }

        void ComputeButterflyLookupTable()
        {
            m_butterflyLookupTable = new float[m_size * m_passes * 4];

            for (int i = 0; i < m_passes; i++)
            {
                int nBlocks = (int)Mathf.Pow(2, m_passes - 1 - i);
                int nHInputs = (int)Mathf.Pow(2, i);

                for (int j = 0; j < nBlocks; j++)
                {
                    for (int k = 0; k < nHInputs; k++)
                    {
                        int i1, i2, j1, j2;
                        if (i == 0)
                        {
                            i1 = j * nHInputs * 2 + k;
                            i2 = j * nHInputs * 2 + nHInputs + k;
                            j1 = BitReverse(i1);
                            j2 = BitReverse(i2);
                        }
                        else
                        {
                            i1 = j * nHInputs * 2 + k;
                            i2 = j * nHInputs * 2 + nHInputs + k;
                            j1 = i1;
                            j2 = i2;
                        }

                        float wr = Mathf.Cos(2.0f * Mathf.PI * (float)(k * nBlocks) / m_fsize);
                        float wi = Mathf.Sin(2.0f * Mathf.PI * (float)(k * nBlocks) / m_fsize);

                        int offset1 = 4 * (i1 + i * m_size);
                        m_butterflyLookupTable[offset1 + 0] = j1;
                        m_butterflyLookupTable[offset1 + 1] = j2;
                        m_butterflyLookupTable[offset1 + 2] = wr;
                        m_butterflyLookupTable[offset1 + 3] = wi;

                        int offset2 = 4 * (i2 + i * m_size);
                        m_butterflyLookupTable[offset2 + 0] = j1;
                        m_butterflyLookupTable[offset2 + 1] = j2;
                        m_butterflyLookupTable[offset2 + 2] = -wr;
                        m_butterflyLookupTable[offset2 + 3] = -wi;

                    }
                }
            }
        }

        //Performs two FFTs on two complex numbers packed in a vector4
        Vector4 FFT(Vector2 w, Vector4 input1, Vector4 input2)
        {
            input1.x += w.x * input2.x - w.y * input2.y;
            input1.y += w.y * input2.x + w.x * input2.y;
            input1.z += w.x * input2.z - w.y * input2.w;
            input1.w += w.y * input2.z + w.x * input2.w;

            return input1;
        }

        //Performs one FFT on a complex number
        Vector2 FFT(Vector2 w, Vector2 input1, Vector2 input2)
        {
            input1.x += w.x * input2.x - w.y * input2.y;
            input1.y += w.y * input2.x + w.x * input2.y;

            return input1;
        }

		public int PeformFFT(int startIdx, IList<Vector4[]> data0)
		{
			
			
			int x; int y; int i;
			int idx = 0; int idx1; int bftIdx;
			int X; int Y;
			Vector2 w;
			int ii, xi, yi;
			
			int j = startIdx;
			
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];

				Vector4[] read0 = data0[idx1];

				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (x + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = X + y * m_size;
						yi = Y + y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);

						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
					}
				}
			}
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];

				Vector4[] read0 = data0[idx1];

				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (y + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = x + X * m_size;
						yi = x + Y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);

						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;

						
					}
				}
			}
			
			return idx;
		}

		public int PeformFFT(int startIdx, IList<Vector4[]> data0, IList<Vector4[]> data1)
		{
			
			
			int x; int y; int i;
			int idx = 0; int idx1; int bftIdx;
			int X; int Y;
			Vector2 w;
			int ii, xi, yi;
			
			int j = startIdx;
			
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];

				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];
				
				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (x + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = X + y * m_size;
						yi = Y + y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
						
						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;
						
					}
				}
			}
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];

				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];

				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (y + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = x + X * m_size;
						yi = x + Y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
			
						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;
						
					}
				}
			}
			
			return idx;
		}

		public int PeformFFT(int startIdx, IList<Vector4[]> data0, IList<Vector4[]> data1, IList<Vector4[]> data2)
        {


            int x; int y; int i;
            int idx = 0; int idx1; int bftIdx;
            int X; int Y;
            Vector2 w;
			int ii, xi, yi;

            int j = startIdx;


            for (i = 0; i < m_passes; i++, j++)
            {
                idx = j % 2;
                idx1 = (j + 1) % 2;

				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];
				Vector4[] write2 = data2[idx];

				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];
				Vector4[] read2 = data2[idx1];

                for (x = 0; x < m_size; x++)
                {
                    for (y = 0; y < m_size; y++)
                    {
                        bftIdx = 4 * (x + i * m_size);

                        X = (int)m_butterflyLookupTable[bftIdx + 0];
                        Y = (int)m_butterflyLookupTable[bftIdx + 1];
                        w.x = m_butterflyLookupTable[bftIdx + 2];
                        w.y = m_butterflyLookupTable[bftIdx + 3];

						ii = x + y * m_size;
						xi = X + y * m_size;
						yi = Y + y * m_size;

//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
//						write2[ii] = FFT(w, read2[xi], read2[yi]);

						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;

						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;

						write2[ii].x = read2[xi].x + w.x * read2[yi].x - w.y * read2[yi].y;
						write2[ii].y = read2[xi].y + w.y * read2[yi].x + w.x * read2[yi].y;
						write2[ii].z = read2[xi].z + w.x * read2[yi].z - w.y * read2[yi].w;
						write2[ii].w = read2[xi].w + w.y * read2[yi].z + w.x * read2[yi].w;

                    }
                }
            }

            for (i = 0; i < m_passes; i++, j++)
            {
                idx = j % 2;
                idx1 = (j + 1) % 2;

				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];
				Vector4[] write2 = data2[idx];
				
				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];
				Vector4[] read2 = data2[idx1];

                for (x = 0; x < m_size; x++)
                {
                    for (y = 0; y < m_size; y++)
                    {
                        bftIdx = 4 * (y + i * m_size);

                        X = (int)m_butterflyLookupTable[bftIdx + 0];
                        Y = (int)m_butterflyLookupTable[bftIdx + 1];
                        w.x = m_butterflyLookupTable[bftIdx + 2];
                        w.y = m_butterflyLookupTable[bftIdx + 3];

						ii = x + y * m_size;
						xi = x + X * m_size;
						yi = x + Y * m_size;

//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
//						write2[ii] = FFT(w, read2[xi], read2[yi]);

						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;
						
						write2[ii].x = read2[xi].x + w.x * read2[yi].x - w.y * read2[yi].y;
						write2[ii].y = read2[xi].y + w.y * read2[yi].x + w.x * read2[yi].y;
						write2[ii].z = read2[xi].z + w.x * read2[yi].z - w.y * read2[yi].w;
						write2[ii].w = read2[xi].w + w.y * read2[yi].z + w.x * read2[yi].w;

                    }
                }
            }

            return idx;
        }

		public int PeformFFT(int startIdx, IList<Vector4[]> data0, IList<Vector4[]> data1, IList<Vector4[]> data2, IList<Vector4[]> data3)
		{
			
			
			int x; int y; int i;
			int idx = 0; int idx1; int bftIdx;
			int X; int Y;
			Vector2 w;
			int ii, xi, yi;
			
			int j = startIdx;
			
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];
				Vector4[] write2 = data2[idx];
				Vector4[] write3 = data3[idx];
				
				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];
				Vector4[] read2 = data2[idx1];
				Vector4[] read3 = data3[idx1];
				
				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (x + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = X + y * m_size;
						yi = Y + y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
//						write2[ii] = FFT(w, read2[xi], read2[yi]);
//						write3[ii] = FFT(w, read3[xi], read3[yi]);
						
						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;
						
						write2[ii].x = read2[xi].x + w.x * read2[yi].x - w.y * read2[yi].y;
						write2[ii].y = read2[xi].y + w.y * read2[yi].x + w.x * read2[yi].y;
						write2[ii].z = read2[xi].z + w.x * read2[yi].z - w.y * read2[yi].w;
						write2[ii].w = read2[xi].w + w.y * read2[yi].z + w.x * read2[yi].w;

						write3[ii].x = read3[xi].x + w.x * read3[yi].x - w.y * read3[yi].y;
						write3[ii].y = read3[xi].y + w.y * read3[yi].x + w.x * read3[yi].y;
						write3[ii].z = read3[xi].z + w.x * read3[yi].z - w.y * read3[yi].w;
						write3[ii].w = read3[xi].w + w.y * read3[yi].z + w.x * read3[yi].w;
						
					}
				}
			}
			
			for (i = 0; i < m_passes; i++, j++)
			{
				idx = j % 2;
				idx1 = (j + 1) % 2;
				
				Vector4[] write0 = data0[idx];
				Vector4[] write1 = data1[idx];
				Vector4[] write2 = data2[idx];
				Vector4[] write3 = data3[idx];
				
				Vector4[] read0 = data0[idx1];
				Vector4[] read1 = data1[idx1];
				Vector4[] read2 = data2[idx1];
				Vector4[] read3 = data3[idx1];
				
				for (x = 0; x < m_size; x++)
				{
					for (y = 0; y < m_size; y++)
					{
						bftIdx = 4 * (y + i * m_size);
						
						X = (int)m_butterflyLookupTable[bftIdx + 0];
						Y = (int)m_butterflyLookupTable[bftIdx + 1];
						w.x = m_butterflyLookupTable[bftIdx + 2];
						w.y = m_butterflyLookupTable[bftIdx + 3];
						
						ii = x + y * m_size;
						xi = x + X * m_size;
						yi = x + Y * m_size;
						
//						write0[ii] = FFT(w, read0[xi], read0[yi]);
//						write1[ii] = FFT(w, read1[xi], read1[yi]);
//						write2[ii] = FFT(w, read2[xi], read2[yi]);
//						write3[ii] = FFT(w, read3[xi], read3[yi]);
						
						write0[ii].x = read0[xi].x + w.x * read0[yi].x - w.y * read0[yi].y;
						write0[ii].y = read0[xi].y + w.y * read0[yi].x + w.x * read0[yi].y;
						write0[ii].z = read0[xi].z + w.x * read0[yi].z - w.y * read0[yi].w;
						write0[ii].w = read0[xi].w + w.y * read0[yi].z + w.x * read0[yi].w;
						
						write1[ii].x = read1[xi].x + w.x * read1[yi].x - w.y * read1[yi].y;
						write1[ii].y = read1[xi].y + w.y * read1[yi].x + w.x * read1[yi].y;
						write1[ii].z = read1[xi].z + w.x * read1[yi].z - w.y * read1[yi].w;
						write1[ii].w = read1[xi].w + w.y * read1[yi].z + w.x * read1[yi].w;
						
						write2[ii].x = read2[xi].x + w.x * read2[yi].x - w.y * read2[yi].y;
						write2[ii].y = read2[xi].y + w.y * read2[yi].x + w.x * read2[yi].y;
						write2[ii].z = read2[xi].z + w.x * read2[yi].z - w.y * read2[yi].w;
						write2[ii].w = read2[xi].w + w.y * read2[yi].z + w.x * read2[yi].w;

						write3[ii].x = read3[xi].x + w.x * read3[yi].x - w.y * read3[yi].y;
						write3[ii].y = read3[xi].y + w.y * read3[yi].x + w.x * read3[yi].y;
						write3[ii].z = read3[xi].z + w.x * read3[yi].z - w.y * read3[yi].w;
						write3[ii].w = read3[xi].w + w.y * read3[yi].z + w.x * read3[yi].w;
						
					}
				}
			}
			
			return idx;
		}
	

    }

}

















