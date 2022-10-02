using UnityEngine;

public class MentalState
{
	private EntityBase entity;

	public float normal;

	public float fear;

	public float curiosity;

	public float hostile;

	public MentalState(EntityBase entity)
	{
		this.entity = entity;
		normal = 0.5f;
		curiosity = Mathf.Pow(Random.value, 8f);
		hostile = Mathf.Pow(Random.value, 2f);
	}

	public void Update()
	{
		fear = GameController.Instance.sharedKnowledge.CheckDanger(entity) * (1f - curiosity);
		if (fear > Random.value)
		{
			fear = 1f;
		}
		else
		{
			fear = 0f;
		}
	}

	public EntityBase ChooseTarget()
	{
		EntityBase dangerEntity = GameController.Instance.sharedKnowledge.dangerEntity;
		if (dangerEntity == null)
		{
			return null;
		}
		if (entity.transform.IsChildOf(dangerEntity.transform))
		{
			return null;
		}
		return dangerEntity;
	}
}
