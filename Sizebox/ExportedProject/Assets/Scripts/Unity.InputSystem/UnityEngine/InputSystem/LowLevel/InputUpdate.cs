using System;

namespace UnityEngine.InputSystem.LowLevel
{
	internal static class InputUpdate
	{
		[Serializable]
		public struct SerializedState
		{
			public InputUpdateType lastUpdateType;

			public uint updateStepCount;

			public uint lastUpdateRetainedEventBytes;

			public uint lastUpdateRetainedEventCount;
		}

		public static InputUpdateType s_LastUpdateType;

		public static uint s_UpdateStepCount;

		public static uint s_LastUpdateRetainedEventBytes;

		public static uint s_LastUpdateRetainedEventCount;

		public static SerializedState Save()
		{
			SerializedState result = default(SerializedState);
			result.lastUpdateType = s_LastUpdateType;
			result.updateStepCount = s_UpdateStepCount;
			result.lastUpdateRetainedEventBytes = s_LastUpdateRetainedEventBytes;
			result.lastUpdateRetainedEventCount = s_LastUpdateRetainedEventCount;
			return result;
		}

		public static void Restore(SerializedState state)
		{
			s_LastUpdateType = state.lastUpdateType;
			s_UpdateStepCount = state.updateStepCount;
			s_LastUpdateRetainedEventBytes = state.lastUpdateRetainedEventBytes;
			s_LastUpdateRetainedEventCount = state.lastUpdateRetainedEventCount;
		}
	}
}
