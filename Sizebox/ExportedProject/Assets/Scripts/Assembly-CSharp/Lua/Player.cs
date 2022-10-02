using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class Player
	{
		private global::Player player;

		private ResizeCharacter resizeCharater;

		private LuaPlayerRaygun _raygun;

		public Entity entity
		{
			get
			{
				return player.Entity;
			}
		}

		public bool climbing
		{
			get
			{
				return player.Control.IsClimbing;
			}
		}

		public bool isAiming
		{
			get
			{
				return player.Control.IsAiming;
			}
		}

		public float walkSpeed
		{
			get
			{
				return player.Control.WalkSpeed;
			}
			set
			{
				player.Control.WalkSpeed = value;
			}
		}

		public float runSpeed
		{
			get
			{
				return player.Control.RunSpeed;
			}
			set
			{
				player.Control.RunSpeed = value;
			}
		}

		public float sprintSpeed
		{
			get
			{
				return player.Control.SprintSpeed;
			}
			set
			{
				player.Control.SprintSpeed = value;
			}
		}

		public float flySpeed
		{
			get
			{
				return player.Control.FlySpeed;
			}
			set
			{
				player.Control.FlySpeed = value;
			}
		}

		public float superFlySpeed
		{
			get
			{
				return player.Control.SuperSpeed;
			}
			set
			{
				player.Control.SuperSpeed = value;
			}
		}

		public float climbSpeed
		{
			get
			{
				return player.Control.ClimbSpeed;
			}
			set
			{
				player.Control.ClimbSpeed = value;
			}
		}

		public float jumpPower
		{
			get
			{
				return player.Control.JumpHeight;
			}
			set
			{
				player.Control.JumpHeight = value;
			}
		}

		public bool autowalk
		{
			get
			{
				return player.Control.AutoWalk;
			}
			set
			{
				player.Control.AutoWalk = value;
			}
		}

		public float sizeChangeSpeed
		{
			get
			{
				return resizeCharater.sizeChangeRate;
			}
			set
			{
				resizeCharater.sizeChangeRate = value;
			}
		}

		public float minSize
		{
			get
			{
				return player.minSize;
			}
			set
			{
				player.minSize = value;
			}
		}

		public float maxSize
		{
			get
			{
				return player.maxSize;
			}
			set
			{
				player.maxSize = value;
			}
		}

		public float scale
		{
			get
			{
				return player.Scale;
			}
			set
			{
				resizeCharater.ChangeScale(value);
			}
		}

		public LuaPlayerRaygun raygun
		{
			get
			{
				if (_raygun == null && player != null)
				{
					_raygun = new LuaPlayerRaygun(player);
				}
				return _raygun;
			}
		}

		[MoonSharpHidden]
		public Player(LocalClient client)
		{
			player = client.Player;
			resizeCharater = player.GetComponent<ResizeCharacter>();
		}
	}
}
