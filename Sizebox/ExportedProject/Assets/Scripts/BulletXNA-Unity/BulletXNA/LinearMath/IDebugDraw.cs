namespace BulletXNA.LinearMath
{
	public interface IDebugDraw
	{
		void DrawLine(IndexedVector3 from, IndexedVector3 to, IndexedVector3 color);

		void DrawLine(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 fromColor);

		void DrawLine(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 fromColor, ref IndexedVector3 toColor);

		void DrawBox(ref IndexedVector3 bbMin, ref IndexedVector3 bbMax, ref IndexedVector3 color);

		void DrawBox(ref IndexedVector3 bbMin, ref IndexedVector3 bbMax, ref IndexedMatrix trans, ref IndexedVector3 color);

		void DrawSphere(IndexedVector3 p, float radius, IndexedVector3 color);

		void DrawSphere(ref IndexedVector3 p, float radius, ref IndexedVector3 color);

		void DrawSphere(float radius, ref IndexedMatrix transform, ref IndexedVector3 color);

		void DrawTriangle(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 n0, ref IndexedVector3 n1, ref IndexedVector3 n2, ref IndexedVector3 color, float alpha);

		void DrawTriangle(ref IndexedVector3 v0, ref IndexedVector3 v1, ref IndexedVector3 v2, ref IndexedVector3 color, float alpha);

		void DrawContactPoint(IndexedVector3 pointOnB, IndexedVector3 normalOnB, float distance, int lifeTime, IndexedVector3 color);

		void DrawContactPoint(ref IndexedVector3 pointOnB, ref IndexedVector3 normalOnB, float distance, int lifeTime, ref IndexedVector3 color);

		void ReportErrorWarning(string warningString);

		void Draw3dText(ref IndexedVector3 location, string textString);

		void SetDebugMode(DebugDrawModes debugMode);

		DebugDrawModes GetDebugMode();

		void DrawAabb(IndexedVector3 from, IndexedVector3 to, IndexedVector3 color);

		void DrawAabb(ref IndexedVector3 from, ref IndexedVector3 to, ref IndexedVector3 color);

		void DrawTransform(ref IndexedMatrix transform, float orthoLen);

		void DrawArc(ref IndexedVector3 center, ref IndexedVector3 normal, ref IndexedVector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, ref IndexedVector3 color, bool drawSect);

		void DrawArc(ref IndexedVector3 center, ref IndexedVector3 normal, ref IndexedVector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, ref IndexedVector3 color, bool drawSect, float stepDegrees);

		void DrawSpherePatch(ref IndexedVector3 center, ref IndexedVector3 up, ref IndexedVector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, ref IndexedVector3 color);

		void DrawSpherePatch(ref IndexedVector3 center, ref IndexedVector3 up, ref IndexedVector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, ref IndexedVector3 color, float stepDegrees);

		void DrawCapsule(float radius, float halfHeight, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color);

		void DrawCylinder(float radius, float halfHeight, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color);

		void DrawCone(float radius, float height, int upAxis, ref IndexedMatrix transform, ref IndexedVector3 color);

		void DrawPlane(ref IndexedVector3 planeNormal, float planeConst, ref IndexedMatrix transform, ref IndexedVector3 color);
	}
}
