using System;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[Serializable]
	public abstract class SubBehaviourBase
	{
		protected BehaviourBase behaviour;

		protected static Vector2 XZ(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		protected static Vector3 XYZ(Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		protected static Vector3 Flatten(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		protected static Vector3 SetY(Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}
	}
}
