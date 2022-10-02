using System;
using System.Collections.Generic;
using AI;

namespace SaveDataStructures
{
	[Serializable]
	public class BehaviorSaveData
	{
		public CommandSaveData mainData;

		public List<CommandSaveData> secondaryData;

		public List<CommandSaveData> queueData;

		public BehaviorSaveData(BehaviorController.Command main, List<BehaviorController.Command> secondary, Queue<BehaviorController.Command> queue)
		{
			if (main != null)
			{
				mainData = main.GetSaveData();
			}
			secondaryData = new List<CommandSaveData>();
			foreach (BehaviorController.Command item in secondary)
			{
				secondaryData.Add(item.GetSaveData());
			}
			BehaviorController.Command[] array = queue.ToArray();
			queueData = new List<CommandSaveData>();
			BehaviorController.Command[] array2 = array;
			foreach (BehaviorController.Command command in array2)
			{
				queueData.Add(command.GetSaveData());
			}
		}
	}
}
