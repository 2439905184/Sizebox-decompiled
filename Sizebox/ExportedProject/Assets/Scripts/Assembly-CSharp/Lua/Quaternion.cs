using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Quaternion
	{
		[MoonSharpHidden]
		public UnityEngine.Quaternion quaternion;

		public float w
		{
			get
			{
				return quaternion.w;
			}
			set
			{
				quaternion.w = value;
			}
		}

		public float x
		{
			get
			{
				return quaternion.x;
			}
			set
			{
				quaternion.x = value;
			}
		}

		public float y
		{
			get
			{
				return quaternion.y;
			}
			set
			{
				quaternion.y = value;
			}
		}

		public float z
		{
			get
			{
				return quaternion.z;
			}
			set
			{
				quaternion.z = value;
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return new Vector3(quaternion.eulerAngles);
			}
		}

		public static Quaternion identity
		{
			get
			{
				return new Quaternion(UnityEngine.Quaternion.identity);
			}
		}

		public static Quaternion New(float x, float y, float z, float w)
		{
			return new Quaternion(new UnityEngine.Quaternion(x, y, z, w));
		}

		[MoonSharpHidden]
		public Quaternion(UnityEngine.Quaternion quaternion)
		{
			this.quaternion = quaternion;
		}

		public override string ToString()
		{
			return quaternion.ToString();
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(Quaternion o, string s)
		{
			return o.ToString() + s;
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(string s, Quaternion o)
		{
			return s + o.ToString();
		}

		[MoonSharpUserDataMetamethod("__concat")]
		public static string Concat(Quaternion o1, Quaternion o2)
		{
			return o1.ToString() + o2.ToString();
		}

		[MoonSharpUserDataMetamethod("__eq")]
		public static bool Eq(Quaternion o1, Quaternion o2)
		{
			return o1.quaternion == o2.quaternion;
		}

		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(lhs.quaternion * rhs.quaternion);
		}

		public static Vector3 operator *(Quaternion rotation, Vector3 vector)
		{
			return new Vector3(rotation.quaternion * vector.virtualPosition);
		}

		public void Set(float x, float y, float z, float w)
		{
			quaternion.Set(x, y, z, w);
		}

		public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			quaternion.SetFromToRotation(fromDirection.virtualPosition, toDirection.virtualPosition);
		}

		public void SetLookRotation(Vector3 view)
		{
			quaternion.SetLookRotation(view.virtualPosition);
		}

		public void SetLookRotation(Vector3 view, Vector3 up)
		{
			quaternion.SetLookRotation(view.virtualPosition, up.virtualPosition);
		}

		public static float Angle(Quaternion a, Quaternion b)
		{
			return UnityEngine.Quaternion.Angle(a.quaternion, b.quaternion);
		}

		public static Quaternion AngleAxis(float angle, Vector3 axis)
		{
			return new Quaternion(UnityEngine.Quaternion.AngleAxis(angle, axis.virtualPosition));
		}

		public static float Dot(Quaternion a, Quaternion b)
		{
			return UnityEngine.Quaternion.Dot(a.quaternion, b.quaternion);
		}

		public static Quaternion Euler(float x, float y, float z)
		{
			return new Quaternion(UnityEngine.Quaternion.Euler(x, y, z));
		}

		public static Quaternion Euler(Vector3 euler)
		{
			return new Quaternion(UnityEngine.Quaternion.Euler(euler.virtualPosition));
		}

		public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			return new Quaternion(UnityEngine.Quaternion.FromToRotation(fromDirection.virtualPosition, toDirection.virtualPosition));
		}

		public static Quaternion Inverse(Quaternion rotation)
		{
			return new Quaternion(UnityEngine.Quaternion.Inverse(rotation.quaternion));
		}

		public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
		{
			return new Quaternion(UnityEngine.Quaternion.Lerp(a.quaternion, b.quaternion, t));
		}

		public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			return new Quaternion(UnityEngine.Quaternion.LerpUnclamped(a.quaternion, b.quaternion, t));
		}

		public static Quaternion LookRotation(Vector3 forward)
		{
			return new Quaternion(UnityEngine.Quaternion.LookRotation(forward.virtualPosition));
		}

		public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
		{
			return new Quaternion(UnityEngine.Quaternion.LookRotation(forward.virtualPosition, upwards.virtualPosition));
		}

		public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
		{
			return new Quaternion(UnityEngine.Quaternion.RotateTowards(from.quaternion, to.quaternion, maxDegreesDelta));
		}

		public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
		{
			return new Quaternion(UnityEngine.Quaternion.Slerp(a.quaternion, b.quaternion, t));
		}

		public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			return new Quaternion(UnityEngine.Quaternion.SlerpUnclamped(a.quaternion, b.quaternion, t));
		}
	}
}
