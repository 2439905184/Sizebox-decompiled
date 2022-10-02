using UnityEngine;

namespace SteeringBehaviors
{
	public class AvoidWall : Seek
	{
		private readonly SteeringOutput _zeroSteering;

		private float avoidDistance = 10f;

		private float lookAhead = 5f;

		private readonly ObstacleDetector _obstacleDetector;

		private float AvoidDistance
		{
			get
			{
				return avoidDistance * agent.scale;
			}
		}

		public float LookAhead
		{
			get
			{
				return lookAhead * agent.scale;
			}
		}

		public AvoidWall(MovementCharacter agent, ObstacleDetector obsDetector)
			: base(agent, null)
		{
			_obstacleDetector = obsDetector;
			target = new VectorKinematic(Vector3.zero);
			weight = 1.8f;
			_zeroSteering = new SteeringOutput();
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			if ((bool)_obstacleDetector && _obstacleDetector.CheckObstacle())
			{
				target.position = _obstacleDetector.GetPoint() + _obstacleDetector.GetNormal() * AvoidDistance;
				if (base.GetSteering(out steeringOutput))
				{
					return true;
				}
			}
			steeringOutput = _zeroSteering;
			return true;
		}
	}
}
