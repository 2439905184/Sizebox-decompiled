using UnityEngine;

namespace Assets.Scripts.DynamicClouds
{
	public class CloudShadowDetector : MonoBehaviour
	{
		public bool GtsCastsOnlyLiteShadow;

		public int GtsShadowFactor = 100;

		public ParticleSystem Clouds;

		public Gradient[] Shades;

		private Gradient mOrignalColor;

		public int ShadeAmount;

		private int mPreviousShade;

		public LayerMask GetShadowsFrom;

		private void Start()
		{
			base.transform.eulerAngles = SimpleSunController.SunDirection;
		}

		public void ShadowCheck()
		{
			base.transform.eulerAngles = SimpleSunController.SunDirection;
			RaycastHit[] array = Physics.RaycastAll(new Ray(base.transform.position, -base.transform.forward), (int)GetShadowsFrom);
			int num = array.Length + 1;
			int num2 = 0;
			if (!GtsCastsOnlyLiteShadow)
			{
				bool flag = false;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (raycastHit.collider.gameObject.layer == Layers.gtsBodyLayer)
					{
						flag = true;
					}
					if (raycastHit.collider.gameObject.layer == Layers.cloudProximityLayer)
					{
						num2++;
					}
				}
				if (flag)
				{
					num += GtsShadowFactor;
				}
				num -= num2;
			}
			num *= 2;
			ShadeAmount = Mathf.Clamp(num, 1, Shades.Length - 1);
			if (mPreviousShade != ShadeAmount)
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime = Clouds.colorOverLifetime;
				colorOverLifetime.color = Shades[ShadeAmount];
				mPreviousShade = ShadeAmount;
			}
		}
	}
}
