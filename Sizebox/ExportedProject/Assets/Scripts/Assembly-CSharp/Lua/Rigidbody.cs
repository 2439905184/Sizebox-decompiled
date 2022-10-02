using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Rigidbody
	{
		private UnityEngine.Rigidbody _rigidbody;

		private Gravity _gravity;

		public float angularDrag
		{
			get
			{
				return _rigidbody.angularDrag;
			}
			set
			{
				_rigidbody.angularDrag = value;
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				return new Vector3(_rigidbody.angularVelocity);
			}
			set
			{
				_rigidbody.angularVelocity = value.virtualPosition;
			}
		}

		public float drag
		{
			get
			{
				return _rigidbody.drag;
			}
			set
			{
				_rigidbody.drag = value;
			}
		}

		public bool freezeRotation
		{
			get
			{
				return _rigidbody.freezeRotation;
			}
			set
			{
				_rigidbody.freezeRotation = value;
			}
		}

		public float mass
		{
			get
			{
				return _rigidbody.mass;
			}
			set
			{
				_rigidbody.mass = value;
			}
		}

		public float maxAngularVelocity
		{
			get
			{
				return _rigidbody.maxAngularVelocity;
			}
			set
			{
				_rigidbody.maxAngularVelocity = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return new Vector3(_rigidbody.position);
			}
			set
			{
				_rigidbody.position = value.virtualPosition;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return new Quaternion(_rigidbody.rotation);
			}
			set
			{
				_rigidbody.rotation = value.quaternion;
			}
		}

		public bool useGravity
		{
			get
			{
				if (_gravity == null)
				{
					_gravity = _rigidbody.GetComponent<Gravity>();
				}
				if (_gravity != null)
				{
					return _gravity.enabled;
				}
				return _rigidbody.useGravity;
			}
			set
			{
				if (_gravity == null)
				{
					_gravity = _rigidbody.GetComponent<Gravity>();
				}
				if (_gravity != null)
				{
					_gravity.enabled = value;
				}
				else
				{
					_rigidbody.useGravity = value;
				}
			}
		}

		public Vector3 velocity
		{
			get
			{
				return new Vector3(_rigidbody.velocity);
			}
			set
			{
				_rigidbody.velocity = value.virtualPosition;
			}
		}

		public Vector3 worldCenterOfMass
		{
			get
			{
				return new Vector3(_rigidbody.worldCenterOfMass);
			}
		}

		public bool isKinematic
		{
			get
			{
				return _rigidbody.isKinematic;
			}
			set
			{
				_rigidbody.isKinematic = value;
			}
		}

		public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
		{
			_rigidbody.AddExplosionForce(explosionForce, explosionPosition.virtualPosition, explosionRadius);
		}

		public void AddForce(Vector3 force)
		{
			_rigidbody.AddForce(force.virtualPosition);
		}

		public void AddRelativeForce(Vector3 force)
		{
			_rigidbody.AddRelativeForce(force.virtualPosition);
		}

		public void MovePosition(Vector3 position)
		{
			_rigidbody.MovePosition(position.virtualPosition);
		}

		public void MoveRotation(Quaternion rot)
		{
			_rigidbody.MoveRotation(rot.quaternion);
		}

		public Vector3 ClosestPointOnBounds(Vector3 position)
		{
			return new Vector3(_rigidbody.ClosestPointOnBounds(position.virtualPosition));
		}

		[MoonSharpHidden]
		public Rigidbody(UnityEngine.Rigidbody rigidbody)
		{
			if (rigidbody == null)
			{
				Debug.LogError("Creating empty rigidbody");
			}
			_rigidbody = rigidbody;
		}
	}
}
