using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Vector3
	{
		[MoonSharpHidden]
		public UnityEngine.Vector3 virtualPosition;

		public float x
		{
			get
			{
				return virtualPosition.x;
			}
			set
			{
				virtualPosition.x = value;
			}
		}

		public float y
		{
			get
			{
				return virtualPosition.y;
			}
			set
			{
				virtualPosition.y = value;
			}
		}

		public float z
		{
			get
			{
				return virtualPosition.z;
			}
			set
			{
				virtualPosition.z = value;
			}
		}

		public float magnitude
		{
			get
			{
				return virtualPosition.magnitude;
			}
		}

		public float sqrMagnitude
		{
			get
			{
				return virtualPosition.sqrMagnitude;
			}
		}

		public Vector3 normalized
		{
			get
			{
				return new Vector3(virtualPosition.normalized);
			}
		}

		public static Vector3 back
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.back);
			}
		}

		public static Vector3 down
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.down);
			}
		}

		public static Vector3 forward
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.forward);
			}
		}

		public static Vector3 left
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.left);
			}
		}

		public static Vector3 one
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.one);
			}
		}

		public static Vector3 right
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.right);
			}
		}

		public static Vector3 up
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.up);
			}
		}

		public static Vector3 zero
		{
			get
			{
				return new Vector3(UnityEngine.Vector3.zero);
			}
		}

		public static Vector3 New(float x, float y, float z)
		{
			return new Vector3(new UnityEngine.Vector3(x, y, z));
		}

		[MoonSharpHidden]
		public Vector3(UnityEngine.Vector3 vector3)
		{
			virtualPosition = vector3;
		}

		[MoonSharpHidden]
		public Vector3(float x, float y, float z)
		{
			virtualPosition = new UnityEngine.Vector3(x, y, z);
		}

		public override string ToString()
		{
			return virtualPosition.ToString();
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(Vector3 o, string v)
		{
			return o.ToString() + v;
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(string v, Vector3 o)
		{
			return v + o.ToString();
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(Vector3 o1, Vector3 o2)
		{
			return o1.ToString() + o2.ToString();
		}

		[MoonSharpUserDataMetamethod("__eq")]
		public static bool Eq(Vector3 o1, Vector3 o2)
		{
			if (o1 == null && o2 == null)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			return o1.virtualPosition == o2.virtualPosition;
		}

		public static Vector3 operator +(Vector3 o1, Vector3 o2)
		{
			return new Vector3(o1.virtualPosition + o2.virtualPosition);
		}

		public static Vector3 operator -(Vector3 o1, Vector3 o2)
		{
			return new Vector3(o1.virtualPosition - o2.virtualPosition);
		}

		public static Vector3 operator -(Vector3 o1)
		{
			return new Vector3(-o1.virtualPosition);
		}

		public static Vector3 operator *(Vector3 o1, float f)
		{
			return new Vector3(o1.virtualPosition * f);
		}

		public static Vector3 operator *(float f, Vector3 o1)
		{
			return new Vector3(f * o1.virtualPosition);
		}

		public static Vector3 operator /(Vector3 o1, float f)
		{
			return new Vector3(o1.virtualPosition / f);
		}

		public void Normalize()
		{
			virtualPosition.Normalize();
		}

		public void Set(float x, float y, float z)
		{
			virtualPosition.Set(x, y, z);
		}

		public static float Angle(Vector3 from, Vector3 to)
		{
			return UnityEngine.Vector3.Angle(from.virtualPosition, to.virtualPosition);
		}

		public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
		{
			return new Vector3(UnityEngine.Vector3.ClampMagnitude(vector.virtualPosition, maxLength));
		}

		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(UnityEngine.Vector3.Cross(lhs.virtualPosition, rhs.virtualPosition));
		}

		public static float Distance(Vector3 a, Vector3 b)
		{
			return UnityEngine.Vector3.Distance(a.virtualPosition, b.virtualPosition);
		}

		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return UnityEngine.Vector3.Dot(lhs.virtualPosition, rhs.virtualPosition);
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(UnityEngine.Vector3.Lerp(a.virtualPosition, b.virtualPosition, t));
		}

		public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(UnityEngine.Vector3.LerpUnclamped(a.virtualPosition, b.virtualPosition, t));
		}

		public static Vector3 Max(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(UnityEngine.Vector3.Max(lhs.virtualPosition, rhs.virtualPosition));
		}

		public static Vector3 Min(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(UnityEngine.Vector3.Min(lhs.virtualPosition, rhs.virtualPosition));
		}

		public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
		{
			return new Vector3(UnityEngine.Vector3.MoveTowards(current.virtualPosition, target.virtualPosition, maxDistanceDelta));
		}

		public static Vector3 Project(Vector3 vector, Vector3 onNormal)
		{
			return new Vector3(UnityEngine.Vector3.Project(vector.virtualPosition, onNormal.virtualPosition));
		}

		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
		{
			return new Vector3(UnityEngine.Vector3.ProjectOnPlane(vector.virtualPosition, planeNormal.virtualPosition));
		}

		public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
		{
			return new Vector3(UnityEngine.Vector3.Reflect(inDirection.virtualPosition, inNormal.virtualPosition));
		}

		public static Vector3 RotateTowards(Vector3 current, Vector3 target, float maxRadiansDelta, float maxMagnitudeDelta)
		{
			return new Vector3(UnityEngine.Vector3.RotateTowards(current.virtualPosition, target.virtualPosition, maxRadiansDelta, maxMagnitudeDelta));
		}

		public static Vector3 Scale(Vector3 a, Vector3 b)
		{
			return new Vector3(UnityEngine.Vector3.Scale(a.virtualPosition, b.virtualPosition));
		}

		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(UnityEngine.Vector3.Slerp(a.virtualPosition, b.virtualPosition, t));
		}

		public static Vector3 SlerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(UnityEngine.Vector3.SlerpUnclamped(a.virtualPosition, b.virtualPosition, t));
		}
	}
}
