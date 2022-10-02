using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class EditBone : MonoBehaviour
	{
		private const float MIN_SCALE = 1E-05f;

		private const float MAX_SCALE = 10000f;

		private const string NAME_PREFIX = "   EDIT BONE 00: ";

		private const string DEFAULT_DATA_ID = "body";

		private Vector3 cachedLocalPos;

		private SkeletonEdit Editor;

		private Dictionary<string, BoneTransformData> Data = new Dictionary<string, BoneTransformData>();

		private Vector3 rotationCorrection;

		private Vector3 normalizationCorrection;

		[SerializeField]
		private Transform editBone0;

		[SerializeField]
		private Transform editBone1;

		private List<EditBone> childrenBones = new List<EditBone>();

		private EntityBase entity;

		public bool Enabled { get; private set; }

		public string Key { get; private set; }

		public Transform EditBone0
		{
			get
			{
				return editBone0;
			}
		}

		public Transform EditBone1
		{
			get
			{
				return editBone1;
			}
		}

		public Transform RealTransform { get; private set; }

		public EditBone ParentBone { get; private set; }

		public EditBone SiblingBone { get; private set; }

		public void Initialize(string key, SkeletonEdit editor, Transform realBone, EditBone parentBone = null)
		{
			Key = key;
			Editor = editor;
			RealTransform = realBone;
			cachedLocalPos = RealTransform.localPosition;
			base.name = "   EDIT BONE 00: " + realBone.name;
			SetDataScale("body", realBone.localScale);
			editBone0.parent = realBone.parent;
			editBone0.localPosition = Vector3.zero;
			editBone0.localRotation = Quaternion.identity;
			editBone0.localScale = Vector3.one;
			if ((bool)parentBone)
			{
				ParentBone = parentBone;
				parentBone.RegisterChild(this);
			}
			entity = editor.GetComponent<EntityBase>();
			Enable();
		}

		public void SetParent(EditBone parent)
		{
			ParentBone = parent;
		}

		public void RegisterChild(EditBone child)
		{
			if (!childrenBones.Contains(child))
			{
				childrenBones.Add(child);
			}
			child.SetParent(this);
		}

		public void UnregisterChild(EditBone child)
		{
			if (childrenBones.Contains(child))
			{
				childrenBones.Remove(child);
			}
		}

		public void SetSibling(EditBone sibling)
		{
			SiblingBone = sibling;
		}

		public Vector3 Move(string id, Vector3 movement, SkeletonEditMovementOptions options, bool scaleInput = true)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			Vector3 movement2 = movement;
			movement2.x = 0f - movement2.x;
			movement *= options.speed;
			if (scaleInput)
			{
				movement = Vector3.Scale(movement, editBone1.lossyScale / entity.ModelScale);
			}
			movement = Editor.transform.rotation * movement;
			Vector3 localPosition = editBone1.localPosition;
			editBone1.position += movement;
			Vector3 localPosition2 = editBone1.localPosition;
			value.position += localPosition2 - localPosition;
			Data[id] = value;
			if ((bool)SiblingBone && options.effectPairs)
			{
				options.effectPairs = false;
				SiblingBone.Move(id, movement2, options);
			}
			return movement;
		}

		public void Rotate(string id, Quaternion rotation, SkeletonEditRotationOptions options)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.rotation = rotation;
			Data[id] = value;
			if ((bool)SiblingBone && options.effectPairs)
			{
				options.effectPairs = false;
				SiblingBone.Rotate(id, HandleRotationMirroring(rotation, options), options);
			}
			UpdateRotation();
		}

		public void Scale(string id, Vector3 scaling, SkeletonEditScalingOptions options)
		{
			if ((bool)SiblingBone && options.effectPairs)
			{
				options.effectPairs = false;
				SiblingBone.Scale(id, scaling, options);
			}
			EnsureId(id);
			BoneTransformData value = Data[id];
			Vector3 scale = value.scale;
			value.scale = Vector3.Scale(value.scale, scaling);
			value.scale = ClampVector(value.scale, 1E-05f, 10000f);
			Vector3 scaling2 = default(Vector3);
			scaling2.x = value.scale.x / scale.x;
			scaling2.y = value.scale.y / scale.y;
			scaling2.z = value.scale.z / scale.z;
			if (options.scaleWithoutChildren)
			{
				foreach (EditBone childrenBone in childrenBones)
				{
					childrenBone.ScaleAgainst(id, scaling2);
				}
			}
			Data[id] = value;
			UpdateScale();
		}

		public void ScaleAgainst(string id, Vector3 scaling)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			scaling = InvertScaling(scaling);
			value.scale = Vector3.Scale(value.scale, scaling);
			Data[id] = value;
			UpdateScale();
		}

		public void Enable()
		{
			if (!Enabled && (bool)RealTransform)
			{
				Enabled = true;
				RealTransform.SetParent(editBone1, true);
				RealTransform.localPosition = cachedLocalPos;
			}
		}

		public void Disable()
		{
			if (Enabled && (bool)RealTransform)
			{
				cachedLocalPos = RealTransform.localPosition;
				Enabled = false;
				RealTransform.SetParent(editBone0.parent, true);
			}
		}

		public void ResetAll(string id)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.position = Vector3.zero;
			value.rotation = Quaternion.identity;
			value.scale = Vector3.one;
			Data[id] = value;
			UpdateTransformation();
		}

		public void ResetPosition(string id, SkeletonEditMovementOptions movementOptions)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.position = Vector3.zero;
			Data[id] = value;
			if ((bool)SiblingBone && movementOptions.effectPairs)
			{
				movementOptions.effectPairs = false;
				SiblingBone.ResetPosition(id, movementOptions);
			}
			UpdatePosition();
		}

		public void ResetRotation(string id, SkeletonEditRotationOptions rotationOptions)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.rotation = Quaternion.identity;
			Data[id] = value;
			editBone1.localPosition = Vector3.zero;
			if ((bool)SiblingBone && rotationOptions.effectPairs)
			{
				rotationOptions.effectPairs = false;
				SiblingBone.ResetRotation(id, rotationOptions);
			}
			UpdateRotation();
		}

		public void ResetScaling(string id, SkeletonEditScalingOptions scalingOptions)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			Vector3 scale = value.scale;
			value.scale = Vector3.one;
			Data[id] = value;
			if (scalingOptions.scaleWithoutChildren)
			{
				Vector3 scaling = default(Vector3);
				scaling.x = value.scale.x / scale.x;
				scaling.y = value.scale.y / scale.y;
				scaling.z = value.scale.z / scale.z;
				foreach (EditBone childrenBone in childrenBones)
				{
					childrenBone.ScaleAgainst(id, scaling);
				}
			}
			if ((bool)SiblingBone && scalingOptions.effectPairs)
			{
				scalingOptions.effectPairs = false;
				SiblingBone.ResetScaling(id, scalingOptions);
			}
			UpdateScale();
		}

		public void UpdateTransformation()
		{
			UpdateScale();
			UpdateRotation();
			UpdatePosition();
		}

		public void UpdateTransformationRecursive()
		{
			UpdateScale(false);
			UpdateRotation();
			UpdatePosition();
			foreach (EditBone childrenBone in childrenBones)
			{
				childrenBone.UpdateTransformationRecursive();
			}
		}

		private void UpdatePosition()
		{
			editBone1.localPosition = GetDataPosition();
		}

		private void UpdateRotation()
		{
			editBone1.localRotation = GetDataRotation();
			UpdateRotationCorrection();
			UpdateCorrectionPosition();
		}

		public void UpdateScale(bool scaleChildren = true)
		{
			RealTransform.localScale = Vector3.Scale(GetDataScale(), InvertScaling(editBone0.localScale));
			NormalizeChildrenScale(scaleChildren);
		}

		private void UpdateCorrectionPosition()
		{
			editBone0.localPosition = normalizationCorrection + rotationCorrection;
		}

		private void UpdateNormalizationCorrection(Transform parent)
		{
			editBone0.localPosition = Vector3.zero;
			Vector3 localPosition = editBone1.localPosition;
			Quaternion localRotation = editBone1.localRotation;
			editBone1.localPosition = Vector3.zero;
			editBone1.localRotation = Quaternion.identity;
			Vector3 vector = parent.InverseTransformPoint(RealTransform.position);
			Vector3 vector2 = Vector3.Scale(vector, parent.localScale) - vector;
			editBone1.localPosition = localPosition;
			editBone1.localRotation = localRotation;
			normalizationCorrection = vector2;
		}

		private void UpdateRotationCorrection()
		{
			Vector3 localPosition = editBone0.localPosition;
			editBone0.localPosition = Vector3.zero;
			Quaternion localRotation = editBone1.localRotation;
			editBone1.localRotation = Quaternion.identity;
			Vector3 position = RealTransform.position;
			editBone1.localRotation = localRotation;
			Vector3 position2 = RealTransform.position;
			Vector3 vector = position - position2;
			editBone0.position += vector;
			rotationCorrection = editBone0.localPosition;
			editBone0.localPosition = localPosition;
		}

		private void NormalizeChildrenScale(bool updateScale = true)
		{
			foreach (EditBone childrenBone in childrenBones)
			{
				childrenBone.NormalizeLocalScale(RealTransform, updateScale);
			}
		}

		public void NormalizeLocalScale(Transform parentTransform, bool updateScale = true)
		{
			editBone0.localScale = InvertScaling(parentTransform.localScale);
			UpdateNormalizationCorrection(parentTransform);
			UpdateCorrectionPosition();
			if (updateScale)
			{
				UpdateScale();
			}
		}

		private void EnsureId(string id)
		{
			if (!Data.ContainsKey(id))
			{
				BoneTransformData @default = BoneTransformData.Default;
				Data.Add(id, @default);
			}
		}

		public BoneTransformData GetData(string id)
		{
			EnsureId(id);
			return Data[id];
		}

		public void SetData(string id, BoneTransformData inData)
		{
			EnsureId(id);
			Data[id] = inData;
			NormalizeChildrenScale();
			UpdateTransformation();
		}

		public void SetDataPosition(string id, Vector3 position)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.position = position;
			Data[id] = value;
		}

		public void SetDataRotation(string id, Quaternion rotation)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.rotation = rotation;
			Data[id] = value;
		}

		public void SetDataScale(string id, Vector3 scale)
		{
			EnsureId(id);
			BoneTransformData value = Data[id];
			value.scale = scale;
			Data[id] = value;
		}

		private Vector3 GetDataPosition()
		{
			Vector3 zero = Vector3.zero;
			foreach (KeyValuePair<string, BoneTransformData> datum in Data)
			{
				zero += datum.Value.position;
			}
			return zero;
		}

		private Quaternion GetDataRotation()
		{
			Quaternion identity = Quaternion.identity;
			foreach (KeyValuePair<string, BoneTransformData> datum in Data)
			{
				identity *= datum.Value.rotation;
			}
			return identity;
		}

		private Vector3 GetDataScale()
		{
			Vector3 one = Vector3.one;
			foreach (KeyValuePair<string, BoneTransformData> datum in Data)
			{
				one.Scale(datum.Value.scale);
			}
			return one;
		}

		private void OnDestroy()
		{
			if ((bool)ParentBone)
			{
				ParentBone.UnregisterChild(this);
				foreach (EditBone childrenBone in childrenBones)
				{
					ParentBone.RegisterChild(childrenBone);
					childrenBone.SetParent(ParentBone);
				}
			}
			Editor.BoneMap.Remove(Key);
		}

		private Quaternion HandleRotationMirroring(Quaternion rotation, SkeletonEditRotationOptions rotationOptions)
		{
			if (!rotationOptions.invertX && !rotationOptions.invertY && !rotationOptions.invertZ)
			{
				return rotation;
			}
			if (rotationOptions.invertX && rotationOptions.invertY && rotationOptions.invertZ)
			{
				return Quaternion.Inverse(rotation);
			}
			if (rotationOptions.invertX)
			{
				rotation.y *= -1f;
				rotation.z *= -1f;
			}
			if (rotationOptions.invertY)
			{
				rotation.x *= -1f;
				rotation.z *= -1f;
			}
			if (rotationOptions.invertZ)
			{
				rotation.x *= -1f;
				rotation.y *= -1f;
			}
			return rotation;
		}

		private Vector3 ClampVector(Vector3 vector, float min, float max)
		{
			if (vector.x < min)
			{
				vector.x = min;
			}
			else if (vector.x > max)
			{
				vector.x = max;
			}
			if (vector.y < min)
			{
				vector.y = min;
			}
			else if (vector.y > max)
			{
				vector.y = max;
			}
			if (vector.z < min)
			{
				vector.z = min;
			}
			else if (vector.z > max)
			{
				vector.z = max;
			}
			return vector;
		}

		private Vector3 InvertScaling(Vector3 scaling)
		{
			scaling.x = 1f / scaling.x;
			scaling.y = 1f / scaling.y;
			scaling.z = 1f / scaling.z;
			return scaling;
		}
	}
}
