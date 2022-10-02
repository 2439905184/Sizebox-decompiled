using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.XR
{
	[Preserve]
	public class EyesControl : InputControl<Eyes>
	{
		[Preserve]
		[InputControl(offset = 0u, displayName = "LeftEyePosition")]
		public Vector3Control leftEyePosition { get; private set; }

		[Preserve]
		[InputControl(offset = 12u, displayName = "LeftEyeRotation")]
		public QuaternionControl leftEyeRotation { get; private set; }

		[Preserve]
		[InputControl(offset = 28u, displayName = "RightEyePosition")]
		public Vector3Control rightEyePosition { get; private set; }

		[Preserve]
		[InputControl(offset = 40u, displayName = "RightEyeRotation")]
		public QuaternionControl rightEyeRotation { get; private set; }

		[Preserve]
		[InputControl(offset = 56u, displayName = "FixationPoint")]
		public Vector3Control fixationPoint { get; private set; }

		[Preserve]
		[InputControl(offset = 68u, displayName = "LeftEyeOpenAmount")]
		public AxisControl leftEyeOpenAmount { get; private set; }

		[Preserve]
		[InputControl(offset = 72u, displayName = "RightEyeOpenAmount")]
		public AxisControl rightEyeOpenAmount { get; private set; }

		protected override void FinishSetup()
		{
			leftEyePosition = GetChildControl<Vector3Control>("leftEyePosition");
			leftEyeRotation = GetChildControl<QuaternionControl>("leftEyeRotation");
			rightEyePosition = GetChildControl<Vector3Control>("rightEyePosition");
			rightEyeRotation = GetChildControl<QuaternionControl>("rightEyeRotation");
			fixationPoint = GetChildControl<Vector3Control>("fixationPoint");
			leftEyeOpenAmount = GetChildControl<AxisControl>("leftEyeOpenAmount");
			rightEyeOpenAmount = GetChildControl<AxisControl>("rightEyeOpenAmount");
			base.FinishSetup();
		}

		public unsafe override Eyes ReadUnprocessedValueFromState(void* statePtr)
		{
			Eyes result = default(Eyes);
			result.leftEyePosition = leftEyePosition.ReadUnprocessedValueFromState(statePtr);
			result.leftEyeRotation = leftEyeRotation.ReadUnprocessedValueFromState(statePtr);
			result.rightEyePosition = rightEyePosition.ReadUnprocessedValueFromState(statePtr);
			result.rightEyeRotation = rightEyeRotation.ReadUnprocessedValueFromState(statePtr);
			result.fixationPoint = fixationPoint.ReadUnprocessedValueFromState(statePtr);
			result.leftEyeOpenAmount = leftEyeOpenAmount.ReadUnprocessedValueFromState(statePtr);
			result.rightEyeOpenAmount = rightEyeOpenAmount.ReadUnprocessedValueFromState(statePtr);
			return result;
		}

		public unsafe override void WriteValueIntoState(Eyes value, void* statePtr)
		{
			leftEyePosition.WriteValueIntoState(value.leftEyePosition, statePtr);
			leftEyeRotation.WriteValueIntoState(value.leftEyeRotation, statePtr);
			rightEyePosition.WriteValueIntoState(value.rightEyePosition, statePtr);
			rightEyeRotation.WriteValueIntoState(value.rightEyeRotation, statePtr);
			fixationPoint.WriteValueIntoState(value.fixationPoint, statePtr);
			leftEyeOpenAmount.WriteValueIntoState(value.leftEyeOpenAmount, statePtr);
			rightEyeOpenAmount.WriteValueIntoState(value.rightEyeOpenAmount, statePtr);
		}
	}
}
