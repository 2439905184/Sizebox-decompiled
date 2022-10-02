using UnityEngine;

namespace SteeringBehaviors
{
	public abstract class Kinematic
	{
		public Vector3 position
		{
			get
			{
				return GetPosition();
			}
			set
			{
				SetPosition(value);
			}
		}

		protected abstract Vector3 GetPosition();

		protected virtual void SetPosition(Vector3 pos)
		{
		}

		public static implicit operator bool(Kinematic kinematic)
		{
			if (kinematic != null)
			{
				return kinematic.ToBool();
			}
			return false;
		}

		protected virtual bool ToBool()
		{
			return true;
		}

		public abstract bool TryGetPosition(out Vector3 pos);
	}
}
