using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Transform
	{
		[MoonSharpHidden]
		public UnityEngine.Transform _tf;

		public int childCount
		{
			get
			{
				return _tf.childCount;
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return new Vector3(_tf.eulerAngles);
			}
			set
			{
				_tf.eulerAngles = value.virtualPosition;
			}
		}

		public Vector3 forward
		{
			get
			{
				return new Vector3(_tf.forward);
			}
			set
			{
				_tf.forward = value.virtualPosition;
			}
		}

		public Vector3 localEulerAngles
		{
			get
			{
				return new Vector3(_tf.localEulerAngles);
			}
			set
			{
				_tf.localEulerAngles = value.virtualPosition;
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return new Vector3(_tf.localPosition);
			}
			set
			{
				_tf.localPosition = value.virtualPosition;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				return new Quaternion(_tf.localRotation);
			}
			set
			{
				_tf.localRotation = value.quaternion;
			}
		}

		public Vector3 localScale
		{
			get
			{
				return new Vector3(_tf.localScale);
			}
			set
			{
				_tf.localScale = value.virtualPosition;
			}
		}

		public Vector3 lossyScale
		{
			get
			{
				return new Vector3(_tf.lossyScale);
			}
		}

		public string name
		{
			get
			{
				return _tf.name;
			}
			set
			{
				_tf.name = value;
			}
		}

		public Transform parent
		{
			get
			{
				if (!(_tf != null))
				{
					return null;
				}
				return GetTransform(_tf.parent);
			}
		}

		public Entity entity
		{
			get
			{
				UnityEngine.Transform tf = _tf;
				while (!tf.GetComponent<EntityBase>())
				{
					tf = tf.parent;
				}
				return tf.GetComponent<EntityBase>().GetLuaEntity();
			}
		}

		public Vector3 position
		{
			get
			{
				return new Vector3(_tf.position.ToVirtual());
			}
			set
			{
				_tf.position = value.virtualPosition.ToWorld();
			}
		}

		public Vector3 right
		{
			get
			{
				return new Vector3(_tf.right);
			}
			set
			{
				_tf.right = value.virtualPosition;
			}
		}

		public Transform root
		{
			get
			{
				return GetTransform(_tf.root);
			}
		}

		public Quaternion rotation
		{
			get
			{
				return new Quaternion(_tf.rotation);
			}
			set
			{
				_tf.rotation = value.quaternion;
			}
		}

		public Vector3 up
		{
			get
			{
				return new Vector3(_tf.up);
			}
			set
			{
				_tf.up = value.virtualPosition;
			}
		}

		private Transform GetTransform(UnityEngine.Transform unityTransform)
		{
			if (unityTransform == null)
			{
				return null;
			}
			return new Transform(unityTransform);
		}

		public void DetachChildren()
		{
			_tf.DetachChildren();
		}

		public Transform Find(string name)
		{
			return GetTransform(_tf.Find(name));
		}

		public Transform GetChild(int index)
		{
			return GetTransform(_tf.GetChild(index));
		}

		public int GetSiblingIndex()
		{
			return _tf.GetSiblingIndex();
		}

		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			return new Vector3(_tf.InverseTransformDirection(direction.virtualPosition));
		}

		public Vector3 InverseTransformDirection(float x, float y, float z)
		{
			return new Vector3(_tf.InverseTransformDirection(x, y, z));
		}

		public Vector3 InverseTransformPoint(Vector3 direction)
		{
			UnityEngine.Vector3 vector = direction.virtualPosition.ToWorld();
			return new Vector3(_tf.InverseTransformPoint(vector));
		}

		public Vector3 InverseTransformPoint(float x, float y, float z)
		{
			return InverseTransformPoint(Vector3.New(x, y, z));
		}

		public Vector3 InverseTransformVector(Vector3 direction)
		{
			return new Vector3(_tf.InverseTransformVector(direction.virtualPosition));
		}

		public Vector3 InverseTransformVector(float x, float y, float z)
		{
			return new Vector3(_tf.InverseTransformVector(x, y, z));
		}

		public bool IsChildOf(Transform parent)
		{
			if (parent != null)
			{
				return _tf.IsChildOf(parent._tf);
			}
			return false;
		}

		public void LookAt(Transform target)
		{
			if (target != null)
			{
				_tf.LookAt(target._tf);
			}
			else
			{
				Debug.LogError("target is nil");
			}
		}

		public void LookAt(Transform target, Vector3 worldUp)
		{
			if (target != null)
			{
				_tf.LookAt(target._tf, worldUp.virtualPosition);
			}
			else
			{
				Debug.LogError("target is nil");
			}
		}

		public void LookAt(Vector3 worldPosition)
		{
			_tf.LookAt(worldPosition.virtualPosition);
		}

		public void LookAt(Vector3 worldPosition, Vector3 worldUp)
		{
			_tf.LookAt(worldPosition.virtualPosition, worldUp.virtualPosition);
		}

		public void Rotate(Vector3 eulerAngles)
		{
			_tf.Rotate(eulerAngles.virtualPosition);
		}

		public void Rotate(float xAngle, float yAngle, float zAngle)
		{
			_tf.Rotate(xAngle, yAngle, zAngle);
		}

		public void Rotate(Vector3 axis, float angle)
		{
			_tf.Rotate(axis.virtualPosition, angle);
		}

		public void Rotate(Vector3 point, Vector3 axis, float angle)
		{
			_tf.RotateAround(point.virtualPosition, axis.virtualPosition, angle);
		}

		public void SetParent(Transform parent)
		{
			if (parent != null)
			{
				_tf.SetParent(parent._tf);
			}
			else
			{
				_tf.parent = null;
			}
		}

		public void SetParent(Transform parent, bool worldPositionStays)
		{
			if (parent != null)
			{
				_tf.SetParent(parent._tf, worldPositionStays);
			}
			else
			{
				_tf.parent = null;
			}
		}

		public Vector3 TransformDirection(Vector3 direction)
		{
			return new Vector3(_tf.TransformDirection(direction.virtualPosition));
		}

		public Vector3 TransformDirection(float x, float y, float z)
		{
			return new Vector3(_tf.TransformDirection(x, y, z));
		}

		public Vector3 TransformPoint(Vector3 direction)
		{
			return new Vector3(_tf.TransformPoint(direction.virtualPosition).ToVirtual());
		}

		public Vector3 TransformPoint(float x, float y, float z)
		{
			return TransformPoint(Vector3.New(x, y, z));
		}

		public Vector3 TransformVector(Vector3 direction)
		{
			return new Vector3(_tf.TransformVector(direction.virtualPosition));
		}

		public Vector3 TransformVector(float x, float y, float z)
		{
			return new Vector3(_tf.TransformVector(x, y, z));
		}

		public void Translate(Vector3 translation)
		{
			_tf.Translate(translation.virtualPosition);
		}

		public void Translate(float x, float y, float z)
		{
			_tf.Translate(x, y, z);
		}

		public void Translate(Vector3 translation, Transform relativeTo)
		{
			if (relativeTo != null)
			{
				_tf.Translate(translation.virtualPosition, relativeTo._tf);
			}
			else
			{
				Debug.LogError("relativeTo is nil");
			}
		}

		public void Translate(float x, float y, float z, Transform relativeTo)
		{
			if (relativeTo != null)
			{
				_tf.Translate(x, y, z, relativeTo._tf);
			}
			else
			{
				Debug.LogError("relativeTo is nil");
			}
		}

		[MoonSharpUserDataMetamethod("__eq")]
		public static bool Equals(Transform a, Transform b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			return a._tf == b._tf;
		}

		[MoonSharpHidden]
		public Transform(UnityEngine.Transform transform)
		{
			if (transform == null)
			{
				Debug.LogError("Creating empty transform");
			}
			_tf = transform;
		}
	}
}
