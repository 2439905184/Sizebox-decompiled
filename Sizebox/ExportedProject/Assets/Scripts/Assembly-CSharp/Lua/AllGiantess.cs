using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Lua
{
	[MoonSharpUserData]
	public class AllGiantess
	{
		private Dictionary<int, Entity> instances;

		public float globalSpeed
		{
			get
			{
				return GameController.macroSpeed;
			}
			set
			{
				GameController.macroSpeed = value;
			}
		}

		public float maxSize
		{
			get
			{
				return MapSettingInternal.maxGtsSize;
			}
			set
			{
				MapSettingInternal.maxGtsSize = value;
			}
		}

		public float minSize
		{
			get
			{
				return MapSettingInternal.minGtsSize;
			}
			set
			{
				MapSettingInternal.minGtsSize = value;
			}
		}

		public IDictionary<int, Entity> list
		{
			get
			{
				return instances;
			}
		}

		[MoonSharpHidden]
		public AllGiantess()
		{
			instances = new Dictionary<int, Entity>();
			instances.Clear();
			ObjectManager.Instance.OnGiantessAdd += AddGTS;
			ObjectManager.Instance.OnGiantessRemove += RemoveGTS;
		}

		private void AddGTS(Giantess giantess)
		{
			int id = giantess.id;
			if (!instances.ContainsKey(id))
			{
				instances.Add(id, giantess);
			}
		}

		private void RemoveGTS(Giantess giantess)
		{
			int id = giantess.id;
			if (instances.ContainsKey(id))
			{
				instances.Remove(id);
			}
		}
	}
}
