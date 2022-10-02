using System;

namespace SaveDataStructures
{
	[Serializable]
	public class MacroSaveData : HumanoidSaveData
	{
		public override SavableDataType DataType
		{
			get
			{
				return SavableDataType.MacroData;
			}
		}

		public MacroSaveData(Giantess giantess)
			: base(giantess)
		{
		}
	}
}
