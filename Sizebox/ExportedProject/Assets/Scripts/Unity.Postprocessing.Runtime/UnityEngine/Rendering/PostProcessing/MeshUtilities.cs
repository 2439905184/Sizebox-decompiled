using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	internal static class MeshUtilities
	{
		private static Dictionary<PrimitiveType, Mesh> s_Primitives;

		private static Dictionary<Type, PrimitiveType> s_ColliderPrimitives;

		static MeshUtilities()
		{
			s_Primitives = new Dictionary<PrimitiveType, Mesh>();
			s_ColliderPrimitives = new Dictionary<Type, PrimitiveType>
			{
				{
					typeof(BoxCollider),
					PrimitiveType.Cube
				},
				{
					typeof(SphereCollider),
					PrimitiveType.Sphere
				},
				{
					typeof(CapsuleCollider),
					PrimitiveType.Capsule
				}
			};
		}

		internal static Mesh GetColliderMesh(Collider collider)
		{
			Type type = collider.GetType();
			if (type == typeof(MeshCollider))
			{
				return ((MeshCollider)collider).sharedMesh;
			}
			return GetPrimitive(s_ColliderPrimitives[type]);
		}

		internal static Mesh GetPrimitive(PrimitiveType primitiveType)
		{
			Mesh value;
			if (!s_Primitives.TryGetValue(primitiveType, out value))
			{
				value = GetBuiltinMesh(primitiveType);
				s_Primitives.Add(primitiveType, value);
			}
			return value;
		}

		private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
		{
			GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
			Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			RuntimeUtilities.Destroy(gameObject);
			return sharedMesh;
		}
	}
}
