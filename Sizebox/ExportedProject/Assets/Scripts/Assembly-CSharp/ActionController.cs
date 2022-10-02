using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ActionController
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action<AgentAction> _003C_003E9__5_0;

		public static Action<AgentAction> _003C_003E9__6_0;

		public static Action<AgentAction> _003C_003E9__8_0;

		internal void _003CExecute_003Eb__5_0(AgentAction action)
		{
			action.Execute();
		}

		internal void _003CFixedExecute_003Eb__6_0(AgentAction action)
		{
			action.FixedExecute();
		}

		internal void _003CClearAll_003Eb__8_0(AgentAction action)
		{
			action.Interrupt();
		}
	}

	private readonly Humanoid _agent;

	private readonly List<AgentAction> _active = new List<AgentAction>();

	private readonly LinkedList<AgentAction> _queue;

	public ActionController(Humanoid agent)
	{
		_queue = new LinkedList<AgentAction>();
		_agent = agent;
	}

	public void ScheduleAction(AgentAction action)
	{
		action.agent = _agent;
		if (_agent.isGiantess)
		{
			_agent.GetComponent<Giantess>().gtsMovement.doNotMoveGts = false;
		}
		if (action.priority)
		{
			_queue.AddFirst(action);
		}
		else
		{
			_queue.AddLast(action);
		}
	}

	public void Execute()
	{
		if (_queue.Count != 0)
		{
			AgentAction value = _queue.First.Value;
			bool flag = true;
			foreach (AgentAction item in _active)
			{
				flag &= item.CanDoBoth(value) || value.CanDoBoth(item);
			}
			if (flag)
			{
				_queue.RemoveFirst();
				_active.Add(value);
			}
		}
		for (int i = 0; i < _active.Count; i++)
		{
			if (_active[i].IsCompleted())
			{
				EventManager.SendEvent(new ActionCompleteEvent(_agent, _active[i]));
				_active.RemoveAt(i--);
			}
		}
		_active.ForEach(_003C_003Ec._003C_003E9__5_0 ?? (_003C_003Ec._003C_003E9__5_0 = _003C_003Ec._003C_003E9._003CExecute_003Eb__5_0));
	}

	public void FixedExecute()
	{
		for (int i = 0; i < _active.Count; i++)
		{
			if (_active[i].IsCompleted())
			{
				EventManager.SendEvent(new ActionCompleteEvent(_agent, _active[i]));
				_active.RemoveAt(i--);
			}
		}
		_active.ForEach(_003C_003Ec._003C_003E9__6_0 ?? (_003C_003Ec._003C_003E9__6_0 = _003C_003Ec._003C_003E9._003CFixedExecute_003Eb__6_0));
	}

	public void ClearQueue()
	{
		_queue.Clear();
	}

	public void ClearAll()
	{
		ClearQueue();
		_active.ForEach(_003C_003Ec._003C_003E9__8_0 ?? (_003C_003Ec._003C_003E9__8_0 = _003C_003Ec._003C_003E9._003CClearAll_003Eb__8_0));
		_active.Clear();
	}

	public bool IsIdle()
	{
		if (_queue.Count == 0)
		{
			return _active.Count == 0;
		}
		return false;
	}

	public bool IsQueueEmpty()
	{
		return _queue.Count == 0;
	}
}
