using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AI
{
	public class GtsDecisionMaker : DecisionMaker
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Predicate<IBehavior> _003C_003E9__2_0;

			internal bool _003CMakeDecision_003Eb__2_0(IBehavior beh)
			{
				return beh.IsAI();
			}
		}

		public GtsDecisionMaker(Humanoid entity)
			: base(entity)
		{
		}

		public override void Execute()
		{
			if (AI.IsIdle())
			{
				MakeDecision();
			}
		}

		private void MakeDecision()
		{
			List<TargetType> agentDefs = BehaviorLists.Instance.FindDefs(Entity);
			List<TargetType> list = new List<TargetType>();
			list.Add(TargetType.Oneself);
			list.Add(TargetType.None);
			Player player = GameController.LocalClient.Player;
			if ((bool)player.Entity && player.Entity.IsTargetAble())
			{
				list.Add(TargetType.Player);
			}
			Micro randomMicro = MicroManager.GetRandomMicro(Entity);
			if ((bool)randomMicro)
			{
				list.Add(TargetType.Micro);
			}
			Giantess randomGiantess = GetRandomGiantess();
			if ((bool)randomGiantess)
			{
				list.Add(TargetType.Giantess);
			}
			List<IBehavior> list2 = BehaviorLists.Instance.FindBehaviors(agentDefs, list);
			list2 = list2.FindAll(_003C_003Ec._003C_003E9__2_0 ?? (_003C_003Ec._003C_003E9__2_0 = _003C_003Ec._003C_003E9._003CMakeDecision_003Eb__2_0));
			if (list2.Count == 0)
			{
				return;
			}
			IBehavior behavior = ChooseBehavior(list2);
			if (behavior == null)
			{
				return;
			}
			if (GlobalPreferences.ScriptAuxLogging.value)
			{
				Debug.Log("Giantess Behavior: " + behavior.GetName());
			}
			List<EntityBase> list3 = new List<EntityBase>();
			foreach (TargetType item in behavior.GetTargetDef().include)
			{
				if (item == TargetType.Micro && (bool)randomMicro)
				{
					list3.Add(randomMicro);
				}
				else if (item == TargetType.Player && (bool)player.Entity)
				{
					list3.Add(player.Entity);
				}
				else if (item == TargetType.Giantess && (bool)randomGiantess)
				{
					list3.Add(randomGiantess);
				}
				else if (item == TargetType.Humanoid)
				{
					if ((bool)randomMicro)
					{
						list3.Add(randomMicro);
					}
					if ((bool)randomGiantess)
					{
						list3.Add(randomGiantess);
					}
				}
			}
			EntityBase target = null;
			if (list3.Count > 0)
			{
				target = list3[UnityEngine.Random.Range(0, list3.Count)];
			}
			AI.ScheduleCommand(behavior, target, GetRandomPoint());
		}

		private Giantess GetRandomGiantess()
		{
			List<Giantess> list = ObjectManager.Instance.GiantessDictionary.Values.ToList();
			int count = list.Count;
			if (count > 0)
			{
				return list[UnityEngine.Random.Range(0, count)];
			}
			return null;
		}
	}
}
