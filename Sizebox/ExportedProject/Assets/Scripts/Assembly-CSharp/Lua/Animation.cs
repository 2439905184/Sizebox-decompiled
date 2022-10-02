using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Animation
	{
		private Humanoid entity;

		private AnimationManager animation;

		public float minSpeed
		{
			get
			{
				return animation.minSpeed;
			}
			set
			{
				animation.minSpeed = value;
			}
		}

		public float maxSpeed
		{
			get
			{
				return animation.maxSpeed;
			}
			set
			{
				animation.maxSpeed = value;
			}
		}

		public float transitionDuration
		{
			get
			{
				return animation.transitionDuration;
			}
			set
			{
				animation.transitionDuration = value;
			}
		}

		public float speedMultiplier
		{
			get
			{
				return animation.speedMultiplier;
			}
		}

		[MoonSharpHidden]
		public Animation(Humanoid e)
		{
			if (e == null)
			{
				Debug.LogError("Creating Animation with no entity");
			}
			entity = e;
			animation = e.animationManager;
		}

		public void Set(string animationName)
		{
			if (entity.ActionManager != null)
			{
				entity.ActionManager.ScheduleAction(new AnimationAction(animationName));
			}
			else
			{
				animation.PlayAnimation(animationName);
			}
		}

		public void SetAndWait(string animationName)
		{
			if (entity.ActionManager != null)
			{
				entity.ActionManager.ScheduleAction(new AnimationAction(animationName, true, true));
			}
			Giantess component = entity.GetComponent<Giantess>();
			if (component != null)
			{
				component.gtsMovement.doNotMoveGts = true;
			}
		}

		public string Get()
		{
			return animation.nameAnimation;
		}

		public void SetPose(string poseName)
		{
			if (entity.ActionManager != null)
			{
				entity.ActionManager.ScheduleAction(new PoseAction(poseName));
			}
			else
			{
				animation.PlayAnimation(poseName, true);
			}
		}

		public float GetSpeed()
		{
			return animation.GetCurrentSpeed();
		}

		public void SetSpeed(float speed)
		{
			animation.ChangeSpeed(speed);
		}

		public float GetTime()
		{
			return animation.GetAnimationTime();
		}

		public float GetLength()
		{
			return animation.GetAnimationLength();
		}

		public float GetProgress()
		{
			return animation.GetAnimationProgress();
		}

		public bool IsCompleted()
		{
			return animation.AnimationHasFinished();
		}

		public bool IsInTransition()
		{
			return !animation.TransitionEnded();
		}

		public bool IsInPose()
		{
			return animation.IsInPose();
		}

		public static bool AnimationExists(string animationName)
		{
			return AnimationManager.AnimationExists(animationName);
		}

		public static IList<string> GetAnimationList()
		{
			return IOManager.Instance.GetAnimationList();
		}

		public static bool PoseExists(string poseName)
		{
			return IOManager.Instance.posesList.Contains(poseName);
		}

		public static IList<string> GetPoseList()
		{
			return IOManager.Instance.posesList;
		}

		public static void SetGlobalSpeed(float speed)
		{
			GameController.ChangeSpeed(speed);
		}

		public static void GetGlobalSpeed(float speed)
		{
			GameController.ChangeSpeed(speed);
		}
	}
}
