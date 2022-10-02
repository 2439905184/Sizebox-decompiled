using BulletXNA.LinearMath;

namespace BulletXNA.BulletCollision
{
	public class MinkowskiPenetrationDepthSolver : IConvexPenetrationDepthSolver
	{
		private const int NUM_UNITSPHERE_POINTS = 42;

		private static readonly IndexedVector3[] sPenetrationDirections = new IndexedVector3[62]
		{
			new IndexedVector3(0f, -0f, -1f),
			new IndexedVector3(0.723608f, -0.525725f, -0.447219f),
			new IndexedVector3(-0.276388f, -0.850649f, -0.447219f),
			new IndexedVector3(-0.894426f, -0f, -0.447216f),
			new IndexedVector3(-0.276388f, 0.850649f, -0.44722f),
			new IndexedVector3(0.723608f, 0.525725f, -0.447219f),
			new IndexedVector3(0.276388f, -0.850649f, 0.44722f),
			new IndexedVector3(-0.723608f, -0.525725f, 0.447219f),
			new IndexedVector3(-0.723608f, 0.525725f, 0.447219f),
			new IndexedVector3(0.276388f, 0.850649f, 0.447219f),
			new IndexedVector3(0.894426f, 0f, 0.447216f),
			new IndexedVector3(-0f, 0f, 1f),
			new IndexedVector3(0.425323f, -0.309011f, -0.850654f),
			new IndexedVector3(-0.162456f, -0.499995f, -0.850654f),
			new IndexedVector3(0.262869f, -0.809012f, -0.525738f),
			new IndexedVector3(0.425323f, 0.309011f, -0.850654f),
			new IndexedVector3(0.850648f, -0f, -0.525736f),
			new IndexedVector3(-0.52573f, -0f, -0.850652f),
			new IndexedVector3(-0.68819f, -0.499997f, -0.525736f),
			new IndexedVector3(-0.162456f, 0.499995f, -0.850654f),
			new IndexedVector3(-0.68819f, 0.499997f, -0.525736f),
			new IndexedVector3(0.262869f, 0.809012f, -0.525738f),
			new IndexedVector3(0.951058f, 0.309013f, 0f),
			new IndexedVector3(0.951058f, -0.309013f, 0f),
			new IndexedVector3(0.587786f, -0.809017f, 0f),
			new IndexedVector3(0f, -1f, 0f),
			new IndexedVector3(-0.587786f, -0.809017f, 0f),
			new IndexedVector3(-0.951058f, -0.309013f, -0f),
			new IndexedVector3(-0.951058f, 0.309013f, -0f),
			new IndexedVector3(-0.587786f, 0.809017f, -0f),
			new IndexedVector3(-0f, 1f, -0f),
			new IndexedVector3(0.587786f, 0.809017f, -0f),
			new IndexedVector3(0.68819f, -0.499997f, 0.525736f),
			new IndexedVector3(-0.262869f, -0.809012f, 0.525738f),
			new IndexedVector3(-0.850648f, 0f, 0.525736f),
			new IndexedVector3(-0.262869f, 0.809012f, 0.525738f),
			new IndexedVector3(0.68819f, 0.499997f, 0.525736f),
			new IndexedVector3(0.52573f, 0f, 0.850652f),
			new IndexedVector3(0.162456f, -0.499995f, 0.850654f),
			new IndexedVector3(-0.425323f, -0.309011f, 0.850654f),
			new IndexedVector3(-0.425323f, 0.309011f, 0.850654f),
			new IndexedVector3(0.162456f, 0.499995f, 0.850654f),
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero,
			IndexedVector3.Zero
		};

		public bool CalcPenDepth(ISimplexSolverInterface simplexSolver, ConvexShape convexA, ConvexShape convexB, ref IndexedMatrix transA, ref IndexedMatrix transB, ref IndexedVector3 v, ref IndexedVector3 pa, ref IndexedVector3 pb, IDebugDraw debugDraw)
		{
			bool flag = convexA.IsConvex2d() && convexB.IsConvex2d();
			float num = float.MaxValue;
			IndexedVector3 indexedVector = IndexedVector3.Zero;
			IndexedVector3 indexedVector2 = IndexedVector3.Zero;
			IndexedVector3 indexedVector3 = IndexedVector3.Zero;
			IndexedVector4[] array = new IndexedVector4[62];
			IndexedVector4[] array2 = new IndexedVector4[62];
			IndexedVector3[] array3 = new IndexedVector3[62];
			IndexedVector3[] array4 = new IndexedVector3[62];
			int num2 = 42;
			for (int i = 0; i < num2; i++)
			{
				IndexedVector3 vin = sPenetrationDirections[i];
				IndexedVector3 vin2 = -vin;
				IndexedBasisMatrix.Multiply(ref array3[i], ref vin2, ref transA._basis);
				IndexedBasisMatrix.Multiply(ref array4[i], ref vin, ref transB._basis);
			}
			int numPreferredPenetrationDirections = convexA.GetNumPreferredPenetrationDirections();
			if (numPreferredPenetrationDirections > 0)
			{
				for (int j = 0; j < numPreferredPenetrationDirections; j++)
				{
					IndexedVector3 penetrationVector;
					convexA.GetPreferredPenetrationDirection(j, out penetrationVector);
					IndexedBasisMatrix.Multiply(ref penetrationVector, ref transA._basis, ref penetrationVector);
					sPenetrationDirections[num2] = penetrationVector;
					IndexedVector3 vin3 = -penetrationVector;
					IndexedBasisMatrix.Multiply(ref array3[num2], ref vin3, ref transA._basis);
					IndexedBasisMatrix.Multiply(ref array4[num2], ref penetrationVector, ref transB._basis);
					num2++;
				}
			}
			int numPreferredPenetrationDirections2 = convexB.GetNumPreferredPenetrationDirections();
			if (numPreferredPenetrationDirections2 > 0)
			{
				for (int k = 0; k < numPreferredPenetrationDirections2; k++)
				{
					IndexedVector3 penetrationVector2;
					convexB.GetPreferredPenetrationDirection(k, out penetrationVector2);
					IndexedBasisMatrix.Multiply(ref penetrationVector2, ref transB._basis, ref penetrationVector2);
					sPenetrationDirections[num2] = penetrationVector2;
					IndexedVector3 vin4 = -penetrationVector2;
					IndexedBasisMatrix.Multiply(ref array3[num2], ref vin4, ref transA._basis);
					IndexedBasisMatrix.Multiply(ref array4[num2], ref penetrationVector2, ref transB._basis);
					num2++;
				}
			}
			convexA.BatchedUnitVectorGetSupportingVertexWithoutMargin(array3, array, num2);
			convexB.BatchedUnitVectorGetSupportingVertexWithoutMargin(array4, array2, num2);
			for (int l = 0; l < num2; l++)
			{
				IndexedVector3 a = sPenetrationDirections[l];
				if (flag)
				{
					a.Z = 0f;
				}
				if (a.LengthSquared() > 0.01f)
				{
					IndexedVector3 indexedVector5 = array3[l];
					IndexedVector3 indexedVector6 = array4[l];
					IndexedVector3 vin5 = new IndexedVector3(array[l].X, array[l].Y, array[l].Z);
					IndexedVector3 vin6 = new IndexedVector3(array2[l].X, array2[l].Y, array2[l].Z);
					IndexedVector3 vout;
					IndexedMatrix.Multiply(out vout, ref transA, ref vin5);
					IndexedVector3 vout2;
					IndexedMatrix.Multiply(out vout2, ref transB, ref vin6);
					if (flag)
					{
						vout.Z = 0f;
						vout2.Z = 0f;
					}
					IndexedVector3 output;
					IndexedVector3.Subtract(out output, ref vout2, ref vout);
					float num3 = IndexedVector3.Dot(ref a, ref output);
					if (num3 < num)
					{
						num = num3;
						indexedVector = a;
						indexedVector2 = vout;
						indexedVector3 = vout2;
					}
				}
			}
			indexedVector2 += indexedVector * convexA.GetMarginNonVirtual();
			indexedVector3 -= indexedVector * convexB.GetMarginNonVirtual();
			if (num < 0f)
			{
				return false;
			}
			float num4 = 0.5f;
			num += num4 + (convexA.GetMarginNonVirtual() + convexB.GetMarginNonVirtual());
			GjkPairDetector gjkPairDetector = BulletGlobals.GjkPairDetectorPool.Get();
			gjkPairDetector.Initialize(convexA, convexB, simplexSolver, null);
			float num5 = num;
			IndexedVector3 indexedVector4 = indexedVector * num5;
			ClosestPointInput input = ClosestPointInput.Default();
			IndexedVector3 origin = transA._origin + indexedVector4;
			IndexedMatrix transformA = transA;
			transformA._origin = origin;
			input.m_transformA = transformA;
			input.m_transformB = transB;
			input.m_maximumDistanceSquared = float.MaxValue;
			MinkowskiIntermediateResult minkowskiIntermediateResult = new MinkowskiIntermediateResult();
			gjkPairDetector.SetCachedSeperatingAxis(-indexedVector);
			gjkPairDetector.GetClosestPoints(ref input, minkowskiIntermediateResult, debugDraw, false);
			float num6 = num - minkowskiIntermediateResult.m_depth;
			float num7 = 1f;
			indexedVector *= num7;
			if (minkowskiIntermediateResult.m_hasResult)
			{
				pa = minkowskiIntermediateResult.m_pointInWorld - indexedVector * num6;
				pb = minkowskiIntermediateResult.m_pointInWorld;
				v = indexedVector;
			}
			BulletGlobals.GjkPairDetectorPool.Free(gjkPairDetector);
			return minkowskiIntermediateResult.m_hasResult;
		}
	}
}
