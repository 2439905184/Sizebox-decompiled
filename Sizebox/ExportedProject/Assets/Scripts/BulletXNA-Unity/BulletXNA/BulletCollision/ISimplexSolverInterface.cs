using System.Collections.Generic;
using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public interface ISimplexSolverInterface
	{
		void Reset();

		void AddVertex(ref IndexedVector3 w, ref IndexedVector3 p, ref IndexedVector3 q);

		bool Closest(out IndexedVector3 v);

		float MaxVertex();

		bool FullSimplex();

		int GetSimplex(IList<IndexedVector3> pBuf, IList<IndexedVector3> qBuf, IList<IndexedVector3> yBuf);

		bool InSimplex(ref IndexedVector3 w);

		void BackupClosest(ref IndexedVector3 v);

		bool EmptySimplex();

		void ComputePoints(out IndexedVector3 p1, out IndexedVector3 p2);

		int NumVertices();
	}
}
