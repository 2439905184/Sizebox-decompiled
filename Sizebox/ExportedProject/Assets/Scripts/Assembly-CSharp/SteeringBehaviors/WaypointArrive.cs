using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviors
{
	public class WaypointArrive : SteerBehavior
	{
		private readonly Queue<Kinematic> _waypointsQueue;

		private Kinematic _currentPoint;

		private readonly Arrive _goalArrive;

		private Seek _seekWaypoint;

		private float _lastPathFind = -99f;

		private float timeTolerance = 1f;

		private float _width;

		private bool _ignorePathfinding;

		private int maxTry = 10;

		private int _tries;

		private float _scale = 1.6f;

		private float seekWeight = 2f;

		private float arriveWeight = 0.5f;

		public WaypointArrive(MovementCharacter agent, Kinematic target)
			: base(agent, target)
		{
			weight = 2f;
			_goalArrive = new Arrive(agent, target);
			_waypointsQueue = new Queue<Kinematic>();
			_width = agent.tileWidth;
		}

		public override bool GetSteering(out SteeringOutput steeringOutput)
		{
			_scale = agent.Entity.Height;
			float num = _scale * _width * 4f;
			if (_seekWaypoint == null)
			{
				if (_waypointsQueue.Count > 0)
				{
					_currentPoint = _waypointsQueue.Dequeue();
					if ((bool)_currentPoint)
					{
						Debug.DrawRay(_currentPoint.position, Vector3.up * _scale / 5f, Color.yellow, 5f);
						_seekWaypoint = new Seek(agent, _currentPoint);
						weight = seekWeight;
						return _seekWaypoint.GetSteering(out steeringOutput);
					}
				}
				else
				{
					if (!target)
					{
						_seekWaypoint = null;
						steeringOutput = null;
						agent.Stop();
						return true;
					}
					if ((target.position - agent.transform.position).magnitude > num * 1.2f)
					{
						FindPathToTarget();
					}
				}
				weight = arriveWeight;
				return _goalArrive.GetSteering(out steeringOutput);
			}
			weight = seekWeight;
			SteeringOutput steeringOutput2;
			_seekWaypoint.GetSteering(out steeringOutput2);
			if ((agent.transform.position - _currentPoint.position).magnitude < num)
			{
				_seekWaypoint = null;
			}
			steeringOutput = steeringOutput2;
			return true;
		}

		internal void PurgeSteeringQueue()
		{
			_waypointsQueue.Clear();
			_seekWaypoint = null;
		}

		internal int QueueLength()
		{
			return _waypointsQueue.Count;
		}

		private void FindPathToTarget()
		{
			float time = Time.time;
			if (_ignorePathfinding)
			{
				weight = 0.5f;
				return;
			}
			if (time < _lastPathFind + timeTolerance)
			{
				_width *= 1.5f;
				_tries++;
			}
			else
			{
				_tries = 0;
				_width = agent.tileWidth;
			}
			if (_tries > maxTry)
			{
				_ignorePathfinding = true;
			}
			_lastPathFind = time;
			_waypointsQueue.Clear();
			List<Kinematic> list = Pathfinder.instance.PlanRoute(agent.transform.position, target.position, _scale, _scale * _width, _scale * 0.1f, 0f);
			if (list.Count <= 0)
			{
				return;
			}
			foreach (Kinematic item in list)
			{
				_waypointsQueue.Enqueue(item);
			}
		}
	}
}
