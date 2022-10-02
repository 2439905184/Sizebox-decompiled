using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnityInputSystemRebind : MonoBehaviour
{
	[Serializable]
	public class UpdateBindingUIEvent : UnityEvent<UnityInputSystemRebind, string, string, string>
	{
	}

	[Serializable]
	public class InteractiveRebindEvent : UnityEvent<UnityInputSystemRebind, InputActionRebindingExtensions.RebindingOperation>
	{
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass29_0
	{
		public Guid bindingId;

		internal bool _003CResolveActionAndBinding_003Eb__0(InputBinding x)
		{
			return x.id == bindingId;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass33_0
	{
		public UnityInputSystemRebind _003C_003E4__this;

		public bool allCompositeParts;

		public int bindingIndex;

		public InputAction action;

		internal void _003CPerformInteractiveRebind_003Eg__CleanUp_007C0()
		{
			InputActionRebindingExtensions.RebindingOperation rebindOperation = _003C_003E4__this.m_RebindOperation;
			if (rebindOperation != null)
			{
				rebindOperation.Dispose();
			}
			_003C_003E4__this.m_RebindOperation = null;
		}

		internal void _003CPerformInteractiveRebind_003Eb__1(InputActionRebindingExtensions.RebindingOperation operation)
		{
			InteractiveRebindEvent rebindStopEvent = _003C_003E4__this.m_RebindStopEvent;
			if (rebindStopEvent != null)
			{
				rebindStopEvent.Invoke(_003C_003E4__this, operation);
			}
			GameObject rebindOverlay = _003C_003E4__this.m_RebindOverlay;
			if ((object)rebindOverlay != null)
			{
				rebindOverlay.SetActive(false);
			}
			_003C_003E4__this.UpdateBindingDisplay();
			_003CPerformInteractiveRebind_003Eg__CleanUp_007C0();
		}

		internal void _003CPerformInteractiveRebind_003Eb__2(InputActionRebindingExtensions.RebindingOperation operation)
		{
			GameObject rebindOverlay = _003C_003E4__this.m_RebindOverlay;
			if ((object)rebindOverlay != null)
			{
				rebindOverlay.SetActive(false);
			}
			InteractiveRebindEvent rebindStopEvent = _003C_003E4__this.m_RebindStopEvent;
			if (rebindStopEvent != null)
			{
				rebindStopEvent.Invoke(_003C_003E4__this, operation);
			}
			_003C_003E4__this.UpdateBindingDisplay();
			_003CPerformInteractiveRebind_003Eg__CleanUp_007C0();
			if (allCompositeParts)
			{
				int num = bindingIndex + 1;
				if (num < action.bindings.Count && action.bindings[num].isPartOfComposite)
				{
					_003C_003E4__this.PerformInteractiveRebind(action, num, true);
				}
			}
		}
	}

	[Tooltip("Reference to action that is to be rebound from the UI.")]
	[SerializeField]
	private InputActionReference m_Action;

	[SerializeField]
	private string m_BindingId;

	[SerializeField]
	private InputBinding.DisplayStringOptions m_DisplayStringOptions;

	[Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the rebind UI not show a label for the action.")]
	[SerializeField]
	private Text m_ActionLabel;

	[Tooltip("Text label that will receive the current, formatted binding string.")]
	[SerializeField]
	private Text m_BindingText;

	[Tooltip("Optional UI that will be shown while a rebind is in progress.")]
	[SerializeField]
	private GameObject m_RebindOverlay;

	[Tooltip("Optional text label that will be updated with prompt for user input.")]
	[SerializeField]
	private Text m_RebindText;

	[Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying bindings in custom ways, e.g. using images instead of text.")]
	[SerializeField]
	private UpdateBindingUIEvent m_UpdateBindingUIEvent;

	[Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, to implement custom UI behavior while a rebind is in progress. It can also be used to further customize the rebind.")]
	[SerializeField]
	private InteractiveRebindEvent m_RebindStartEvent;

	[Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
	[SerializeField]
	private InteractiveRebindEvent m_RebindStopEvent;

	private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

	private static List<UnityInputSystemRebind> s_RebindActionUIs;

	public InputActionReference actionReference
	{
		get
		{
			return m_Action;
		}
		set
		{
			m_Action = value;
			UpdateActionLabel();
			UpdateBindingDisplay();
		}
	}

	public string bindingId
	{
		get
		{
			return m_BindingId;
		}
		set
		{
			m_BindingId = value;
			UpdateBindingDisplay();
		}
	}

	public InputBinding.DisplayStringOptions displayStringOptions
	{
		get
		{
			return m_DisplayStringOptions;
		}
		set
		{
			m_DisplayStringOptions = value;
			UpdateBindingDisplay();
		}
	}

	public Text actionLabel
	{
		get
		{
			return m_ActionLabel;
		}
		set
		{
			m_ActionLabel = value;
			UpdateActionLabel();
		}
	}

	public Text bindingText
	{
		get
		{
			return m_BindingText;
		}
		set
		{
			m_BindingText = value;
			UpdateBindingDisplay();
		}
	}

	public Text rebindPrompt
	{
		get
		{
			return m_RebindText;
		}
		set
		{
			m_RebindText = value;
		}
	}

	public GameObject rebindOverlay
	{
		get
		{
			return m_RebindOverlay;
		}
		set
		{
			m_RebindOverlay = value;
		}
	}

	public UpdateBindingUIEvent updateBindingUIEvent
	{
		get
		{
			if (m_UpdateBindingUIEvent == null)
			{
				m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
			}
			return m_UpdateBindingUIEvent;
		}
	}

	public InteractiveRebindEvent startRebindEvent
	{
		get
		{
			if (m_RebindStartEvent == null)
			{
				m_RebindStartEvent = new InteractiveRebindEvent();
			}
			return m_RebindStartEvent;
		}
	}

	public InteractiveRebindEvent stopRebindEvent
	{
		get
		{
			if (m_RebindStopEvent == null)
			{
				m_RebindStopEvent = new InteractiveRebindEvent();
			}
			return m_RebindStopEvent;
		}
	}

	public InputActionRebindingExtensions.RebindingOperation ongoingRebind
	{
		get
		{
			return m_RebindOperation;
		}
	}

	public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
	{
		_003C_003Ec__DisplayClass29_0 _003C_003Ec__DisplayClass29_ = new _003C_003Ec__DisplayClass29_0();
		bindingIndex = -1;
		InputActionReference action2 = m_Action;
		action = (((object)action2 != null) ? action2.action : null);
		if (action == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(m_BindingId))
		{
			return false;
		}
		_003C_003Ec__DisplayClass29_.bindingId = new Guid(m_BindingId);
		bindingIndex = action.bindings.IndexOf(_003C_003Ec__DisplayClass29_._003CResolveActionAndBinding_003Eb__0);
		if (bindingIndex == -1)
		{
			Debug.LogError(string.Format("Cannot find binding with ID '{0}' on '{1}'", _003C_003Ec__DisplayClass29_.bindingId, action), this);
			return false;
		}
		return true;
	}

	public void UpdateBindingDisplay()
	{
		string text = string.Empty;
		string deviceLayoutName = null;
		string controlPath = null;
		InputActionReference action = m_Action;
		InputAction inputAction = (((object)action != null) ? action.action : null);
		if (inputAction != null)
		{
			int num = inputAction.bindings.IndexOf(_003CUpdateBindingDisplay_003Eb__30_0);
			if (num != -1)
			{
				text = inputAction.GetBindingDisplayString(num, out deviceLayoutName, out controlPath, displayStringOptions);
			}
		}
		if (m_BindingText != null)
		{
			m_BindingText.text = text;
		}
		UpdateBindingUIEvent obj = m_UpdateBindingUIEvent;
		if (obj != null)
		{
			obj.Invoke(this, text, deviceLayoutName, controlPath);
		}
	}

	public void ResetToDefault()
	{
		InputAction action;
		int bindingIndex;
		if (!ResolveActionAndBinding(out action, out bindingIndex))
		{
			return;
		}
		if (action.bindings[bindingIndex].isComposite)
		{
			for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
			{
				action.RemoveBindingOverride(i);
			}
		}
		else
		{
			action.RemoveBindingOverride(bindingIndex);
		}
		UpdateBindingDisplay();
	}

	public void StartInteractiveRebind()
	{
		InputAction action;
		int bindingIndex;
		if (!ResolveActionAndBinding(out action, out bindingIndex))
		{
			return;
		}
		if (action.bindings[bindingIndex].isComposite)
		{
			int num = bindingIndex + 1;
			if (num < action.bindings.Count && action.bindings[num].isPartOfComposite)
			{
				PerformInteractiveRebind(action, num, true);
			}
		}
		else
		{
			PerformInteractiveRebind(action, bindingIndex);
		}
	}

	private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
	{
		_003C_003Ec__DisplayClass33_0 _003C_003Ec__DisplayClass33_ = new _003C_003Ec__DisplayClass33_0();
		_003C_003Ec__DisplayClass33_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass33_.allCompositeParts = allCompositeParts;
		_003C_003Ec__DisplayClass33_.bindingIndex = bindingIndex;
		_003C_003Ec__DisplayClass33_.action = action;
		InputActionRebindingExtensions.RebindingOperation rebindOperation = m_RebindOperation;
		if (rebindOperation != null)
		{
			rebindOperation.Cancel();
		}
		m_RebindOperation = _003C_003Ec__DisplayClass33_.action.PerformInteractiveRebinding(_003C_003Ec__DisplayClass33_.bindingIndex).WithCancelingThrough("<Keyboard>/escape").WithMagnitudeHavingToBeGreaterThan(0.9f)
			.OnMatchWaitForAnother(0.1f)
			.OnCancel(_003C_003Ec__DisplayClass33_._003CPerformInteractiveRebind_003Eb__1)
			.OnComplete(_003C_003Ec__DisplayClass33_._003CPerformInteractiveRebind_003Eb__2);
		string text = null;
		if (_003C_003Ec__DisplayClass33_.action.bindings[_003C_003Ec__DisplayClass33_.bindingIndex].isPartOfComposite)
		{
			text = "Binding '" + _003C_003Ec__DisplayClass33_.action.bindings[_003C_003Ec__DisplayClass33_.bindingIndex].name + "'. ";
		}
		GameObject obj = m_RebindOverlay;
		if ((object)obj != null)
		{
			obj.SetActive(true);
		}
		if (m_RebindText != null)
		{
			string text2 = ((!string.IsNullOrEmpty(m_RebindOperation.expectedControlType)) ? (text + "Waiting for " + m_RebindOperation.expectedControlType + " input... Press 'Esc' to cancel") : (text + "Waiting for input... Press 'Esc' to cancel"));
			m_RebindText.text = text2;
		}
		if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
		{
			m_BindingText.text = "<Waiting...>";
		}
		InteractiveRebindEvent rebindStartEvent = m_RebindStartEvent;
		if (rebindStartEvent != null)
		{
			rebindStartEvent.Invoke(this, m_RebindOperation);
		}
		m_RebindOperation.Start();
	}

	protected void OnEnable()
	{
		if (s_RebindActionUIs == null)
		{
			s_RebindActionUIs = new List<UnityInputSystemRebind>();
		}
		s_RebindActionUIs.Add(this);
		if (s_RebindActionUIs.Count == 1)
		{
			InputSystem.onActionChange += OnActionChange;
		}
	}

	protected void OnDisable()
	{
		InputActionRebindingExtensions.RebindingOperation rebindOperation = m_RebindOperation;
		if (rebindOperation != null)
		{
			rebindOperation.Dispose();
		}
		m_RebindOperation = null;
		s_RebindActionUIs.Remove(this);
		if (s_RebindActionUIs.Count == 0)
		{
			s_RebindActionUIs = null;
			InputSystem.onActionChange -= OnActionChange;
		}
	}

	private static void OnActionChange(object obj, InputActionChange change)
	{
		if (change != InputActionChange.BoundControlsChanged)
		{
			return;
		}
		InputAction inputAction = obj as InputAction;
		InputActionMap inputActionMap = ((inputAction != null) ? inputAction.actionMap : null) ?? (obj as InputActionMap);
		InputActionAsset inputActionAsset = ((inputActionMap != null) ? inputActionMap.asset : null) ?? (obj as InputActionAsset);
		for (int i = 0; i < s_RebindActionUIs.Count; i++)
		{
			UnityInputSystemRebind unityInputSystemRebind = s_RebindActionUIs[i];
			InputActionReference inputActionReference = unityInputSystemRebind.actionReference;
			InputAction inputAction2 = (((object)inputActionReference != null) ? inputActionReference.action : null);
			if (inputAction2 == null)
			{
				continue;
			}
			if (inputAction2 != inputAction && inputAction2.actionMap != inputActionMap)
			{
				InputActionMap actionMap = inputAction2.actionMap;
				if (!(((actionMap != null) ? actionMap.asset : null) == inputActionAsset))
				{
					continue;
				}
			}
			unityInputSystemRebind.UpdateBindingDisplay();
		}
	}

	private void UpdateActionLabel()
	{
		if (m_ActionLabel != null)
		{
			InputActionReference action = m_Action;
			InputAction inputAction = (((object)action != null) ? action.action : null);
			m_ActionLabel.text = ((inputAction != null) ? inputAction.name : string.Empty);
		}
	}

	[CompilerGenerated]
	private bool _003CUpdateBindingDisplay_003Eb__30_0(InputBinding x)
	{
		return x.id.ToString() == m_BindingId;
	}
}
