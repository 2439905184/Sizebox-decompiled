using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AI
{
	public class MicroDecisionMaker : DecisionMaker
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public bool onlyReactive;

			internal bool _003CMakeDecision_003Eb__0(IBehavior beh)
			{
				if (beh.IsAI())
				{
					if (!beh.IsReactive())
					{
						return !onlyReactive;
					}
					return true;
				}
				return false;
			}
		}

		private float _lastCheck;

		private float _checkInterval = 1f;

		private IBehavior _lastDecision;

		public MicroDecisionMaker(Humanoid entity)
			: base(entity)
		{
		}

		public override void Execute()
		{
			if (!Entity.locked && Entity.CanDecide())
			{
				if (AI.IsIdle())
				{
					MakeDecision();
				}
				else if (_lastDecision != null && !_lastDecision.IsReactive() && Time.time > _lastCheck + _checkInterval)
				{
					MakeDecision(true);
				}
			}
		}

		private void MakeDecision(bool onlyReactive = false)
		{
			_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = new _003C_003Ec__DisplayClass5_0();
			_003C_003Ec__DisplayClass5_.onlyReactive = onlyReactive;
			_lastCheck = Time.time;
			AI.mentalState.Update();
			EntityBase entityBase = AI.mentalState.ChooseTarget();
			List<TargetType> agentDefs = BehaviorLists.Instance.FindDefs(Entity);
			List<TargetType> list = BehaviorLists.Instance.FindDefs(entityBase);
			list.Add(TargetType.Oneself);
			list.Add(TargetType.None);
			List<IBehavior> list2 = BehaviorLists.Instance.FindBehaviors(agentDefs, list);
			list2 = list2.FindAll(_003C_003Ec__DisplayClass5_._003CMakeDecision_003Eb__0);
			IBehavior behavior = ChooseBehavior(list2);
			if (behavior != null)
			{
				_lastDecision = behavior;
				if (_003C_003Ec__DisplayClass5_.onlyReactive)
				{
					AI.ImmediateCommand(_lastDecision, entityBase, GetRandomPoint());
				}
				else
				{
					AI.ScheduleCommand(_lastDecision, entityBase, GetRandomPoint());
				}
			}
		}
	}
}
