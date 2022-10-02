using SteeringBehaviors;
using UnityEngine;

internal class ChaseAction : AgentAction
{
	private EntityBase entity;

	private Kinematic target;

	private Kinematic self;

	private bool mComplete;

	private int countdown;

	private bool mInitialCheck;

	private float gtsSlowDownFactor = 2f;

	private float newGtsSpeed;

	private WaypointArrive mSteering;

	private const int cycleTime = 60;

	public void GtsSpeedUpdate()
	{
		agent.Movement.currentSpeedDivider = newGtsSpeed;
	}

	public ChaseAction(Kinematic target, Kinematic self, EntityBase entity)
	{
		this.target = target;
		this.self = self;
		this.entity = entity;
	}

	public override void StartAction()
	{
		Humanoid humanoid = entity as Humanoid;
		if (!entity || ((bool)humanoid && humanoid.IsDead))
		{
			mComplete = true;
			return;
		}
		mSteering = (WaypointArrive)agent.Movement.StartArriveBehavior(new VectorKinematic(GetExpectedInterceptPoint()));
		if (IsInDistance())
		{
			mComplete = true;
			return;
		}
		mComplete = false;
		agent.Movement.move = true;
		countdown = 60;
	}

	public override void UpdateAction()
	{
		Humanoid humanoid = entity as Humanoid;
		if (!entity || ((bool)humanoid && humanoid.IsDead))
		{
			mComplete = true;
			agent.Movement.move = false;
		}
		else
		{
			if (IsInDistance() || !ShouldExecute())
			{
				return;
			}
			Vector3 expectedInterceptPoint = GetExpectedInterceptPoint();
			if (expectedInterceptPoint != Vector3.zero)
			{
				if (mSteering == null || mSteering.QueueLength() == 0)
				{
					mSteering = (WaypointArrive)agent.Movement.StartArriveBehavior(new VectorKinematic(expectedInterceptPoint));
					return;
				}
				mSteering.PurgeSteeringQueue();
				mSteering.target = new VectorKinematic(expectedInterceptPoint);
			}
			else
			{
				mComplete = true;
				mSteering = null;
			}
		}
	}

	private bool IsInDistance()
	{
		newGtsSpeed = 1f;
		if (mSteering != null)
		{
			float num = mSteering.agent.Entity.Height * mSteering.agent.tileWidth * 2f;
			float num2 = mSteering.agent.Entity.Height * mSteering.agent.tileWidth * 8f;
			if (Vector3.Distance(target.position, self.position) < num)
			{
				agent.Movement.move = false;
				mComplete = true;
				mSteering.PurgeSteeringQueue();
				mSteering = null;
				GtsSpeedUpdate();
				return true;
			}
			if (Vector3.Distance(target.position, self.position) < num2)
			{
				float num3 = Vector3.Distance(target.position, self.position) / num2;
				if (num3 < 0f || num3 > 1f)
				{
					Debug.Log("Targeting math ended with an incorrect range 0 <> 1");
				}
				newGtsSpeed = 1f + (1f - num3) * gtsSlowDownFactor;
			}
		}
		GtsSpeedUpdate();
		return false;
	}

	public override bool IsCompleted()
	{
		if (mComplete)
		{
			agent.Movement.Stop();
		}
		return mComplete;
	}

	public override void Interrupt()
	{
		agent.Movement.Stop();
	}

	private bool ShouldExecute()
	{
		if (agent.Movement.currentSpeedDivider < 0.5f)
		{
			countdown = 0;
			return true;
		}
		if (target != null && Vector2.Distance(new Vector2(self.position.x, self.position.z), new Vector2(target.position.x, target.position.z)) < 10f)
		{
			countdown = 0;
			return true;
		}
		if (countdown == 60)
		{
			countdown = 0;
			return true;
		}
		countdown++;
		return false;
	}

	private Vector3 GetExpectedInterceptPoint()
	{
		float y = target.position.y;
		Vector2 vector = new Vector2(self.position.x, self.position.z);
		Vector2 vector2 = new Vector2(target.position.x, target.position.z);
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 vector3 = InterceptionCalculator.CalculateInterceptionCourse(vector2, zero, vector, zero2);
		return new Vector3(vector3.x, y, vector3.y);
	}
}
