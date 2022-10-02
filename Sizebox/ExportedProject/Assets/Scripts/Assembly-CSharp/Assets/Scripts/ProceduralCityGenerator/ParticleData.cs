using UnityEngine;

namespace Assets.Scripts.ProceduralCityGenerator
{
	public class ParticleData
	{
		public readonly Transform particleRoot;

		public readonly ParticleSystem system;

		public ParticleSystem.MainModule main;

		public ParticleSystem.ShapeModule shape;

		public ParticleData(GameObject root)
		{
			particleRoot = root.transform;
			system = root.transform.GetChild(0).GetComponent<ParticleSystem>();
			main = system.main;
			shape = system.shape;
		}
	}
}
