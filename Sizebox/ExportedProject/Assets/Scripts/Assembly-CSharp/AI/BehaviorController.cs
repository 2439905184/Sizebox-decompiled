using System.Collections.Generic;
using System.Diagnostics;
using SaveDataStructures;
using UnityEngine;

namespace AI
{
	public class BehaviorController
	{
		[DebuggerDisplay("{behavior.internalName}")]
		public class Command
		{
			public IBehavior behavior;

			public IBehaviorInstance instance;

			public Vector3 cursorPoint;

			public EntityBase target;

			public EntityBase agent;

			private bool hasStarted;

			public void Execute()
			{
				if (!hasStarted)
				{
					Start();
				}
			}

			public void Exit()
			{
				instance.Exit();
			}

			public void Start()
			{
				instance = behavior.CreateInstance(agent, target, cursorPoint);
				instance.Start();
				hasStarted = true;
			}

			public bool AutoFinish()
			{
				return instance.AutoFinish();
			}

			public bool IsSecondary()
			{
				return behavior.IsSecondary();
			}

			public List<string> GetFlags()
			{
				return behavior.GetFlags();
			}

			public CommandSaveData GetSaveData()
			{
				return new CommandSaveData(behavior.GetName(), target ? target.id : (-1), cursorPoint);
			}
		}

		private Humanoid agent;

		private ActionController actionController;

		private Command mainBehavior;

		private List<Command> secondaryBehaviors;

		private Queue<Command> commandQueue;

		public BehaviorController(Humanoid entity)
		{
			commandQueue = new Queue<Command>();
			secondaryBehaviors = new List<Command>();
			actionController = entity.ActionManager;
			agent = entity;
		}

		public string GetCurrentBehaviorName()
		{
			if (mainBehavior != null)
			{
				return mainBehavior.behavior.GetName();
			}
			return null;
		}

		private Command CreateCommand(BehaviorInstruction behaviorInstruction)
		{
			return new Command
			{
				behavior = behaviorInstruction.newBehavior,
				target = behaviorInstruction.target,
				cursorPoint = behaviorInstruction.cursorPoint.ToVirtual(),
				agent = agent
			};
		}

		public void SetBehaviorByName(string name, EntityBase target, Vector3 point)
		{
			if (BehaviorLists.Instance != null)
			{
				IBehavior behavior = BehaviorLists.Instance.GetBehavior(name);
				if (behavior != null)
				{
					BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
					behaviorInstruction.newBehavior = behavior;
					behaviorInstruction.target = target;
					behaviorInstruction.cursorPoint = point;
					ChangeBehavior(behaviorInstruction);
				}
			}
		}

		public void QueueBehaviorByName(string name, EntityBase target, Vector3 point)
		{
			if (BehaviorLists.Instance != null)
			{
				IBehavior behavior = BehaviorLists.Instance.GetBehavior(name);
				if (behavior != null)
				{
					BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
					behaviorInstruction.newBehavior = behavior;
					behaviorInstruction.target = target;
					behaviorInstruction.cursorPoint = point;
					ScheduleBehavior(behaviorInstruction);
				}
			}
		}

		public void SetBehaviorFromData(CommandSaveData commandData)
		{
			if (BehaviorLists.Instance != null && commandData != null && !string.IsNullOrEmpty(commandData.commandName))
			{
				IBehavior behavior = BehaviorLists.Instance.GetBehavior(commandData.commandName);
				if (behavior != null)
				{
					BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
					behaviorInstruction.newBehavior = behavior;
					behaviorInstruction.target = ObjectManager.Instance.GetById(commandData.id);
					behaviorInstruction.cursorPoint = commandData.virtualPoint.ToWorld();
					ChangeBehavior(behaviorInstruction);
				}
			}
		}

		public void QueueBehaviorFromData(CommandSaveData commandData)
		{
			if (BehaviorLists.Instance != null && commandData != null && !string.IsNullOrEmpty(commandData.commandName))
			{
				IBehavior behavior = BehaviorLists.Instance.GetBehavior(commandData.commandName);
				if (behavior != null)
				{
					BehaviorInstruction behaviorInstruction = default(BehaviorInstruction);
					behaviorInstruction.newBehavior = behavior;
					behaviorInstruction.target = ObjectManager.Instance.GetById(commandData.id);
					behaviorInstruction.cursorPoint = commandData.virtualPoint.ToWorld();
					ScheduleBehavior(behaviorInstruction);
				}
			}
		}

		public void ChangeBehavior(BehaviorInstruction behaviorInstruction)
		{
			commandQueue.Clear();
			if (!behaviorInstruction.newBehavior.IsSecondary() && mainBehavior != null)
			{
				StopMainBehavior();
			}
			ScheduleBehavior(behaviorInstruction);
		}

		public void ScheduleBehavior(BehaviorInstruction behaviorInstruction)
		{
			Command item = CreateCommand(behaviorInstruction);
			commandQueue.Enqueue(item);
		}

		public void Execute()
		{
			CheckQueueForBehaviors();
			ExecuteMainBehavior();
			ExecuteSecondaryActions();
		}

		public void FixedExecute()
		{
			FixedExecuteMainBehavior();
		}

		private void CheckQueueForBehaviors()
		{
			if (commandQueue.Count != 0)
			{
				Command command = commandQueue.Peek();
				if (command.IsSecondary())
				{
					command = commandQueue.Dequeue();
					RunSecondaryBehavior(command);
				}
				else if (!HasMainBehavior())
				{
					mainBehavior = commandQueue.Dequeue();
				}
			}
		}

		private void ExecuteMainBehavior()
		{
			if (mainBehavior != null)
			{
				mainBehavior.Execute();
				if (mainBehavior != null && mainBehavior.AutoFinish() && actionController.IsIdle())
				{
					StopMainBehavior();
				}
			}
		}

		private void FixedExecuteMainBehavior()
		{
			if (mainBehavior != null && mainBehavior != null && mainBehavior.AutoFinish() && actionController.IsIdle())
			{
				StopMainBehavior();
			}
		}

		private void ExecuteSecondaryActions()
		{
			for (int i = 0; i < secondaryBehaviors.Count; i++)
			{
				secondaryBehaviors[i].Execute();
			}
		}

		private void RunSecondaryBehavior(Command command)
		{
			foreach (string flag in command.GetFlags())
			{
				StopSecondaryBehavior(flag);
			}
			secondaryBehaviors.Add(command);
		}

		public void StopSecondaryBehavior(string flag)
		{
			foreach (Command secondaryBehavior in secondaryBehaviors)
			{
				foreach (string flag2 in secondaryBehavior.GetFlags())
				{
					if (flag == flag2)
					{
						if (GlobalPreferences.ScriptAuxLogging.value)
						{
							UnityEngine.Debug.Log("Flag match: " + flag);
						}
						secondaryBehaviors.Remove(secondaryBehavior);
						secondaryBehavior.Exit();
						return;
					}
				}
			}
		}

		public void StopMainBehavior()
		{
			if (mainBehavior != null)
			{
				actionController.ClearAll();
				mainBehavior.Exit();
				mainBehavior = null;
			}
		}

		public void StopAllBehaviors()
		{
			StopMainBehavior();
			foreach (Command secondaryBehavior in secondaryBehaviors)
			{
				secondaryBehavior.Exit();
			}
			secondaryBehaviors.Clear();
		}

		public bool HasMainBehavior()
		{
			return mainBehavior != null;
		}

		public bool IsIdle()
		{
			if (commandQueue.Count == 0)
			{
				return !HasMainBehavior();
			}
			return false;
		}

		public bool IsQueueEmpty()
		{
			return commandQueue.Count == 0;
		}

		public void ClearQueue()
		{
			commandQueue.Clear();
		}

		public BehaviorSaveData GetSaveData()
		{
			return new BehaviorSaveData(mainBehavior, secondaryBehaviors, commandQueue);
		}

		public void LoadSaveData(BehaviorSaveData data)
		{
			if (data == null)
			{
				return;
			}
			if (data.secondaryData != null)
			{
				foreach (CommandSaveData secondaryDatum in data.secondaryData)
				{
					SetBehaviorFromData(secondaryDatum);
				}
			}
			SetBehaviorFromData(data.mainData);
			if (data.queueData == null)
			{
				return;
			}
			foreach (CommandSaveData queueDatum in data.queueData)
			{
				QueueBehaviorFromData(queueDatum);
			}
		}
	}
}
