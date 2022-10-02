using System;

namespace SaveDataStructures
{
	[Serializable]
	public class CitySaveData : EntitySaveData
	{
		public int seed;

		public bool isPlaced;

		public float populationOffset;

		public int radius;

		public float buildingHeight;

		public bool populate;

		public override SavableDataType DataType
		{
			get
			{
				return SavableDataType.CityData;
			}
		}

		public CitySaveData(CityBuilder city, int seed, bool isPlaced, float populationOffset, int radius, float buildingHeight, bool populate)
			: base(city)
		{
			this.seed = seed;
			this.isPlaced = isPlaced;
			this.populationOffset = populationOffset;
			this.radius = radius;
			this.buildingHeight = buildingHeight;
			this.populate = populate;
		}
	}
}
