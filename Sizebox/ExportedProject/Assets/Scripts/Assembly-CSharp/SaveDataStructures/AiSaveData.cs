using System;

namespace SaveDataStructures
{
	[Serializable]
	public class AiSaveData
	{
		public bool aiEnabled;

		public BehaviorSaveData behaviorData;

		public AiSaveData(bool aiEnabled, BehaviorSaveData behaviorData)
		{
			this.aiEnabled = aiEnabled;
			this.behaviorData = behaviorData;
		}
	}
}
