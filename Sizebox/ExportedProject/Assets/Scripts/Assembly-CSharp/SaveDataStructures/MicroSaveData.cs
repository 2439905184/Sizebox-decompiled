using System;

namespace SaveDataStructures
{
	[Serializable]
	public class MicroSaveData : HumanoidSaveData
	{
		public override SavableDataType DataType
		{
			get
			{
				return SavableDataType.MicroData;
			}
		}

		public MicroSaveData(Micro micro)
			: base(micro)
		{
		}
	}
}
