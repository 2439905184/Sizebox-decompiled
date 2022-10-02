using System;
using UnityEngine;

namespace SA
{
	public abstract class FullBodyIKBehaviourBase : MonoBehaviour
	{
		[NonSerialized]
		private FullBodyIK _cache_fullBodyIK;

		public abstract FullBodyIK fullBodyIK { get; }

		public virtual void Prefix()
		{
			if (_cache_fullBodyIK == null)
			{
				_cache_fullBodyIK = fullBodyIK;
			}
			if (_cache_fullBodyIK != null)
			{
				_cache_fullBodyIK.Prefix(base.transform);
			}
		}

		protected virtual void Awake()
		{
			if (_cache_fullBodyIK == null)
			{
				_cache_fullBodyIK = fullBodyIK;
			}
			if (_cache_fullBodyIK != null)
			{
				_cache_fullBodyIK.Awake(base.transform);
			}
		}

		protected virtual void OnDestroy()
		{
			if (_cache_fullBodyIK == null)
			{
				_cache_fullBodyIK = fullBodyIK;
			}
			if (_cache_fullBodyIK != null)
			{
				_cache_fullBodyIK.Destroy();
			}
		}

		protected virtual void LateUpdate()
		{
			if (_cache_fullBodyIK == null)
			{
				_cache_fullBodyIK = fullBodyIK;
			}
			if (_cache_fullBodyIK != null)
			{
				_cache_fullBodyIK.Update();
			}
		}
	}
}
