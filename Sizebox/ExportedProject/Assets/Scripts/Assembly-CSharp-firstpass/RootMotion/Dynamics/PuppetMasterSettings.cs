using System;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.Dynamics
{
	[AddComponentMenu("Scripts/RootMotion.Dynamics/PuppetMaster/PuppetMaster Settings")]
	public class PuppetMasterSettings : Singleton<PuppetMasterSettings>
	{
		[Serializable]
		public class PuppetUpdateLimit
		{
			[Range(1f, 100f)]
			public int puppetsPerFrame;

			private int index;

			public PuppetUpdateLimit()
			{
				puppetsPerFrame = 100;
			}

			public void Step(int puppetCount)
			{
				index += puppetsPerFrame;
				if (index >= puppetCount)
				{
					index -= puppetCount;
				}
			}

			public bool Update(List<PuppetMaster> puppets, PuppetMaster puppetMaster)
			{
				if (puppetsPerFrame >= puppets.Count)
				{
					return true;
				}
				if (index >= puppets.Count)
				{
					return false;
				}
				for (int i = 0; i < puppetsPerFrame; i++)
				{
					int num = index + i;
					if (num >= puppets.Count)
					{
						num -= puppets.Count;
					}
					if (puppets[num] == puppetMaster)
					{
						return true;
					}
				}
				return false;
			}
		}

		[Header("Optimizations")]
		public PuppetUpdateLimit kinematicCollidersUpdateLimit = new PuppetUpdateLimit();

		public PuppetUpdateLimit freeUpdateLimit = new PuppetUpdateLimit();

		public PuppetUpdateLimit fixedUpdateLimit = new PuppetUpdateLimit();

		public bool collisionStayMessages = true;

		public bool collisionExitMessages = true;

		public float activePuppetCollisionThresholdMlp;

		private List<PuppetMaster> _puppets = new List<PuppetMaster>();

		public int currentlyActivePuppets { get; private set; }

		public int currentlyKinematicPuppets { get; private set; }

		public int currentlyDisabledPuppets { get; private set; }

		public List<PuppetMaster> puppets
		{
			get
			{
				return _puppets;
			}
		}

		public void Register(PuppetMaster puppetMaster)
		{
			if (!_puppets.Contains(puppetMaster))
			{
				_puppets.Add(puppetMaster);
			}
		}

		public void Unregister(PuppetMaster puppetMaster)
		{
			_puppets.Remove(puppetMaster);
		}

		public bool UpdateMoveToTarget(PuppetMaster puppetMaster)
		{
			return kinematicCollidersUpdateLimit.Update(_puppets, puppetMaster);
		}

		public bool UpdateFree(PuppetMaster puppetMaster)
		{
			return freeUpdateLimit.Update(_puppets, puppetMaster);
		}

		public bool UpdateFixed(PuppetMaster puppetMaster)
		{
			return fixedUpdateLimit.Update(_puppets, puppetMaster);
		}

		private void Update()
		{
			currentlyActivePuppets = 0;
			currentlyKinematicPuppets = 0;
			currentlyDisabledPuppets = 0;
			foreach (PuppetMaster puppet in _puppets)
			{
				if (puppet.isActive && puppet.isActiveAndEnabled)
				{
					currentlyActivePuppets++;
				}
				if (puppet.mode == PuppetMaster.Mode.Kinematic)
				{
					currentlyKinematicPuppets++;
				}
				if ((puppet.mode == PuppetMaster.Mode.Disabled && !puppet.isActive) || !puppet.isActiveAndEnabled)
				{
					currentlyDisabledPuppets++;
				}
			}
			freeUpdateLimit.Step(_puppets.Count);
			kinematicCollidersUpdateLimit.Step(_puppets.Count);
		}

		private void FixedUpdate()
		{
			fixedUpdateLimit.Step(_puppets.Count);
		}
	}
}
