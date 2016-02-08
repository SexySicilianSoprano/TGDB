using UnityEngine;
using System;
using System.Collections;

namespace Ceto
{

    /// <summary>
    /// Base class of ocean components like wave spectrum, projected grid etc.
    /// </summary>
	[RequireComponent(typeof(Ocean))]
	public abstract class OceanComponent : MonoBehaviour 
	{

        /// <summary>
        /// The components ocean parent.
        /// </summary>
		protected Ocean m_ocean;

        /// <summary>
        /// true if there was a error.
        /// This will shut component down.
        /// </summary>
        public bool WasError { get; protected set; }

        /// <summary>
        /// On awake find the ocean and store a reference to it.
        /// </summary>
		protected virtual void Awake() 
		{

			try
			{
				m_ocean = GetComponent<Ocean>();
				m_ocean.Register(this);
			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}

		}

		protected virtual void OnEnable()
		{
            //If there was a error prevent from re-enabling. 
			if(WasError || (m_ocean == null || m_ocean.WasError))
				enabled = false;

		}

		protected virtual void OnDisable()
		{

		}

		protected virtual void OnDestroy()
		{

			try
			{
				m_ocean.Deregister(this);
			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}

		}

	}

}



