using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Movement
	{
		private EntityBase entity;

		private MovementCharacter movement;

		private WalkController walk;

		public float speed
		{
			get
			{
				return movement.maxSpeedBase;
			}
			set
			{
				movement.maxSpeedBase = value;
			}
		}

		[MoonSharpHidden]
		public Movement(EntityBase e)
		{
			if (e == null)
			{
				Debug.LogError("Creating Movement with no entity");
			}
			entity = e;
			movement = e.Movement;
			walk = movement.walkController;
		}

		public void MoveTowards(Vector3 point)
		{
			walk.MoveTowards(point.virtualPosition);
		}

		public void MoveDirection(Vector3 direction)
		{
			walk.MoveWorldDirection(direction.virtualPosition);
		}

		public void Turn(float degrees)
		{
			entity.transform.Rotate(UnityEngine.Vector3.up, degrees);
		}
	}
}
