using System;
using UnityEngine;

namespace UltimateFracturing
{
	[Serializable]
	public class SupportPlane
	{
		public bool GUIExpanded;

		public string GUIName;

		public bool GUIShowInScene;

		public Vector3 v3PlanePosition;

		public Quaternion qPlaneRotation;

		public Vector3 v3PlaneScale;

		public Mesh planeMesh;

		public FracturedObject fracturedObject;

		public SupportPlane(FracturedObject fracturedObject)
		{
			GUIExpanded = true;
			GUIName = "New Support Plane";
			GUIShowInScene = true;
			this.fracturedObject = fracturedObject;
			planeMesh = new Mesh();
			Vector3[] array = new Vector3[4];
			Vector3[] array2 = new Vector3[4];
			Vector2[] uv = new Vector2[4];
			int[] triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
			array[0] = new Vector3(-1f, 0f, -1f);
			array[1] = new Vector3(-1f, 0f, 1f);
			array[2] = new Vector3(1f, 0f, 1f);
			array[3] = new Vector3(1f, 0f, -1f);
			array2[0] = new Vector3(0f, 1f, 0f);
			array2[1] = new Vector3(0f, 1f, 0f);
			array2[2] = new Vector3(0f, 1f, 0f);
			array2[3] = new Vector3(0f, 1f, 0f);
			bool flag = false;
			float num = 1f;
			if ((bool)fracturedObject.SourceObject && (bool)fracturedObject.SourceObject.GetComponent<Renderer>())
			{
				flag = true;
			}
			if (flag)
			{
				Bounds bounds = fracturedObject.SourceObject.GetComponent<Renderer>().bounds;
				num = bounds.extents.y;
				for (int i = 0; i < array.Length; i++)
				{
					float num2 = 1.3f;
					float num3 = Mathf.Max(bounds.extents.z * num2, bounds.extents.x * num2);
					array[i] = Vector3.Scale(array[i], new Vector3(num3, num3, num3)) + fracturedObject.transform.position;
					array[i] = fracturedObject.transform.InverseTransformPoint(array[i]);
				}
				v3PlanePosition = fracturedObject.transform.position - new Vector3(0f, num - 0.05f, 0f);
				v3PlanePosition = fracturedObject.transform.InverseTransformPoint(v3PlanePosition);
				qPlaneRotation = Quaternion.identity;
			}
			else
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[j] += fracturedObject.transform.position;
					array[j] = fracturedObject.transform.InverseTransformPoint(array[j]);
				}
				v3PlanePosition = new Vector3(0f, (0f - num) * 0.5f + 0.05f, 0f);
				qPlaneRotation = Quaternion.identity;
			}
			v3PlaneScale = Vector3.one;
			planeMesh.vertices = array;
			planeMesh.normals = array2;
			planeMesh.uv = uv;
			planeMesh.triangles = triangles;
		}

		public Matrix4x4 GetLocalMatrix()
		{
			return Matrix4x4.TRS(v3PlanePosition, qPlaneRotation, v3PlaneScale);
		}

		public Vector3[] GetBoundingBoxSegments(Bounds bounds)
		{
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			Vector3[] array = new Vector3[8]
			{
				new Vector3(min.x, min.y, min.z),
				new Vector3(min.x, min.y, max.z),
				new Vector3(max.x, min.y, max.z),
				new Vector3(max.x, min.y, min.z),
				new Vector3(min.x, max.y, min.z),
				new Vector3(min.x, max.y, max.z),
				new Vector3(max.x, max.y, max.z),
				new Vector3(max.x, max.y, min.z)
			};
			Vector3[] array2 = new Vector3[24];
			for (int i = 0; i < 4; i++)
			{
				array2[i * 2] = array[i % 4];
				array2[i * 2 + 1] = array[(i + 1) % 4];
			}
			for (int j = 4; j < 8; j++)
			{
				array2[j * 2] = array[j % 4 + 4];
				array2[j * 2 + 1] = array[(j + 1) % 4 + 4];
			}
			for (int k = 8; k < 12; k++)
			{
				array2[k * 2] = array[k % 4];
				array2[k * 2 + 1] = array[k % 4 + 4];
			}
			return array2;
		}

		public bool IntersectsWith(GameObject otherGameObject, bool bBelowIsAlsoValid = false)
		{
			MeshFilter component = otherGameObject.GetComponent<MeshFilter>();
			if (planeMesh == null || component == null)
			{
				return false;
			}
			Vector3[] vertices = planeMesh.vertices;
			Matrix4x4 matrix4x = fracturedObject.transform.localToWorldMatrix * GetLocalMatrix();
			for (int i = 0; i < 4; i++)
			{
				vertices[i] = matrix4x.MultiplyPoint3x4(vertices[i]);
				vertices[i] = otherGameObject.transform.InverseTransformPoint(vertices[i]);
			}
			Plane plane = new Plane(vertices[0], vertices[1], vertices[2]);
			Vector3 normalized = (vertices[1] - vertices[0]).normalized;
			Vector3 normalized2 = (vertices[2] - vertices[1]).normalized;
			Matrix4x4 inverse = Matrix4x4.TRS(vertices[0], Quaternion.LookRotation(normalized, Vector3.Cross(normalized, normalized2)), Vector3.one).inverse;
			float magnitude = (vertices[2] - vertices[1]).magnitude;
			float fLimitUp = component.sharedMesh.bounds.max.y - component.sharedMesh.bounds.min.y;
			float magnitude2 = (vertices[1] - vertices[0]).magnitude;
			Vector3[] boundingBoxSegments = GetBoundingBoxSegments(component.sharedMesh.bounds);
			for (int j = 0; j < 12; j++)
			{
				if (TestSegmentVsPlane(boundingBoxSegments[j * 2], boundingBoxSegments[j * 2 + 1], plane, inverse, magnitude, fLimitUp, magnitude2))
				{
					return true;
				}
			}
			if (bBelowIsAlsoValid)
			{
				float distanceToPoint = plane.GetDistanceToPoint(component.sharedMesh.bounds.center);
				if (distanceToPoint < 0f)
				{
					for (int k = 0; k < 24; k++)
					{
						boundingBoxSegments[k] += plane.normal * (0f - distanceToPoint);
					}
					for (int l = 0; l < 12; l++)
					{
						if (TestSegmentVsPlane(boundingBoxSegments[l * 2], boundingBoxSegments[l * 2 + 1], plane, inverse, magnitude, fLimitUp, magnitude2))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool TestSegmentVsPlane(Vector3 v1, Vector3 v2, Plane plane, Matrix4x4 mtxToPlaneLocal, float fLimitRight, float fLimitUp, float fLimitForward)
		{
			float num = v1.x * plane.normal.x + v1.y * plane.normal.y + v1.z * plane.normal.z + plane.distance;
			float num2 = v2.x * plane.normal.x + v2.y * plane.normal.y + v2.z * plane.normal.z + plane.distance;
			if (num * num2 > 0f)
			{
				return false;
			}
			float enter = 0f;
			Ray ray = new Ray(v1, (v2 - v1).normalized);
			if (plane.Raycast(ray, out enter))
			{
				Vector3 point = v1 + (v2 - v1).normalized * enter;
				Vector3 vector = mtxToPlaneLocal.MultiplyPoint3x4(point);
				if (enter <= fLimitUp && vector.x >= 0f && vector.x <= fLimitRight && vector.z >= 0f && vector.z <= fLimitForward)
				{
					return true;
				}
			}
			return false;
		}
	}
}
