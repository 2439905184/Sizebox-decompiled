using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.XR
{
	[Preserve]
	public class BoneControl : InputControl<Bone>
	{
		[Preserve]
		[InputControl(offset = 0u, displayName = "parentBoneIndex")]
		public IntegerControl parentBoneIndex { get; private set; }

		[Preserve]
		[InputControl(offset = 4u, displayName = "Position")]
		public Vector3Control position { get; private set; }

		[Preserve]
		[InputControl(offset = 16u, displayName = "Rotation")]
		public QuaternionControl rotation { get; private set; }

		protected override void FinishSetup()
		{
			parentBoneIndex = GetChildControl<IntegerControl>("parentBoneIndex");
			position = GetChildControl<Vector3Control>("position");
			rotation = GetChildControl<QuaternionControl>("rotation");
			base.FinishSetup();
		}

		public unsafe override Bone ReadUnprocessedValueFromState(void* statePtr)
		{
			Bone result = default(Bone);
			result.parentBoneIndex = (uint)parentBoneIndex.ReadUnprocessedValueFromState(statePtr);
			result.position = position.ReadUnprocessedValueFromState(statePtr);
			result.rotation = rotation.ReadUnprocessedValueFromState(statePtr);
			return result;
		}

		public unsafe override void WriteValueIntoState(Bone value, void* statePtr)
		{
			parentBoneIndex.WriteValueIntoState((int)value.parentBoneIndex, statePtr);
			position.WriteValueIntoState(value.position, statePtr);
			rotation.WriteValueIntoState(value.rotation, statePtr);
		}
	}
}
