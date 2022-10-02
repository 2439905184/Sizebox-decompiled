using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Shooting
	{
		private AIShooterController shooterController;

		public bool isAiming
		{
			get
			{
				return shooterController.aiming;
			}
		}

		public bool isFiring
		{
			get
			{
				return shooterController.isFiring;
			}
		}

		public bool accurateFiring
		{
			get
			{
				return shooterController.accurateFire;
			}
			set
			{
				shooterController.accurateFire = value;
			}
		}

		public float inaccuracyFactor
		{
			get
			{
				return shooterController.inaccuracyFactor;
			}
			set
			{
				shooterController.inaccuracyFactor = value;
			}
		}

		public bool predictiveAiming
		{
			get
			{
				return shooterController.predictiveAiming;
			}
			set
			{
				shooterController.predictiveAiming = value;
			}
		}

		public float firingInterval
		{
			get
			{
				return shooterController.firingInterval;
			}
			set
			{
				shooterController.firingInterval = value;
			}
		}

		public bool burstFire
		{
			get
			{
				return shooterController.burstFire;
			}
		}

		public int burstFireRounds
		{
			get
			{
				return shooterController.burstFireRounds;
			}
			set
			{
				shooterController.burstFireRounds = value;
			}
		}

		public float burstFireInterval
		{
			get
			{
				return shooterController.burstFireInterval;
			}
			set
			{
				shooterController.burstFireInterval = value;
			}
		}

		[MoonSharpHidden]
		public Shooting(EntityBase e)
		{
			if (e == null)
			{
				Debug.LogError("Creating Shooting with no entity");
			}
			shooterController = e.GetComponent<AIShooterController>();
		}

		public void SetBurstFire(bool enable)
		{
			shooterController.SetBurstFireMode(enable);
		}

		public void SetProjectileColor(int r, int g, int b)
		{
			shooterController.SetProjectileColor(r, g, b);
		}

		public void SetProjectileSpeed(float speedMult)
		{
			shooterController.SetProjectileSpeed(speedMult);
		}

		public void SetProjectileScale(float scaleMult)
		{
			shooterController.SetProjectileScale(scaleMult);
		}

		public void SetFiringSFX(string clip)
		{
			shooterController.SetRaygunFiringSound(clip);
		}

		public void SetProjectileImpactSFX(string clip)
		{
			shooterController.SetProjectileImpactSound(clip);
		}

		public void FixGunAimingOrientation()
		{
			shooterController.ReorientGun();
		}
	}
}
