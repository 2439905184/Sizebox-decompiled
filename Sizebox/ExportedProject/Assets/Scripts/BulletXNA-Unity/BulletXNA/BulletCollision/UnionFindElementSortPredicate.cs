using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class UnionFindElementSortPredicate : IQSComparer<Element>
	{
		public bool Compare(Element lhs, Element rhs)
		{
			return lhs.m_id < rhs.m_id;
		}
	}
}
