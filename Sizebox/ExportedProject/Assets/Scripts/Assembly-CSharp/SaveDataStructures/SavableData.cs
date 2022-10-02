using System;

namespace SaveDataStructures
{
	[Serializable]
	public abstract class SavableData
	{
		public abstract SavableDataType DataType { get; }
	}
}
