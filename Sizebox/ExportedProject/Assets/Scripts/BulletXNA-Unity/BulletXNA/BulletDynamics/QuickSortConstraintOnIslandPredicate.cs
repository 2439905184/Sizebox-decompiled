using BulletXNA.LinearMath;

namespace BulletXNA.BulletDynamics
{
	public class QuickSortConstraintOnIslandPredicate : IQSComparer<TypedConstraint>
	{
		public bool Compare(TypedConstraint lhs, TypedConstraint rhs)
		{
			int constraintIslandId = DiscreteDynamicsWorld.GetConstraintIslandId(rhs);
			int constraintIslandId2 = DiscreteDynamicsWorld.GetConstraintIslandId(lhs);
			return constraintIslandId2 < constraintIslandId;
		}
	}
}
