using System.Collections.Generic;
using UnityEngine;

public class ResizeManager : MonoBehaviour
{
	public class Resizer
	{
		private EntityBase entity;

		private float startTime;

		private float duration;

		private float factor;

		public readonly bool loop;

		private float originalScale;

		public Resizer(float duration, float factor, bool loop)
		{
			this.duration = duration;
			this.factor = factor;
			this.loop = loop;
		}

		public void Init(EntityBase entity)
		{
			this.entity = entity;
			startTime = Time.time;
			originalScale = entity.transform.localScale.y;
		}

		public bool Update()
		{
			if (factor == 0f)
			{
				return true;
			}
			float y = entity.transform.localScale.y;
			float num = ((duration != 0f) ? (Mathf.Min(b: startTime + duration - Time.time + Time.smoothDeltaTime, a: Time.smoothDeltaTime) / duration) : 1f);
			float num2 = originalScale * factor * num;
			float num3 = y + num2;
			entity.ChangeScale(num3);
			if (num3 > entity.maxSize || num3 < entity.minSize)
			{
				factor = 0f;
			}
			if (Time.time >= startTime + duration)
			{
				if (loop)
				{
					startTime += duration;
					originalScale = num3;
				}
				else
				{
					factor = 0f;
				}
			}
			return false;
		}

		public bool IsCompleted()
		{
			if (!loop)
			{
				return factor == 0f;
			}
			return true;
		}

		public void Interrupt()
		{
			if (!loop)
			{
				factor = 0f;
			}
		}
	}

	private Resizer loopResizer = new Resizer(1f, 0f, true);

	private List<Resizer> resizers = new List<Resizer>();

	public void AddResizer(Resizer resizer)
	{
		resizer.Init(GetComponent<EntityBase>());
		if (resizer.loop)
		{
			loopResizer = resizer;
		}
		else
		{
			resizers.Add(resizer);
		}
	}

	private void Update()
	{
		loopResizer.Update();
		for (int num = resizers.Count - 1; num >= 0; num--)
		{
			if (resizers[num].Update())
			{
				resizers.RemoveAt(num);
			}
		}
	}
}
