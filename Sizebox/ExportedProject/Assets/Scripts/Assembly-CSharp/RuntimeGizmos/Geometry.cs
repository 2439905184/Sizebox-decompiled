using UnityEngine;

namespace RuntimeGizmos
{
	public static class Geometry
	{
		public static float LinePlaneDistance(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
		{
			float num = Vector3.Dot(planePoint - linePoint, planeNormal);
			float num2 = Vector3.Dot(lineVec, planeNormal);
			if (num2 != 0f)
			{
				return num / num2;
			}
			return 0f;
		}

		public static Vector3 LinePlaneIntersect(Vector3 linePoint, Vector3 lineVec, Vector3 planePoint, Vector3 planeNormal)
		{
			float num = LinePlaneDistance(linePoint, lineVec, planePoint, planeNormal);
			if (num != 0f)
			{
				return linePoint + lineVec * num;
			}
			return Vector3.zero;
		}

		public static IntersectPoints ClosestPointsOnTwoLines(Vector3 point1, Vector3 point1Direction, Vector3 point2, Vector3 point2Direction)
		{
			IntersectPoints result = default(IntersectPoints);
			float num = Vector3.Dot(point1Direction, point1Direction);
			float num2 = Vector3.Dot(point1Direction, point2Direction);
			float num3 = Vector3.Dot(point2Direction, point2Direction);
			float num4 = num * num3 - num2 * num2;
			if (num4 != 0f)
			{
				Vector3 rhs = point1 - point2;
				float num5 = Vector3.Dot(point1Direction, rhs);
				float num6 = Vector3.Dot(point2Direction, rhs);
				float num7 = (num2 * num6 - num5 * num3) / num4;
				float num8 = (num * num6 - num5 * num2) / num4;
				result.first = point1 + point1Direction * num7;
				result.second = point2 + point2Direction * num8;
			}
			else
			{
				result.first = point1;
				result.second = point2 + Vector3.Project(point1 - point2, point2Direction);
			}
			return result;
		}

		public static IntersectPoints ClosestPointsOnSegmentToLine(Vector3 segment0, Vector3 segment1, Vector3 linePoint, Vector3 lineDirection)
		{
			IntersectPoints result = ClosestPointsOnTwoLines(segment0, segment1 - segment0, linePoint, lineDirection);
			result.first = ClampToSegment(result.first, segment0, segment1);
			return result;
		}

		public static Vector3 ClampToSegment(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
		{
			Vector3 otherDirection = linePoint2 - linePoint1;
			if (!ExtVector3.IsInDirection(point - linePoint1, otherDirection))
			{
				point = linePoint1;
			}
			else if (ExtVector3.IsInDirection(point - linePoint2, otherDirection))
			{
				point = linePoint2;
			}
			return point;
		}
	}
}
