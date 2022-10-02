using System;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class HitReactionVRIK : OffsetModifierVRIK
	{
		[Serializable]
		public abstract class Offset
		{
			[Tooltip("Just for visual clarity, not used at all")]
			public string name;

			[Tooltip("Linking this hit point to a collider")]
			public Collider collider;

			[Tooltip("Only used if this hit point gets hit when already processing another hit")]
			[SerializeField]
			private float crossFadeTime = 0.1f;

			private float length;

			private float crossFadeSpeed;

			private float lastTime;

			protected float crossFader { get; private set; }

			protected float timer { get; private set; }

			protected Vector3 force { get; private set; }

			protected Vector3 point { get; private set; }

			public void Hit(Vector3 force, AnimationCurve[] curves, Vector3 point)
			{
				if (length == 0f)
				{
					length = GetLength(curves);
				}
				if (length <= 0f)
				{
					Debug.LogError("Hit Point WeightCurve length is zero.");
					return;
				}
				if (timer < 1f)
				{
					crossFader = 0f;
				}
				crossFadeSpeed = ((crossFadeTime > 0f) ? (1f / crossFadeTime) : 0f);
				CrossFadeStart();
				timer = 0f;
				this.force = force;
				this.point = point;
			}

			public void Apply(VRIK ik, AnimationCurve[] curves, float weight)
			{
				float num = Time.time - lastTime;
				lastTime = Time.time;
				if (!(timer >= length))
				{
					timer = Mathf.Clamp(timer + num, 0f, length);
					if (crossFadeSpeed > 0f)
					{
						crossFader = Mathf.Clamp(crossFader + num * crossFadeSpeed, 0f, 1f);
					}
					else
					{
						crossFader = 1f;
					}
					OnApply(ik, curves, weight);
				}
			}

			protected abstract float GetLength(AnimationCurve[] curves);

			protected abstract void CrossFadeStart();

			protected abstract void OnApply(VRIK ik, AnimationCurve[] curves, float weight);
		}

		[Serializable]
		public class PositionOffset : Offset
		{
			[Serializable]
			public class PositionOffsetLink
			{
				[Tooltip("The FBBIK effector type")]
				public IKSolverVR.PositionOffset positionOffset;

				[Tooltip("The weight of this effector (could also be negative)")]
				public float weight;

				private Vector3 lastValue;

				private Vector3 current;

				public void Apply(VRIK ik, Vector3 offset, float crossFader)
				{
					current = Vector3.Lerp(lastValue, offset * weight, crossFader);
					ik.solver.AddPositionOffset(positionOffset, current);
				}

				public void CrossFadeStart()
				{
					lastValue = current;
				}
			}

			[Tooltip("Offset magnitude in the direction of the hit force")]
			public int forceDirCurveIndex;

			[Tooltip("Offset magnitude in the direction of character.up")]
			public int upDirCurveIndex = 1;

			[Tooltip("Linking this offset to the VRIK position offsets")]
			public PositionOffsetLink[] offsetLinks;

			protected override float GetLength(AnimationCurve[] curves)
			{
				float num = ((curves[forceDirCurveIndex].keys.Length != 0) ? curves[forceDirCurveIndex].keys[curves[forceDirCurveIndex].length - 1].time : 0f);
				float min = ((curves[upDirCurveIndex].keys.Length != 0) ? curves[upDirCurveIndex].keys[curves[upDirCurveIndex].length - 1].time : 0f);
				return Mathf.Clamp(num, min, num);
			}

			protected override void CrossFadeStart()
			{
				PositionOffsetLink[] array = offsetLinks;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].CrossFadeStart();
				}
			}

			protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight)
			{
				Vector3 vector = ik.transform.up * base.force.magnitude;
				Vector3 offset = curves[forceDirCurveIndex].Evaluate(base.timer) * base.force + curves[upDirCurveIndex].Evaluate(base.timer) * vector;
				offset *= weight;
				PositionOffsetLink[] array = offsetLinks;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Apply(ik, offset, base.crossFader);
				}
			}
		}

		[Serializable]
		public class RotationOffset : Offset
		{
			[Serializable]
			public class RotationOffsetLink
			{
				[Tooltip("Reference to the bone that this hit point rotates")]
				public IKSolverVR.RotationOffset rotationOffset;

				[Tooltip("Weight of rotating the bone")]
				[Range(0f, 1f)]
				public float weight;

				private Quaternion lastValue = Quaternion.identity;

				private Quaternion current = Quaternion.identity;

				public void Apply(VRIK ik, Quaternion offset, float crossFader)
				{
					current = Quaternion.Lerp(lastValue, Quaternion.Lerp(Quaternion.identity, offset, weight), crossFader);
					ik.solver.AddRotationOffset(rotationOffset, current);
				}

				public void CrossFadeStart()
				{
					lastValue = current;
				}
			}

			[Tooltip("The angle to rotate the bone around it's rigidbody's world center of mass")]
			public int curveIndex;

			[Tooltip("Linking this hit point to bone(s)")]
			public RotationOffsetLink[] offsetLinks;

			private Rigidbody rigidbody;

			protected override float GetLength(AnimationCurve[] curves)
			{
				if (curves[curveIndex].keys.Length == 0)
				{
					return 0f;
				}
				return curves[curveIndex].keys[curves[curveIndex].length - 1].time;
			}

			protected override void CrossFadeStart()
			{
				RotationOffsetLink[] array = offsetLinks;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].CrossFadeStart();
				}
			}

			protected override void OnApply(VRIK ik, AnimationCurve[] curves, float weight)
			{
				if (collider == null)
				{
					Debug.LogError("No collider assigned for a HitPointBone in the HitReaction component.");
					return;
				}
				if (rigidbody == null)
				{
					rigidbody = collider.GetComponent<Rigidbody>();
				}
				if (rigidbody != null)
				{
					Vector3 axis = Vector3.Cross(base.force, base.point - rigidbody.worldCenterOfMass);
					Quaternion offset = Quaternion.AngleAxis(curves[curveIndex].Evaluate(base.timer) * weight, axis);
					RotationOffsetLink[] array = offsetLinks;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Apply(ik, offset, base.crossFader);
					}
				}
			}
		}

		public AnimationCurve[] offsetCurves;

		[Tooltip("Hit points for the FBBIK effectors")]
		public PositionOffset[] positionOffsets;

		[Tooltip(" Hit points for bones without an effector, such as the head")]
		public RotationOffset[] rotationOffsets;

		protected override void OnModifyOffset()
		{
			PositionOffset[] array = positionOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Apply(ik, offsetCurves, weight);
			}
			RotationOffset[] array2 = rotationOffsets;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Apply(ik, offsetCurves, weight);
			}
		}

		public void Hit(Collider collider, Vector3 force, Vector3 point)
		{
			if (ik == null)
			{
				Debug.LogError("No IK assigned in HitReaction");
				return;
			}
			PositionOffset[] array = positionOffsets;
			foreach (PositionOffset positionOffset in array)
			{
				if (positionOffset.collider == collider)
				{
					positionOffset.Hit(force, offsetCurves, point);
				}
			}
			RotationOffset[] array2 = rotationOffsets;
			foreach (RotationOffset rotationOffset in array2)
			{
				if (rotationOffset.collider == collider)
				{
					rotationOffset.Hit(force, offsetCurves, point);
				}
			}
		}
	}
}
