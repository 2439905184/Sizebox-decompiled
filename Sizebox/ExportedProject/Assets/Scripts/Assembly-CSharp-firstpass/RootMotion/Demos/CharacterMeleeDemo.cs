using System;
using RootMotion.Dynamics;
using UnityEngine;

namespace RootMotion.Demos
{
	public class CharacterMeleeDemo : CharacterPuppet
	{
		[Serializable]
		public class Action
		{
			[Serializable]
			public class Anim
			{
				public string stateName;

				public int layer;

				public float transitionDuration;

				public float fixedTime;
			}

			public string name;

			public int inputActionIndex = 1;

			public float duration;

			public float minFrequency;

			public Anim anim;

			public int[] requiredPropTypes;

			public Booster[] boosters;
		}

		[Header("Melee")]
		public Action[] actions;

		[HideInInspector]
		public int currentActionIndex = -1;

		[HideInInspector]
		public float lastActionTime;

		private float lastActionMoveMag;

		public Action currentAction
		{
			get
			{
				if (currentActionIndex < 0)
				{
					return null;
				}
				return actions[currentActionIndex];
			}
		}

		protected override void Start()
		{
			currentActionIndex = -1;
			lastActionTime = 0f;
			base.Start();
		}

		protected override void Update()
		{
			if (base.puppet.state == BehaviourPuppet.State.Puppet)
			{
				for (int i = 0; i < actions.Length; i++)
				{
					if (StartAction(actions[i]))
					{
						currentActionIndex = i;
						Booster[] boosters = actions[i].boosters;
						for (int j = 0; j < boosters.Length; j++)
						{
							boosters[j].Boost(base.puppet);
						}
						if (propRoot.currentProp is PropMelee)
						{
							(propRoot.currentProp as PropMelee).StartAction(actions[i].duration);
						}
						lastActionTime = Time.time;
						lastActionMoveMag = moveDirection.magnitude;
					}
				}
			}
			if (Time.time < lastActionTime + 0.5f)
			{
				moveDirection = moveDirection.normalized * Mathf.Max(moveDirection.magnitude, lastActionMoveMag);
			}
			base.Update();
		}

		private bool StartAction(Action action)
		{
			if (Time.time < lastActionTime + action.minFrequency)
			{
				return false;
			}
			if (userControl.state.actionIndex != action.inputActionIndex)
			{
				return false;
			}
			if (action.requiredPropTypes.Length != 0)
			{
				if (propRoot.currentProp == null && action.requiredPropTypes[0] == -1)
				{
					return true;
				}
				if (propRoot.currentProp == null)
				{
					return false;
				}
				bool flag = false;
				for (int i = 0; i < action.requiredPropTypes.Length; i++)
				{
					if (action.requiredPropTypes[i] == propRoot.currentProp.propType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
	}
}
