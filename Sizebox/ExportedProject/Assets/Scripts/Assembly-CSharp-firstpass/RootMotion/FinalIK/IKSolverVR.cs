using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RootMotion.FinalIK
{
	[Serializable]
	public class IKSolverVR : IKSolver
	{
		[Serializable]
		public class Arm : BodyPart
		{
			[Serializable]
			public enum ShoulderRotationMode
			{
				YawPitch = 0,
				FromTo = 1
			}

			[Tooltip("The hand target")]
			public Transform target;

			[Tooltip("The elbow will be bent towards this Transform if 'Bend Goal Weight' > 0.")]
			public Transform bendGoal;

			[Tooltip("Positional weight of the hand target.")]
			[Range(0f, 1f)]
			public float positionWeight = 1f;

			[Tooltip("Rotational weight of the hand target")]
			[Range(0f, 1f)]
			public float rotationWeight = 1f;

			[Tooltip("Different techniques for shoulder bone rotation.")]
			public ShoulderRotationMode shoulderRotationMode;

			[Tooltip("The weight of shoulder rotation")]
			[Range(0f, 1f)]
			public float shoulderRotationWeight = 1f;

			[Tooltip("If greater than 0, will bend the elbow towards the 'Bend Goal' Transform.")]
			[Range(0f, 1f)]
			public float bendGoalWeight;

			[Tooltip("Angular offset of the elbow bending direction.")]
			[Range(-180f, 180f)]
			public float swivelOffset;

			[Tooltip("Local axis of the hand bone that points from the wrist towards the palm. Used for defining hand bone orientation.")]
			public Vector3 wristToPalmAxis = Vector3.zero;

			[Tooltip("Local axis of the hand bone that points from the palm towards the thumb. Used for defining hand bone orientation.")]
			public Vector3 palmToThumbAxis = Vector3.zero;

			[Tooltip("Use this to make the arm shorter/longer.")]
			[Range(0.01f, 2f)]
			public float armLengthMlp = 1f;

			[Tooltip("Evaluates stretching of the arm by target distance relative to arm length. Value at time 1 represents stretching amount at the point where distance to the target is equal to arm length. Value at time 2 represents stretching amount at the point where distance to the target is double the arm length. Value represents the amount of stretching. Linear stretching would be achieved with a linear curve going up by 45 degrees. Increase the range of stretching by moving the last key up and right at the same amount. Smoothing in the curve can help reduce elbow snapping (start stretching the arm slightly before target distance reaches arm length).")]
			public AnimationCurve stretchCurve;

			[NonSerialized]
			[HideInInspector]
			public Vector3 IKPosition;

			[NonSerialized]
			[HideInInspector]
			public Quaternion IKRotation = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Vector3 bendDirection = Vector3.back;

			[NonSerialized]
			[HideInInspector]
			public Vector3 handPositionOffset;

			private bool hasShoulder;

			private Vector3 chestForwardAxis;

			private Vector3 chestUpAxis;

			private Quaternion chestRotation = Quaternion.identity;

			private Vector3 chestForward;

			private Vector3 chestUp;

			private Quaternion forearmRelToUpperArm = Quaternion.identity;

			private const float yawOffsetAngle = 45f;

			private const float pitchOffsetAngle = -30f;

			public Vector3 position { get; private set; }

			public Quaternion rotation { get; private set; }

			private VirtualBone shoulder
			{
				get
				{
					return bones[0];
				}
			}

			private VirtualBone upperArm
			{
				get
				{
					return bones[1];
				}
			}

			private VirtualBone forearm
			{
				get
				{
					return bones[2];
				}
			}

			private VirtualBone hand
			{
				get
				{
					return bones[3];
				}
			}

			protected override void OnRead(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index)
			{
				Vector3 vector = positions[index];
				Quaternion quaternion = rotations[index];
				Vector3 vector2 = positions[index + 1];
				Quaternion quaternion2 = rotations[index + 1];
				Vector3 vector3 = positions[index + 2];
				Quaternion quaternion3 = rotations[index + 2];
				Vector3 iKPosition = positions[index + 3];
				Quaternion iKRotation = rotations[index + 3];
				if (!initiated)
				{
					IKPosition = iKPosition;
					IKRotation = iKRotation;
					rotation = IKRotation;
					hasShoulder = hasShoulders;
					bones = new VirtualBone[hasShoulder ? 4 : 3];
					if (hasShoulder)
					{
						bones[0] = new VirtualBone(vector, quaternion);
						bones[1] = new VirtualBone(vector2, quaternion2);
						bones[2] = new VirtualBone(vector3, quaternion3);
						bones[3] = new VirtualBone(iKPosition, iKRotation);
					}
					else
					{
						bones[0] = new VirtualBone(vector2, quaternion2);
						bones[1] = new VirtualBone(vector3, quaternion3);
						bones[2] = new VirtualBone(iKPosition, iKRotation);
					}
					chestForwardAxis = Quaternion.Inverse(rootRotation) * (rotations[0] * Vector3.forward);
					chestUpAxis = Quaternion.Inverse(rootRotation) * (rotations[0] * Vector3.up);
				}
				if (hasShoulder)
				{
					bones[0].Read(vector, quaternion);
					bones[1].Read(vector2, quaternion2);
					bones[2].Read(vector3, quaternion3);
					bones[3].Read(iKPosition, iKRotation);
				}
				else
				{
					bones[0].Read(vector2, quaternion2);
					bones[1].Read(vector3, quaternion3);
					bones[2].Read(iKPosition, iKRotation);
				}
			}

			public override void PreSolve()
			{
				if (target != null)
				{
					IKPosition = target.position;
					IKRotation = target.rotation;
				}
				position = V3Tools.Lerp(hand.solverPosition, IKPosition, positionWeight);
				rotation = QuaTools.Lerp(hand.solverRotation, IKRotation, rotationWeight);
				shoulder.axis = shoulder.axis.normalized;
				forearmRelToUpperArm = Quaternion.Inverse(upperArm.solverRotation) * forearm.solverRotation;
			}

			public override void ApplyOffsets()
			{
				position += handPositionOffset;
			}

			private void Stretching()
			{
				float num = upperArm.length + forearm.length;
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				if (armLengthMlp != 1f)
				{
					num *= armLengthMlp;
					zero = (forearm.solverPosition - upperArm.solverPosition) * (armLengthMlp - 1f);
					zero2 = (hand.solverPosition - forearm.solverPosition) * (armLengthMlp - 1f);
					forearm.solverPosition += zero;
					hand.solverPosition += zero + zero2;
				}
				float time = Vector3.Distance(upperArm.solverPosition, position) / num;
				float num2 = stretchCurve.Evaluate(time);
				num2 *= positionWeight;
				zero = (forearm.solverPosition - upperArm.solverPosition) * num2;
				zero2 = (hand.solverPosition - forearm.solverPosition) * num2;
				forearm.solverPosition += zero;
				hand.solverPosition += zero + zero2;
			}

			public void Solve(bool isLeft)
			{
				chestRotation = Quaternion.LookRotation(rootRotation * chestForwardAxis, rootRotation * chestUpAxis);
				chestForward = chestRotation * Vector3.forward;
				chestUp = chestRotation * Vector3.up;
				if (hasShoulder && shoulderRotationWeight > 0f)
				{
					switch (shoulderRotationMode)
					{
					case ShoulderRotationMode.YawPitch:
					{
						Vector3 normalized = (position - shoulder.solverPosition).normalized;
						float num3 = (isLeft ? 45f : (-45f));
						Quaternion quaternion2 = Quaternion.AngleAxis((isLeft ? (-90f) : 90f) + num3, chestUp) * chestRotation;
						Vector3 lhs = Quaternion.Inverse(quaternion2) * normalized;
						float num4 = Mathf.Atan2(lhs.x, lhs.z) * 57.29578f;
						float f = Vector3.Dot(lhs, Vector3.up);
						f = 1f - Mathf.Abs(f);
						num4 *= f;
						num4 -= num3;
						float num5 = (isLeft ? (-20f) : (-50f));
						float num6 = (isLeft ? 50f : 20f);
						num4 = DamperValue(num4, num5 - num3, num6 - num3, 0.7f);
						Vector3 fromDirection = shoulder.solverRotation * shoulder.axis;
						Vector3 toDirection = quaternion2 * (Quaternion.AngleAxis(num4, Vector3.up) * Vector3.forward);
						Quaternion quaternion3 = Quaternion.FromToRotation(fromDirection, toDirection);
						quaternion2 = Quaternion.AngleAxis(isLeft ? (-90f) : 90f, chestUp) * chestRotation;
						quaternion2 = Quaternion.AngleAxis(isLeft ? (-30f) : 30f, chestForward) * quaternion2;
						normalized = position - (shoulder.solverPosition + chestRotation * (isLeft ? Vector3.right : Vector3.left) * base.mag);
						lhs = Quaternion.Inverse(quaternion2) * normalized;
						float num7 = Mathf.Atan2(lhs.y, lhs.z) * 57.29578f;
						num7 -= -30f;
						num7 = DamperValue(num7, -15f, 75f);
						Quaternion b2 = Quaternion.AngleAxis(0f - num7, quaternion2 * Vector3.right) * quaternion3;
						if (shoulderRotationWeight * positionWeight < 1f)
						{
							b2 = Quaternion.Lerp(Quaternion.identity, b2, shoulderRotationWeight * positionWeight);
						}
						VirtualBone.RotateBy(bones, b2);
						Stretching();
						VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
						float angle = Mathf.Clamp(num7 * 2f * positionWeight, 0f, 180f);
						shoulder.solverRotation = Quaternion.AngleAxis(angle, shoulder.solverRotation * (isLeft ? shoulder.axis : (-shoulder.axis))) * shoulder.solverRotation;
						upperArm.solverRotation = Quaternion.AngleAxis(angle, upperArm.solverRotation * (isLeft ? upperArm.axis : (-upperArm.axis))) * upperArm.solverRotation;
						break;
					}
					case ShoulderRotationMode.FromTo:
					{
						Quaternion solverRotation = shoulder.solverRotation;
						Quaternion b = Quaternion.FromToRotation((upperArm.solverPosition - shoulder.solverPosition).normalized + chestForward, position - shoulder.solverPosition);
						b = Quaternion.Slerp(Quaternion.identity, b, 0.5f * shoulderRotationWeight * positionWeight);
						VirtualBone.RotateBy(bones, b);
						Stretching();
						VirtualBone.SolveTrigonometric(bones, 0, 2, 3, position, Vector3.Cross(forearm.solverPosition - shoulder.solverPosition, hand.solverPosition - shoulder.solverPosition), 0.5f * shoulderRotationWeight * positionWeight);
						VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
						Quaternion quaternion = Quaternion.Inverse(Quaternion.LookRotation(chestUp, chestForward));
						Vector3 vector = quaternion * (solverRotation * shoulder.axis);
						Vector3 vector2 = quaternion * (shoulder.solverRotation * shoulder.axis);
						float current = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
						float num = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
						float num2 = Mathf.DeltaAngle(current, num);
						if (isLeft)
						{
							num2 = 0f - num2;
						}
						num2 = Mathf.Clamp(num2 * 2f * positionWeight, 0f, 180f);
						shoulder.solverRotation = Quaternion.AngleAxis(num2, shoulder.solverRotation * (isLeft ? shoulder.axis : (-shoulder.axis))) * shoulder.solverRotation;
						upperArm.solverRotation = Quaternion.AngleAxis(num2, upperArm.solverRotation * (isLeft ? upperArm.axis : (-upperArm.axis))) * upperArm.solverRotation;
						break;
					}
					}
				}
				else
				{
					Stretching();
					VirtualBone.SolveTrigonometric(bones, 1, 2, 3, position, GetBendNormal(position - upperArm.solverPosition), positionWeight);
				}
				Quaternion quaternion4 = upperArm.solverRotation * forearmRelToUpperArm;
				Quaternion quaternion5 = Quaternion.FromToRotation(quaternion4 * forearm.axis, hand.solverPosition - forearm.solverPosition);
				RotateTo(forearm, quaternion5 * quaternion4, positionWeight);
				if (rotationWeight >= 1f)
				{
					hand.solverRotation = rotation;
				}
				else if (rotationWeight > 0f)
				{
					hand.solverRotation = Quaternion.Lerp(hand.solverRotation, rotation, rotationWeight);
				}
			}

			public override void ResetOffsets()
			{
				handPositionOffset = Vector3.zero;
			}

			public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
			{
				if (hasShoulder)
				{
					solvedPositions[index] = shoulder.solverPosition;
					solvedRotations[index] = shoulder.solverRotation;
				}
				solvedPositions[index + 1] = upperArm.solverPosition;
				solvedPositions[index + 2] = forearm.solverPosition;
				solvedPositions[index + 3] = hand.solverPosition;
				solvedRotations[index + 1] = upperArm.solverRotation;
				solvedRotations[index + 2] = forearm.solverRotation;
				solvedRotations[index + 3] = hand.solverRotation;
			}

			private float DamperValue(float value, float min, float max, float weight = 1f)
			{
				float num = max - min;
				if (weight < 1f)
				{
					float num2 = max - num * 0.5f;
					float num3 = value - num2;
					num3 *= 0.5f;
					value = num2 + num3;
				}
				value -= min;
				float t = Interp.Float(Mathf.Clamp(value / num, 0f, 1f), InterpolationMode.InOutQuintic);
				return Mathf.Lerp(min, max, t);
			}

			private Vector3 GetBendNormal(Vector3 dir)
			{
				if (bendGoal != null)
				{
					bendDirection = bendGoal.position - bones[1].solverPosition;
				}
				Vector3 vector = bones[0].solverRotation * bones[0].axis;
				Vector3 down = Vector3.down;
				Vector3 toDirection = Quaternion.Inverse(chestRotation) * dir.normalized + Vector3.forward;
				Vector3 vector2 = Quaternion.FromToRotation(down, toDirection) * Vector3.back;
				Vector3 fromDirection = Quaternion.Inverse(chestRotation) * vector;
				toDirection = Quaternion.Inverse(chestRotation) * dir;
				vector2 = Quaternion.FromToRotation(fromDirection, toDirection) * vector2;
				vector2 = chestRotation * vector2;
				vector2 += vector;
				vector2 -= rotation * wristToPalmAxis;
				vector2 -= rotation * palmToThumbAxis * 0.5f;
				if (bendGoalWeight > 0f)
				{
					vector2 = Vector3.Slerp(vector2, bendDirection, bendGoalWeight);
				}
				if (swivelOffset != 0f)
				{
					vector2 = Quaternion.AngleAxis(swivelOffset, -dir) * vector2;
				}
				return Vector3.Cross(vector2, dir);
			}

			private void Visualize(VirtualBone bone1, VirtualBone bone2, VirtualBone bone3, Color color)
			{
				Debug.DrawLine(bone1.solverPosition, bone2.solverPosition, color);
				Debug.DrawLine(bone2.solverPosition, bone3.solverPosition, color);
			}
		}

		[Serializable]
		public abstract class BodyPart
		{
			[HideInInspector]
			public VirtualBone[] bones = new VirtualBone[0];

			protected bool initiated;

			protected Vector3 rootPosition;

			protected Quaternion rootRotation = Quaternion.identity;

			protected int index = -1;

			public float sqrMag { get; private set; }

			public float mag { get; private set; }

			protected abstract void OnRead(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index);

			public abstract void PreSolve();

			public abstract void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations);

			public abstract void ApplyOffsets();

			public abstract void ResetOffsets();

			public void Read(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index)
			{
				this.index = index;
				rootPosition = positions[rootIndex];
				rootRotation = rotations[rootIndex];
				OnRead(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, rootIndex, index);
				mag = VirtualBone.PreSolve(ref bones);
				sqrMag = mag * mag;
				initiated = true;
			}

			public void MovePosition(Vector3 position)
			{
				Vector3 vector = position - bones[0].solverPosition;
				VirtualBone[] array = bones;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].solverPosition += vector;
				}
			}

			public void MoveRotation(Quaternion rotation)
			{
				Quaternion rotation2 = QuaTools.FromToRotation(bones[0].solverRotation, rotation);
				VirtualBone.RotateAroundPoint(bones, 0, bones[0].solverPosition, rotation2);
			}

			public void Translate(Vector3 position, Quaternion rotation)
			{
				MovePosition(position);
				MoveRotation(rotation);
			}

			public void TranslateRoot(Vector3 newRootPos, Quaternion newRootRot)
			{
				Vector3 vector = newRootPos - rootPosition;
				rootPosition = newRootPos;
				VirtualBone[] array = bones;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].solverPosition += vector;
				}
				Quaternion rotation = QuaTools.FromToRotation(rootRotation, newRootRot);
				rootRotation = newRootRot;
				VirtualBone.RotateAroundPoint(bones, 0, newRootPos, rotation);
			}

			public void RotateTo(VirtualBone bone, Quaternion rotation, float weight = 1f)
			{
				if (weight <= 0f)
				{
					return;
				}
				Quaternion quaternion = QuaTools.FromToRotation(bone.solverRotation, rotation);
				if (weight < 1f)
				{
					quaternion = Quaternion.Slerp(Quaternion.identity, quaternion, weight);
				}
				for (int i = 0; i < bones.Length; i++)
				{
					if (bones[i] == bone)
					{
						VirtualBone.RotateAroundPoint(bones, i, bones[i].solverPosition, quaternion);
						break;
					}
				}
			}

			public void Visualize(Color color)
			{
				for (int i = 0; i < bones.Length - 1; i++)
				{
					Debug.DrawLine(bones[i].solverPosition, bones[i + 1].solverPosition, color);
				}
			}

			public void Visualize()
			{
				Visualize(Color.white);
			}
		}

		[Serializable]
		public class Footstep
		{
			public float stepSpeed = 3f;

			public Vector3 characterSpaceOffset;

			public Vector3 position;

			public Quaternion rotation = Quaternion.identity;

			public Quaternion stepToRootRot = Quaternion.identity;

			public bool isSupportLeg;

			public Vector3 stepFrom;

			public Vector3 stepTo;

			public Quaternion stepFromRot = Quaternion.identity;

			public Quaternion stepToRot = Quaternion.identity;

			private Quaternion footRelativeToRoot = Quaternion.identity;

			private float supportLegW;

			private float supportLegWV;

			public bool isStepping
			{
				get
				{
					return stepProgress < 1f;
				}
			}

			public float stepProgress { get; private set; }

			public Footstep(Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation, Vector3 characterSpaceOffset)
			{
				this.characterSpaceOffset = characterSpaceOffset;
				Reset(rootRotation, footPosition, footRotation);
			}

			public void Reset(Quaternion rootRotation, Vector3 footPosition, Quaternion footRotation)
			{
				position = footPosition;
				rotation = footRotation;
				stepFrom = position;
				stepTo = position;
				stepFromRot = rotation;
				stepToRot = rotation;
				stepToRootRot = rootRotation;
				stepProgress = 1f;
				footRelativeToRoot = Quaternion.Inverse(rootRotation) * rotation;
			}

			public void StepTo(Vector3 p, Quaternion rootRotation)
			{
				stepFrom = position;
				stepTo = p;
				stepFromRot = rotation;
				stepToRootRot = rootRotation;
				stepToRot = rootRotation * footRelativeToRoot;
				stepProgress = 0f;
			}

			public void UpdateStepping(Vector3 p, Quaternion rootRotation, float speed)
			{
				stepTo = Vector3.Lerp(stepTo, p, Time.deltaTime * speed);
				stepToRot = Quaternion.Lerp(stepToRot, rootRotation * footRelativeToRoot, Time.deltaTime * speed);
				stepToRootRot = stepToRot * Quaternion.Inverse(footRelativeToRoot);
			}

			public void UpdateStanding(Quaternion rootRotation, float minAngle, float speed)
			{
				if (!(speed <= 0f) && !(minAngle >= 180f))
				{
					Quaternion quaternion = rootRotation * footRelativeToRoot;
					float num = Quaternion.Angle(rotation, quaternion);
					if (num > minAngle)
					{
						rotation = Quaternion.RotateTowards(rotation, quaternion, Mathf.Min(Time.deltaTime * speed * (1f - supportLegW), num - minAngle));
					}
				}
			}

			public void Update(InterpolationMode interpolation, UnityEvent onStep)
			{
				float target = (isSupportLeg ? 1f : 0f);
				supportLegW = Mathf.SmoothDamp(supportLegW, target, ref supportLegWV, 0.2f);
				if (isStepping)
				{
					stepProgress = Mathf.MoveTowards(stepProgress, 1f, Time.deltaTime * stepSpeed);
					if (stepProgress >= 1f)
					{
						onStep.Invoke();
					}
					float t = Interp.Float(stepProgress, interpolation);
					position = Vector3.Lerp(stepFrom, stepTo, t);
					rotation = Quaternion.Lerp(stepFromRot, stepToRot, t);
				}
			}
		}

		[Serializable]
		public class Leg : BodyPart
		{
			[Tooltip("The toe/foot target.")]
			public Transform target;

			[Tooltip("The knee will be bent towards this Transform if 'Bend Goal Weight' > 0.")]
			public Transform bendGoal;

			[Tooltip("Positional weight of the toe/foot target.")]
			[Range(0f, 1f)]
			public float positionWeight;

			[Tooltip("Rotational weight of the toe/foot target.")]
			[Range(0f, 1f)]
			public float rotationWeight;

			[Tooltip("If greater than 0, will bend the knee towards the 'Bend Goal' Transform.")]
			[Range(0f, 1f)]
			public float bendGoalWeight;

			[Tooltip("Angular offset of the knee bending direction.")]
			[Range(-180f, 180f)]
			public float swivelOffset;

			[NonSerialized]
			[HideInInspector]
			public Vector3 IKPosition;

			[NonSerialized]
			[HideInInspector]
			public Quaternion IKRotation = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Vector3 footPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Vector3 heelPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Quaternion footRotationOffset = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public float currentMag;

			private Vector3 footPosition;

			private Quaternion footRotation = Quaternion.identity;

			private Vector3 bendNormal;

			private Quaternion calfRelToThigh = Quaternion.identity;

			public Vector3 position { get; private set; }

			public Quaternion rotation { get; private set; }

			public bool hasToes { get; private set; }

			public VirtualBone thigh
			{
				get
				{
					return bones[0];
				}
			}

			private VirtualBone calf
			{
				get
				{
					return bones[1];
				}
			}

			private VirtualBone foot
			{
				get
				{
					return bones[2];
				}
			}

			private VirtualBone toes
			{
				get
				{
					return bones[3];
				}
			}

			public VirtualBone lastBone
			{
				get
				{
					return bones[bones.Length - 1];
				}
			}

			public Vector3 thighRelativeToPelvis { get; private set; }

			protected override void OnRead(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index)
			{
				Vector3 vector = positions[index];
				Quaternion quaternion = rotations[index];
				Vector3 vector2 = positions[index + 1];
				Quaternion quaternion2 = rotations[index + 1];
				Vector3 iKPosition = positions[index + 2];
				Quaternion iKRotation = rotations[index + 2];
				Vector3 iKPosition2 = positions[index + 3];
				Quaternion iKRotation2 = rotations[index + 3];
				if (!initiated)
				{
					this.hasToes = hasToes;
					bones = new VirtualBone[hasToes ? 4 : 3];
					if (hasToes)
					{
						bones[0] = new VirtualBone(vector, quaternion);
						bones[1] = new VirtualBone(vector2, quaternion2);
						bones[2] = new VirtualBone(iKPosition, iKRotation);
						bones[3] = new VirtualBone(iKPosition2, iKRotation2);
						IKPosition = iKPosition2;
						IKRotation = iKRotation2;
					}
					else
					{
						bones[0] = new VirtualBone(vector, quaternion);
						bones[1] = new VirtualBone(vector2, quaternion2);
						bones[2] = new VirtualBone(iKPosition, iKRotation);
						IKPosition = iKPosition;
						IKRotation = iKRotation;
					}
					rotation = IKRotation;
				}
				if (hasToes)
				{
					bones[0].Read(vector, quaternion);
					bones[1].Read(vector2, quaternion2);
					bones[2].Read(iKPosition, iKRotation);
					bones[3].Read(iKPosition2, iKRotation2);
				}
				else
				{
					bones[0].Read(vector, quaternion);
					bones[1].Read(vector2, quaternion2);
					bones[2].Read(iKPosition, iKRotation);
				}
			}

			public override void PreSolve()
			{
				if (target != null)
				{
					IKPosition = target.position;
					IKRotation = target.rotation;
				}
				footPosition = foot.solverPosition;
				footRotation = foot.solverRotation;
				position = lastBone.solverPosition;
				rotation = lastBone.solverRotation;
				if (rotationWeight > 0f)
				{
					ApplyRotationOffset(QuaTools.FromToRotation(rotation, IKRotation), rotationWeight);
				}
				if (positionWeight > 0f)
				{
					ApplyPositionOffset(IKPosition - position, positionWeight);
				}
				thighRelativeToPelvis = Quaternion.Inverse(rootRotation) * (thigh.solverPosition - rootPosition);
				calfRelToThigh = Quaternion.Inverse(thigh.solverRotation) * calf.solverRotation;
				bendNormal = Vector3.Cross(calf.solverPosition - thigh.solverPosition, foot.solverPosition - calf.solverPosition);
			}

			public override void ApplyOffsets()
			{
				ApplyPositionOffset(footPositionOffset, 1f);
				ApplyRotationOffset(footRotationOffset, 1f);
				Quaternion quaternion = Quaternion.FromToRotation(footPosition - position, footPosition + heelPositionOffset - position);
				footPosition = position + quaternion * (footPosition - position);
				footRotation = quaternion * footRotation;
				float num = 0f;
				if (bendGoal != null && bendGoalWeight > 0f)
				{
					Vector3 vector = Vector3.Cross(bendGoal.position - thigh.solverPosition, position - thigh.solverPosition);
					Vector3 vector2 = Quaternion.Inverse(Quaternion.LookRotation(bendNormal, thigh.solverPosition - foot.solverPosition)) * vector;
					num = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f * bendGoalWeight;
				}
				float num2 = swivelOffset + num;
				if (num2 != 0f)
				{
					bendNormal = Quaternion.AngleAxis(num2, thigh.solverPosition - lastBone.solverPosition) * bendNormal;
					thigh.solverRotation = Quaternion.AngleAxis(0f - num2, thigh.solverRotation * thigh.axis) * thigh.solverRotation;
				}
			}

			private void ApplyPositionOffset(Vector3 offset, float weight)
			{
				if (!(weight <= 0f))
				{
					offset *= weight;
					footPosition += offset;
					position += offset;
				}
			}

			private void ApplyRotationOffset(Quaternion offset, float weight)
			{
				if (!(weight <= 0f))
				{
					if (weight < 1f)
					{
						offset = Quaternion.Lerp(Quaternion.identity, offset, weight);
					}
					footRotation = offset * footRotation;
					rotation = offset * rotation;
					bendNormal = offset * bendNormal;
					footPosition = position + offset * (footPosition - position);
				}
			}

			public void Solve()
			{
				VirtualBone.SolveTrigonometric(bones, 0, 1, 2, footPosition, bendNormal, 1f);
				RotateTo(foot, footRotation);
				if (hasToes)
				{
					Vector3 vector = Vector3.Cross(foot.solverPosition - thigh.solverPosition, toes.solverPosition - foot.solverPosition);
					VirtualBone.SolveTrigonometric(bones, 0, 2, 3, position, vector, 1f);
					Quaternion quaternion = thigh.solverRotation * calfRelToThigh;
					Quaternion quaternion2 = Quaternion.FromToRotation(quaternion * calf.axis, foot.solverPosition - calf.solverPosition);
					calf.solverRotation = quaternion2 * quaternion;
					toes.solverRotation = rotation;
				}
			}

			public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
			{
				solvedRotations[index] = thigh.solverRotation;
				solvedRotations[index + 1] = calf.solverRotation;
				solvedRotations[index + 2] = foot.solverRotation;
				if (hasToes)
				{
					solvedRotations[index + 3] = toes.solverRotation;
				}
			}

			public override void ResetOffsets()
			{
				footPositionOffset = Vector3.zero;
				footRotationOffset = Quaternion.identity;
				heelPositionOffset = Vector3.zero;
			}
		}

		[Serializable]
		public class Locomotion
		{
			[Tooltip("Used for blending in/out of procedural locomotion.")]
			[Range(0f, 1f)]
			public float weight = 1f;

			[Tooltip("Tries to maintain this distance between the legs.")]
			public float footDistance = 0.3f;

			[Tooltip("Makes a step only if step target position is at least this far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past the 'Angle Threshold'.")]
			public float stepThreshold = 0.4f;

			[Tooltip("Makes a step only if step target position is at least 'Step Threshold' far from the current footstep or the foot does not reach the current footstep anymore or footstep angle is past this value.")]
			public float angleThreshold = 60f;

			[Tooltip("Multiplies angle of the center of mass - center of pressure vector. Larger value makes the character step sooner if losing balance.")]
			public float comAngleMlp = 1f;

			[Tooltip("Maximum magnitude of head/hand target velocity used in prediction.")]
			public float maxVelocity = 0.4f;

			[Tooltip("The amount of head/hand target velocity prediction.")]
			public float velocityFactor = 0.4f;

			[Tooltip("How much can a leg be extended before it is forced to step to another position? 1 means fully stretched.")]
			[Range(0.9f, 1f)]
			public float maxLegStretch = 1f;

			[Tooltip("The speed of lerping the root of the character towards the horizontal mid-point of the footsteps.")]
			public float rootSpeed = 20f;

			[Tooltip("The speed of steps.")]
			public float stepSpeed = 3f;

			[Tooltip("The height of the foot by normalized step progress (0 - 1).")]
			public AnimationCurve stepHeight;

			[Tooltip("The height offset of the heel by normalized step progress (0 - 1).")]
			public AnimationCurve heelHeight;

			[Tooltip("Rotates the foot while the leg is not stepping to relax the twist rotation of the leg if ideal rotation is past this angle.")]
			[Range(0f, 180f)]
			public float relaxLegTwistMinAngle = 20f;

			[Tooltip("The speed of rotating the foot while the leg is not stepping to relax the twist rotation of the leg.")]
			public float relaxLegTwistSpeed = 400f;

			[Tooltip("Interpolation mode of the step.")]
			public InterpolationMode stepInterpolation = InterpolationMode.InOutSine;

			[Tooltip("Offset for the approximated center of mass.")]
			public Vector3 offset;

			[HideInInspector]
			public bool blockingEnabled;

			[HideInInspector]
			public LayerMask blockingLayers;

			[HideInInspector]
			public float raycastRadius = 0.2f;

			[HideInInspector]
			public float raycastHeight = 0.2f;

			[Tooltip("Called when the left foot has finished a step.")]
			public UnityEvent onLeftFootstep = new UnityEvent();

			[Tooltip("Called when the right foot has finished a step")]
			public UnityEvent onRightFootstep = new UnityEvent();

			private Footstep[] footsteps = new Footstep[0];

			private Vector3 lastComPosition;

			private Vector3 comVelocity;

			private int leftFootIndex;

			private int rightFootIndex;

			public Vector3 centerOfMass { get; private set; }

			public Vector3 leftFootstepPosition
			{
				get
				{
					return footsteps[0].position;
				}
			}

			public Vector3 rightFootstepPosition
			{
				get
				{
					return footsteps[1].position;
				}
			}

			public Quaternion leftFootstepRotation
			{
				get
				{
					return footsteps[0].rotation;
				}
			}

			public Quaternion rightFootstepRotation
			{
				get
				{
					return footsteps[1].rotation;
				}
			}

			public void Initiate(Vector3[] positions, Quaternion[] rotations, bool hasToes)
			{
				leftFootIndex = (hasToes ? 17 : 16);
				rightFootIndex = (hasToes ? 21 : 20);
				footsteps = new Footstep[2]
				{
					new Footstep(rotations[0], positions[leftFootIndex], rotations[leftFootIndex], footDistance * Vector3.left),
					new Footstep(rotations[0], positions[rightFootIndex], rotations[rightFootIndex], footDistance * Vector3.right)
				};
			}

			public void Reset(Vector3[] positions, Quaternion[] rotations)
			{
				lastComPosition = Vector3.Lerp(positions[1], positions[5], 0.25f) + rotations[0] * offset;
				comVelocity = Vector3.zero;
				footsteps[0].Reset(rotations[0], positions[leftFootIndex], rotations[leftFootIndex]);
				footsteps[1].Reset(rotations[0], positions[rightFootIndex], rotations[rightFootIndex]);
			}

			public void AddDeltaRotation(Quaternion delta, Vector3 pivot)
			{
				Vector3 vector = lastComPosition - pivot;
				lastComPosition = pivot + delta * vector;
				Footstep[] array = footsteps;
				foreach (Footstep footstep in array)
				{
					footstep.rotation = delta * footstep.rotation;
					footstep.stepFromRot = delta * footstep.stepFromRot;
					footstep.stepToRot = delta * footstep.stepToRot;
					footstep.stepToRootRot = delta * footstep.stepToRootRot;
					Vector3 vector2 = footstep.position - pivot;
					footstep.position = pivot + delta * vector2;
					Vector3 vector3 = footstep.stepFrom - pivot;
					footstep.stepFrom = pivot + delta * vector3;
					Vector3 vector4 = footstep.stepTo - pivot;
					footstep.stepTo = pivot + delta * vector4;
				}
			}

			public void AddDeltaPosition(Vector3 delta)
			{
				lastComPosition += delta;
				Footstep[] array = footsteps;
				foreach (Footstep obj in array)
				{
					obj.position += delta;
					obj.stepFrom += delta;
					obj.stepTo += delta;
				}
			}

			public void Solve(VirtualBone rootBone, Spine spine, Leg leftLeg, Leg rightLeg, Arm leftArm, Arm rightArm, int supportLegIndex, out Vector3 leftFootPosition, out Vector3 rightFootPosition, out Quaternion leftFootRotation, out Quaternion rightFootRotation, out float leftFootOffset, out float rightFootOffset, out float leftHeelOffset, out float rightHeelOffset)
			{
				if (weight <= 0f)
				{
					leftFootPosition = Vector3.zero;
					rightFootPosition = Vector3.zero;
					leftFootRotation = Quaternion.identity;
					rightFootRotation = Quaternion.identity;
					leftFootOffset = 0f;
					rightFootOffset = 0f;
					leftHeelOffset = 0f;
					rightHeelOffset = 0f;
					return;
				}
				Vector3 vector = rootBone.solverRotation * Vector3.up;
				Vector3 vector2 = spine.pelvis.solverPosition + spine.pelvis.solverRotation * leftLeg.thighRelativeToPelvis;
				Vector3 vector3 = spine.pelvis.solverPosition + spine.pelvis.solverRotation * rightLeg.thighRelativeToPelvis;
				footsteps[0].characterSpaceOffset = footDistance * Vector3.left;
				footsteps[1].characterSpaceOffset = footDistance * Vector3.right;
				Vector3 faceDirection = spine.faceDirection;
				Vector3 vector4 = V3Tools.ExtractVertical(faceDirection, vector, 1f);
				Quaternion quaternion = Quaternion.LookRotation(faceDirection - vector4, vector);
				float num = 1f;
				float num2 = 1f;
				float num3 = 0.2f;
				float num4 = num + num2 + 2f * num3;
				centerOfMass = Vector3.zero;
				centerOfMass += spine.pelvis.solverPosition * num;
				centerOfMass += spine.head.solverPosition * num2;
				centerOfMass += leftArm.position * num3;
				centerOfMass += rightArm.position * num3;
				centerOfMass /= num4;
				centerOfMass += rootBone.solverRotation * offset;
				comVelocity = ((Time.deltaTime > 0f) ? ((centerOfMass - lastComPosition) / Time.deltaTime) : Vector3.zero);
				lastComPosition = centerOfMass;
				comVelocity = Vector3.ClampMagnitude(comVelocity, maxVelocity) * velocityFactor;
				Vector3 vector5 = centerOfMass + comVelocity;
				Vector3 vector6 = V3Tools.PointToPlane(spine.pelvis.solverPosition, rootBone.solverPosition, vector);
				Vector3 vector7 = V3Tools.PointToPlane(vector5, rootBone.solverPosition, vector);
				Vector3 vector8 = Vector3.Lerp(footsteps[0].position, footsteps[1].position, 0.5f);
				float num5 = Vector3.Angle(vector5 - vector8, rootBone.solverRotation * Vector3.up) * comAngleMlp;
				for (int i = 0; i < footsteps.Length; i++)
				{
					footsteps[i].isSupportLeg = supportLegIndex == i;
				}
				for (int j = 0; j < footsteps.Length; j++)
				{
					if (footsteps[j].isStepping)
					{
						Vector3 vector9 = vector7 + rootBone.solverRotation * footsteps[j].characterSpaceOffset;
						if (!StepBlocked(footsteps[j].stepFrom, vector9, rootBone.solverPosition))
						{
							footsteps[j].UpdateStepping(vector9, quaternion, 10f);
						}
					}
					else
					{
						footsteps[j].UpdateStanding(quaternion, relaxLegTwistMinAngle, relaxLegTwistSpeed);
					}
				}
				if (CanStep())
				{
					int num6 = -1;
					float num7 = float.NegativeInfinity;
					for (int k = 0; k < footsteps.Length; k++)
					{
						if (footsteps[k].isStepping)
						{
							continue;
						}
						Vector3 vector10 = vector7 + rootBone.solverRotation * footsteps[k].characterSpaceOffset;
						float num8 = ((k == 0) ? leftLeg.mag : rightLeg.mag);
						Vector3 b = ((k == 0) ? vector2 : vector3);
						float num9 = Vector3.Distance(footsteps[k].position, b);
						bool flag = false;
						if (num9 >= num8 * maxLegStretch)
						{
							vector10 = vector6 + rootBone.solverRotation * footsteps[k].characterSpaceOffset;
							flag = true;
						}
						bool flag2 = false;
						for (int l = 0; l < footsteps.Length; l++)
						{
							if (l != k && !flag)
							{
								if (!(Vector3.Distance(footsteps[k].position, footsteps[l].position) < 0.25f) || !((footsteps[k].position - vector10).sqrMagnitude < (footsteps[l].position - vector10).sqrMagnitude))
								{
									flag2 = GetLineSphereCollision(footsteps[k].position, vector10, footsteps[l].position, 0.25f);
								}
								if (flag2)
								{
									break;
								}
							}
						}
						float num10 = Quaternion.Angle(quaternion, footsteps[k].stepToRootRot);
						if (flag2 && !(num10 > angleThreshold))
						{
							continue;
						}
						float num11 = Vector3.Distance(footsteps[k].position, vector10);
						float num12 = Mathf.Lerp(stepThreshold, stepThreshold * 0.1f, num5 * 0.015f);
						if (flag)
						{
							num12 *= 0.5f;
						}
						if (k == 0)
						{
							num12 *= 0.9f;
						}
						if (!StepBlocked(footsteps[k].position, vector10, rootBone.solverPosition) && (num11 > num12 || num10 > angleThreshold))
						{
							float num13 = 0f;
							num13 -= num11;
							if (num13 > num7)
							{
								num6 = k;
								num7 = num13;
							}
						}
					}
					if (num6 != -1)
					{
						Vector3 p = vector7 + rootBone.solverRotation * footsteps[num6].characterSpaceOffset;
						footsteps[num6].stepSpeed = UnityEngine.Random.Range(stepSpeed, stepSpeed * 1.5f);
						footsteps[num6].StepTo(p, quaternion);
					}
				}
				footsteps[0].Update(stepInterpolation, onLeftFootstep);
				footsteps[1].Update(stepInterpolation, onRightFootstep);
				leftFootPosition = footsteps[0].position;
				rightFootPosition = footsteps[1].position;
				leftFootPosition = V3Tools.PointToPlane(leftFootPosition, leftLeg.lastBone.readPosition, vector);
				rightFootPosition = V3Tools.PointToPlane(rightFootPosition, rightLeg.lastBone.readPosition, vector);
				leftFootOffset = stepHeight.Evaluate(footsteps[0].stepProgress);
				rightFootOffset = stepHeight.Evaluate(footsteps[1].stepProgress);
				leftHeelOffset = heelHeight.Evaluate(footsteps[0].stepProgress);
				rightHeelOffset = heelHeight.Evaluate(footsteps[1].stepProgress);
				leftFootRotation = footsteps[0].rotation;
				rightFootRotation = footsteps[1].rotation;
			}

			private bool StepBlocked(Vector3 fromPosition, Vector3 toPosition, Vector3 rootPosition)
			{
				if ((int)blockingLayers == -1 || !blockingEnabled)
				{
					return false;
				}
				Vector3 vector = fromPosition;
				vector.y = rootPosition.y + raycastHeight + raycastRadius;
				Vector3 direction = toPosition - vector;
				direction.y = 0f;
				RaycastHit hitInfo;
				if (raycastRadius <= 0f)
				{
					return Physics.Raycast(vector, direction, out hitInfo, direction.magnitude, blockingLayers);
				}
				return Physics.SphereCast(vector, raycastRadius, direction, out hitInfo, direction.magnitude, blockingLayers);
			}

			private bool CanStep()
			{
				Footstep[] array = footsteps;
				foreach (Footstep footstep in array)
				{
					if (footstep.isStepping && footstep.stepProgress < 0.8f)
					{
						return false;
					}
				}
				return true;
			}

			private static bool GetLineSphereCollision(Vector3 lineStart, Vector3 lineEnd, Vector3 sphereCenter, float sphereRadius)
			{
				Vector3 forward = lineEnd - lineStart;
				Vector3 vector = sphereCenter - lineStart;
				float num = vector.magnitude - sphereRadius;
				if (num > forward.magnitude)
				{
					return false;
				}
				Vector3 vector2 = Quaternion.Inverse(Quaternion.LookRotation(forward, vector)) * vector;
				if (vector2.z < 0f)
				{
					return num < 0f;
				}
				return vector2.y - sphereRadius < 0f;
			}
		}

		[Serializable]
		public class Spine : BodyPart
		{
			[Tooltip("The head target.")]
			public Transform headTarget;

			[Tooltip("The pelvis target, useful with seated rigs.")]
			public Transform pelvisTarget;

			[Tooltip("Positional weight of the head target.")]
			[Range(0f, 1f)]
			public float positionWeight = 1f;

			[Tooltip("Rotational weight of the head target.")]
			[Range(0f, 1f)]
			public float rotationWeight = 1f;

			[Tooltip("Positional weight of the pelvis target.")]
			[Range(0f, 1f)]
			public float pelvisPositionWeight;

			[Tooltip("Rotational weight of the pelvis target.")]
			[Range(0f, 1f)]
			public float pelvisRotationWeight;

			[Tooltip("If 'Chest Goal Weight' is greater than 0, the chest will be turned towards this Transform.")]
			public Transform chestGoal;

			[Tooltip("Rotational weight of the chest target.")]
			[Range(0f, 1f)]
			public float chestGoalWeight;

			[Tooltip("Minimum height of the head from the root of the character.")]
			public float minHeadHeight = 0.8f;

			[Tooltip("Determines how much the body will follow the position of the head.")]
			[Range(0f, 1f)]
			public float bodyPosStiffness = 0.55f;

			[Tooltip("Determines how much the body will follow the rotation of the head.")]
			[Range(0f, 1f)]
			public float bodyRotStiffness = 0.1f;

			[Tooltip("Determines how much the chest will rotate to the rotation of the head.")]
			[FormerlySerializedAs("chestRotationWeight")]
			[Range(0f, 1f)]
			public float neckStiffness = 0.2f;

			[Tooltip("The amount of rotation applied to the chest based on hand positions.")]
			[Range(0f, 1f)]
			public float rotateChestByHands = 1f;

			[Tooltip("Clamps chest rotation.")]
			[Range(0f, 1f)]
			public float chestClampWeight = 0.5f;

			[Tooltip("Clamps head rotation.")]
			[Range(0f, 1f)]
			public float headClampWeight = 0.6f;

			[Tooltip("Moves the body horizontally along -character.forward axis by that value when the player is crouching.")]
			public float moveBodyBackWhenCrouching = 0.5f;

			[Tooltip("How much will the pelvis maintain it's animated position?")]
			[Range(0f, 1f)]
			public float maintainPelvisPosition = 0.2f;

			[Tooltip("Will automatically rotate the root of the character if the head target has turned past this angle.")]
			[Range(0f, 180f)]
			public float maxRootAngle = 25f;

			[NonSerialized]
			[HideInInspector]
			public Vector3 IKPositionHead;

			[NonSerialized]
			[HideInInspector]
			public Quaternion IKRotationHead = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Vector3 IKPositionPelvis;

			[NonSerialized]
			[HideInInspector]
			public Quaternion IKRotationPelvis = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Vector3 goalPositionChest;

			[NonSerialized]
			[HideInInspector]
			public Vector3 pelvisPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Vector3 chestPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Vector3 headPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Quaternion pelvisRotationOffset = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Quaternion chestRotationOffset = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Quaternion headRotationOffset = Quaternion.identity;

			[NonSerialized]
			[HideInInspector]
			public Vector3 faceDirection;

			[NonSerialized]
			[HideInInspector]
			public Vector3 locomotionHeadPositionOffset;

			[NonSerialized]
			[HideInInspector]
			public Vector3 headPosition;

			private Quaternion headRotation = Quaternion.identity;

			private Quaternion anchorRelativeToHead = Quaternion.identity;

			private Quaternion pelvisRelativeRotation = Quaternion.identity;

			private Quaternion chestRelativeRotation = Quaternion.identity;

			private Vector3 headDeltaPosition;

			private Quaternion pelvisDeltaRotation = Quaternion.identity;

			private Quaternion chestTargetRotation = Quaternion.identity;

			private int pelvisIndex;

			private int spineIndex = 1;

			private int chestIndex = -1;

			private int neckIndex = -1;

			private int headIndex = -1;

			private float length;

			private bool hasChest;

			private bool hasNeck;

			private float headHeight;

			private float sizeMlp;

			private Vector3 chestForward;

			public VirtualBone pelvis
			{
				get
				{
					return bones[pelvisIndex];
				}
			}

			public VirtualBone firstSpineBone
			{
				get
				{
					return bones[spineIndex];
				}
			}

			public VirtualBone chest
			{
				get
				{
					if (hasChest)
					{
						return bones[chestIndex];
					}
					return bones[spineIndex];
				}
			}

			private VirtualBone neck
			{
				get
				{
					return bones[neckIndex];
				}
			}

			public VirtualBone head
			{
				get
				{
					return bones[headIndex];
				}
			}

			public Quaternion anchorRotation { get; private set; }

			protected override void OnRead(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, int rootIndex, int index)
			{
				Vector3 vector = positions[index];
				Quaternion quaternion = rotations[index];
				Vector3 vector2 = positions[index + 1];
				Quaternion quaternion2 = rotations[index + 1];
				Vector3 vector3 = positions[index + 2];
				Quaternion quaternion3 = rotations[index + 2];
				Vector3 position = positions[index + 3];
				Quaternion rotation = rotations[index + 3];
				Vector3 vector4 = positions[index + 4];
				Quaternion quaternion4 = rotations[index + 4];
				if (!hasChest)
				{
					vector3 = vector2;
					quaternion3 = quaternion2;
				}
				if (!initiated)
				{
					this.hasChest = hasChest;
					this.hasNeck = hasNeck;
					headHeight = V3Tools.ExtractVertical(vector4 - positions[0], rotations[0] * Vector3.up, 1f).magnitude;
					int num = 3;
					if (hasChest)
					{
						num++;
					}
					if (hasNeck)
					{
						num++;
					}
					bones = new VirtualBone[num];
					chestIndex = ((!hasChest) ? 1 : 2);
					neckIndex = 1;
					if (hasChest)
					{
						neckIndex++;
					}
					if (hasNeck)
					{
						neckIndex++;
					}
					headIndex = 2;
					if (hasChest)
					{
						headIndex++;
					}
					if (hasNeck)
					{
						headIndex++;
					}
					bones[0] = new VirtualBone(vector, quaternion);
					bones[1] = new VirtualBone(vector2, quaternion2);
					if (hasChest)
					{
						bones[chestIndex] = new VirtualBone(vector3, quaternion3);
					}
					if (hasNeck)
					{
						bones[neckIndex] = new VirtualBone(position, rotation);
					}
					bones[headIndex] = new VirtualBone(vector4, quaternion4);
					pelvisRotationOffset = Quaternion.identity;
					chestRotationOffset = Quaternion.identity;
					headRotationOffset = Quaternion.identity;
					anchorRelativeToHead = Quaternion.Inverse(quaternion4) * rotations[0];
					pelvisRelativeRotation = Quaternion.Inverse(quaternion4) * quaternion;
					chestRelativeRotation = Quaternion.Inverse(quaternion4) * quaternion3;
					chestForward = Quaternion.Inverse(quaternion3) * (rotations[0] * Vector3.forward);
					faceDirection = rotations[0] * Vector3.forward;
					IKPositionHead = vector4;
					IKRotationHead = quaternion4;
					IKPositionPelvis = vector;
					IKRotationPelvis = quaternion;
					goalPositionChest = vector3 + rotations[0] * Vector3.forward;
				}
				bones[0].Read(vector, quaternion);
				bones[1].Read(vector2, quaternion2);
				if (hasChest)
				{
					bones[chestIndex].Read(vector3, quaternion3);
				}
				if (hasNeck)
				{
					bones[neckIndex].Read(position, rotation);
				}
				bones[headIndex].Read(vector4, quaternion4);
				float num2 = Vector3.Distance(vector, vector4);
				sizeMlp = num2 / 0.7f;
			}

			public override void PreSolve()
			{
				if (headTarget != null)
				{
					IKPositionHead = headTarget.position;
					IKRotationHead = headTarget.rotation;
				}
				if (chestGoal != null)
				{
					goalPositionChest = chestGoal.position;
				}
				if (pelvisTarget != null)
				{
					IKPositionPelvis = pelvisTarget.position;
					IKRotationPelvis = pelvisTarget.rotation;
				}
				headPosition = V3Tools.Lerp(head.solverPosition, IKPositionHead, positionWeight);
				headRotation = QuaTools.Lerp(head.solverRotation, IKRotationHead, rotationWeight);
			}

			public override void ApplyOffsets()
			{
				headPosition += headPositionOffset;
				Vector3 vector = rootRotation * Vector3.up;
				if (vector == Vector3.up)
				{
					headPosition.y = Math.Max(rootPosition.y + minHeadHeight, headPosition.y);
				}
				else
				{
					Vector3 vector2 = headPosition - rootPosition;
					Vector3 vector3 = V3Tools.ExtractHorizontal(vector2, vector, 1f);
					Vector3 vector4 = vector2 - vector3;
					if (Vector3.Dot(vector4, vector) > 0f)
					{
						if (vector4.magnitude < minHeadHeight)
						{
							vector4 = vector4.normalized * minHeadHeight;
						}
					}
					else
					{
						vector4 = -vector4.normalized * minHeadHeight;
					}
					headPosition = rootPosition + vector3 + vector4;
				}
				headRotation = headRotationOffset * headRotation;
				headDeltaPosition = headPosition - head.solverPosition;
				pelvisDeltaRotation = QuaTools.FromToRotation(pelvis.solverRotation, headRotation * pelvisRelativeRotation);
				anchorRotation = headRotation * anchorRelativeToHead;
			}

			private void CalculateChestTargetRotation(VirtualBone rootBone, Arm[] arms)
			{
				chestTargetRotation = headRotation * chestRelativeRotation;
				AdjustChestByHands(ref chestTargetRotation, arms);
				faceDirection = Vector3.Cross(anchorRotation * Vector3.right, rootBone.readRotation * Vector3.up) + anchorRotation * Vector3.forward;
			}

			public void Solve(VirtualBone rootBone, Leg[] legs, Arm[] arms)
			{
				CalculateChestTargetRotation(rootBone, arms);
				if (maxRootAngle < 180f)
				{
					Vector3 vector = Quaternion.Inverse(rootBone.solverRotation) * faceDirection;
					float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
					float angle = 0f;
					float num2 = maxRootAngle;
					if (num > num2)
					{
						angle = num - num2;
					}
					if (num < 0f - num2)
					{
						angle = num + num2;
					}
					rootBone.solverRotation = Quaternion.AngleAxis(angle, rootBone.readRotation * Vector3.up) * rootBone.solverRotation;
				}
				Vector3 solverPosition = pelvis.solverPosition;
				Vector3 rootUp = rootBone.solverRotation * Vector3.up;
				TranslatePelvis(legs, headDeltaPosition, pelvisDeltaRotation);
				FABRIKPass(solverPosition, rootUp);
				Bend(bones, pelvisIndex, chestIndex, chestTargetRotation, chestRotationOffset, chestClampWeight, false, neckStiffness);
				if (chestGoalWeight > 0f)
				{
					Quaternion targetRotation = Quaternion.FromToRotation(bones[chestIndex].solverRotation * chestForward, goalPositionChest - bones[chestIndex].solverPosition) * bones[chestIndex].solverRotation;
					Bend(bones, pelvisIndex, chestIndex, targetRotation, chestRotationOffset, chestClampWeight, false, chestGoalWeight);
				}
				InverseTranslateToHead(legs, false, false, Vector3.zero, 1f);
				FABRIKPass(solverPosition, rootUp);
				Bend(bones, neckIndex, headIndex, headRotation, headClampWeight, true, 1f);
				SolvePelvis();
			}

			private void FABRIKPass(Vector3 animatedPelvisPos, Vector3 rootUp)
			{
				Vector3 startPosition = Vector3.Lerp(pelvis.solverPosition, animatedPelvisPos, maintainPelvisPosition) + pelvisPositionOffset - chestPositionOffset;
				Vector3 targetPosition = headPosition - chestPositionOffset;
				Vector3 startOffset = rootUp * (bones[bones.Length - 1].solverPosition - bones[0].solverPosition).magnitude;
				VirtualBone.SolveFABRIK(bones, startPosition, targetPosition, 1f, 1f, 1, base.mag, startOffset);
			}

			private void SolvePelvis()
			{
				if (pelvisPositionWeight > 0f)
				{
					Quaternion solverRotation = head.solverRotation;
					Vector3 vector = (IKPositionPelvis + pelvisPositionOffset - pelvis.solverPosition) * pelvisPositionWeight;
					VirtualBone[] array = bones;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].solverPosition += vector;
					}
					Vector3 bendNormal = anchorRotation * Vector3.right;
					if (hasChest && hasNeck)
					{
						VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
						VirtualBone.SolveTrigonometric(bones, spineIndex, chestIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.6f);
						VirtualBone.SolveTrigonometric(bones, chestIndex, neckIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
					}
					else if (hasChest && !hasNeck)
					{
						VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.75f);
						VirtualBone.SolveTrigonometric(bones, spineIndex, chestIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
					}
					else if (!hasChest && hasNeck)
					{
						VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 0.75f);
						VirtualBone.SolveTrigonometric(bones, spineIndex, neckIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight * 1f);
					}
					else if (!hasNeck && !hasChest)
					{
						VirtualBone.SolveTrigonometric(bones, pelvisIndex, spineIndex, headIndex, headPosition, bendNormal, pelvisPositionWeight);
					}
					head.solverRotation = solverRotation;
				}
			}

			public override void Write(ref Vector3[] solvedPositions, ref Quaternion[] solvedRotations)
			{
				solvedPositions[index] = bones[0].solverPosition;
				solvedRotations[index] = bones[0].solverRotation;
				solvedRotations[index + 1] = bones[1].solverRotation;
				if (hasChest)
				{
					solvedRotations[index + 2] = bones[chestIndex].solverRotation;
				}
				if (hasNeck)
				{
					solvedRotations[index + 3] = bones[neckIndex].solverRotation;
				}
				solvedRotations[index + 4] = bones[headIndex].solverRotation;
			}

			public override void ResetOffsets()
			{
				pelvisPositionOffset = Vector3.zero;
				chestPositionOffset = Vector3.zero;
				headPositionOffset = locomotionHeadPositionOffset;
				pelvisRotationOffset = Quaternion.identity;
				chestRotationOffset = Quaternion.identity;
				headRotationOffset = Quaternion.identity;
			}

			private void AdjustChestByHands(ref Quaternion chestTargetRotation, Arm[] arms)
			{
				Quaternion quaternion = Quaternion.Inverse(anchorRotation);
				Vector3 vector = quaternion * (arms[0].position - headPosition) / sizeMlp;
				Vector3 vector2 = quaternion * (arms[1].position - headPosition) / sizeMlp;
				Vector3 forward = Vector3.forward;
				forward.x += vector.x * Mathf.Abs(vector.x);
				forward.x += vector.z * Mathf.Abs(vector.z);
				forward.x += vector2.x * Mathf.Abs(vector2.x);
				forward.x -= vector2.z * Mathf.Abs(vector2.z);
				forward.x *= 5f * rotateChestByHands;
				Quaternion quaternion2 = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, rootRotation * Vector3.up);
				chestTargetRotation = quaternion2 * chestTargetRotation;
				Vector3 up = Vector3.up;
				up.x += vector.y;
				up.x -= vector2.y;
				up.x *= 0.5f * rotateChestByHands;
				quaternion2 = Quaternion.AngleAxis(Mathf.Atan2(up.x, up.y) * 57.29578f, rootRotation * Vector3.back);
				chestTargetRotation = quaternion2 * chestTargetRotation;
			}

			public void InverseTranslateToHead(Leg[] legs, bool limited, bool useCurrentLegMag, Vector3 offset, float w)
			{
				Vector3 vector = (headPosition + offset - head.solverPosition) * w * (1f - pelvisPositionWeight);
				Vector3 vector2 = pelvis.solverPosition + vector;
				MovePosition(limited ? LimitPelvisPosition(legs, vector2, useCurrentLegMag) : vector2);
			}

			private void TranslatePelvis(Leg[] legs, Vector3 deltaPosition, Quaternion deltaRotation)
			{
				Vector3 solverPosition = head.solverPosition;
				deltaRotation = QuaTools.ClampRotation(deltaRotation, chestClampWeight, 2);
				Quaternion a = Quaternion.Slerp(Quaternion.identity, deltaRotation, bodyRotStiffness);
				a = Quaternion.Slerp(a, QuaTools.FromToRotation(pelvis.solverRotation, IKRotationPelvis), pelvisRotationWeight);
				VirtualBone.RotateAroundPoint(bones, 0, pelvis.solverPosition, pelvisRotationOffset * a);
				deltaPosition -= head.solverPosition - solverPosition;
				Vector3 vector = rootRotation * Vector3.forward;
				float num = V3Tools.ExtractVertical(deltaPosition, rootRotation * Vector3.up, 1f).magnitude * (0f - moveBodyBackWhenCrouching) * headHeight;
				deltaPosition += vector * num;
				MovePosition(LimitPelvisPosition(legs, pelvis.solverPosition + deltaPosition * bodyPosStiffness, false));
			}

			private Vector3 LimitPelvisPosition(Leg[] legs, Vector3 pelvisPosition, bool useCurrentLegMag, int it = 2)
			{
				if (useCurrentLegMag)
				{
					Leg[] array = legs;
					foreach (Leg leg in array)
					{
						leg.currentMag = Vector3.Distance(leg.thigh.solverPosition, leg.lastBone.solverPosition);
					}
				}
				for (int j = 0; j < it; j++)
				{
					Leg[] array = legs;
					foreach (Leg leg2 in array)
					{
						Vector3 vector = pelvisPosition - pelvis.solverPosition;
						Vector3 vector2 = leg2.thigh.solverPosition + vector;
						Vector3 vector3 = vector2 - leg2.position;
						float maxLength = (useCurrentLegMag ? leg2.currentMag : leg2.mag);
						Vector3 vector4 = leg2.position + Vector3.ClampMagnitude(vector3, maxLength);
						pelvisPosition += vector4 - vector2;
					}
				}
				return pelvisPosition;
			}

			private void Bend(VirtualBone[] bones, int firstIndex, int lastIndex, Quaternion targetRotation, float clampWeight, bool uniformWeight, float w)
			{
				if (w <= 0f || bones.Length == 0)
				{
					return;
				}
				int num = lastIndex + 1 - firstIndex;
				if (num < 1)
				{
					return;
				}
				Quaternion rotation = QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation);
				rotation = QuaTools.ClampRotation(rotation, clampWeight, 2);
				float num2 = (uniformWeight ? (1f / (float)num) : 0f);
				for (int i = firstIndex; i < lastIndex + 1; i++)
				{
					if (!uniformWeight)
					{
						num2 = Mathf.Clamp((i - firstIndex + 1) / num, 0f, 1f);
					}
					VirtualBone.RotateAroundPoint(bones, i, bones[i].solverPosition, Quaternion.Slerp(Quaternion.identity, rotation, num2 * w));
				}
			}

			private void Bend(VirtualBone[] bones, int firstIndex, int lastIndex, Quaternion targetRotation, Quaternion rotationOffset, float clampWeight, bool uniformWeight, float w)
			{
				if (w <= 0f || bones.Length == 0)
				{
					return;
				}
				int num = lastIndex + 1 - firstIndex;
				if (num < 1)
				{
					return;
				}
				Quaternion rotation = QuaTools.FromToRotation(bones[lastIndex].solverRotation, targetRotation);
				rotation = QuaTools.ClampRotation(rotation, clampWeight, 2);
				float num2 = (uniformWeight ? (1f / (float)num) : 0f);
				for (int i = firstIndex; i < lastIndex + 1; i++)
				{
					if (!uniformWeight)
					{
						num2 = Mathf.Clamp((i - firstIndex + 1) / num, 0f, 1f);
					}
					VirtualBone.RotateAroundPoint(bones, i, bones[i].solverPosition, Quaternion.Slerp(Quaternion.Slerp(Quaternion.identity, rotationOffset, num2), rotation, num2 * w));
				}
			}
		}

		[Serializable]
		public enum PositionOffset
		{
			Pelvis = 0,
			Chest = 1,
			Head = 2,
			LeftHand = 3,
			RightHand = 4,
			LeftFoot = 5,
			RightFoot = 6,
			LeftHeel = 7,
			RightHeel = 8
		}

		[Serializable]
		public enum RotationOffset
		{
			Pelvis = 0,
			Chest = 1,
			Head = 2
		}

		[Serializable]
		public class VirtualBone
		{
			public Vector3 readPosition;

			public Quaternion readRotation;

			public Vector3 solverPosition;

			public Quaternion solverRotation;

			public float length;

			public float sqrMag;

			public Vector3 axis;

			public VirtualBone(Vector3 position, Quaternion rotation)
			{
				Read(position, rotation);
			}

			public void Read(Vector3 position, Quaternion rotation)
			{
				readPosition = position;
				readRotation = rotation;
				solverPosition = position;
				solverRotation = rotation;
			}

			public static void SwingRotation(VirtualBone[] bones, int index, Vector3 swingTarget, float weight = 1f)
			{
				if (!(weight <= 0f))
				{
					Quaternion quaternion = Quaternion.FromToRotation(bones[index].solverRotation * bones[index].axis, swingTarget - bones[index].solverPosition);
					if (weight < 1f)
					{
						quaternion = Quaternion.Lerp(Quaternion.identity, quaternion, weight);
					}
					for (int i = index; i < bones.Length; i++)
					{
						bones[i].solverRotation = quaternion * bones[i].solverRotation;
					}
				}
			}

			public static float PreSolve(ref VirtualBone[] bones)
			{
				float num = 0f;
				for (int i = 0; i < bones.Length; i++)
				{
					if (i < bones.Length - 1)
					{
						bones[i].sqrMag = (bones[i + 1].solverPosition - bones[i].solverPosition).sqrMagnitude;
						bones[i].length = Mathf.Sqrt(bones[i].sqrMag);
						num += bones[i].length;
						bones[i].axis = Quaternion.Inverse(bones[i].solverRotation) * (bones[i + 1].solverPosition - bones[i].solverPosition);
					}
					else
					{
						bones[i].sqrMag = 0f;
						bones[i].length = 0f;
					}
				}
				return num;
			}

			public static void RotateAroundPoint(VirtualBone[] bones, int index, Vector3 point, Quaternion rotation)
			{
				for (int i = index; i < bones.Length; i++)
				{
					if (bones[i] != null)
					{
						Vector3 vector = bones[i].solverPosition - point;
						bones[i].solverPosition = point + rotation * vector;
						bones[i].solverRotation = rotation * bones[i].solverRotation;
					}
				}
			}

			public static void RotateBy(VirtualBone[] bones, int index, Quaternion rotation)
			{
				for (int i = index; i < bones.Length; i++)
				{
					if (bones[i] != null)
					{
						Vector3 vector = bones[i].solverPosition - bones[index].solverPosition;
						bones[i].solverPosition = bones[index].solverPosition + rotation * vector;
						bones[i].solverRotation = rotation * bones[i].solverRotation;
					}
				}
			}

			public static void RotateBy(VirtualBone[] bones, Quaternion rotation)
			{
				for (int i = 0; i < bones.Length; i++)
				{
					if (bones[i] != null)
					{
						if (i > 0)
						{
							Vector3 vector = bones[i].solverPosition - bones[0].solverPosition;
							bones[i].solverPosition = bones[0].solverPosition + rotation * vector;
						}
						bones[i].solverRotation = rotation * bones[i].solverRotation;
					}
				}
			}

			public static void RotateTo(VirtualBone[] bones, int index, Quaternion rotation)
			{
				Quaternion rotation2 = QuaTools.FromToRotation(bones[index].solverRotation, rotation);
				RotateAroundPoint(bones, index, bones[index].solverPosition, rotation2);
			}

			public static void SolveTrigonometric(VirtualBone[] bones, int first, int second, int third, Vector3 targetPosition, Vector3 bendNormal, float weight)
			{
				if (weight <= 0f)
				{
					return;
				}
				targetPosition = Vector3.Lerp(bones[third].solverPosition, targetPosition, weight);
				Vector3 vector = targetPosition - bones[first].solverPosition;
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude != 0f)
				{
					float directionMag = Mathf.Sqrt(sqrMagnitude);
					float sqrMagnitude2 = (bones[second].solverPosition - bones[first].solverPosition).sqrMagnitude;
					float sqrMagnitude3 = (bones[third].solverPosition - bones[second].solverPosition).sqrMagnitude;
					Vector3 bendDirection = Vector3.Cross(vector, bendNormal);
					Vector3 directionToBendPoint = GetDirectionToBendPoint(vector, directionMag, bendDirection, sqrMagnitude2, sqrMagnitude3);
					Quaternion quaternion = Quaternion.FromToRotation(bones[second].solverPosition - bones[first].solverPosition, directionToBendPoint);
					if (weight < 1f)
					{
						quaternion = Quaternion.Lerp(Quaternion.identity, quaternion, weight);
					}
					RotateAroundPoint(bones, first, bones[first].solverPosition, quaternion);
					Quaternion quaternion2 = Quaternion.FromToRotation(bones[third].solverPosition - bones[second].solverPosition, targetPosition - bones[second].solverPosition);
					if (weight < 1f)
					{
						quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, weight);
					}
					RotateAroundPoint(bones, second, bones[second].solverPosition, quaternion2);
				}
			}

			private static Vector3 GetDirectionToBendPoint(Vector3 direction, float directionMag, Vector3 bendDirection, float sqrMag1, float sqrMag2)
			{
				float num = (directionMag * directionMag + (sqrMag1 - sqrMag2)) / 2f / directionMag;
				float y = (float)Math.Sqrt(Mathf.Clamp(sqrMag1 - num * num, 0f, float.PositiveInfinity));
				if (direction == Vector3.zero)
				{
					return Vector3.zero;
				}
				return Quaternion.LookRotation(direction, bendDirection) * new Vector3(0f, y, num);
			}

			public static void SolveFABRIK(VirtualBone[] bones, Vector3 startPosition, Vector3 targetPosition, float weight, float minNormalizedTargetDistance, int iterations, float length, Vector3 startOffset)
			{
				if (weight <= 0f)
				{
					return;
				}
				if (minNormalizedTargetDistance > 0f)
				{
					Vector3 vector = targetPosition - startPosition;
					float magnitude = vector.magnitude;
					targetPosition = startPosition + vector / magnitude * Mathf.Max(length * minNormalizedTargetDistance, magnitude);
				}
				for (int i = 0; i < bones.Length; i++)
				{
					bones[i].solverPosition += startOffset;
				}
				for (int j = 0; j < iterations; j++)
				{
					bones[bones.Length - 1].solverPosition = Vector3.Lerp(bones[bones.Length - 1].solverPosition, targetPosition, weight);
					for (int num = bones.Length - 2; num > -1; num--)
					{
						bones[num].solverPosition = SolveFABRIKJoint(bones[num].solverPosition, bones[num + 1].solverPosition, bones[num].length);
					}
					bones[0].solverPosition = startPosition;
					for (int k = 1; k < bones.Length; k++)
					{
						bones[k].solverPosition = SolveFABRIKJoint(bones[k].solverPosition, bones[k - 1].solverPosition, bones[k - 1].length);
					}
				}
				for (int l = 0; l < bones.Length - 1; l++)
				{
					SwingRotation(bones, l, bones[l + 1].solverPosition);
				}
			}

			private static Vector3 SolveFABRIKJoint(Vector3 pos1, Vector3 pos2, float length)
			{
				return pos2 + (pos1 - pos2).normalized * length;
			}

			public static void SolveCCD(VirtualBone[] bones, Vector3 targetPosition, float weight, int iterations)
			{
				if (weight <= 0f)
				{
					return;
				}
				for (int i = 0; i < iterations; i++)
				{
					for (int num = bones.Length - 2; num > -1; num--)
					{
						Vector3 fromDirection = bones[bones.Length - 1].solverPosition - bones[num].solverPosition;
						Vector3 toDirection = targetPosition - bones[num].solverPosition;
						Quaternion quaternion = Quaternion.FromToRotation(fromDirection, toDirection);
						if (weight >= 1f)
						{
							RotateBy(bones, num, quaternion);
						}
						else
						{
							RotateBy(bones, num, Quaternion.Lerp(Quaternion.identity, quaternion, weight));
						}
					}
				}
			}
		}

		private Transform[] solverTransforms = new Transform[0];

		private bool hasChest;

		private bool hasNeck;

		private bool hasShoulders;

		private bool hasToes;

		private Vector3[] readPositions = new Vector3[0];

		private Quaternion[] readRotations = new Quaternion[0];

		private Vector3[] solvedPositions = new Vector3[22];

		private Quaternion[] solvedRotations = new Quaternion[22];

		private Quaternion[] defaultLocalRotations = new Quaternion[21];

		private Vector3[] defaultLocalPositions = new Vector3[21];

		private Vector3 rootV;

		private Vector3 rootVelocity;

		private Vector3 bodyOffset;

		private int supportLegIndex;

		[Tooltip("If true, will keep the toes planted even if head target is out of reach.")]
		public bool plantFeet = true;

		[Tooltip("The spine solver.")]
		public Spine spine = new Spine();

		[Tooltip("The left arm solver.")]
		public Arm leftArm = new Arm();

		[Tooltip("The right arm solver.")]
		public Arm rightArm = new Arm();

		[Tooltip("The left leg solver.")]
		public Leg leftLeg = new Leg();

		[Tooltip("The right leg solver.")]
		public Leg rightLeg = new Leg();

		[Tooltip("The procedural locomotion solver.")]
		public Locomotion locomotion = new Locomotion();

		private Leg[] legs = new Leg[2];

		private Arm[] arms = new Arm[2];

		private Vector3 headPosition;

		private Vector3 headDeltaPosition;

		private Vector3 raycastOriginPelvis;

		private Vector3 lastOffset;

		private Vector3 debugPos1;

		private Vector3 debugPos2;

		private Vector3 debugPos3;

		private Vector3 debugPos4;

		[HideInInspector]
		public VirtualBone rootBone { get; private set; }

		public void SetToReferences(VRIK.References references)
		{
			if (!references.isFilled)
			{
				Debug.LogError("Invalid references, one or more Transforms are missing.");
				return;
			}
			solverTransforms = references.GetTransforms();
			hasChest = solverTransforms[3] != null;
			hasNeck = solverTransforms[4] != null;
			hasShoulders = solverTransforms[6] != null && solverTransforms[10] != null;
			hasToes = solverTransforms[17] != null && solverTransforms[21] != null;
			readPositions = new Vector3[solverTransforms.Length];
			readRotations = new Quaternion[solverTransforms.Length];
			DefaultAnimationCurves();
			GuessHandOrientations(references, true);
		}

		public void GuessHandOrientations(VRIK.References references, bool onlyIfZero)
		{
			if (!references.isFilled)
			{
				Debug.LogWarning("VRIK References are not filled in, can not guess hand orientations. Right-click on VRIK header and slect 'Guess Hand Orientations' when you have filled in the References.");
				return;
			}
			if (leftArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
			{
				leftArm.wristToPalmAxis = GuessWristToPalmAxis(references.leftHand, references.leftForearm);
			}
			if (leftArm.palmToThumbAxis == Vector3.zero || !onlyIfZero)
			{
				leftArm.palmToThumbAxis = GuessPalmToThumbAxis(references.leftHand, references.leftForearm);
			}
			if (rightArm.wristToPalmAxis == Vector3.zero || !onlyIfZero)
			{
				rightArm.wristToPalmAxis = GuessWristToPalmAxis(references.rightHand, references.rightForearm);
			}
			if (rightArm.palmToThumbAxis == Vector3.zero || !onlyIfZero)
			{
				rightArm.palmToThumbAxis = GuessPalmToThumbAxis(references.rightHand, references.rightForearm);
			}
		}

		public void DefaultAnimationCurves()
		{
			if (locomotion.stepHeight == null)
			{
				locomotion.stepHeight = new AnimationCurve();
			}
			if (locomotion.heelHeight == null)
			{
				locomotion.heelHeight = new AnimationCurve();
			}
			if (locomotion.stepHeight.keys.Length == 0)
			{
				locomotion.stepHeight.keys = GetSineKeyframes(0.03f);
			}
			if (locomotion.heelHeight.keys.Length == 0)
			{
				locomotion.heelHeight.keys = GetSineKeyframes(0.03f);
			}
		}

		public void AddPositionOffset(PositionOffset positionOffset, Vector3 value)
		{
			switch (positionOffset)
			{
			case PositionOffset.Pelvis:
				spine.pelvisPositionOffset += value;
				break;
			case PositionOffset.Chest:
				spine.chestPositionOffset += value;
				break;
			case PositionOffset.Head:
				spine.headPositionOffset += value;
				break;
			case PositionOffset.LeftHand:
				leftArm.handPositionOffset += value;
				break;
			case PositionOffset.RightHand:
				rightArm.handPositionOffset += value;
				break;
			case PositionOffset.LeftFoot:
				leftLeg.footPositionOffset += value;
				break;
			case PositionOffset.RightFoot:
				rightLeg.footPositionOffset += value;
				break;
			case PositionOffset.LeftHeel:
				leftLeg.heelPositionOffset += value;
				break;
			case PositionOffset.RightHeel:
				rightLeg.heelPositionOffset += value;
				break;
			}
		}

		public void AddRotationOffset(RotationOffset rotationOffset, Vector3 value)
		{
			AddRotationOffset(rotationOffset, Quaternion.Euler(value));
		}

		public void AddRotationOffset(RotationOffset rotationOffset, Quaternion value)
		{
			switch (rotationOffset)
			{
			case RotationOffset.Pelvis:
				spine.pelvisRotationOffset = value * spine.pelvisRotationOffset;
				break;
			case RotationOffset.Chest:
				spine.chestRotationOffset = value * spine.chestRotationOffset;
				break;
			case RotationOffset.Head:
				spine.headRotationOffset = value * spine.headRotationOffset;
				break;
			}
		}

		public void AddPlatformMotion(Vector3 deltaPosition, Quaternion deltaRotation, Vector3 platformPivot)
		{
			locomotion.AddDeltaPosition(deltaPosition);
			raycastOriginPelvis += deltaPosition;
			locomotion.AddDeltaRotation(deltaRotation, platformPivot);
			spine.faceDirection = deltaRotation * spine.faceDirection;
		}

		public void Reset()
		{
			if (base.initiated)
			{
				UpdateSolverTransforms();
				Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
				spine.faceDirection = rootBone.readRotation * Vector3.forward;
				locomotion.Reset(readPositions, readRotations);
				raycastOriginPelvis = spine.pelvis.readPosition;
			}
		}

		public override void StoreDefaultLocalState()
		{
			for (int i = 1; i < solverTransforms.Length; i++)
			{
				if (solverTransforms[i] != null)
				{
					defaultLocalPositions[i - 1] = solverTransforms[i].localPosition;
					defaultLocalRotations[i - 1] = solverTransforms[i].localRotation;
				}
			}
		}

		public override void FixTransforms()
		{
			if (!base.initiated)
			{
				return;
			}
			for (int i = 1; i < solverTransforms.Length; i++)
			{
				if (solverTransforms[i] != null)
				{
					bool num = i == 1;
					bool flag = i > 5 && i < 14;
					if (num || flag)
					{
						solverTransforms[i].localPosition = defaultLocalPositions[i - 1];
					}
					solverTransforms[i].localRotation = defaultLocalRotations[i - 1];
				}
			}
		}

		public override Point[] GetPoints()
		{
			Debug.LogError("GetPoints() is not applicable to IKSolverVR.");
			return null;
		}

		public override Point GetPoint(Transform transform)
		{
			Debug.LogError("GetPoint is not applicable to IKSolverVR.");
			return null;
		}

		public override bool IsValid(ref string message)
		{
			if (solverTransforms == null || solverTransforms.Length == 0)
			{
				message = "Trying to initiate IKSolverVR with invalid bone references.";
				return false;
			}
			if (leftArm.wristToPalmAxis == Vector3.zero)
			{
				message = "Left arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
				return false;
			}
			if (rightArm.wristToPalmAxis == Vector3.zero)
			{
				message = "Right arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
				return false;
			}
			if (leftArm.palmToThumbAxis == Vector3.zero)
			{
				message = "Left arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
				return false;
			}
			if (rightArm.palmToThumbAxis == Vector3.zero)
			{
				message = "Right arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
				return false;
			}
			return true;
		}

		private Vector3 GetNormal(Transform[] transforms)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int i = 0; i < transforms.Length; i++)
			{
				zero2 += transforms[i].position;
			}
			zero2 /= (float)transforms.Length;
			for (int j = 0; j < transforms.Length - 1; j++)
			{
				zero += Vector3.Cross(transforms[j].position - zero2, transforms[j + 1].position - zero2).normalized;
			}
			return zero;
		}

		private Vector3 GuessWristToPalmAxis(Transform hand, Transform forearm)
		{
			Vector3 vector = forearm.position - hand.position;
			Vector3 vector2 = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector));
			if (Vector3.Dot(vector, hand.rotation * vector2) > 0f)
			{
				vector2 = -vector2;
			}
			return vector2;
		}

		private Vector3 GuessPalmToThumbAxis(Transform hand, Transform forearm)
		{
			if (hand.childCount == 0)
			{
				Debug.LogWarning("Hand " + hand.name + " does not have any fingers, VRIK can not guess the hand bone's orientation. Please assign 'Wrist To Palm Axis' and 'Palm To Thumb Axis' manually for both arms in VRIK settings.", hand);
				return Vector3.zero;
			}
			float num = float.PositiveInfinity;
			int index = 0;
			for (int i = 0; i < hand.childCount; i++)
			{
				float num2 = Vector3.SqrMagnitude(hand.GetChild(i).position - hand.position);
				if (num2 < num)
				{
					num = num2;
					index = i;
				}
			}
			Vector3 vector = Vector3.Cross(Vector3.Cross(hand.position - forearm.position, hand.GetChild(index).position - hand.position), hand.position - forearm.position);
			Vector3 vector2 = AxisTools.ToVector3(AxisTools.GetAxisToDirection(hand, vector));
			if (Vector3.Dot(vector, hand.rotation * vector2) < 0f)
			{
				vector2 = -vector2;
			}
			return vector2;
		}

		private static Keyframe[] GetSineKeyframes(float mag)
		{
			Keyframe[] array = new Keyframe[3];
			array[0].time = 0f;
			array[0].value = 0f;
			array[1].time = 0.5f;
			array[1].value = mag;
			array[2].time = 1f;
			array[2].value = 0f;
			return array;
		}

		private void UpdateSolverTransforms()
		{
			for (int i = 0; i < solverTransforms.Length; i++)
			{
				if (solverTransforms[i] != null)
				{
					readPositions[i] = solverTransforms[i].position;
					readRotations[i] = solverTransforms[i].rotation;
				}
			}
		}

		protected override void OnInitiate()
		{
			UpdateSolverTransforms();
			Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
		}

		protected override void OnUpdate()
		{
			if (IKPositionWeight > 0f)
			{
				UpdateSolverTransforms();
				Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes);
				Solve();
				Write();
				WriteTransforms();
			}
		}

		private void WriteTransforms()
		{
			for (int i = 0; i < solverTransforms.Length; i++)
			{
				if (solverTransforms[i] != null)
				{
					bool num = i < 2;
					bool flag = i > 5 && i < 14;
					if (num || flag)
					{
						solverTransforms[i].position = V3Tools.Lerp(solverTransforms[i].position, GetPosition(i), IKPositionWeight);
					}
					solverTransforms[i].rotation = QuaTools.Lerp(solverTransforms[i].rotation, GetRotation(i), IKPositionWeight);
				}
			}
		}

		private void Read(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes)
		{
			if (rootBone == null)
			{
				rootBone = new VirtualBone(positions[0], rotations[0]);
			}
			else
			{
				rootBone.Read(positions[0], rotations[0]);
			}
			spine.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 0, 1);
			leftArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 6);
			rightArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasChest ? 3 : 2, 10);
			leftLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 14);
			rightLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, 1, 18);
			for (int i = 0; i < rotations.Length; i++)
			{
				solvedPositions[i] = positions[i];
				solvedRotations[i] = rotations[i];
			}
			if (!base.initiated)
			{
				legs = new Leg[2] { leftLeg, rightLeg };
				arms = new Arm[2] { leftArm, rightArm };
				locomotion.Initiate(positions, rotations, hasToes);
				raycastOriginPelvis = spine.pelvis.readPosition;
				spine.faceDirection = readRotations[0] * Vector3.forward;
			}
		}

		private void Solve()
		{
			spine.PreSolve();
			Arm[] array = arms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PreSolve();
			}
			Leg[] array2 = legs;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].PreSolve();
			}
			array = arms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ApplyOffsets();
			}
			spine.ApplyOffsets();
			spine.Solve(rootBone, legs, arms);
			if (spine.pelvisPositionWeight > 0f && plantFeet)
			{
				Warning.Log("If VRIK 'Pelvis Position Weight' is > 0, 'Plant Feet' should be disabled to improve performance and stability.", root);
			}
			if (locomotion.weight > 0f)
			{
				Vector3 leftFootPosition = Vector3.zero;
				Vector3 rightFootPosition = Vector3.zero;
				Quaternion leftFootRotation = Quaternion.identity;
				Quaternion rightFootRotation = Quaternion.identity;
				float leftFootOffset = 0f;
				float rightFootOffset = 0f;
				float leftHeelOffset = 0f;
				float rightHeelOffset = 0f;
				locomotion.Solve(rootBone, spine, leftLeg, rightLeg, leftArm, rightArm, supportLegIndex, out leftFootPosition, out rightFootPosition, out leftFootRotation, out rightFootRotation, out leftFootOffset, out rightFootOffset, out leftHeelOffset, out rightHeelOffset);
				leftFootPosition += root.up * leftFootOffset;
				rightFootPosition += root.up * rightFootOffset;
				leftLeg.footPositionOffset += (leftFootPosition - leftLeg.lastBone.solverPosition) * IKPositionWeight * (1f - leftLeg.positionWeight) * locomotion.weight;
				rightLeg.footPositionOffset += (rightFootPosition - rightLeg.lastBone.solverPosition) * IKPositionWeight * (1f - rightLeg.positionWeight) * locomotion.weight;
				leftLeg.heelPositionOffset += root.up * leftHeelOffset * locomotion.weight;
				rightLeg.heelPositionOffset += root.up * rightHeelOffset * locomotion.weight;
				Quaternion b = QuaTools.FromToRotation(leftLeg.lastBone.solverRotation, leftFootRotation);
				Quaternion b2 = QuaTools.FromToRotation(rightLeg.lastBone.solverRotation, rightFootRotation);
				b = Quaternion.Lerp(Quaternion.identity, b, IKPositionWeight * (1f - leftLeg.rotationWeight) * locomotion.weight);
				b2 = Quaternion.Lerp(Quaternion.identity, b2, IKPositionWeight * (1f - rightLeg.rotationWeight) * locomotion.weight);
				leftLeg.footRotationOffset = b * leftLeg.footRotationOffset;
				rightLeg.footRotationOffset = b2 * rightLeg.footRotationOffset;
				Vector3 point = Vector3.Lerp(leftLeg.position + leftLeg.footPositionOffset, rightLeg.position + rightLeg.footPositionOffset, 0.5f);
				point = V3Tools.PointToPlane(point, rootBone.solverPosition, root.up);
				Vector3 a = rootBone.solverPosition + rootVelocity * Time.deltaTime * 2f * locomotion.weight;
				a = Vector3.Lerp(a, point, Time.deltaTime * locomotion.rootSpeed * locomotion.weight);
				rootBone.solverPosition = a;
				rootVelocity += (point - rootBone.solverPosition) * Time.deltaTime * 10f;
				Vector3 vector = V3Tools.ExtractVertical(rootVelocity, root.up, 1f);
				rootVelocity -= vector;
				float num = leftFootOffset + rightFootOffset;
				bodyOffset = Vector3.Lerp(bodyOffset, root.up * num, Time.deltaTime * 3f);
				bodyOffset = Vector3.Lerp(Vector3.zero, bodyOffset, locomotion.weight);
			}
			array2 = legs;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].ApplyOffsets();
			}
			if (!plantFeet)
			{
				spine.InverseTranslateToHead(legs, false, false, bodyOffset, 1f);
				array2 = legs;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
				}
				array2 = legs;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Solve();
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					spine.InverseTranslateToHead(legs, true, j == 0, bodyOffset, 1f);
					array2 = legs;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
					}
					array2 = legs;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].Solve();
					}
				}
			}
			for (int k = 0; k < arms.Length; k++)
			{
				arms[k].TranslateRoot(spine.chest.solverPosition, spine.chest.solverRotation);
			}
			for (int l = 0; l < arms.Length; l++)
			{
				arms[l].Solve(l == 0);
			}
			spine.ResetOffsets();
			array2 = legs;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].ResetOffsets();
			}
			array = arms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetOffsets();
			}
			spine.pelvisPositionOffset += GetPelvisOffset();
			spine.chestPositionOffset += spine.pelvisPositionOffset;
			Write();
			supportLegIndex = -1;
			float num2 = float.PositiveInfinity;
			for (int m = 0; m < legs.Length; m++)
			{
				float num3 = Vector3.SqrMagnitude(legs[m].lastBone.solverPosition - legs[m].bones[0].solverPosition);
				if (num3 < num2)
				{
					supportLegIndex = m;
					num2 = num3;
				}
			}
		}

		private Vector3 GetPosition(int index)
		{
			return solvedPositions[index];
		}

		private Quaternion GetRotation(int index)
		{
			return solvedRotations[index];
		}

		private void Write()
		{
			solvedPositions[0] = rootBone.solverPosition;
			solvedRotations[0] = rootBone.solverRotation;
			spine.Write(ref solvedPositions, ref solvedRotations);
			Leg[] array = legs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Write(ref solvedPositions, ref solvedRotations);
			}
			Arm[] array2 = arms;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Write(ref solvedPositions, ref solvedRotations);
			}
		}

		private Vector3 GetPelvisOffset()
		{
			if (locomotion.weight <= 0f)
			{
				return Vector3.zero;
			}
			if ((int)locomotion.blockingLayers == -1)
			{
				return Vector3.zero;
			}
			Vector3 vector = raycastOriginPelvis;
			vector.y = spine.pelvis.solverPosition.y;
			Vector3 vector2 = spine.pelvis.readPosition;
			vector2.y = spine.pelvis.solverPosition.y;
			Vector3 direction = vector2 - vector;
			RaycastHit hitInfo;
			if (locomotion.raycastRadius <= 0f)
			{
				if (Physics.Raycast(vector, direction, out hitInfo, direction.magnitude * 1.1f, locomotion.blockingLayers))
				{
					vector2 = hitInfo.point;
				}
			}
			else if (Physics.SphereCast(vector, locomotion.raycastRadius * 1.1f, direction, out hitInfo, direction.magnitude, locomotion.blockingLayers))
			{
				vector2 = vector + direction.normalized * hitInfo.distance / 1.1f;
			}
			Vector3 vector3 = spine.pelvis.solverPosition;
			direction = vector3 - vector2;
			if (locomotion.raycastRadius <= 0f)
			{
				if (Physics.Raycast(vector2, direction, out hitInfo, direction.magnitude, locomotion.blockingLayers))
				{
					vector3 = hitInfo.point;
				}
			}
			else if (Physics.SphereCast(vector2, locomotion.raycastRadius, direction, out hitInfo, direction.magnitude, locomotion.blockingLayers))
			{
				vector3 = vector2 + direction.normalized * hitInfo.distance;
			}
			lastOffset = Vector3.Lerp(lastOffset, Vector3.zero, Time.deltaTime * 3f);
			vector3 += Vector3.ClampMagnitude(lastOffset, 0.75f);
			vector3.y = spine.pelvis.solverPosition.y;
			lastOffset = Vector3.Lerp(lastOffset, vector3 - spine.pelvis.solverPosition, Time.deltaTime * 15f);
			return lastOffset;
		}
	}
}
