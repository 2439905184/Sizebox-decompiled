using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CenterOrigin : MonoBehaviour
{
	[SerializeField]
	private Transform cameraTransform;

	private static Vector3 virtualOrigin;

	private float threshold = 5000f;

	private float freecamThreshold = 1000f;

	private Player player;

	private float timeDelay = 3f;

	private float lastUpdate;

	private float currentTime;

	private List<GameObject> rootObjects;

	private ParticleSystem.Particle[] parts;

	private static Vector3 aux;

	public static UnityAction<Vector3> onCentering;

	public static CenterOrigin Instance { get; private set; }

	private void Start()
	{
		if (!Instance)
		{
			Instance = this;
			virtualOrigin = Vector3.zero;
			lastUpdate = -100f;
		}
		else
		{
			Debug.LogError("More than one CenterOrigin script is running. Deleting duplicate.");
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		currentTime = Time.time;
		if (currentTime > lastUpdate + timeDelay)
		{
			UpdateOrigin();
		}
	}

	public void ForceUpdate()
	{
		Vector3 position = player.Entity.transform.position;
		lastUpdate = currentTime;
		MoveOrigin(position);
	}

	private void UpdateOrigin()
	{
		if (!player && (bool)GameController.LocalClient)
		{
			player = GameController.LocalClient.Player;
		}
		if ((bool)player)
		{
			float num;
			Vector3 position;
			if (player.Entity != null)
			{
				num = player.Entity.Scale * threshold * (player.Entity.isGiantess ? 50f : 1f);
				position = player.Entity.transform.position;
			}
			else
			{
				num = cameraTransform.localScale.y * freecamThreshold;
				position = cameraTransform.position;
			}
			if (position.sqrMagnitude > num * num)
			{
				lastUpdate = currentTime;
				MoveOrigin(position);
			}
		}
	}

	private void MoveOrigin(Vector3 position)
	{
		Vector3 vector = -position;
		Scene activeScene = SceneManager.GetActiveScene();
		if (rootObjects == null || rootObjects.Capacity < activeScene.rootCount)
		{
			rootObjects = new List<GameObject>(Mathf.RoundToInt((float)activeScene.rootCount * 1.5f));
		}
		activeScene.GetRootGameObjects(rootObjects);
		foreach (GameObject rootObject in rootObjects)
		{
			rootObject.transform.position += vector;
		}
		MoveParticles(vector);
		virtualOrigin += vector;
		UnityAction<Vector3> unityAction = onCentering;
		if (unityAction != null)
		{
			unityAction(vector);
		}
	}

	private void MoveParticles(Vector3 offset)
	{
		Object[] array = Object.FindObjectsOfType(typeof(ParticleSystem));
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = (ParticleSystem)array[i];
			if (particleSystem.main.simulationSpace != ParticleSystemSimulationSpace.World)
			{
				continue;
			}
			int maxParticles = particleSystem.main.maxParticles;
			if (maxParticles > 0)
			{
				bool isPaused = particleSystem.isPaused;
				bool isPlaying = particleSystem.isPlaying;
				if (!isPaused)
				{
					particleSystem.Pause();
				}
				if (parts == null || parts.Length < maxParticles)
				{
					parts = new ParticleSystem.Particle[maxParticles];
				}
				int particles = particleSystem.GetParticles(parts);
				for (int j = 0; j < particles; j++)
				{
					parts[j].position += offset;
				}
				particleSystem.SetParticles(parts, particles);
				if (isPlaying)
				{
					particleSystem.Play();
				}
			}
		}
	}

	public static Vector3 WorldToVirtual(Vector3 worldPosition)
	{
		worldPosition.x -= virtualOrigin.x;
		worldPosition.y -= virtualOrigin.y;
		worldPosition.z -= virtualOrigin.z;
		return worldPosition;
	}

	public static Vector3 WorldToVirtual(float x, float y, float z)
	{
		aux.x = x - virtualOrigin.x;
		aux.y = y - virtualOrigin.y;
		aux.z = z - virtualOrigin.z;
		return aux;
	}

	public static Vector3 VirtualToWorld(Vector3 virtualPosition)
	{
		virtualPosition.x += virtualOrigin.x;
		virtualPosition.y += virtualOrigin.y;
		virtualPosition.z += virtualOrigin.z;
		return virtualPosition;
	}

	public static Vector3 VirtualToWorld(float x, float y, float z)
	{
		aux.x = x + virtualOrigin.x;
		aux.y = y + virtualOrigin.y;
		aux.z = z + virtualOrigin.z;
		return aux;
	}
}
