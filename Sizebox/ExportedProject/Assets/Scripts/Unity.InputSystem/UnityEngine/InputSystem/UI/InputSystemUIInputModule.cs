using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

namespace UnityEngine.InputSystem.UI
{
	public class InputSystemUIInputModule : BaseInputModule
	{
		[FormerlySerializedAs("m_RepeatDelay")]
		[Tooltip("The Initial delay (in seconds) between an initial move action and a repeated move action.")]
		[SerializeField]
		private float m_MoveRepeatDelay = 0.5f;

		[FormerlySerializedAs("m_RepeatRate")]
		[Tooltip("The speed (in seconds) that the move action repeats itself once repeating (max 1 per frame).")]
		[SerializeField]
		private float m_MoveRepeatRate = 0.1f;

		[Tooltip("Scales the Eventsystem.DragThreshold, for tracked devices, to make selection easier.")]
		private float m_TrackedDeviceDragThresholdMultiplier = 2f;

		internal const float kPixelPerLine = 20f;

		[SerializeField]
		[HideInInspector]
		private InputActionAsset m_ActionsAsset;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_PointAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_MoveAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_SubmitAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_CancelAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_LeftClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_MiddleClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_RightClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_ScrollWheelAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_TrackedDevicePositionAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_TrackedDeviceOrientationAction;

		[SerializeField]
		private bool m_DeselectOnBackgroundClick = true;

		[SerializeField]
		private UIPointerBehavior m_PointerBehavior;

		private bool m_OwnsEnabledState;

		private bool m_ActionsHooked;

		private Action<InputAction.CallbackContext> m_OnPointDelegate;

		private Action<InputAction.CallbackContext> m_OnMoveDelegate;

		private Action<InputAction.CallbackContext> m_OnSubmitDelegate;

		private Action<InputAction.CallbackContext> m_OnCancelDelegate;

		private Action<InputAction.CallbackContext> m_OnLeftClickDelegate;

		private Action<InputAction.CallbackContext> m_OnRightClickDelegate;

		private Action<InputAction.CallbackContext> m_OnMiddleClickDelegate;

		private Action<InputAction.CallbackContext> m_OnScrollWheelDelegate;

		private Action<InputAction.CallbackContext> m_OnTrackedDevicePositionDelegate;

		private Action<InputAction.CallbackContext> m_OnTrackedDeviceOrientationDelegate;

		private Action<object> m_OnControlsChangedDelegate;

		private int m_CurrentPointerId = -1;

		private int m_CurrentPointerIndex = -1;

		private UIPointerType m_CurrentPointerType;

		private InlinedArray<int> m_PointerIds;

		private InlinedArray<PointerModel> m_PointerStates;

		private NavigationModel m_NavigationState;

		public bool deselectOnBackgroundClick
		{
			get
			{
				return m_DeselectOnBackgroundClick;
			}
			set
			{
				m_DeselectOnBackgroundClick = value;
			}
		}

		public UIPointerBehavior pointerBehavior
		{
			get
			{
				return m_PointerBehavior;
			}
			set
			{
				m_PointerBehavior = value;
			}
		}

		public float moveRepeatDelay
		{
			get
			{
				return m_MoveRepeatDelay;
			}
			set
			{
				m_MoveRepeatDelay = value;
			}
		}

		public float moveRepeatRate
		{
			get
			{
				return m_MoveRepeatRate;
			}
			set
			{
				m_MoveRepeatRate = value;
			}
		}

		[Obsolete("'repeatRate' has been obsoleted; use 'moveRepeatRate' instead. (UnityUpgradable) -> moveRepeatRate", false)]
		public float repeatRate
		{
			get
			{
				return moveRepeatRate;
			}
			set
			{
				moveRepeatRate = value;
			}
		}

		[Obsolete("'repeatDelay' has been obsoleted; use 'moveRepeatDelay' instead. (UnityUpgradable) -> moveRepeatDelay", false)]
		public float repeatDelay
		{
			get
			{
				return moveRepeatDelay;
			}
			set
			{
				moveRepeatDelay = value;
			}
		}

		public float trackedDeviceDragThresholdMultiplier
		{
			get
			{
				return m_TrackedDeviceDragThresholdMultiplier;
			}
			set
			{
				m_TrackedDeviceDragThresholdMultiplier = value;
			}
		}

		public InputActionReference point
		{
			get
			{
				return m_PointAction;
			}
			set
			{
				SwapAction(ref m_PointAction, value, m_ActionsHooked, m_OnPointDelegate);
			}
		}

		public InputActionReference scrollWheel
		{
			get
			{
				return m_ScrollWheelAction;
			}
			set
			{
				SwapAction(ref m_ScrollWheelAction, value, m_ActionsHooked, m_OnScrollWheelDelegate);
			}
		}

		public InputActionReference leftClick
		{
			get
			{
				return m_LeftClickAction;
			}
			set
			{
				SwapAction(ref m_LeftClickAction, value, m_ActionsHooked, m_OnLeftClickDelegate);
			}
		}

		public InputActionReference middleClick
		{
			get
			{
				return m_MiddleClickAction;
			}
			set
			{
				SwapAction(ref m_MiddleClickAction, value, m_ActionsHooked, m_OnMiddleClickDelegate);
			}
		}

		public InputActionReference rightClick
		{
			get
			{
				return m_RightClickAction;
			}
			set
			{
				SwapAction(ref m_RightClickAction, value, m_ActionsHooked, m_OnRightClickDelegate);
			}
		}

		public InputActionReference move
		{
			get
			{
				return m_MoveAction;
			}
			set
			{
				SwapAction(ref m_MoveAction, value, m_ActionsHooked, m_OnMoveDelegate);
			}
		}

		public InputActionReference submit
		{
			get
			{
				return m_SubmitAction;
			}
			set
			{
				SwapAction(ref m_SubmitAction, value, m_ActionsHooked, m_OnSubmitDelegate);
			}
		}

		public InputActionReference cancel
		{
			get
			{
				return m_CancelAction;
			}
			set
			{
				SwapAction(ref m_CancelAction, value, m_ActionsHooked, m_OnCancelDelegate);
			}
		}

		public InputActionReference trackedDeviceOrientation
		{
			get
			{
				return m_TrackedDeviceOrientationAction;
			}
			set
			{
				SwapAction(ref m_TrackedDeviceOrientationAction, value, m_ActionsHooked, m_OnTrackedDeviceOrientationDelegate);
			}
		}

		public InputActionReference trackedDevicePosition
		{
			get
			{
				return m_TrackedDevicePositionAction;
			}
			set
			{
				SwapAction(ref m_TrackedDevicePositionAction, value, m_ActionsHooked, m_OnTrackedDevicePositionDelegate);
			}
		}

		[Obsolete("'trackedDeviceSelect' has been obsoleted; use 'leftClick' instead.", true)]
		public InputActionReference trackedDeviceSelect
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public InputActionAsset actionsAsset
		{
			get
			{
				return m_ActionsAsset;
			}
			set
			{
				if (value != m_ActionsAsset)
				{
					bool num = IsAnyActionEnabled();
					DisableAllActions();
					m_ActionsAsset = value;
					point = UpdateReferenceForNewAsset(point);
					move = UpdateReferenceForNewAsset(move);
					leftClick = UpdateReferenceForNewAsset(leftClick);
					rightClick = UpdateReferenceForNewAsset(rightClick);
					middleClick = UpdateReferenceForNewAsset(middleClick);
					scrollWheel = UpdateReferenceForNewAsset(scrollWheel);
					submit = UpdateReferenceForNewAsset(submit);
					cancel = UpdateReferenceForNewAsset(cancel);
					if (num)
					{
						EnableAllActions();
					}
				}
			}
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
		}

		public override bool IsPointerOverGameObject(int pointerOrTouchId)
		{
			int num = -1;
			if (pointerOrTouchId < 0)
			{
				if (m_CurrentPointerId != -1)
				{
					num = m_CurrentPointerIndex;
				}
				else if (m_PointerStates.length > 0)
				{
					num = 0;
				}
			}
			else
			{
				num = GetPointerStateIndexFor(pointerOrTouchId);
				if (num == -1)
				{
					for (int i = 0; i < m_PointerStates.length; i++)
					{
						ExtendedPointerEventData eventData = m_PointerStates[i].eventData;
						if (eventData.touchId == pointerOrTouchId || (eventData.touchId != 0 && eventData.device.deviceId == pointerOrTouchId))
						{
							return eventData.pointerEnter != null;
						}
					}
				}
			}
			if (num == -1)
			{
				return false;
			}
			return m_PointerStates[num].eventData.pointerEnter != null;
		}

		private RaycastResult PerformRaycast(ExtendedPointerEventData eventData)
		{
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			if (eventData.pointerType == UIPointerType.Tracked && TrackedDeviceRaycaster.s_Instances.length > 0)
			{
				for (int i = 0; i < TrackedDeviceRaycaster.s_Instances.length; i++)
				{
					TrackedDeviceRaycaster trackedDeviceRaycaster = TrackedDeviceRaycaster.s_Instances[i];
					m_RaycastResultCache.Clear();
					trackedDeviceRaycaster.PerformRaycast(eventData, m_RaycastResultCache);
					if (m_RaycastResultCache.Count > 0)
					{
						RaycastResult result = m_RaycastResultCache[0];
						m_RaycastResultCache.Clear();
						return result;
					}
				}
				return default(RaycastResult);
			}
			base.eventSystem.RaycastAll(eventData, m_RaycastResultCache);
			RaycastResult result2 = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
			return result2;
		}

		private void ProcessPointer(ref PointerModel state)
		{
			if (state.changedThisFrame)
			{
				ExtendedPointerEventData eventData = state.eventData;
				UIPointerType pointerType = eventData.pointerType;
				if (pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked)
				{
					eventData.position = new Vector2(-1f, -1f);
					eventData.delta = default(Vector2);
				}
				else if (pointerType == UIPointerType.Tracked)
				{
					eventData.trackedDeviceOrientation = state.worldOrientation;
					eventData.trackedDevicePosition = state.worldPosition;
				}
				else
				{
					eventData.delta = state.screenPosition - eventData.position;
					eventData.position = state.screenPosition;
				}
				eventData.Reset();
				eventData.pointerCurrentRaycast = PerformRaycast(eventData);
				if (pointerType == UIPointerType.Tracked && eventData.pointerCurrentRaycast.isValid)
				{
					Vector2 screenPosition = eventData.pointerCurrentRaycast.screenPosition;
					eventData.delta = screenPosition - eventData.position;
					eventData.position = eventData.pointerCurrentRaycast.screenPosition;
				}
				eventData.button = PointerEventData.InputButton.Left;
				state.leftButton.CopyPressStateTo(eventData);
				ProcessPointerMovement(ref state, eventData);
				ProcessPointerButton(ref state.leftButton, eventData);
				ProcessPointerButtonDrag(ref state.leftButton, eventData);
				ProcessPointerScroll(ref state, eventData);
				eventData.button = PointerEventData.InputButton.Right;
				state.rightButton.CopyPressStateTo(eventData);
				ProcessPointerButton(ref state.rightButton, eventData);
				ProcessPointerButtonDrag(ref state.rightButton, eventData);
				eventData.button = PointerEventData.InputButton.Middle;
				state.middleButton.CopyPressStateTo(eventData);
				ProcessPointerButton(ref state.middleButton, eventData);
				ProcessPointerButtonDrag(ref state.middleButton, eventData);
				state.OnFrameFinished();
			}
		}

		private bool PointerShouldIgnoreTransform(Transform t)
		{
			MultiplayerEventSystem multiplayerEventSystem;
			if ((object)(multiplayerEventSystem = base.eventSystem as MultiplayerEventSystem) != null && multiplayerEventSystem.playerRoot != null && !t.IsChildOf(multiplayerEventSystem.playerRoot.transform))
			{
				return true;
			}
			return false;
		}

		private void ProcessPointerMovement(ref PointerModel pointer, ExtendedPointerEventData eventData)
		{
			GameObject currentPointerTarget = (((eventData.pointerType == UIPointerType.Touch && pointer.leftButton.wasReleasedThisFrame) || (eventData.pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked)) ? null : eventData.pointerCurrentRaycast.gameObject);
			ProcessPointerMovement(eventData, currentPointerTarget);
		}

		private void ProcessPointerMovement(ExtendedPointerEventData eventData, GameObject currentPointerTarget)
		{
			if (currentPointerTarget == null || eventData.pointerEnter == null)
			{
				for (int i = 0; i < eventData.hovered.Count; i++)
				{
					ExecuteEvents.Execute(eventData.hovered[i], eventData, ExecuteEvents.pointerExitHandler);
				}
				eventData.hovered.Clear();
				if (currentPointerTarget == null)
				{
					eventData.pointerEnter = null;
					return;
				}
			}
			if (eventData.pointerEnter == currentPointerTarget && (bool)currentPointerTarget)
			{
				return;
			}
			GameObject obj = BaseInputModule.FindCommonRoot(eventData.pointerEnter, currentPointerTarget);
			Transform transform = (((object)obj != null) ? obj.transform : null);
			if (eventData.pointerEnter != null)
			{
				Transform parent = eventData.pointerEnter.transform;
				while (parent != null && parent != transform)
				{
					ExecuteEvents.Execute(parent.gameObject, eventData, ExecuteEvents.pointerExitHandler);
					eventData.hovered.Remove(parent.gameObject);
					parent = parent.parent;
				}
			}
			eventData.pointerEnter = currentPointerTarget;
			if (currentPointerTarget != null)
			{
				Transform parent2 = currentPointerTarget.transform;
				while (parent2 != null && parent2 != transform && !PointerShouldIgnoreTransform(parent2))
				{
					ExecuteEvents.Execute(parent2.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
					eventData.hovered.Add(parent2.gameObject);
					parent2 = parent2.parent;
				}
			}
		}

		private void ProcessPointerButton(ref PointerModel.ButtonState button, PointerEventData eventData)
		{
			GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if (gameObject != null && PointerShouldIgnoreTransform(gameObject.transform))
			{
				return;
			}
			if (button.wasPressedThisFrame)
			{
				eventData.delta = Vector2.zero;
				eventData.dragging = false;
				eventData.pressPosition = eventData.position;
				eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
				eventData.eligibleForClick = true;
				GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(gameObject);
				if (eventHandler != base.eventSystem.currentSelectedGameObject && (eventHandler != null || m_DeselectOnBackgroundClick))
				{
					base.eventSystem.SetSelectedGameObject(null, eventData);
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, eventData, ExecuteEvents.pointerDownHandler);
				float unscaledGameTime = InputRuntime.s_Instance.unscaledGameTime;
				if (gameObject2 == eventData.lastPress && unscaledGameTime - eventData.clickTime < 0.3f)
				{
					int clickCount = eventData.clickCount + 1;
					eventData.clickCount = clickCount;
				}
				else
				{
					eventData.clickCount = 1;
				}
				eventData.clickTime = unscaledGameTime;
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				eventData.pointerPress = gameObject2;
				eventData.rawPointerPress = gameObject;
				eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (button.wasReleasedThisFrame)
			{
				ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (eventData.pointerPress == eventHandler2 && eventData.eligibleForClick)
				{
					ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
				}
				else if (eventData.dragging && eventData.pointerDrag != null)
				{
					ExecuteEvents.ExecuteHierarchy(gameObject, eventData, ExecuteEvents.dropHandler);
				}
				eventData.eligibleForClick = false;
				eventData.pointerPress = null;
				eventData.rawPointerPress = null;
				if (eventData.dragging && eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
				}
				eventData.dragging = false;
				eventData.pointerDrag = null;
			}
			button.CopyPressStateFrom(eventData);
		}

		private void ProcessPointerButtonDrag(ref PointerModel.ButtonState button, ExtendedPointerEventData eventData)
		{
			if (!eventData.IsPointerMoving() || (eventData.pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked) || eventData.pointerDrag == null)
			{
				return;
			}
			if (!eventData.dragging && (!eventData.useDragThreshold || (double)(eventData.pressPosition - eventData.position).sqrMagnitude >= (double)base.eventSystem.pixelDragThreshold * (double)base.eventSystem.pixelDragThreshold * (double)((eventData.pointerType == UIPointerType.Tracked) ? m_TrackedDeviceDragThresholdMultiplier : 1f)))
			{
				ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
				eventData.dragging = true;
			}
			if (eventData.dragging)
			{
				if (eventData.pointerPress != eventData.pointerDrag)
				{
					ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);
					eventData.eligibleForClick = false;
					eventData.pointerPress = null;
					eventData.rawPointerPress = null;
				}
				ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
				button.CopyPressStateFrom(eventData);
			}
		}

		private static void ProcessPointerScroll(ref PointerModel pointer, PointerEventData eventData)
		{
			Vector2 scrollDelta = pointer.scrollDelta;
			if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0f))
			{
				eventData.scrollDelta = scrollDelta;
				ExecuteEvents.ExecuteHierarchy(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter), eventData, ExecuteEvents.scrollHandler);
			}
		}

		internal void ProcessNavigation(ref NavigationModel navigationState)
		{
			bool flag = false;
			if (base.eventSystem.currentSelectedGameObject != null)
			{
				BaseEventData baseEventData = GetBaseEventData();
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
				flag = baseEventData.used;
			}
			if (!base.eventSystem.sendNavigationEvents)
			{
				return;
			}
			Vector2 vector = navigationState.move;
			if (!flag && (!Mathf.Approximately(vector.x, 0f) || !Mathf.Approximately(vector.y, 0f)))
			{
				float unscaledGameTime = InputRuntime.s_Instance.unscaledGameTime;
				Vector2 moveVector = navigationState.move;
				MoveDirection moveDirection = MoveDirection.None;
				if (moveVector.sqrMagnitude > 0f)
				{
					moveDirection = ((!(Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y))) ? ((moveVector.y > 0f) ? MoveDirection.Up : MoveDirection.Down) : ((moveVector.x > 0f) ? MoveDirection.Right : MoveDirection.Left));
				}
				if (moveDirection != m_NavigationState.lastMoveDirection)
				{
					m_NavigationState.consecutiveMoveCount = 0;
				}
				if (moveDirection != MoveDirection.None)
				{
					bool flag2 = true;
					if (m_NavigationState.consecutiveMoveCount != 0)
					{
						flag2 = ((m_NavigationState.consecutiveMoveCount <= 1) ? (unscaledGameTime > m_NavigationState.lastMoveTime + moveRepeatDelay) : (unscaledGameTime > m_NavigationState.lastMoveTime + moveRepeatRate));
					}
					if (flag2)
					{
						AxisEventData axisEventData = m_NavigationState.eventData;
						if (axisEventData == null)
						{
							axisEventData = new AxisEventData(base.eventSystem);
							m_NavigationState.eventData = axisEventData;
						}
						axisEventData.Reset();
						axisEventData.moveVector = moveVector;
						axisEventData.moveDir = moveDirection;
						ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
						flag = axisEventData.used;
						m_NavigationState.consecutiveMoveCount++;
						m_NavigationState.lastMoveTime = unscaledGameTime;
						m_NavigationState.lastMoveDirection = moveDirection;
					}
				}
				else
				{
					m_NavigationState.consecutiveMoveCount = 0;
				}
			}
			else
			{
				m_NavigationState.consecutiveMoveCount = 0;
			}
			if (!flag && base.eventSystem.currentSelectedGameObject != null)
			{
				BaseEventData baseEventData2 = GetBaseEventData();
				if (m_NavigationState.cancelButton.wasPressedThisFrame)
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData2, ExecuteEvents.cancelHandler);
				}
				if (!baseEventData2.used && m_NavigationState.submitButton.wasPressedThisFrame)
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData2, ExecuteEvents.submitHandler);
				}
			}
			m_NavigationState.OnFrameFinished();
		}

		private void SwapAction(ref InputActionReference property, InputActionReference newValue, bool actionsHooked, Action<InputAction.CallbackContext> actionCallback)
		{
			if (!(property == newValue) && (!(property != null) || !(newValue != null) || property.action != newValue.action))
			{
				if (property != null && actionsHooked)
				{
					property.action.performed -= actionCallback;
					property.action.canceled -= actionCallback;
				}
				property = newValue;
				if (newValue != null && actionsHooked)
				{
					property.action.performed += actionCallback;
					property.action.canceled += actionCallback;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			m_NavigationState.Reset();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UnhookActions();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (m_OnControlsChangedDelegate == null)
			{
				m_OnControlsChangedDelegate = OnControlsChanged;
			}
			InputActionState.s_OnActionControlsChanged.AppendWithCapacity(m_OnControlsChangedDelegate);
			HookActions();
			EnableAllActions();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			int num = InputActionState.s_OnActionControlsChanged.IndexOfReference(m_OnControlsChangedDelegate);
			if (num != -1)
			{
				InputActionState.s_OnActionControlsChanged.RemoveAtWithCapacity(num);
			}
			DisableAllActions();
			UnhookActions();
		}

		private bool IsAnyActionEnabled()
		{
			InputActionReference pointAction = m_PointAction;
			bool? obj;
			if ((object)pointAction == null)
			{
				obj = null;
			}
			else
			{
				InputAction action = pointAction.action;
				obj = ((action != null) ? new bool?(action.enabled) : null);
			}
			if (obj ?? true)
			{
				InputActionReference leftClickAction = m_LeftClickAction;
				bool? obj2;
				if ((object)leftClickAction == null)
				{
					obj2 = null;
				}
				else
				{
					InputAction action2 = leftClickAction.action;
					obj2 = ((action2 != null) ? new bool?(action2.enabled) : null);
				}
				if (obj2 ?? true)
				{
					InputActionReference rightClickAction = m_RightClickAction;
					bool? obj3;
					if ((object)rightClickAction == null)
					{
						obj3 = null;
					}
					else
					{
						InputAction action3 = rightClickAction.action;
						obj3 = ((action3 != null) ? new bool?(action3.enabled) : null);
					}
					if (obj3 ?? true)
					{
						InputActionReference middleClickAction = m_MiddleClickAction;
						bool? obj4;
						if ((object)middleClickAction == null)
						{
							obj4 = null;
						}
						else
						{
							InputAction action4 = middleClickAction.action;
							obj4 = ((action4 != null) ? new bool?(action4.enabled) : null);
						}
						if (obj4 ?? true)
						{
							InputActionReference moveAction = m_MoveAction;
							bool? obj5;
							if ((object)moveAction == null)
							{
								obj5 = null;
							}
							else
							{
								InputAction action5 = moveAction.action;
								obj5 = ((action5 != null) ? new bool?(action5.enabled) : null);
							}
							if (obj5 ?? true)
							{
								InputActionReference submitAction = m_SubmitAction;
								bool? obj6;
								if ((object)submitAction == null)
								{
									obj6 = null;
								}
								else
								{
									InputAction action6 = submitAction.action;
									obj6 = ((action6 != null) ? new bool?(action6.enabled) : null);
								}
								if (obj6 ?? true)
								{
									InputActionReference cancelAction = m_CancelAction;
									bool? obj7;
									if ((object)cancelAction == null)
									{
										obj7 = null;
									}
									else
									{
										InputAction action7 = cancelAction.action;
										obj7 = ((action7 != null) ? new bool?(action7.enabled) : null);
									}
									if (obj7 ?? true)
									{
										InputActionReference scrollWheelAction = m_ScrollWheelAction;
										bool? obj8;
										if ((object)scrollWheelAction == null)
										{
											obj8 = null;
										}
										else
										{
											InputAction action8 = scrollWheelAction.action;
											obj8 = ((action8 != null) ? new bool?(action8.enabled) : null);
										}
										if (obj8 ?? true)
										{
											InputActionReference trackedDeviceOrientationAction = m_TrackedDeviceOrientationAction;
											bool? obj9;
											if ((object)trackedDeviceOrientationAction == null)
											{
												obj9 = null;
											}
											else
											{
												InputAction action9 = trackedDeviceOrientationAction.action;
												obj9 = ((action9 != null) ? new bool?(action9.enabled) : null);
											}
											if (obj9 ?? true)
											{
												InputActionReference trackedDevicePositionAction = m_TrackedDevicePositionAction;
												bool? obj10;
												if ((object)trackedDevicePositionAction == null)
												{
													obj10 = null;
												}
												else
												{
													InputAction action10 = trackedDevicePositionAction.action;
													obj10 = ((action10 != null) ? new bool?(action10.enabled) : null);
												}
												return obj10 ?? true;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		private void EnableAllActions()
		{
			if (IsAnyActionEnabled())
			{
				return;
			}
			InputActionReference pointAction = m_PointAction;
			if ((object)pointAction != null)
			{
				InputAction action = pointAction.action;
				if (action != null)
				{
					action.Enable();
				}
			}
			InputActionReference leftClickAction = m_LeftClickAction;
			if ((object)leftClickAction != null)
			{
				InputAction action2 = leftClickAction.action;
				if (action2 != null)
				{
					action2.Enable();
				}
			}
			InputActionReference rightClickAction = m_RightClickAction;
			if ((object)rightClickAction != null)
			{
				InputAction action3 = rightClickAction.action;
				if (action3 != null)
				{
					action3.Enable();
				}
			}
			InputActionReference middleClickAction = m_MiddleClickAction;
			if ((object)middleClickAction != null)
			{
				InputAction action4 = middleClickAction.action;
				if (action4 != null)
				{
					action4.Enable();
				}
			}
			InputActionReference moveAction = m_MoveAction;
			if ((object)moveAction != null)
			{
				InputAction action5 = moveAction.action;
				if (action5 != null)
				{
					action5.Enable();
				}
			}
			InputActionReference submitAction = m_SubmitAction;
			if ((object)submitAction != null)
			{
				InputAction action6 = submitAction.action;
				if (action6 != null)
				{
					action6.Enable();
				}
			}
			InputActionReference cancelAction = m_CancelAction;
			if ((object)cancelAction != null)
			{
				InputAction action7 = cancelAction.action;
				if (action7 != null)
				{
					action7.Enable();
				}
			}
			InputActionReference scrollWheelAction = m_ScrollWheelAction;
			if ((object)scrollWheelAction != null)
			{
				InputAction action8 = scrollWheelAction.action;
				if (action8 != null)
				{
					action8.Enable();
				}
			}
			InputActionReference trackedDeviceOrientationAction = m_TrackedDeviceOrientationAction;
			if ((object)trackedDeviceOrientationAction != null)
			{
				InputAction action9 = trackedDeviceOrientationAction.action;
				if (action9 != null)
				{
					action9.Enable();
				}
			}
			InputActionReference trackedDevicePositionAction = m_TrackedDevicePositionAction;
			if ((object)trackedDevicePositionAction != null)
			{
				InputAction action10 = trackedDevicePositionAction.action;
				if (action10 != null)
				{
					action10.Enable();
				}
			}
			m_OwnsEnabledState = true;
		}

		private void DisableAllActions()
		{
			if (!m_OwnsEnabledState)
			{
				return;
			}
			m_OwnsEnabledState = false;
			InputActionReference pointAction = m_PointAction;
			if ((object)pointAction != null)
			{
				InputAction action = pointAction.action;
				if (action != null)
				{
					action.Disable();
				}
			}
			InputActionReference leftClickAction = m_LeftClickAction;
			if ((object)leftClickAction != null)
			{
				InputAction action2 = leftClickAction.action;
				if (action2 != null)
				{
					action2.Disable();
				}
			}
			InputActionReference rightClickAction = m_RightClickAction;
			if ((object)rightClickAction != null)
			{
				InputAction action3 = rightClickAction.action;
				if (action3 != null)
				{
					action3.Disable();
				}
			}
			InputActionReference middleClickAction = m_MiddleClickAction;
			if ((object)middleClickAction != null)
			{
				InputAction action4 = middleClickAction.action;
				if (action4 != null)
				{
					action4.Disable();
				}
			}
			InputActionReference moveAction = m_MoveAction;
			if ((object)moveAction != null)
			{
				InputAction action5 = moveAction.action;
				if (action5 != null)
				{
					action5.Disable();
				}
			}
			InputActionReference submitAction = m_SubmitAction;
			if ((object)submitAction != null)
			{
				InputAction action6 = submitAction.action;
				if (action6 != null)
				{
					action6.Disable();
				}
			}
			InputActionReference cancelAction = m_CancelAction;
			if ((object)cancelAction != null)
			{
				InputAction action7 = cancelAction.action;
				if (action7 != null)
				{
					action7.Disable();
				}
			}
			InputActionReference scrollWheelAction = m_ScrollWheelAction;
			if ((object)scrollWheelAction != null)
			{
				InputAction action8 = scrollWheelAction.action;
				if (action8 != null)
				{
					action8.Disable();
				}
			}
			InputActionReference trackedDeviceOrientationAction = m_TrackedDeviceOrientationAction;
			if ((object)trackedDeviceOrientationAction != null)
			{
				InputAction action9 = trackedDeviceOrientationAction.action;
				if (action9 != null)
				{
					action9.Disable();
				}
			}
			InputActionReference trackedDevicePositionAction = m_TrackedDevicePositionAction;
			if ((object)trackedDevicePositionAction != null)
			{
				InputAction action10 = trackedDevicePositionAction.action;
				if (action10 != null)
				{
					action10.Disable();
				}
			}
		}

		private int GetPointerStateIndexFor(int pointerId)
		{
			if (pointerId == m_CurrentPointerId)
			{
				return m_CurrentPointerIndex;
			}
			for (int i = 0; i < m_PointerIds.length; i++)
			{
				if (m_PointerIds[i] == pointerId)
				{
					return i;
				}
			}
			return -1;
		}

		private ref PointerModel GetPointerStateForIndex(int index)
		{
			if (index == 0)
			{
				return ref m_PointerStates.firstValue;
			}
			return ref m_PointerStates.additionalValues[index - 1];
		}

		private ref PointerModel GetPointerStateFor(ref InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = GetPointerStateIndexFor(context.control);
			return ref GetPointerStateForIndex(pointerStateIndexFor);
		}

		private int GetPointerStateIndexFor(InputControl control)
		{
			InputDevice device = control.device;
			int num = device.deviceId;
			int num2 = 0;
			InputControl parent = control.parent;
			TouchControl touchControl;
			Touchscreen touchscreen;
			if ((touchControl = parent as TouchControl) != null)
			{
				num2 = touchControl.touchId.ReadValue();
			}
			else if ((touchscreen = parent as Touchscreen) != null)
			{
				num2 = touchscreen.primaryTouch.touchId.ReadValue();
			}
			if (num2 != 0)
			{
				num = ExtendedPointerEventData.MakePointerIdForTouch(num, num2);
			}
			if (m_CurrentPointerId == num)
			{
				return m_CurrentPointerIndex;
			}
			for (int i = 0; i < m_PointerIds.length; i++)
			{
				if (m_PointerIds[i] == num)
				{
					m_CurrentPointerId = num;
					m_CurrentPointerIndex = i;
					m_CurrentPointerType = m_PointerStates[i].pointerType;
					return i;
				}
			}
			UIPointerType uIPointerType = UIPointerType.None;
			if (num2 != 0)
			{
				uIPointerType = UIPointerType.Touch;
			}
			else if (HaveControlForDevice(device, point))
			{
				uIPointerType = UIPointerType.MouseOrPen;
			}
			else if (HaveControlForDevice(device, trackedDevicePosition))
			{
				uIPointerType = UIPointerType.Tracked;
			}
			if (m_PointerBehavior == UIPointerBehavior.SingleMouseOrPenButMultiTouchAndTrack)
			{
				switch (uIPointerType)
				{
				case UIPointerType.MouseOrPen:
				{
					for (int k = 0; k < m_PointerStates.length; k++)
					{
						if (m_PointerStates[k].pointerType != UIPointerType.MouseOrPen)
						{
							SendPointerExitEventsAndRemovePointer(k);
							k--;
						}
					}
					break;
				}
				default:
				{
					for (int j = 0; j < m_PointerStates.length; j++)
					{
						if (m_PointerStates[j].pointerType == UIPointerType.MouseOrPen)
						{
							SendPointerExitEventsAndRemovePointer(j);
							j--;
						}
					}
					break;
				}
				case UIPointerType.None:
					break;
				}
			}
			if ((m_PointerBehavior == UIPointerBehavior.SingleUnifiedPointer && uIPointerType != 0) || (m_PointerBehavior == UIPointerBehavior.SingleMouseOrPenButMultiTouchAndTrack && uIPointerType == UIPointerType.MouseOrPen))
			{
				if (m_CurrentPointerIndex == -1)
				{
					m_CurrentPointerIndex = AllocatePointer(num, num2, uIPointerType, device);
				}
				else
				{
					ExtendedPointerEventData eventData = GetPointerStateForIndex(m_CurrentPointerIndex).eventData;
					eventData.device = device;
					eventData.pointerType = uIPointerType;
					eventData.pointerId = num;
					eventData.touchId = num2;
					eventData.trackedDeviceOrientation = default(Quaternion);
					eventData.trackedDevicePosition = default(Vector3);
				}
				m_CurrentPointerId = num;
				m_CurrentPointerType = uIPointerType;
				return m_CurrentPointerIndex;
			}
			int num3 = -1;
			if (uIPointerType != 0)
			{
				num3 = AllocatePointer(num, num2, uIPointerType, device);
			}
			else
			{
				if (m_CurrentPointerId != -1)
				{
					return m_CurrentPointerIndex;
				}
				InputActionReference inputActionReference = point;
				ReadOnlyArray<InputControl>? obj;
				if ((object)inputActionReference == null)
				{
					obj = null;
				}
				else
				{
					InputAction action = inputActionReference.action;
					obj = ((action != null) ? new ReadOnlyArray<InputControl>?(action.controls) : null);
				}
				ReadOnlyArray<InputControl>? readOnlyArray = obj;
				InputDevice inputDevice = ((readOnlyArray.HasValue && readOnlyArray.Value.Count > 0) ? readOnlyArray.Value[0].device : null);
				if (inputDevice != null && !(inputDevice is Touchscreen))
				{
					num3 = AllocatePointer(inputDevice.deviceId, 0, UIPointerType.MouseOrPen, inputDevice);
				}
				else
				{
					InputActionReference inputActionReference2 = trackedDevicePosition;
					ReadOnlyArray<InputControl>? obj2;
					if ((object)inputActionReference2 == null)
					{
						obj2 = null;
					}
					else
					{
						InputAction action2 = inputActionReference2.action;
						obj2 = ((action2 != null) ? new ReadOnlyArray<InputControl>?(action2.controls) : null);
					}
					ReadOnlyArray<InputControl>? readOnlyArray2 = obj2;
					InputDevice inputDevice2 = ((readOnlyArray2.HasValue && readOnlyArray2.Value.Count > 0) ? readOnlyArray2.Value[0].device : null);
					num3 = ((inputDevice2 == null) ? AllocatePointer(num, 0, UIPointerType.None, device) : AllocatePointer(inputDevice2.deviceId, 0, UIPointerType.Tracked, inputDevice2));
				}
			}
			m_CurrentPointerId = num;
			m_CurrentPointerIndex = num3;
			m_CurrentPointerType = uIPointerType;
			return num3;
		}

		private int AllocatePointer(int pointerId, int touchId, UIPointerType pointerType, InputDevice device)
		{
			ExtendedPointerEventData extendedPointerEventData = null;
			if (m_PointerStates.Capacity > m_PointerStates.length)
			{
				extendedPointerEventData = ((m_PointerStates.length != 0) ? m_PointerStates.additionalValues[m_PointerStates.length - 1].eventData : m_PointerStates.firstValue.eventData);
			}
			if (extendedPointerEventData == null)
			{
				extendedPointerEventData = new ExtendedPointerEventData(base.eventSystem);
			}
			m_PointerIds.AppendWithCapacity(pointerId);
			return m_PointerStates.AppendWithCapacity(new PointerModel(pointerId, touchId, pointerType, device, extendedPointerEventData));
		}

		private void SendPointerExitEventsAndRemovePointer(int index)
		{
			ExtendedPointerEventData eventData = m_PointerStates[index].eventData;
			if (eventData.pointerEnter != null)
			{
				ProcessPointerMovement(eventData, null);
			}
			RemovePointerAtIndex(index);
		}

		private void RemovePointerAtIndex(int index)
		{
			ExtendedPointerEventData eventData = m_PointerStates[index].eventData;
			m_PointerIds.RemoveAtByMovingTailWithCapacity(index);
			m_PointerStates.RemoveAtByMovingTailWithCapacity(index);
			if (index == m_CurrentPointerIndex)
			{
				m_CurrentPointerId = -1;
				m_CurrentPointerIndex = -1;
			}
			eventData.hovered.Clear();
			eventData.device = null;
			eventData.pointerCurrentRaycast = default(RaycastResult);
			eventData.pointerPressRaycast = default(RaycastResult);
			eventData.pointerPress = null;
			eventData.pointerPress = null;
			eventData.pointerDrag = null;
			eventData.pointerEnter = null;
			eventData.rawPointerPress = null;
			if (m_PointerStates.length == 0)
			{
				m_PointerStates.firstValue.eventData = eventData;
			}
			else
			{
				m_PointerStates.additionalValues[m_PointerStates.length - 1].eventData = eventData;
			}
		}

		private void PurgeStalePointers()
		{
			for (int i = 0; i < m_PointerStates.length; i++)
			{
				InputDevice device = GetPointerStateForIndex(i).eventData.device;
				if (!HaveControlForDevice(device, point) && !HaveControlForDevice(device, trackedDevicePosition) && !HaveControlForDevice(device, trackedDeviceOrientation))
				{
					SendPointerExitEventsAndRemovePointer(i);
					i--;
				}
			}
		}

		private static bool HaveControlForDevice(InputDevice device, InputActionReference actionReference)
		{
			InputAction inputAction = (((object)actionReference != null) ? actionReference.action : null);
			if (inputAction == null)
			{
				return false;
			}
			ReadOnlyArray<InputControl> controls = inputAction.controls;
			for (int i = 0; i < controls.Count; i++)
			{
				if (controls[i].device == device)
				{
					return true;
				}
			}
			return false;
		}

		private void OnPoint(InputAction.CallbackContext context)
		{
			GetPointerStateFor(ref context).screenPosition = context.ReadValue<Vector2>();
		}

		private void OnLeftClick(InputAction.CallbackContext context)
		{
			ref PointerModel pointerStateFor = ref GetPointerStateFor(ref context);
			pointerStateFor.leftButton.isPressed = context.ReadValueAsButton();
			pointerStateFor.changedThisFrame = true;
		}

		private void OnRightClick(InputAction.CallbackContext context)
		{
			ref PointerModel pointerStateFor = ref GetPointerStateFor(ref context);
			pointerStateFor.rightButton.isPressed = context.ReadValueAsButton();
			pointerStateFor.changedThisFrame = true;
		}

		private void OnMiddleClick(InputAction.CallbackContext context)
		{
			ref PointerModel pointerStateFor = ref GetPointerStateFor(ref context);
			pointerStateFor.middleButton.isPressed = context.ReadValueAsButton();
			pointerStateFor.changedThisFrame = true;
		}

		private void OnScroll(InputAction.CallbackContext context)
		{
			GetPointerStateFor(ref context).scrollDelta = context.ReadValue<Vector2>() * 0.05f;
		}

		private void OnMove(InputAction.CallbackContext context)
		{
			m_NavigationState.move = context.ReadValue<Vector2>();
		}

		private void OnSubmit(InputAction.CallbackContext context)
		{
			m_NavigationState.submitButton.isPressed = context.ReadValueAsButton();
		}

		private void OnCancel(InputAction.CallbackContext context)
		{
			m_NavigationState.cancelButton.isPressed = context.ReadValueAsButton();
		}

		private void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
		{
			GetPointerStateFor(ref context).worldOrientation = context.ReadValue<Quaternion>();
		}

		private void OnTrackedDevicePosition(InputAction.CallbackContext context)
		{
			GetPointerStateFor(ref context).worldPosition = context.ReadValue<Vector3>();
		}

		private void OnControlsChanged(object obj)
		{
			PurgeStalePointers();
		}

		public override void Process()
		{
			if (!base.eventSystem.isFocused)
			{
				m_NavigationState.OnFrameFinished();
				for (int i = 0; i < m_PointerStates.length; i++)
				{
					m_PointerStates[i].OnFrameFinished();
				}
				return;
			}
			ProcessNavigation(ref m_NavigationState);
			for (int j = 0; j < m_PointerStates.length; j++)
			{
				ref PointerModel pointerStateForIndex = ref GetPointerStateForIndex(j);
				ProcessPointer(ref pointerStateForIndex);
				if (pointerStateForIndex.pointerType == UIPointerType.Touch && !pointerStateForIndex.leftButton.isPressed)
				{
					RemovePointerAtIndex(j);
					j--;
				}
			}
		}

		private void HookActions()
		{
			if (!m_ActionsHooked)
			{
				if (m_OnPointDelegate == null)
				{
					m_OnPointDelegate = OnPoint;
				}
				if (m_OnLeftClickDelegate == null)
				{
					m_OnLeftClickDelegate = OnLeftClick;
				}
				if (m_OnRightClickDelegate == null)
				{
					m_OnRightClickDelegate = OnRightClick;
				}
				if (m_OnMiddleClickDelegate == null)
				{
					m_OnMiddleClickDelegate = OnMiddleClick;
				}
				if (m_OnScrollWheelDelegate == null)
				{
					m_OnScrollWheelDelegate = OnScroll;
				}
				if (m_OnMoveDelegate == null)
				{
					m_OnMoveDelegate = OnMove;
				}
				if (m_OnSubmitDelegate == null)
				{
					m_OnSubmitDelegate = OnSubmit;
				}
				if (m_OnCancelDelegate == null)
				{
					m_OnCancelDelegate = OnCancel;
				}
				if (m_OnTrackedDeviceOrientationDelegate == null)
				{
					m_OnTrackedDeviceOrientationDelegate = OnTrackedDeviceOrientation;
				}
				if (m_OnTrackedDevicePositionDelegate == null)
				{
					m_OnTrackedDevicePositionDelegate = OnTrackedDevicePosition;
				}
				SetActionCallbacks(true);
			}
		}

		private void UnhookActions()
		{
			if (m_ActionsHooked)
			{
				SetActionCallbacks(false);
			}
		}

		private void SetActionCallbacks(bool install)
		{
			m_ActionsHooked = install;
			SetActionCallback(m_PointAction, m_OnPointDelegate, install);
			SetActionCallback(m_MoveAction, m_OnMoveDelegate, install);
			SetActionCallback(m_LeftClickAction, m_OnLeftClickDelegate, install);
			SetActionCallback(m_RightClickAction, m_OnRightClickDelegate, install);
			SetActionCallback(m_MiddleClickAction, m_OnMiddleClickDelegate, install);
			SetActionCallback(m_SubmitAction, m_OnSubmitDelegate, install);
			SetActionCallback(m_CancelAction, m_OnCancelDelegate, install);
			SetActionCallback(m_ScrollWheelAction, m_OnScrollWheelDelegate, install);
			SetActionCallback(m_TrackedDeviceOrientationAction, m_OnTrackedDeviceOrientationDelegate, install);
			SetActionCallback(m_TrackedDevicePositionAction, m_OnTrackedDevicePositionDelegate, install);
		}

		private static void SetActionCallback(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool install)
		{
			if ((!install && callback == null) || actionReference == null)
			{
				return;
			}
			InputAction action = actionReference.action;
			if (action != null)
			{
				if (install)
				{
					action.performed += callback;
					action.canceled += callback;
				}
				else
				{
					action.performed -= callback;
					action.canceled -= callback;
				}
			}
		}

		private InputActionReference UpdateReferenceForNewAsset(InputActionReference actionReference)
		{
			InputAction inputAction = (((object)actionReference != null) ? actionReference.action : null);
			if (inputAction == null)
			{
				return null;
			}
			InputActionMap actionMap = inputAction.actionMap;
			InputActionMap inputActionMap = m_ActionsAsset.FindActionMap(actionMap.name);
			if (inputActionMap == null)
			{
				return null;
			}
			InputAction inputAction2 = inputActionMap.FindAction(inputAction.name);
			if (inputAction2 == null)
			{
				return null;
			}
			return InputActionReference.Create(inputAction2);
		}
	}
}
