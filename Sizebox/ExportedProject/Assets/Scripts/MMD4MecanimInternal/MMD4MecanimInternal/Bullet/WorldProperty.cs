using System;
using UnityEngine;

namespace MMD4MecanimInternal.Bullet
{
	[Serializable]
	public class WorldProperty
	{
		public bool accurateStep = true;

		public int framePerSecond;

		public int resetFrameRate;

		public int limitDeltaFrames;

		public float axisSweepDistance;

		public float gravityScale = 10f;

		public float gravityNoise;

		public Vector3 gravityDirection = new Vector3(0f, -1f, 0f);

		public float vertexScale = 8f;

		public float importScale = 0.01f;

		public int worldSolverInfoNumIterations;

		public bool worldSolverInfoSplitImpulse = true;

		public bool worldAddFloorPlane;

		public bool optimizeSettings = true;

		public bool multiThreading = true;

		public bool parallelDispatcher;

		public bool parallelSolver;

		public float worldScale
		{
			get
			{
				return vertexScale * importScale;
			}
		}
	}
}
