using System.Collections.Generic;

namespace BulletXNA.BulletDynamics
{
	public class SortConstraintOnIslandPredicate : IComparer<TypedConstraint>
	{
		public int Compare(TypedConstraint lhs, TypedConstraint rhs)
		{
			int constraintIslandId = DiscreteDynamicsWorld.GetConstraintIslandId(rhs);
			int constraintIslandId2 = DiscreteDynamicsWorld.GetConstraintIslandId(lhs);
			return constraintIslandId2 - constraintIslandId;
		}
	}
}
