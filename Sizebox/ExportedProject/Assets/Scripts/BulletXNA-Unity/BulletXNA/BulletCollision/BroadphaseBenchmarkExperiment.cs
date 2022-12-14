namespace BulletXNA.BulletCollision
{
	public struct BroadphaseBenchmarkExperiment
	{
		public string name;

		public int object_count;

		public int update_count;

		public int spawn_count;

		public int iterations;

		public float speed;

		public float amplitude;

		public BroadphaseBenchmarkExperiment(string name, int object_count, int update_count, int spawn_count, int iterations, float speed, float amplitude)
		{
			this.name = name;
			this.object_count = object_count;
			this.update_count = update_count;
			this.spawn_count = spawn_count;
			this.iterations = iterations;
			this.speed = speed;
			this.amplitude = amplitude;
		}
	}
}
