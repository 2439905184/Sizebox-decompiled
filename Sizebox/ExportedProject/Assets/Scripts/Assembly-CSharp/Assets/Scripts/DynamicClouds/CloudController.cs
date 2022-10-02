using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DynamicClouds
{
	public class CloudController : MonoBehaviour
	{
		public GameObject SimpleClouds;

		public GameObject CloudHubPreFab;

		public bool UseSimpleClouds;

		[Range(0f, 20f)]
		public float CloudAltitude;

		public bool Overcast;

		[Range(5f, 50f)]
		public int CloudSpawnAreaRandomX;

		[Range(5f, 50f)]
		public int CloudSpawnAreaRandomZ;

		[Range(1f, 10f)]
		public int CloudSpawnAreaRandomHieght;

		[Range(0f, 100f)]
		public int CloudSpawnAreaRandomFillPercent;

		[Range(5f, 30f)]
		public int CloudSpawnAreaRandomReductionFactorPerLayer;

		[Range(1f, 40000f)]
		public float CloudScale;

		public List<CloudHub> CloudHubs = new List<CloudHub>();

		[Range(0f, 200000f)]
		public float CloudGtsActivationRange;

		[Range(0f, 200000f)]
		public float CloudPlayerActivationRange;

		public bool CloudSleepingByCoroutine;

		public bool ActiveShadows;

		[Range(1f, 1000f)]
		public int CloudPhysicsWakeSleepPerFrame;

		[HideInInspector]
		public int ChangedPhysicsStates;

		[Range(1f, 1000f)]
		public int CloudShadowsCheckedPerFrame;

		[HideInInspector]
		public int CheckedShadows;

		[Range(1f, 1000f)]
		public int CloudLODAdjustmentsPerFrame;

		[HideInInspector]
		public int SetLODs;

		public bool CloudPhysics;

		public bool CloudCompression;

		private bool mPrevCloudCompressionState;

		public bool DisableCloudElastisity;

		public bool LimitCloudpush;

		[Range(0.002f, 0.2f)]
		public float CloudElastisityFactor;

		[Range(0.1f, 100f)]
		public float CloudElastisityPositionTolerence;

		[Range(0.01f, 100f)]
		public float CloudElastisityPositionMaxLimit;

		public bool LockCloudTileElevation;

		public bool LockCloudTileRotation;

		[Range(1f, 7f)]
		public float GtsAirComressionFactor;

		public bool CloudLODEnabled;

		[Range(1500f, 150000f)]
		public float MidDistanceLOD;

		[Range(1f, 10f)]
		public int SkyFill;

		[Range(0f, 5f)]
		public int CloudHubSpawnRange;

		private int mIterationX;

		private int mIterationZ;

		public bool BeingInShadowRangeOverridesPhysics;

		public bool PlayerProximityCanActivateShadowChecks;

		public bool UseTestVisuals;

		public bool Populate;

		public bool WipeClouds;

		private void Awake()
		{
			GameController.cloudController = base.gameObject.GetComponent<CloudController>();
		}

		private void Update()
		{
			if (CloudCompression != mPrevCloudCompressionState)
			{
				Physics.IgnoreLayerCollision(21, 21, !CloudCompression);
				mPrevCloudCompressionState = CloudCompression;
			}
			if (Populate)
			{
				SpawnClouds();
				Populate = false;
			}
			if (WipeClouds)
			{
				foreach (CloudHub cloudHub in CloudHubs)
				{
					Object.Destroy(cloudHub.gameObject);
				}
				CloudHubs.Clear();
				WipeClouds = false;
			}
			if (SimpleClouds != null)
			{
				if (!UseSimpleClouds && SimpleClouds.activeInHierarchy)
				{
					SimpleClouds.SetActive(false);
				}
				else if (UseSimpleClouds && !SimpleClouds.activeInHierarchy)
				{
					SimpleClouds.SetActive(true);
				}
			}
		}

		private void PlaceCloudHub(Vector3 position)
		{
			GameObject obj = Object.Instantiate(CloudHubPreFab, position, base.transform.localRotation);
			obj.transform.position = position;
			CloudHub component = obj.GetComponent<CloudHub>();
			CloudHubs.Add(component);
		}

		public void SpawnClouds()
		{
			Vector3 position = GameController.LocalClient.transform.position;
			position += new Vector3(0f, CloudAltitude * CloudScale, 0f);
			float num = (float)CloudSpawnAreaRandomX * CloudScale * 2f;
			float num2 = (float)CloudSpawnAreaRandomZ * CloudScale * 2f;
			mIterationZ = CloudHubSpawnRange * -1;
			if (CloudHubSpawnRange < 1)
			{
				PlaceCloudHub(position);
			}
			else
			{
				while (mIterationZ < CloudHubSpawnRange)
				{
					for (mIterationX = CloudHubSpawnRange * -1; mIterationX < CloudHubSpawnRange; mIterationX++)
					{
						if (Random.Range(SkyFill, 11) == 10)
						{
							PlaceCloudHub(new Vector3(position.x + num * (float)mIterationX, position.y, position.z + num2 * (float)mIterationZ));
						}
					}
					mIterationZ++;
					mIterationX = CloudHubSpawnRange * -1;
				}
				mIterationZ = CloudHubSpawnRange * -1;
			}
			foreach (CloudHub cloudHub in CloudHubs)
			{
				cloudHub.GenerateCumulous();
			}
		}
	}
}
