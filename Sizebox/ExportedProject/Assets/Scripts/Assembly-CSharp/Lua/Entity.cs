using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts.AI.Actions.Interaction;
using Assets.Scripts.ProceduralCityGenerator;
using MoonSharp.Interpreter;
using SteeringBehaviors;
using UnityEngine;

namespace Lua
{
	[MoonSharpUserData]
	public class Entity
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass140_0
		{
			public EntityInitializedCallback luaCallback;

			internal void _003CSpawnGiantess_003Eb__0(EntityBase entity)
			{
				luaCallback.Call(entity);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass142_0
		{
			public EntityInitializedCallback luaCallback;

			internal void _003CSpawnFemaleMicro_003Eb__0(EntityBase entity)
			{
				luaCallback.Call(entity);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass145_0
		{
			public EntityInitializedCallback luaCallback;

			internal void _003CSpawnMaleMicro_003Eb__0(EntityBase entity)
			{
				luaCallback.Call(entity);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass148_0
		{
			public EntityInitializedCallback luaCallback;

			internal void _003CSpawnObject_003Eb__0(EntityBase entity)
			{
				luaCallback.Call(entity);
			}
		}

		[MoonSharpHidden]
		public EntityBase entity;

		private Table _dict;

		private Transform _transform;

		private Rigidbody _rigidbody;

		private AI _ai;

		private Animation _animation;

		private Bones _bones;

		private Animator _animator;

		private IK _ik;

		private Senses _senses;

		private Morphs _morphs;

		private Movement _movement;

		private Shooting _shooting;

		public Table dict
		{
			get
			{
				if (_dict == null)
				{
					_dict = new Table(null);
				}
				return _dict;
			}
		}

		public Transform transform
		{
			get
			{
				if (_transform == null)
				{
					_transform = new Transform(entity.transform);
				}
				return _transform;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				if (_rigidbody == null && entity.Rigidbody != null)
				{
					_rigidbody = new Rigidbody(entity.Rigidbody);
				}
				return _rigidbody;
			}
		}

		public AI ai
		{
			get
			{
				if (_ai != null)
				{
					return _ai;
				}
				if ((bool)(entity as Humanoid))
				{
					_ai = new AI(entity as Humanoid);
				}
				return _ai;
			}
		}

		public Animation animation
		{
			get
			{
				if (_animation != null)
				{
					return _animation;
				}
				if ((bool)(entity as Humanoid))
				{
					_animation = new Animation(entity as Humanoid);
				}
				return _animation;
			}
		}

		public Bones bones
		{
			get
			{
				if (_bones == null)
				{
					_animator = entity.model.GetComponent<Animator>();
					if (_animator != null)
					{
						_bones = new Bones(_animator);
					}
				}
				return _bones;
			}
		}

		public IK ik
		{
			get
			{
				Humanoid component = entity.GetComponent<Humanoid>();
				if (!component || component.ik == null)
				{
					return null;
				}
				if (_ik == null)
				{
					_ik = new IK(component.ik);
				}
				return _ik;
			}
		}

		public Senses senses
		{
			get
			{
				if (_senses != null)
				{
					return _senses;
				}
				if ((bool)(entity as Humanoid))
				{
					_senses = new Senses(entity as Humanoid);
				}
				return _senses;
			}
		}

		public Morphs morphs
		{
			get
			{
				if (!entity.isGiantess)
				{
					return null;
				}
				if (_morphs == null)
				{
					_morphs = new Morphs(entity);
				}
				return _morphs;
			}
		}

		public Movement movement
		{
			get
			{
				if (_movement == null && entity.Movement != null)
				{
					_movement = new Movement(entity);
				}
				return _movement;
			}
		}

		public Shooting shooting
		{
			get
			{
				if (_shooting == null && entity.GetComponent<AIShooterController>() != null)
				{
					_shooting = new Shooting(entity);
				}
				return _shooting;
			}
		}

		public Vector3 position
		{
			get
			{
				return transform.position;
			}
			set
			{
				transform.position = value;
			}
		}

		public bool CanLookAtPlayer
		{
			get
			{
				if (!entity.isGiantess)
				{
					return false;
				}
				return entity.GetComponent<Giantess>().canLookAtPlayer;
			}
			set
			{
				if (entity.isGiantess)
				{
					entity.GetComponent<Giantess>().canLookAtPlayer = value;
				}
			}
		}

		public int id
		{
			get
			{
				return entity.id;
			}
		}

		public string name
		{
			get
			{
				return entity.name;
			}
			set
			{
				entity.name = value;
			}
		}

		public string modelName
		{
			get
			{
				return entity.asset.AssetFullName;
			}
		}

		public virtual float scale
		{
			get
			{
				return entity.AccurateScale;
			}
			set
			{
				entity.AccurateScale = value;
			}
		}

		public virtual float baseHeight
		{
			get
			{
				return entity.baseHeight;
			}
		}

		public virtual float height
		{
			get
			{
				return entity.Height;
			}
			set
			{
				entity.ChangeScale(value / entity.baseHeight);
			}
		}

		public float metricHeight
		{
			get
			{
				return entity.MeshHeight;
			}
			set
			{
				entity.MeshHeight = value;
			}
		}

		public virtual float maxSize
		{
			get
			{
				return entity.maxSize;
			}
			set
			{
				entity.maxSize = value;
			}
		}

		public virtual float minSize
		{
			get
			{
				return entity.minSize;
			}
			set
			{
				entity.minSize = value;
			}
		}

		public void Delete()
		{
			if ((bool)entity)
			{
				entity.DestroyObject();
			}
		}

		public float Distance(Entity target)
		{
			if (target == null)
			{
				return 0f;
			}
			return Distance(target.position);
		}

		public float Distance(Vector3 point)
		{
			return Vector3.Distance(transform.position, point);
		}

		public float Distance2d(Entity target)
		{
			if (target == null)
			{
				return 0f;
			}
			return Distance2d(target.position);
		}

		public float Distance2d(Vector3 point)
		{
			Vector3 vector = transform.position;
			float num = vector.x - point.x;
			float num2 = vector.z - point.z;
			return Mathf.Sqrt(num * num + num2 * num2);
		}

		public float DistanceTo(Entity target)
		{
			return Distance2d(target);
		}

		public float DistanceTo(Vector3 point)
		{
			return Distance2d(point);
		}

		public float DistanceVertical(Entity target)
		{
			return DistanceVertical(target.position);
		}

		public float DistanceVertical(Vector3 point)
		{
			return Mathf.Abs(transform.position.y - point.y);
		}

		public float DistanceVerticalSigned(Entity target)
		{
			return DistanceVerticalSigned(target.position);
		}

		public float DistanceVerticalSigned(Vector3 point)
		{
			return transform.position.y - point.y;
		}

		public Entity FindClosestMicro()
		{
			return MicroManager.FindClosestMicro(entity, entity.Height);
		}

		public Entity FindClosestGiantess()
		{
			return GiantessManager.FindClosestGiantess(entity, entity.Height);
		}

		public bool isHumanoid()
		{
			return entity.isHumanoid;
		}

		public bool isGiantess()
		{
			return entity.isGiantess;
		}

		public bool isPlayer()
		{
			return entity.isPlayer;
		}

		public bool isMicro()
		{
			return entity.isMicro;
		}

		public bool ActionsCompleted()
		{
			if (ai != null)
			{
				return !ai.IsActionActive();
			}
			return true;
		}

		public virtual void Sit(Vector3 place)
		{
			Debug.LogError("Only giantess can sit.");
		}

		public virtual void Pursue(Entity target)
		{
			Debug.LogError("Objects can't pursue.");
		}

		public virtual void BE(float speed)
		{
			AddAction(new BEAction(speed));
		}

		public virtual void BE(float speed, float time)
		{
			AddAction(new BEAction(speed, time));
		}

		public void SetAnimation(string animationName)
		{
			if (animation != null)
			{
				animation.Set(animationName);
			}
		}

		public void CompleteAnimation(string animationName)
		{
			if (animation != null)
			{
				animation.SetAndWait(animationName);
			}
		}

		public void SetPose(string pose)
		{
			if (animation != null)
			{
				animation.SetPose(pose);
			}
		}

		public void CancelAction()
		{
			if (ai != null)
			{
				ai.StopAction();
			}
		}

		public void Grow(float factor)
		{
			AddAction(new SizeChangeAction(factor));
		}

		public void Grow(float factor, float time)
		{
			AddAction(new SizeChangeAction(factor, time));
		}

		public void GrowAndWait(float factor, float time)
		{
			SizeChangeAction sizeChangeAction = new SizeChangeAction(factor, time);
			sizeChangeAction.priority = (sizeChangeAction.async = false);
			AddAction(sizeChangeAction);
		}

		public void MoveTo(Vector3 destination)
		{
			UnityEngine.Vector3 worldPos = destination.virtualPosition.ToWorld();
			if (!AddAction(new ArriveAction(new VectorKinematic(worldPos))))
			{
				entity.Move(worldPos);
			}
		}

		public void MoveTo(Entity targetEntity)
		{
			if (!AddAction(new ArriveAction(new TransformKinematic(targetEntity.entity.transform))))
			{
				entity.Move(targetEntity.entity.transform.position);
			}
		}

		public void Chase(Entity target)
		{
			AddAction(new ChaseAction(new TransformKinematic(target.entity.transform), new TransformKinematic(entity.transform), target.entity));
		}

		public void Face(Entity target)
		{
			AddAction(new FaceAction(new TransformKinematic(target.entity.transform)));
		}

		public void Seek(Entity target, float duration = 0f, float separation = -1f)
		{
			AddAction(new SeekAction(new TransformKinematic(target.entity.transform), separation, duration));
		}

		public void Seek(Vector3 position, float duration = 0f, float separation = -1f)
		{
			AddAction(new SeekAction(new VectorKinematic(position.virtualPosition.ToWorld()), separation, duration));
		}

		public void Wander()
		{
			AddAction(new WanderAction());
		}

		public void Wander(float time)
		{
			AddAction(new WanderAction(time));
		}

		public void Wait(float time)
		{
			AddAction(new WaitAction(time));
		}

		public void Flee(Entity target, float time)
		{
			AddAction(new FleeAction(new TransformKinematic(target.entity.transform), time));
		}

		public void Flee(Vector3 position, float time)
		{
			AddAction(new FleeAction(new VectorKinematic(position.virtualPosition.ToWorld()), time));
		}

		public Vector3 FindRandomBuilding(Entity self)
		{
			CityBuilding cityBuilding = CityHelper.FindRandomStructure(self);
			if (cityBuilding != null)
			{
				return new Vector3(cityBuilding.transform.position.ToVirtual());
			}
			return null;
		}

		public void Wreck()
		{
			AddAction(new WreckAction(animation));
		}

		public void StandUp()
		{
			Micro micro = entity as Micro;
			if ((bool)micro)
			{
				micro.StandUp();
			}
		}

		public void Stomp(Entity target)
		{
			AddAction(new StompAction(target.entity));
		}

		public void Stomp(Vector3 target)
		{
			AddAction(new StompAction(target.virtualPosition));
		}

		public void Grab(Entity target)
		{
			AddAction(new GrabAction(target.entity));
		}

		public void LookAt(Entity target)
		{
			if (target == null)
			{
				AddAction(new LookAction(null));
			}
			else
			{
				AddAction(new LookAction(target.entity));
			}
		}

		private AIShooterController getShooterController(bool createIfNecessary = false)
		{
			MicroNpc component = entity.GetComponent<MicroNpc>();
			if (component == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("Only micros can perform shooting-related actions.");
				}
				return null;
			}
			return component.GetShooterController(createIfNecessary);
		}

		public void EquipRaygun()
		{
			MicroNpc component = entity.GetComponent<MicroNpc>();
			if (component == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("Only micros can perform shooting-related actions.");
				}
			}
			else
			{
				component.EquipRaygun();
			}
		}

		public void EquipSMG()
		{
			MicroNpc component = entity.GetComponent<MicroNpc>();
			if (component == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("Only micros can perform shooting-related actions.");
				}
			}
			else
			{
				component.EquipSmg();
			}
		}

		public void UnequipGun()
		{
			if (!entity)
			{
				return;
			}
			MicroNpc component = entity.GetComponent<MicroNpc>();
			if (component == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("Only micros can perform shooting-related actions.");
				}
			}
			else
			{
				component.HolsterGun();
			}
		}

		public void Aim(Entity target)
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new AimAction((target != null) ? target.entity : null));
			}
		}

		public void StopAiming()
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new AimAction(null));
			}
		}

		public void StartFiring()
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new ShootAction());
			}
		}

		public void StopFiring()
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new StopShootAction());
			}
		}

		public void Engage(Entity target = null)
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new EngageShootAction((target != null) ? target.entity : null));
			}
		}

		public void StopEngaging()
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new StopEngageShootAction());
			}
		}

		public void FireOnce()
		{
			if (getShooterController() == null)
			{
				if (GlobalPreferences.ScriptAuxLogging.value)
				{
					Debug.Log("No shooter controller on entity. Please equip a gun first.");
				}
			}
			else
			{
				AddAction(new OneOffShootAction());
			}
		}

		public bool IsDead()
		{
			if (entity == null)
			{
				return true;
			}
			if (!entity.gameObject.activeSelf)
			{
				return true;
			}
			if (entity.isHumanoid)
			{
				return ((Humanoid)entity).IsDead;
			}
			return false;
		}

		public bool IsStuck()
		{
			Micro micro = entity as Micro;
			if (!micro)
			{
				return false;
			}
			return micro.IsStuck();
		}

		public bool IsTargettable()
		{
			if ((bool)entity)
			{
				return entity.IsTargetAble();
			}
			return false;
		}

		public bool IsCrushed()
		{
			Micro micro = entity as Micro;
			if (!micro)
			{
				return false;
			}
			return micro.isCrushed;
		}

		protected bool AddAction(AgentAction action)
		{
			Humanoid humanoid = entity as Humanoid;
			if (!action.CanRunOnHumanoid(humanoid))
			{
				return false;
			}
			humanoid.ActionManager.ScheduleAction(action);
			return true;
		}

		public void UpdateMeshCollider()
		{
			if (entity.isGiantess)
			{
				entity.GetComponent<Giantess>().ForceColliderUpdate();
			}
		}

		public void ShowBreastPhysicsOptions()
		{
			if (entity.isGiantess)
			{
				entity.GetComponent<Giantess>().ManualBreastPhysics();
			}
			else
			{
				Debug.Log("Breast physics menu is only supported on giantess");
			}
		}

		public void PlayAs()
		{
			GameController.LocalClient.Player.PlayAs(entity.GetComponent<IPlayable>());
		}

		public static Entity GetSelectedEntity()
		{
			return InterfaceControl.instance.selectedEntity;
		}

		public static IList<string> GetGtsModelList()
		{
			return AssetManager.Instance.GetGtsAssetNames();
		}

		public static IList<string> GetFemaleMicroList()
		{
			return AssetManager.Instance.GetFemaleMicroAssetNames();
		}

		public static IList<string> GetMaleMicroList()
		{
			return AssetManager.Instance.GetMaleMicroAssetNames();
		}

		public static IList<string> GetObjectList()
		{
			return AssetManager.Instance.GetObjectAssetNames();
		}

		public Entity GetRandomMicro()
		{
			return MicroManager.GetRandomMicro(entity);
		}

		public Entity GetRandomGiantess()
		{
			return GiantessManager.GetRandomGiantess(entity);
		}

		public static Entity SpawnGiantess(string name, Vector3 position, Quaternion rotation, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnGiantess(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale / 1000f);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnGiantess(string name, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnGiantess(assetDescriptionByName, scale / 1000f);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnGiantess(string name, Vector3 position, Quaternion rotation, float scale, DynValue scriptInstance, DynValue initializedCallback)
		{
			_003C_003Ec__DisplayClass140_0 _003C_003Ec__DisplayClass140_ = new _003C_003Ec__DisplayClass140_0();
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			_003C_003Ec__DisplayClass140_.luaCallback = new EntityInitializedCallback(scriptInstance, initializedCallback);
			Action<EntityBase> callback = _003C_003Ec__DisplayClass140_._003CSpawnGiantess_003Eb__0;
			GameObject gameObject = LocalClient.Instance.SpawnGiantess(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale / 1000f, callback);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnFemaleMicro(string name, Vector3 position, Quaternion rotation, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnFemaleMicro(string name, Vector3 position, Quaternion rotation, float scale, DynValue scriptInstance, DynValue initializedCallback)
		{
			_003C_003Ec__DisplayClass142_0 _003C_003Ec__DisplayClass142_ = new _003C_003Ec__DisplayClass142_0();
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			_003C_003Ec__DisplayClass142_.luaCallback = new EntityInitializedCallback(scriptInstance, initializedCallback);
			Action<EntityBase> callback = _003C_003Ec__DisplayClass142_._003CSpawnFemaleMicro_003Eb__0;
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale, callback);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnFemaleMicro(string name, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, scale);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnMaleMicro(string name, Vector3 position, Quaternion rotation, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnMaleMicro(string name, Vector3 position, Quaternion rotation, float scale, DynValue scriptInstance, DynValue initializedCallback)
		{
			_003C_003Ec__DisplayClass145_0 _003C_003Ec__DisplayClass145_ = new _003C_003Ec__DisplayClass145_0();
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			_003C_003Ec__DisplayClass145_.luaCallback = new EntityInitializedCallback(scriptInstance, initializedCallback);
			Action<EntityBase> callback = _003C_003Ec__DisplayClass145_._003CSpawnMaleMicro_003Eb__0;
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, position.virtualPosition.ToWorld(), rotation.quaternion, scale, callback);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnMaleMicro(string name, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnMicro(assetDescriptionByName, scale);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnObject(string name, Vector3 position, Quaternion rotation, float scale)
		{
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			GameObject gameObject = LocalClient.Instance.SpawnObject(assetDescriptionByName, position.virtualPosition, rotation.quaternion, scale);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static Entity SpawnObject(string name, Vector3 position, Quaternion rotation, float scale, DynValue scriptInstance, DynValue initializedCallback)
		{
			_003C_003Ec__DisplayClass148_0 _003C_003Ec__DisplayClass148_ = new _003C_003Ec__DisplayClass148_0();
			AssetDescription assetDescriptionByName = AssetManager.Instance.GetAssetDescriptionByName(name);
			if (assetDescriptionByName == null)
			{
				return null;
			}
			_003C_003Ec__DisplayClass148_.luaCallback = new EntityInitializedCallback(scriptInstance, initializedCallback);
			Action<EntityBase> callback = _003C_003Ec__DisplayClass148_._003CSpawnObject_003Eb__0;
			GameObject gameObject = LocalClient.Instance.SpawnObject(assetDescriptionByName, position.virtualPosition, rotation.quaternion, scale, callback);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<EntityBase>().GetLuaEntity();
		}

		public static implicit operator Entity(EntityBase entity)
		{
			if (!(entity == null))
			{
				return new Entity(entity);
			}
			return null;
		}

		[MoonSharpUserDataMetamethod("__eq")]
		public static bool Equals(Entity a, Entity b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			if (a == null)
			{
				return b.entity == null;
			}
			if (b == null)
			{
				return a.entity == null;
			}
			return a.entity == b.entity;
		}

		[MoonSharpHidden]
		internal Entity(EntityBase entity)
		{
			if (entity == null)
			{
				Debug.LogError("Error, empty entity.");
			}
			this.entity = entity;
		}
	}
}
