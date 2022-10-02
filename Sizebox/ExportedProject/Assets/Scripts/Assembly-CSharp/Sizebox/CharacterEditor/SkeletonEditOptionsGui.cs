using UnityEngine;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class SkeletonEditOptionsGui : MonoBehaviour
	{
		[SerializeField]
		private SkeletonEditOptions options;

		[Header("Movement Options")]
		[SerializeField]
		private Slider movementSpeedSlider;

		[SerializeField]
		private Toggle movePairs;

		[Header("Rotation Options")]
		[SerializeField]
		private Toggle rotatePairs;

		[SerializeField]
		private Toggle invertX;

		[SerializeField]
		private Toggle invertY;

		[SerializeField]
		private Toggle invertZ;

		[Header("Scaling Options")]
		[SerializeField]
		private Toggle scalePairs;

		[SerializeField]
		private Toggle scaleWithoutChildren;

		public SkeletonEditOptions Options
		{
			get
			{
				return options;
			}
		}

		private void Awake()
		{
			if ((bool)movementSpeedSlider)
			{
				movementSpeedSlider.onValueChanged.AddListener(MovementSlider);
				MovementSlider(movementSpeedSlider.value);
			}
			if ((bool)movePairs)
			{
				movePairs.onValueChanged.AddListener(MovePairs);
				MovePairs(movePairs.isOn);
			}
			if ((bool)rotatePairs)
			{
				rotatePairs.onValueChanged.AddListener(RotatePairs);
				RotatePairs(rotatePairs.isOn);
			}
			if ((bool)invertX)
			{
				invertX.onValueChanged.AddListener(InvertX);
				InvertX(invertX.isOn);
			}
			if ((bool)invertY)
			{
				invertY.onValueChanged.AddListener(InvertY);
				InvertY(invertY.isOn);
			}
			if ((bool)invertZ)
			{
				invertZ.onValueChanged.AddListener(InvertZ);
				InvertZ(invertZ.isOn);
			}
			if ((bool)scalePairs)
			{
				scalePairs.onValueChanged.AddListener(ScalePairs);
				ScalePairs(scalePairs.isOn);
			}
			if ((bool)scaleWithoutChildren)
			{
				scaleWithoutChildren.onValueChanged.AddListener(ScaleWithoutChildren);
				ScaleWithoutChildren(scaleWithoutChildren.isOn);
			}
		}

		private void MovementSlider(float value)
		{
			options.movement.speed = value;
		}

		private void MovePairs(bool value)
		{
			options.movement.effectPairs = value;
		}

		private void RotatePairs(bool value)
		{
			options.rotation.effectPairs = value;
		}

		private void InvertX(bool value)
		{
			options.rotation.invertX = value;
		}

		private void InvertY(bool value)
		{
			options.rotation.invertY = value;
		}

		private void InvertZ(bool value)
		{
			options.rotation.invertZ = value;
		}

		private void ScalePairs(bool value)
		{
			options.scaling.effectPairs = value;
		}

		private void ScaleWithoutChildren(bool value)
		{
			options.scaling.scaleWithoutChildren = value;
		}
	}
}
