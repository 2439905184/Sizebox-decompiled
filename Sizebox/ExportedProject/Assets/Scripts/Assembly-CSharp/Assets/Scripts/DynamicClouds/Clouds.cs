using UnityEngine;

namespace Assets.Scripts.DynamicClouds
{
	public class Clouds : MonoBehaviour
	{
		public GameObject ShadowDetector;

		public CloudShadowDetector ShadowDetectorScript;

		public int[] ParticleMaxPerLOD;

		public float[] ParticleRatePerLOD;

		public bool DisableCloudElasticity;

		public bool LimitCloudPush;

		public float CloudElasticityFactor;

		public float PositionTolerence;

		public float PositionMaxLimit;

		public SphereCollider GtsColider;

		public SphereCollider MainCollider;

		public ParticleSystem CloudParticles;

		public ParticleSystem.MainModule CPEditMain;

		public ParticleSystem.ShapeModule CPEditShape;

		public ParticleSystem.EmissionModule CPEditEmission;

		public GameObject TestVisuals;

		public Rigidbody Rbody;

		public Vector3 Origin;

		private float mXLimitMax;

		private float mYLimitMax;

		private float mZLimitMax;

		private float mXLimitMin;

		private float mYLimitMin;

		private float mZLimitMin;

		private void Awake()
		{
			CPEditMain = CloudParticles.main;
			CPEditShape = CloudParticles.shape;
			CPEditEmission = CloudParticles.emission;
			SetConstraints();
			SetElasticityOnOff();
			SetElasticityFactor();
			SetElasticityTolerance();
			SetParticleSize();
			SetParticleShape();
			SetTestMode();
		}

		public void SetConstraints()
		{
			Rbody.constraints = RigidbodyConstraints.None;
			if (GameController.cloudController.LockCloudTileElevation)
			{
				Rbody.constraints = RigidbodyConstraints.FreezePositionY;
			}
			if (GameController.cloudController.LockCloudTileRotation)
			{
				Rbody.constraints = RigidbodyConstraints.FreezeRotation;
			}
		}

		public void SetElasticityOnOff()
		{
			DisableCloudElasticity = GameController.cloudController.DisableCloudElastisity;
		}

		public void SetPushLimitOnOff()
		{
			LimitCloudPush = GameController.cloudController.LimitCloudpush;
		}

		public void SetElasticityFactor()
		{
			CloudElasticityFactor = GameController.cloudController.CloudElastisityFactor;
		}

		public void SetElasticityTolerance()
		{
			PositionTolerence = GameController.cloudController.CloudElastisityPositionTolerence;
		}

		public void SetPushLimitDistance()
		{
			PositionMaxLimit = GameController.cloudController.CloudElastisityPositionMaxLimit * (GameController.cloudController.CloudScale * 2f);
			mXLimitMax = Origin.x + PositionMaxLimit;
			mXLimitMin = Origin.x - PositionMaxLimit;
			mYLimitMax = Origin.y + PositionMaxLimit;
			mYLimitMin = Origin.y - PositionMaxLimit;
			mZLimitMax = Origin.z + PositionMaxLimit;
			mZLimitMin = Origin.z - PositionMaxLimit;
		}

		public void SetParticleSize()
		{
			CPEditMain.startSize = new ParticleSystem.MinMaxCurve(0.75f * GameController.cloudController.CloudScale, 5f * GameController.cloudController.CloudScale);
		}

		public void SetParticleShape()
		{
			CPEditShape.scale = new Vector3(GameController.cloudController.CloudScale * 2f, GameController.cloudController.CloudScale * 2f, GameController.cloudController.CloudScale * 2f);
		}

		public void SetGtsAirCpompressionFactor(float size)
		{
			if (GameController.cloudController.GtsAirComressionFactor > 0f)
			{
				GtsColider.gameObject.SetActive(true);
				GtsColider.radius = GameController.cloudController.GtsAirComressionFactor * size;
			}
			else
			{
				GtsColider.gameObject.SetActive(false);
			}
		}

		public void SetTestMode()
		{
			if (GameController.cloudController.UseTestVisuals)
			{
				TestVisuals.SetActive(true);
				CloudParticles.gameObject.SetActive(false);
				TestVisuals.transform.localScale = new Vector3(GameController.cloudController.CloudScale * 2f, GameController.cloudController.CloudScale * 2f, GameController.cloudController.CloudScale * 2f);
			}
			else
			{
				TestVisuals.SetActive(false);
				CloudParticles.gameObject.SetActive(true);
			}
		}

		public void InitializeCloudTile(float tileSize, Vector3 localOrigin)
		{
			Origin = localOrigin;
			MainCollider.radius = GameController.cloudController.CloudScale * 0.95f;
			SetGtsAirCpompressionFactor(tileSize);
			SetPushLimitDistance();
			SetPushLimitOnOff();
		}

		public void setLOD(int level)
		{
			CPEditMain.maxParticles = ParticleMaxPerLOD[level];
			CPEditEmission.rateOverTime = ParticleRatePerLOD[level];
		}

		private void LateUpdate()
		{
			if (LimitCloudPush)
			{
				base.transform.localPosition = new Vector3(Mathf.Clamp(base.transform.localPosition.x, mXLimitMin, mXLimitMax), Mathf.Clamp(base.transform.localPosition.y, mYLimitMin, mYLimitMax), Mathf.Clamp(base.transform.localPosition.z, mZLimitMin, mZLimitMax));
			}
			if (!DisableCloudElasticity && Vector3.Distance(base.transform.localPosition, Origin) > PositionTolerence)
			{
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, Origin, CloudElasticityFactor);
			}
		}
	}
}
