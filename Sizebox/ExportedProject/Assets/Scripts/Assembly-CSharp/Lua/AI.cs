using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class AI
	{
		private Humanoid entity;

		[MoonSharpHidden]
		public AI(Humanoid entity)
		{
			if (entity == null)
			{
				Debug.LogError("Creating AI with no entity");
			}
			this.entity = entity;
		}

		public void StopAction()
		{
			entity.ActionManager.ClearAll();
		}

		public void CancelQueuedActions()
		{
			entity.ActionManager.ClearQueue();
		}

		public void CancelQueuedBehaviors()
		{
			entity.ai.behaviorController.ClearQueue();
		}

		public bool HasQueuedActions()
		{
			return !entity.ActionManager.IsQueueEmpty();
		}

		public bool HasQueuedBehaviors()
		{
			return !entity.ai.behaviorController.IsQueueEmpty();
		}

		public bool IsActionActive()
		{
			return !entity.ActionManager.IsIdle();
		}

		public bool IsBehaviorActive()
		{
			return !entity.ai.behaviorController.IsIdle();
		}

		public void StopBehavior()
		{
			if (!entity.ai.behaviorController.HasMainBehavior())
			{
				Debug.Log("StopBehavior: No main behavior to stop");
			}
			else
			{
				entity.ai.behaviorController.StopMainBehavior();
			}
		}

		public void StopSecondaryBehavior(string flag)
		{
			entity.ai.behaviorController.StopSecondaryBehavior(flag);
		}

		public void DisableAI()
		{
			entity.ai.DisableAI();
		}

		public void EnableAI()
		{
			entity.ai.EnableAI();
		}

		public bool IsAIEnabled()
		{
			return entity.ai.IsEnabled();
		}

		public void SetBehavior(string name)
		{
			entity.ai.behaviorController.SetBehaviorByName(name, null, UnityEngine.Vector3.zero);
		}

		public void SetBehavior(string name, Entity target)
		{
			entity.ai.behaviorController.SetBehaviorByName(name, target.entity, UnityEngine.Vector3.zero);
		}

		public void SetBehavior(string name, Vector3 position)
		{
			entity.ai.behaviorController.SetBehaviorByName(name, null, position.virtualPosition);
		}

		public void QueueBehavior(string name)
		{
			entity.ai.behaviorController.QueueBehaviorByName(name, null, UnityEngine.Vector3.zero);
		}

		public void QueueBehavior(string name, Entity target)
		{
			entity.ai.behaviorController.QueueBehaviorByName(name, target.entity, UnityEngine.Vector3.zero);
		}

		public void QueueBehavior(string name, Vector3 position)
		{
			entity.ai.behaviorController.QueueBehaviorByName(name, null, position.virtualPosition);
		}
	}
}
