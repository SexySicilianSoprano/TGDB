using UnityEngine;
using System.Collections;

namespace Ceto
{

	public class OverlayBatchKey
	{


		public Texture Tex { get; private set; }

		public Texture Mask { get; private set; }

		public OverlayBatchKey(Texture tex, Texture mask)
		{
			Tex = tex;
			Mask = mask;
		}

		/// <summary>
		/// Are these keys equal.
		/// </summary>
		public static bool operator ==(OverlayBatchKey k1, OverlayBatchKey k2)
		{
			
			// If both are null, or both are same instance, return true.
			if (Object.ReferenceEquals(k1, k2))
			{
				return true;
			}
			
			// If one is null, but not both, return false.
			if (((object)k1 == null) || ((object)k2 == null))
			{
				return false;
			}

			if(!Object.ReferenceEquals(k1.Tex, k2.Tex)) return false;

			if(!Object.ReferenceEquals(k1.Mask, k2.Mask)) return false;
			
			return true;
		}
		
		/// <summary>
		/// Are these keys not equal.
		/// </summary>
		public static bool operator !=(OverlayBatchKey k1, OverlayBatchKey k2)
		{
			return !(k1 == k2);
		}
		
		/// <summary>
		/// Is the key equal to another key.
		/// </summary>
		public override bool Equals(object o)
		{
			if (!(o is OverlayBatchKey)) return false;
			
			OverlayBatchKey k = (OverlayBatchKey)o;
			return k == this;
		}
		
		/// <summary>
		/// Is the key equal to another key.
		/// </summary>
		public bool Equals(OverlayBatchKey k)
		{
			return k == this;
		}
		
		/// <summary>
		/// The keys hash code.
		/// </summary>
		public override int GetHashCode()
		{
			int hashcode = 23;

			if(Tex != null)
				hashcode = (hashcode * 37) + Tex.GetHashCode();

			if(Mask != null)
				hashcode = (hashcode * 37) + Mask.GetHashCode();
			
			return hashcode;
		}
	}

}






