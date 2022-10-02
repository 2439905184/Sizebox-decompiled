using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Senses
	{
		private Humanoid entity;

		private SenseController senses;

		public float baseVisibilityDistance
		{
			get
			{
				return senses.maxDistace;
			}
			set
			{
				senses.maxDistace = value;
			}
		}

		public float fieldOfView
		{
			get
			{
				return senses.fieldOfView;
			}
			set
			{
				senses.fieldOfView = value;
			}
		}

		[MoonSharpHidden]
		public Senses(Humanoid entity)
		{
			if (entity == null)
			{
				Debug.LogError("Creating Senses with no entity");
			}
			this.entity = entity;
			senses = entity.senses;
		}

		public bool CanSee(Entity target)
		{
			return entity.senses.CheckVisibility(target.entity);
		}

		public List<Entity> GetVisibleEntities(float distance)
		{
			List<EntityBase> visibleEntities = entity.senses.GetVisibleEntities(distance);
			List<Entity> list = new List<Entity>();
			foreach (EntityBase item in visibleEntities)
			{
				list.Add(item.GetLuaEntity());
			}
			return list;
		}

		public List<Entity> GetEntitiesInRadius(float distance)
		{
			List<EntityBase> entitiesInRadius = entity.senses.GetEntitiesInRadius(distance);
			List<Entity> list = new List<Entity>();
			foreach (EntityBase item in entitiesInRadius)
			{
				list.Add(item.GetLuaEntity());
			}
			return list;
		}

		public List<Entity> GetVisibleMicros(float distance)
		{
			List<EntityBase> visibleMicros = entity.senses.GetVisibleMicros(distance);
			List<Entity> list = new List<Entity>();
			foreach (EntityBase item in visibleMicros)
			{
				list.Add(item.GetLuaEntity());
			}
			return list;
		}

		public List<Entity> GetMicrosInRadius(float radius)
		{
			List<Micro> list = MicroManager.FindMicrosInRadius(entity, radius);
			List<Entity> list2 = new List<Entity>();
			foreach (Micro item in list)
			{
				list2.Add(item.GetLuaEntity());
			}
			return list2;
		}

		public List<Entity> GetGiantessesInRadius(float radius)
		{
			List<Giantess> list = GiantessManager.FindGiantessesInRadius(entity, radius);
			List<Entity> list2 = new List<Entity>();
			foreach (Giantess item in list)
			{
				list2.Add(item.GetLuaEntity());
			}
			return list2;
		}
	}
}
