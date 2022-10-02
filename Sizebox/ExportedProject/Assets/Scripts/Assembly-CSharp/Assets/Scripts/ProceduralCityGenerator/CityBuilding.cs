using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.ProceduralCityGenerator
{
	public class CityBuilding : MonoBehaviour, IDestructible, IDamagable, IGameObject, ICrushable
	{
		private static Transform cityAudioFolder;

		private static GameObject debrisPrefab;

		private static AudioSource[] audioSources;

		private static int sourcesNumber = 16;

		private static float minimumDistanceBetweenSounds = 0.1f;

		private static float timeLastSound = 0.5f;

		private static int sourceToUse = 0;

		private static GameObject smokeParticles;

		private static List<ParticleData> smokeParticleList;

		private static ParticleData smokeData;

		public static bool smokeEffectEnabled;

		public int xSize = 1;

		public int zSize = 1;

		public Vector3[] CellArea;

		public float BuildingScale;

		private Collider thisCollider;

		private float maxRotation = 30f;

		private float fallAcceleration = 5f;

		private bool falling;

		private FracturedObject mChunks;

		private DestructionStatus mDestructionStatus;

		private const float cDestroyDelay = 20f;

		private const float cEndTimeAnimation = 6f;

		[Header("Destruction Prefab")]
		[SerializeField]
		private GameObject destructionPrefab;

		public Vector3 Position { get; set; }

		public CityDestructionManager DestructionManager { get; set; }

		public bool broken
		{
			get
			{
				if (mDestructionStatus != null)
				{
					return mDestructionStatus.IsBroken();
				}
				return false;
			}
		}

		private void Start()
		{
			thisCollider = GetComponentInChildren<Collider>();
			if (debrisPrefab == null)
			{
				debrisPrefab = Resources.Load<GameObject>("City Prefabs/Debris");
			}
			smokeEffectEnabled = GlobalPreferences.SmokeEnabled.value;
			EntityBase componentInParent = GetComponentInParent<EntityBase>();
			if (smokeParticles == null || smokeData.main.customSimulationSpace == null)
			{
				smokeParticles = Object.Instantiate(Resources.Load<GameObject>("Particles/SmokeParticles"));
				smokeData = new ParticleData(smokeParticles);
				smokeData.main.customSimulationSpace = (componentInParent ? componentInParent.transform : null);
			}
			if (smokeParticleList == null)
			{
				smokeParticleList = new List<ParticleData>();
			}
			mDestructionStatus = new DestructionStatus(mChunks);
		}

		private IEnumerator DebrisAnimation()
		{
			Transform transform = base.transform;
			Vector3 localPosition = transform.localPosition;
			GameObject debris = Object.Instantiate(debrisPrefab, transform.parent, false);
			debris.transform.localPosition = localPosition;
			Vector3 localScale = debris.transform.localScale;
			localScale *= 0.5f;
			debris.transform.localScale = localScale;
			debris.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
			Vector3 initialScale = localScale;
			float finalScale = 1f + Random.value;
			float time = 0.1f;
			while (time < 6f)
			{
				debris.transform.localScale = initialScale * (finalScale * time) / 6f;
				time += Time.deltaTime;
				yield return null;
			}
		}

		private IEnumerator FallAnimation(float power)
		{
			falling = true;
			mDestructionStatus.AnimationCalled();
			thisCollider.enabled = false;
			Quaternion targetRotation = Quaternion.Euler(Random.Range(0f, maxRotation), base.transform.localEulerAngles.y, Random.Range(0f, maxRotation));
			float time = 0.1f;
			PlayDestructionSound(power);
			DoSmokeEffect(power);
			while (time < 6f)
			{
				float num = 0.5f * fallAcceleration * time * time;
				Transform transform = base.transform;
				transform.localPosition -= Vector3.up * (num * Time.deltaTime);
				base.transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, 0.5f * Time.deltaTime);
				time += Time.deltaTime;
				yield return null;
			}
			Object.Destroy(base.gameObject);
		}

		public void IgnoreCollision(Collider externalCollider)
		{
			Physics.IgnoreCollision(GetComponentInChildren<Collider>(), externalCollider);
		}

		public void Collapse()
		{
			mDestructionStatus.AnimationCalled();
			try
			{
				if ((bool)mChunks)
				{
					mChunks.Explode(Position, 1f, 100000f, false, false, false, false);
				}
				if ((bool)DestructionManager)
				{
					DestructionManager.UnregisterBuilding(this);
				}
			}
			catch
			{
			}
			finally
			{
				falling = true;
			}
		}

		private static bool EnoughTimePassedBetweenSounds()
		{
			return Time.time >= timeLastSound + minimumDistanceBetweenSounds;
		}

		private void PlayImpactSound(float power)
		{
			if (EnoughTimePassedBetweenSounds())
			{
				PlaySound(SoundManager.Instance.GetExplosionSound());
				PlaySound(SoundManager.Instance.GetImpactSound());
				timeLastSound = Time.time;
			}
		}

		private void PlayDestructionSound(float power)
		{
			if (EnoughTimePassedBetweenSounds())
			{
				PlaySound(SoundManager.Instance.GetDestructionSound());
			}
			timeLastSound = Time.time;
		}

		private void DoSmokeEffect(float power)
		{
			if (power < 10f)
			{
				power = 10f;
			}
			else if (power > 400f)
			{
				power = 400f;
			}
			if (smokeEffectEnabled)
			{
				SmokeEffect(power);
			}
		}

		private void PlaySound(AudioClip soundClip)
		{
			if (audioSources == null)
			{
				sourceToUse = 0;
				cityAudioFolder = new GameObject("Building Audio Sources").transform;
				audioSources = new AudioSource[sourcesNumber];
				for (int i = 0; i < sourcesNumber; i++)
				{
					audioSources[i] = new GameObject("Building Audio " + i).AddComponent<AudioSource>();
					audioSources[i].transform.SetParent(cityAudioFolder);
					audioSources[i].dopplerLevel = 0f;
					audioSources[i].spatialBlend = 1f;
					audioSources[i].minDistance = 50f;
					audioSources[i].maxDistance = 5000f;
					audioSources[i].outputAudioMixerGroup = SoundManager.AudioMixerDestruction;
				}
			}
			AudioSource audioSource = audioSources[sourceToUse];
			if ((bool)audioSource && !audioSource.isPlaying)
			{
				audioSources[sourceToUse].transform.position = base.transform.position;
				audioSources[sourceToUse].PlayOneShot(soundClip);
			}
			sourceToUse++;
			if (sourceToUse >= audioSources.Length)
			{
				sourceToUse = 0;
			}
		}

		public void Damage(float amount)
		{
		}

		public bool TryToDestroy(float destructionForce, Vector3 contactPoint, EntityBase entity = null)
		{
			if (falling)
			{
				return false;
			}
			if (destructionForce / (thisCollider.bounds.max - base.transform.position).y < 0.66f)
			{
				return false;
			}
			falling = true;
			if (GlobalPreferences.LowEndCities.value)
			{
				LowEndDestruction(destructionForce);
			}
			else
			{
				DynamicDestruction(destructionForce);
			}
			return true;
		}

		public bool TryToCrush(float crushingForce, Vector3 velocity, Collision collision, EntityBase entity = null, Collider crushingCollider = null)
		{
			return TryToCrush(entity);
		}

		public bool TryToCrush(Vector3 force, EntityBase entity = null)
		{
			return TryToCrush(entity);
		}

		private bool TryToCrush(EntityBase entity)
		{
			if (falling || !entity)
			{
				return false;
			}
			float height = entity.Height;
			if (height / (thisCollider.bounds.max - base.transform.position).y < 8f)
			{
				return false;
			}
			falling = true;
			LowEndDestruction(height);
			return true;
		}

		private void DynamicDestruction(float scale)
		{
			PlayImpactSound(scale);
			Transform transform = base.transform;
			transform.Find("U_Char").gameObject.SetActive(false);
			Object.Instantiate(destructionPrefab, transform);
			if ((bool)DestructionManager)
			{
				DestructionManager.RegisterBuilding(this);
			}
			StartCoroutine(DebrisAnimation());
		}

		private void LowEndDestruction(float scale)
		{
			StartCoroutine(FallAnimation(scale));
			StartCoroutine(DebrisAnimation());
		}

		private void SmokeEffect(float power)
		{
			smokeData.particleRoot.position = base.transform.position;
			smokeData.main.startSize = power * BuildingScale;
			smokeData.shape.radius = power * BuildingScale * 1.0072f;
			smokeData.system.Play();
		}

		[SpecialName]
		GameObject IGameObject.get_gameObject()
		{
			return base.gameObject;
		}

		[SpecialName]
		Transform IGameObject.get_transform()
		{
			return base.transform;
		}
	}
}
