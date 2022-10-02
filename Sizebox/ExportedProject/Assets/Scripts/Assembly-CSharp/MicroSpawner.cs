using System.Collections;
using UnityEngine;

public class MicroSpawner : MonoBehaviour
{
	private int _maxMicros = 20;

	private int _microCount;

	private float _timeInterval = 1f;

	private MicroNpc[] _micros;

	private void Start()
	{
		_micros = new MicroNpc[_maxMicros];
		StartCoroutine(Routine());
	}

	private IEnumerator Routine()
	{
		while (!LocalClient.Instance)
		{
			yield return new WaitForSeconds(_timeInterval);
		}
		while ((bool)base.gameObject)
		{
			if (_microCount < _maxMicros)
			{
				SpawnMicro(_microCount);
				_microCount++;
			}
			else
			{
				CheckIfDead();
			}
			yield return new WaitForSeconds(_timeInterval);
		}
	}

	private void CheckIfDead()
	{
		for (int i = 0; i < _maxMicros; i++)
		{
			MicroNpc microNpc = _micros[i];
			if (!microNpc || microNpc.IsDead)
			{
				SpawnMicro(i);
				break;
			}
		}
	}

	private void SpawnMicro(int i)
	{
		AssetDescription randomMicroAsset = AssetManager.Instance.GetRandomMicroAsset();
		if (randomMicroAsset != null)
		{
			Transform transform = base.transform;
			MicroNpc microNpc = ObjectManager.Instance.InstantiateMicro(randomMicroAsset, transform.position, transform.rotation, transform.lossyScale.y);
			_micros[i] = microNpc;
		}
	}
}
