using UnityEngine;

public class SharedKnowledge : IListener
{
	public EntityBase dangerEntity;

	private float baseRadius = 1.5f;

	public SharedKnowledge()
	{
		EventManager.Register(this, EventCode.OnCrush);
	}

	public void OnNotify(IEvent e)
	{
		CrushEvent crushEvent = (CrushEvent)e;
		if (crushEvent.crusher != null)
		{
			dangerEntity = crushEvent.crusher;
		}
	}

	public float CheckDanger(EntityBase agent)
	{
		float result = 0f;
		if (dangerEntity == null)
		{
			return result;
		}
		float num = (dangerEntity.Height - agent.Height) * baseRadius;
		Vector3 vector = agent.transform.position - dangerEntity.transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		result = 1f - magnitude / num;
		return Mathf.Clamp01(result * 4f);
	}
}
