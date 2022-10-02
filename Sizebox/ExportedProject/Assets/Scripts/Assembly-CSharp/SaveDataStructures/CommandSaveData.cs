using System;
using UnityEngine;

namespace SaveDataStructures
{
	[Serializable]
	public class CommandSaveData
	{
		public string commandName;

		public int id;

		public Vector3 virtualPoint;

		public CommandSaveData(string commandName, int id, Vector3 virtualPoint)
		{
			this.commandName = commandName;
			this.id = id;
			this.virtualPoint = virtualPoint;
		}
	}
}
