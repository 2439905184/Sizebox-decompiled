using BulletXNA.LinearMath;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	public class MMDCollider
	{
		public IndexedMatrix transform = IndexedMatrix.Identity;

		public int shape = -1;

		public Vector3 size = Vector3.zero;

		public bool isKinematic;

		public bool isCollision;

		public MMDColliderCircles circles;

		public void Prepare()
		{
			isCollision = false;
			if (!isKinematic && circles == null)
			{
				circles = new MMDColliderCircles(shape, size);
			}
		}
	}
}
