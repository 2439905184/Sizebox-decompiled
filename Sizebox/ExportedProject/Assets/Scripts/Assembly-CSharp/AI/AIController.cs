using SaveDataStructures;
using UnityEngine;

namespace AI
{
	public class AIController : EntityComponent
	{
		private Humanoid agent;

		public MentalState mentalState;

		public DecisionMaker decisionMaker;

		public BehaviorController behaviorController;

		public ActionController actionController;

		private bool ai;

		private void Update()
		{
			if (base.initialized)
			{
				Execute();
			}
		}

		private void FixedUpdate()
		{
			if (base.initialized)
			{
				FixedExecute();
			}
		}

		public override void Initialize(EntityBase entity)
		{
			agent = entity as Humanoid;
			if (!agent)
			{
				Debug.LogError("AIController requires a humanoid entity.");
				return;
			}
			if ((bool)(agent as Micro))
			{
				decisionMaker = new MicroDecisionMaker(agent);
			}
			else if ((bool)(agent as Giantess))
			{
				decisionMaker = new GtsDecisionMaker(agent);
			}
			behaviorController = new BehaviorController(agent);
			actionController = agent.ActionManager;
			mentalState = new MentalState(agent);
			base.initialized = true;
		}

		public void EnableAI()
		{
			if (!IsEnabled())
			{
				ai = true;
			}
		}

		public bool IsEnabled()
		{
			return ai;
		}

		public void DisableAI()
		{
			if (IsEnabled())
			{
				ai = false;
				if (behaviorController != null && !behaviorController.IsIdle())
				{
					behaviorController.StopMainBehavior();
				}
			}
		}

		public void Execute()
		{
			if (BehaviorLists.Instance != null)
			{
				if (ai && decisionMaker != null)
				{
					decisionMaker.Execute();
				}
				if (behaviorController != null)
				{
					behaviorController.Execute();
				}
				if (actionController != null)
				{
					actionController.Execute();
				}
			}
		}

		public void FixedExecute()
		{
			if (BehaviorLists.Instance != null)
			{
				if (behaviorController != null)
				{
					behaviorController.FixedExecute();
				}
				if (actionController != null)
				{
					actionController.FixedExecute();
				}
			}
		}

		public void ImmediateCommand(IBehavior newBehavior, EntityBase target, Vector3 cursorPoint)
		{
			BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
			behaviorInstruction.newBehavior = newBehavior;
			behaviorInstruction.target = target;
			behaviorInstruction.cursorPoint = cursorPoint;
			behaviorController.ChangeBehavior(behaviorInstruction);
		}

		public void ScheduleCommand(IBehavior newBehavior, EntityBase target, Vector3 cursorPoint)
		{
			BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
			behaviorInstruction.newBehavior = newBehavior;
			behaviorInstruction.target = target;
			behaviorInstruction.cursorPoint = cursorPoint;
			behaviorController.ScheduleBehavior(behaviorInstruction);
		}

		public bool IsIdle()
		{
			return behaviorController.IsIdle();
		}

		private void OnEnable()
		{
			if (behaviorController != null)
			{
				behaviorController.StopMainBehavior();
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			if ((bool)agent)
			{
				Gizmos.DrawWireSphere(agent.transform.position, 1f);
			}
		}

		public AiSaveData GetSaveData()
		{
			return new AiSaveData(ai, behaviorController.GetSaveData());
		}

		public void LoadSaveData(AiSaveData data)
		{
			if (data.aiEnabled)
			{
				EnableAI();
			}
			else
			{
				DisableAI();
			}
			behaviorController.LoadSaveData(data.behaviorData);
		}
	}
}
