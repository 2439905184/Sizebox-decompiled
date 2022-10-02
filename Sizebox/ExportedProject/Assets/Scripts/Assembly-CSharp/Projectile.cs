using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	protected float baseSpeed = 20f;

	protected bool alive;

	protected LayerMask mask;

	protected Color mainColor;

	protected float lifeTime;

	[SerializeField]
	protected AudioSource audioSource;

	protected virtual void Start()
	{
		alive = true;
	}

	protected void OnTriggerEnter(Collider collider)
	{
		if (!alive)
		{
			return;
		}
		GameObject gameObject = collider.gameObject;
		if ((mask.value & (1 << gameObject.layer)) == 1 << gameObject.layer)
		{
			if (gameObject.layer == Layers.gtsBodyLayer)
			{
				gameObject = gameObject.GetComponent<GiantessBone>().giantess.gameObject;
			}
			else if (gameObject.layer == Layers.objectLayer)
			{
				GiantessBone component = gameObject.GetComponent<GiantessBone>();
				gameObject = ((!(component != null)) ? gameObject.transform.parent.gameObject : component.giantess.gameObject);
			}
			Impact(gameObject);
		}
	}

	protected abstract void Impact(GameObject hitObject);

	protected abstract void SetColor(Color color, float emissionModifier = 1f);

	public void SetSoundClip(AudioClip clip)
	{
		audioSource.clip = clip;
	}

	protected void SetupAILayerMask()
	{
		List<string> list = new List<string>();
		list.Add(LayerMask.LayerToName(Layers.mapLayer));
		if (GlobalPreferences.AIProjectileGtsMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.gtsBodyLayer));
		}
		if (GlobalPreferences.AIProjectileMicroMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.microLayer));
		}
		if (GlobalPreferences.AIProjectileObjectMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.objectLayer));
		}
		if (GlobalPreferences.AIProjectilePlayerMask.value)
		{
			list.Add(LayerMask.LayerToName(Layers.playerLayer));
		}
		mask = LayerMask.GetMask(list.ToArray());
	}
}
