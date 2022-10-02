using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static Inputs inputs;

	public static InputPreferences inputPreferences;

	private Dictionary<GameMode, HashSet<InputActionMap>> registeredActionMaps = new Dictionary<GameMode, HashSet<InputActionMap>>();

	private HashSet<InputActionMap> activeActionMaps = new HashSet<InputActionMap>();

	public static InputManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		if (inputs == null)
		{
			inputs = new Inputs();
		}
		if (inputPreferences == null)
		{
			inputPreferences = new InputPreferences(inputs);
			inputPreferences.LoadAllOverrides();
		}
		registeredActionMaps.Add(GameMode.Play, new HashSet<InputActionMap>());
		registeredActionMaps.Add(GameMode.Edit, new HashSet<InputActionMap>());
		registeredActionMaps.Add(GameMode.FreeCam, new HashSet<InputActionMap>());
		registeredActionMaps.Add(GameMode.Pause, new HashSet<InputActionMap>());
		registeredActionMaps.Add(GameMode.Typing, new HashSet<InputActionMap>());
	}

	private void Start()
	{
		if ((bool)GameController.Instance)
		{
			GameController instance = GameController.Instance;
			instance.onModeChange = (UnityAction<GameMode>)Delegate.Combine(instance.onModeChange, new UnityAction<GameMode>(RefreshActiveControls));
		}
	}

	public void EnableControls(InputActionMap actionMap, GameMode mode)
	{
		registeredActionMaps[mode].Add(actionMap);
		RefreshActiveControls(GameController.Instance.mode);
	}

	public void DisableControls(InputActionMap actionMap, GameMode mode)
	{
		registeredActionMaps[mode].Remove(actionMap);
		RefreshActiveControls(GameController.Instance.mode);
	}

	private void RefreshActiveControls(GameMode mode)
	{
		foreach (InputActionMap activeActionMap in activeActionMaps)
		{
			activeActionMap.Disable();
		}
		activeActionMaps.Clear();
		foreach (InputActionMap item in registeredActionMaps[mode])
		{
			item.Enable();
			activeActionMaps.Add(item);
		}
	}
}
