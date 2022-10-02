using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DynamicClouds
{
	public class CloudHub : MonoBehaviour
	{
		public GameObject TileCloud;

		public BoxCollider MyCollider;

		private int mTilesLaid;

		private int mRowsLaid;

		[HideInInspector]
		public float ColliderSize;

		[HideInInspector]
		public List<Clouds> CloudScripts = new List<Clouds>();

		private int mItemsInBounds;

		private bool mCurrentPhysicsState;

		private bool mPreviousPhysicsState;

		private bool mShadowsInactive;

		[HideInInspector]
		public bool Ready;

		private int mCurrentLayer;

		private string mSeed;

		private int[,] mCloudShape;

		[HideInInspector]
		public bool BusyChangingPhysics;

		[HideInInspector]
		public bool BusyShadowChecking;

		[HideInInspector]
		public bool BusyLODSeting;

		[HideInInspector]
		public bool PlayerIsNearEnoughForShadows;

		[HideInInspector]
		public int NumberOfGtsNear;

		private bool mWaitForShadowCheckToDeactivatePhysics;

		private float mDistanceFromPlayer;

		private float mPlayerScale;

		private int mCurrentDistanceFromCloudRating;

		private int mPreviousDistanceFromCloudRating;

		private void Start()
		{
			ColliderSize = GameController.cloudController.CloudScale - GameController.cloudController.CloudScale * 0.05f;
			MyCollider.size = new Vector3((float)GameController.cloudController.CloudSpawnAreaRandomX * ColliderSize * 2f, (float)GameController.cloudController.CloudSpawnAreaRandomHieght * ColliderSize * 2f, (float)GameController.cloudController.CloudSpawnAreaRandomX * ColliderSize * 2f);
			MyCollider.center = new Vector3(0f, (float)GameController.cloudController.CloudSpawnAreaRandomHieght * ColliderSize, 0f);
		}

		private void Update()
		{
			EntityBase entity = GameController.LocalClient.Player.Entity;
			if (!Ready || !entity)
			{
				return;
			}
			if (GameController.cloudController.CloudLODEnabled || GameController.cloudController.ActiveShadows)
			{
				mPlayerScale = entity.transform.lossyScale.y;
				mDistanceFromPlayer = Vector3.Distance(entity.transform.position, base.transform.position);
				if (mDistanceFromPlayer > GameController.cloudController.MidDistanceLOD * 4f * entity.transform.lossyScale.y)
				{
					mCurrentDistanceFromCloudRating = 3;
				}
				else if (mDistanceFromPlayer > GameController.cloudController.MidDistanceLOD * 2f * entity.transform.lossyScale.y)
				{
					mCurrentDistanceFromCloudRating = 2;
				}
				else if (mDistanceFromPlayer > GameController.cloudController.MidDistanceLOD * entity.transform.lossyScale.y)
				{
					mCurrentDistanceFromCloudRating = 1;
				}
				else
				{
					mCurrentDistanceFromCloudRating = 0;
				}
				if (mCurrentDistanceFromCloudRating != mPreviousDistanceFromCloudRating && !BusyLODSeting)
				{
					StartCoroutine(LimitedCloudLOD(mCurrentDistanceFromCloudRating));
				}
			}
			if (GameController.cloudController.ActiveShadows)
			{
				if (GameController.cloudController.PlayerProximityCanActivateShadowChecks)
				{
					PlayerIsNearEnoughForShadows = false;
					if (mDistanceFromPlayer < GameController.cloudController.CloudPlayerActivationRange)
					{
						PlayerIsNearEnoughForShadows = true;
					}
					else
					{
						PlayerIsNearEnoughForShadows = false;
					}
				}
				else
				{
					PlayerIsNearEnoughForShadows = false;
				}
				NumberOfGtsNear = 0;
				if (GameController.cloudController.ActiveShadows)
				{
					foreach (GameObject item in GameController.giantessesOnScene)
					{
						if (Vector3.Distance(item.transform.position, base.transform.position) < GameController.cloudController.CloudGtsActivationRange * mPlayerScale)
						{
							NumberOfGtsNear++;
						}
					}
					if (NumberOfGtsNear > 0 || PlayerIsNearEnoughForShadows)
					{
						if (GameController.cloudController.ActiveShadows)
						{
							mShadowsInactive = false;
						}
					}
					else
					{
						mShadowsInactive = true;
					}
				}
			}
			else
			{
				mShadowsInactive = true;
			}
			if (mItemsInBounds != 0)
			{
				mCurrentPhysicsState = true;
			}
			else if (GameController.cloudController.BeingInShadowRangeOverridesPhysics && !mShadowsInactive)
			{
				mCurrentPhysicsState = true;
			}
			else if (mItemsInBounds == 0)
			{
				mCurrentPhysicsState = false;
			}
			if (!mShadowsInactive && !BusyShadowChecking)
			{
				StartCoroutine(LimitedShadowChecks());
			}
			if (mCurrentPhysicsState == mPreviousPhysicsState)
			{
				return;
			}
			if (!GameController.cloudController.CloudSleepingByCoroutine)
			{
				if (mCurrentPhysicsState)
				{
					ActivateCloudPhysics();
				}
				else if (!BusyShadowChecking)
				{
					DeactivateCloudPhysics();
				}
			}
			else if (!BusyChangingPhysics)
			{
				if (mCurrentPhysicsState)
				{
					StartCoroutine(LimitedPhysicsChange(true));
				}
				else if (!BusyShadowChecking)
				{
					StartCoroutine(LimitedPhysicsChange(false));
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			mItemsInBounds++;
		}

		private void OnTriggerExit(Collider other)
		{
			mItemsInBounds--;
		}

		private void PlaceCloudTile(Vector3 position)
		{
			GameObject obj = UnityEngine.Object.Instantiate(TileCloud, position, base.transform.localRotation, base.transform);
			obj.transform.localPosition = position;
			Clouds component = obj.GetComponent<Clouds>();
			CloudScripts.Add(component);
			component.InitializeCloudTile(GameController.cloudController.CloudScale, position);
			if (GameController.cloudController.DisableCloudElastisity)
			{
				component.DisableCloudElasticity = true;
			}
		}

		private void ActivateCloudPhysics()
		{
			int num = 0;
			foreach (Clouds cloudScript in CloudScripts)
			{
				Clouds cloud = cloudScript;
				CloudScripts[num].Rbody.isKinematic = false;
				CloudScripts[num].MainCollider.enabled = true;
				CloudScripts[num].GtsColider.enabled = true;
				CloudScripts[num].GtsColider.enabled = true;
				num++;
			}
			mPreviousPhysicsState = true;
		}

		private void DeactivateCloudPhysics()
		{
			int num = 0;
			foreach (Clouds cloudScript in CloudScripts)
			{
				Clouds cloud = cloudScript;
				CloudScripts[num].Rbody.isKinematic = true;
				CloudScripts[num].MainCollider.enabled = false;
				CloudScripts[num].GtsColider.enabled = false;
				CloudScripts[num].GtsColider.enabled = false;
				num++;
			}
			mPreviousPhysicsState = false;
		}

		public IEnumerator LimitedPhysicsChange(bool onOff)
		{
			BusyChangingPhysics = true;
			int i2 = 0;
			foreach (Clouds cloudScript in CloudScripts)
			{
				Clouds cloud = cloudScript;
				CloudScripts[i2].Rbody.isKinematic = !onOff;
				CloudScripts[i2].MainCollider.enabled = onOff;
				CloudScripts[i2].GtsColider.enabled = onOff;
				i2++;
				GameController.cloudController.ChangedPhysicsStates++;
				if (GameController.cloudController.ChangedPhysicsStates > GameController.cloudController.CloudPhysicsWakeSleepPerFrame - 1)
				{
					GameController.cloudController.ChangedPhysicsStates = 0;
					yield return null;
				}
			}
			mPreviousPhysicsState = onOff;
			BusyChangingPhysics = false;
		}

		public IEnumerator LimitedShadowChecks()
		{
			BusyShadowChecking = true;
			foreach (Clouds cloudScript in CloudScripts)
			{
				cloudScript.ShadowDetectorScript.ShadowCheck();
				GameController.cloudController.CheckedShadows++;
				if (GameController.cloudController.CheckedShadows > GameController.cloudController.CloudShadowsCheckedPerFrame - 1)
				{
					GameController.cloudController.CheckedShadows = 0;
					yield return null;
				}
			}
			BusyShadowChecking = false;
		}

		public IEnumerator LimitedCloudLOD(int level)
		{
			BusyLODSeting = true;
			foreach (Clouds cloudScript in CloudScripts)
			{
				cloudScript.setLOD(level);
				GameController.cloudController.SetLODs++;
				if (GameController.cloudController.SetLODs > GameController.cloudController.CloudLODAdjustmentsPerFrame - 1)
				{
					GameController.cloudController.SetLODs = 0;
					yield return null;
				}
			}
			mPreviousDistanceFromCloudRating = level;
			BusyLODSeting = false;
		}

		public void GenerateCumulous()
		{
			for (int i = 0; i < GameController.cloudController.CloudSpawnAreaRandomHieght; i++)
			{
				mCurrentLayer = i;
				if (i == 0)
				{
					mCloudShape = new int[GameController.cloudController.CloudSpawnAreaRandomX, GameController.cloudController.CloudSpawnAreaRandomZ];
					RandomCloudBace();
					for (int j = 0; j < 5; j++)
					{
						SmoothClouds();
					}
				}
				else
				{
					NextCloudLayerAdjustments();
				}
				PlaceTiles();
			}
			foreach (Clouds cloudScript in CloudScripts)
			{
				cloudScript.ShadowDetector.SetActive(true);
				cloudScript.ShadowDetectorScript.ShadowCheck();
			}
			DeactivateCloudPhysics();
			Ready = true;
		}

		private void RandomCloudBace()
		{
			mSeed = DateTime.Now.ToString();
			System.Random random = new System.Random(mSeed.GetHashCode());
			for (int i = 0; i < GameController.cloudController.CloudSpawnAreaRandomX; i++)
			{
				for (int j = 0; j < GameController.cloudController.CloudSpawnAreaRandomZ; j++)
				{
					if (i == 0 || i == GameController.cloudController.CloudSpawnAreaRandomX - 1 || j == 0 || j == GameController.cloudController.CloudSpawnAreaRandomZ - 1)
					{
						mCloudShape[i, j] = 1;
					}
					else
					{
						mCloudShape[i, j] = ((random.Next(0, 100) < GameController.cloudController.CloudSpawnAreaRandomFillPercent) ? 1 : 0);
					}
				}
			}
		}

		private void SmoothClouds()
		{
			for (int i = 0; i < GameController.cloudController.CloudSpawnAreaRandomX; i++)
			{
				for (int j = 0; j < GameController.cloudController.CloudSpawnAreaRandomZ; j++)
				{
					int num = FindNeighboringCloudTiles(i, j);
					if (num > 4)
					{
						mCloudShape[i, j] = 1;
					}
					else if (num < 4)
					{
						mCloudShape[i, j] = 0;
					}
				}
			}
		}

		private int FindNeighboringCloudTiles(int gridX, int gridZ)
		{
			int num = 0;
			for (int i = gridX - 1; i <= gridX + 1; i++)
			{
				for (int j = gridZ - 1; j <= gridZ + 1; j++)
				{
					if (i >= 0 && i < GameController.cloudController.CloudSpawnAreaRandomX && j >= 0 && j < GameController.cloudController.CloudSpawnAreaRandomZ)
					{
						if (i != gridX || j != gridZ)
						{
							num += mCloudShape[i, j];
						}
					}
					else
					{
						num++;
					}
				}
			}
			return num;
		}

		private void PlaceTiles()
		{
			float cloudScale = GameController.cloudController.CloudScale;
			if (mCloudShape == null)
			{
				return;
			}
			for (int i = 0; i < GameController.cloudController.CloudSpawnAreaRandomX; i++)
			{
				for (int j = 0; j < GameController.cloudController.CloudSpawnAreaRandomZ; j++)
				{
					if (mCloudShape[i, j] == 0 || GameController.cloudController.Overcast)
					{
						PlaceCloudTile(new Vector3((float)i * (cloudScale * 2f) - cloudScale * (float)GameController.cloudController.CloudSpawnAreaRandomX, (float)mCurrentLayer * (cloudScale * 2f), (float)j * (cloudScale * 2f) - cloudScale * (float)GameController.cloudController.CloudSpawnAreaRandomZ));
					}
				}
			}
		}

		private void NextCloudLayerAdjustments()
		{
			for (int i = 0; i < GameController.cloudController.CloudSpawnAreaRandomX; i++)
			{
				for (int j = 0; j < GameController.cloudController.CloudSpawnAreaRandomZ; j++)
				{
					int num = FindNeighboringCloudTiles(i, j);
					if (9 - num < UnityEngine.Random.Range(0, GameController.cloudController.CloudSpawnAreaRandomReductionFactorPerLayer))
					{
						mCloudShape[i, j] = 1;
					}
				}
			}
		}
	}
}
