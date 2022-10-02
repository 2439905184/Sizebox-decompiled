using RuntimeGizmos;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditHandle : MonoBehaviour
	{
		public enum HandleStyle
		{
			Active = 0,
			Inactive = 1,
			JiggleSource = 2,
			JiggleExclude = 3
		}

		private const float GIANTESS_SCALE_MULTIPLIER = 1000f;

		private const float HANDLE_SIZE = 0.025f;

		private const float LINE_WIDTH = 0.0065f;

		[SerializeField]
		private Renderer myRenderer;

		[SerializeField]
		private LineRenderer myLineRenderer;

		[SerializeField]
		private Material activeMaterial;

		[SerializeField]
		private Material inactiveMaterial;

		[SerializeField]
		private Material selectedMaterial;

		[SerializeField]
		private Material pairedMaterial;

		[SerializeField]
		private Material linkedMaterial;

		[SerializeField]
		private Material jiggleMaterial;

		private bool _Selected;

		private bool _ShowPaired;

		private bool _ShowLinked;

		private HandleStyle style;

		private EntityBase target;

		private Vector3 previousLocalPosition;

		private Quaternion previousLocalRotation;

		private float scale;

		private float scaleMult = 1f;

		public bool Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
				SetStyle(style);
			}
		}

		public bool ShowPaired
		{
			get
			{
				return _ShowPaired;
			}
			set
			{
				_ShowPaired = value;
				SetStyle(style);
			}
		}

		public bool ShowLinked
		{
			get
			{
				return _ShowLinked;
			}
			set
			{
				_ShowLinked = value;
				SetStyle(style);
			}
		}

		public EditBone EditBone { get; private set; }

		private Quaternion boneRotation
		{
			get
			{
				return EditBone.RealTransform.rotation;
			}
		}

		public void AssignBone(EditBone bone, EntityBase target)
		{
			this.target = target;
			EditBone = bone;
			base.transform.SetParent(target.transform);
			scale = 0.025f;
			if (target.isGiantess)
			{
				scale *= 1000f;
			}
			UpdatePosition(TransformType.Move);
		}

		private void OnDisable()
		{
			_ShowPaired = false;
			_ShowLinked = false;
			SetStyle(style);
		}

		public void SetStyle(HandleStyle style)
		{
			this.style = style;
			if (Selected)
			{
				myRenderer.material = selectedMaterial;
				return;
			}
			if (ShowPaired)
			{
				myRenderer.material = pairedMaterial;
				return;
			}
			if (ShowLinked)
			{
				myRenderer.material = linkedMaterial;
				return;
			}
			switch (style)
			{
			case HandleStyle.Active:
				myRenderer.material = activeMaterial;
				break;
			case HandleStyle.Inactive:
				myRenderer.material = inactiveMaterial;
				break;
			case HandleStyle.JiggleSource:
				myRenderer.material = jiggleMaterial;
				break;
			case HandleStyle.JiggleExclude:
				myRenderer.material = selectedMaterial;
				break;
			}
		}

		public void SetSize(float size)
		{
			scaleMult = size;
			UpdateLineRenderer();
		}

		public TransformationData UpdateHandle(string id, TransformType transformationType, SkeletonEditOptions options)
		{
			TransformationData result = default(TransformationData);
			result.rotation = Quaternion.identity;
			if (!EditBone)
			{
				return result;
			}
			if (transformationType == TransformType.Move && previousLocalPosition != base.transform.localPosition)
			{
				result.movement = HandleMovement(id, options.movement);
			}
			else if (transformationType == TransformType.Rotate && previousLocalRotation != base.transform.localRotation)
			{
				result.rotation = HandleRotation(id, options.rotation);
			}
			else if (transformationType == TransformType.Scale && base.transform.localScale != Vector3.one * scale)
			{
				result.scaling = HandleScaling(id, options.scaling);
			}
			UpdatePosition(transformationType);
			return result;
		}

		public void UpdateHandle(string id, TransformationData data, TransformType type, SkeletonEditOptions options)
		{
			if ((bool)EditBone)
			{
				if (data.movement != Vector3.zero)
				{
					HandleMovement(id, data.movement, options.movement);
				}
				if (data.rotation != Quaternion.identity)
				{
					HandleRotation(id, data.rotation, options.rotation);
				}
				if (!(data.scaling == Vector3.zero) && !(data.scaling == Vector3.one))
				{
					HandleScaling(id, data.scaling, options.scaling);
				}
				UpdatePosition(type);
			}
		}

		private Vector3 HandleMovement(string id, SkeletonEditMovementOptions movementOptions)
		{
			Vector3 vector = base.transform.localPosition - previousLocalPosition;
			EditBone.Move(id, vector, movementOptions);
			return vector;
		}

		private void HandleMovement(string id, Vector3 movementLine, SkeletonEditMovementOptions movementOptions)
		{
			EditBone.Move(id, movementLine, movementOptions);
		}

		private Quaternion HandleRotation(string id, SkeletonEditRotationOptions rotationOptions)
		{
			Quaternion quaternion = EditBone.EditBone1.localRotation * (Quaternion.Inverse(boneRotation) * base.transform.rotation);
			EditBone.Rotate(id, quaternion, rotationOptions);
			return quaternion;
		}

		private void HandleRotation(string id, Quaternion rotation, SkeletonEditRotationOptions rotationOptions)
		{
			EditBone.Rotate(id, rotation, rotationOptions);
		}

		private Vector3 HandleScaling(string id, SkeletonEditScalingOptions scalingOptions)
		{
			float num = scale;
			if (Selected)
			{
				num *= 2f;
			}
			Vector3 vector = new Vector3(base.transform.localScale.x / num, base.transform.localScale.y / num, base.transform.localScale.z / num);
			if (vector == Vector3.one)
			{
				return vector;
			}
			Vector3 result = vector;
			vector = CorrectToScale(vector);
			EditBone.Scale(id, vector, scalingOptions);
			return result;
		}

		private void HandleScaling(string id, Vector3 scaleChange, SkeletonEditScalingOptions scalingOptions)
		{
			scaleChange = CorrectToScale(scaleChange);
			EditBone.Scale(id, scaleChange, scalingOptions);
		}

		private Vector3 CorrectToScale(Vector3 scaleVector)
		{
			float num = target.Scale;
			scaleVector.x = CorrectAxisToScale(scaleVector.x, num);
			scaleVector.y = CorrectAxisToScale(scaleVector.y, num);
			scaleVector.z = CorrectAxisToScale(scaleVector.z, num);
			return scaleVector;
		}

		private float CorrectAxisToScale(float axis, float scale)
		{
			if (axis >= 1f)
			{
				return (axis - 1f) / scale + 1f;
			}
			return 1f - (1f - axis) / scale;
		}

		private float MultiplyDifference(float value, float mult)
		{
			value = ((!(value > 1f)) ? (1f - (1f - value) * mult) : ((value - 1f) * mult + 1f));
			return value;
		}

		public void UpdatePosition(TransformType transformationType)
		{
			if ((bool)EditBone && (bool)EditBone.RealTransform)
			{
				base.transform.position = EditBone.RealTransform.position;
				previousLocalPosition = base.transform.localPosition;
				switch (transformationType)
				{
				case TransformType.Move:
					base.transform.rotation = target.transform.rotation;
					break;
				case TransformType.Rotate:
					base.transform.rotation = boneRotation;
					break;
				case TransformType.Scale:
					base.transform.rotation = EditBone.RealTransform.rotation;
					break;
				}
				previousLocalRotation = base.transform.localRotation;
				base.transform.localScale = Vector3.one * scale;
				if (Selected)
				{
					base.transform.localScale = base.transform.localScale * 2f;
				}
				float num = scale * scaleMult;
				if (Selected)
				{
					num *= 1.5f;
				}
				Vector3 localScale = default(Vector3);
				localScale.x = num / base.transform.localScale.x;
				localScale.y = num / base.transform.localScale.y;
				localScale.z = num / base.transform.localScale.z;
				myRenderer.transform.localScale = localScale;
				UpdateLineRenderer();
			}
		}

		private void UpdateLineRenderer()
		{
			if ((bool)EditBone && (bool)EditBone.ParentBone)
			{
				Vector3[] positions = new Vector3[2]
				{
					EditBone.RealTransform.position,
					EditBone.ParentBone.RealTransform.position
				};
				myLineRenderer.enabled = true;
				myLineRenderer.SetPositions(positions);
				float num = target.Scale * 0.0065f * scaleMult;
				if (target.isGiantess)
				{
					num *= 1000f;
				}
				myLineRenderer.startWidth = num;
				myLineRenderer.endWidth = num;
			}
			else
			{
				myLineRenderer.enabled = false;
			}
		}

		public void Reset(string id, TransformType transformationType, SkeletonEditOptions options)
		{
			if ((bool)EditBone)
			{
				switch (transformationType)
				{
				case TransformType.Move:
					EditBone.ResetPosition(id, options.movement);
					break;
				case TransformType.Rotate:
					EditBone.ResetRotation(id, options.rotation);
					break;
				case TransformType.Scale:
					EditBone.ResetScaling(id, options.scaling);
					break;
				}
				UpdatePosition(transformationType);
			}
		}

		public void ResetBone(string id, SkeletonEditOptions options)
		{
			if ((bool)EditBone)
			{
				EditBone.ResetPosition(id, options.movement);
				EditBone.ResetRotation(id, options.rotation);
				EditBone.ResetScaling(id, options.scaling);
				UpdatePosition(TransformType.Move);
			}
		}

		public void ResetAll(string id)
		{
			if ((bool)EditBone)
			{
				EditBone.ResetAll(id);
				UpdatePosition(TransformType.Move);
			}
		}

		public void Disable(Transform folder)
		{
			base.transform.SetParent(folder);
			EditBone = null;
			_ShowLinked = false;
			_ShowPaired = false;
			Selected = false;
			base.gameObject.SetActive(false);
		}
	}
}
