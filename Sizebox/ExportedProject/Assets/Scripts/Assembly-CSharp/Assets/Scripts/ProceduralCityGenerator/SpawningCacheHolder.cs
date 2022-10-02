namespace Assets.Scripts.ProceduralCityGenerator
{
	internal class SpawningCacheHolder
	{
		public bool NeedsReInit { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public SpawningCacheHolder()
		{
			NeedsReInit = true;
		}
	}
}
