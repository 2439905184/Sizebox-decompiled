using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class CommandView : MonoBehaviour
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Predicate<IBehavior> _003C_003E9__24_0;

			internal bool _003CGetListPossibleBehaviors_003Eb__24_0(IBehavior beh)
			{
				return !beh.IsHidden();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass28_0
		{
			public CommandView _003C_003E4__this;

			public EntityBase targetEntity;
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass28_1
		{
			public IBehavior behavior;

			public _003C_003Ec__DisplayClass28_0 CS_0024_003C_003E8__locals1;

			internal void _003CLoadMenuEntries_003Eb__0()
			{
				CS_0024_003C_003E8__locals1._003C_003E4__this.OnBehaviorClicked(behavior, CS_0024_003C_003E8__locals1.targetEntity);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass28_2
		{
			public string subDir;

			public _003C_003Ec__DisplayClass28_0 CS_0024_003C_003E8__locals2;

			internal void _003CLoadMenuEntries_003Eb__1()
			{
				CS_0024_003C_003E8__locals2._003C_003E4__this.OpenSubDir(subDir);
			}
		}

		[Header("Required References")]
		[SerializeField]
		private GameObject viewPanel;

		[SerializeField]
		private GameObject buttonPanel;

		[SerializeField]
		private GridLayoutGroup buttonLayout;

		[SerializeField]
		private CommandButton actionButtonPrefab;

		[Header("Options")]
		[SerializeField]
		private Vector2 screenSafeZone = new Vector2(0.05f, 0.08f);

		private Camera _mainCamera;

		private InterfaceControl _control;

		private EntityBase _targetEntity;

		private Vector3 _cursorPosition;

		private float _maxTimeBetweenClick = 0.2f;

		private float _startClickRight;

		private float _startClickLeft;

		private float _startClickCenter;

		private EditPlacement _placement;

		private CommandAction _cancelCommand;

		private CommandAction _playAsCommand;

		private CommandAction _stopPlayingCommand;

		private MenuEntryList _entries;

		private string _rootDir = "";

		private uint _buttonCount;

		private bool showingPanel
		{
			get
			{
				return viewPanel.activeSelf;
			}
		}

		private void Start()
		{
			_mainCamera = Camera.main;
			_control = GuiManager.Instance.InterfaceControl;
			_placement = GuiManager.Instance.EditPlacement;
			_cancelCommand = new CommandAction
			{
				Text = "Cancel",
				Action = HideBehaviors
			};
			_playAsCommand = new CommandAction
			{
				Text = "Play as",
				Action = PlayTargetEntity
			};
			_stopPlayingCommand = new CommandAction
			{
				Text = "Stop playing as",
				Action = StopPlayingEntity
			};
			viewPanel.SetActive(false);
		}

		private void Update()
		{
			if (!_control.commandEnabled)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(ButtonInput.EscapeAlt))
			{
				HideBehaviors();
				return;
			}
			if (Mouse.current.leftButton.wasPressedThisFrame && _placement.state == EditPlacement.State.Idle)
			{
				_startClickLeft = Time.time;
			}
			if (Mouse.current.rightButton.wasPressedThisFrame && _placement.state == EditPlacement.State.Idle)
			{
				_startClickRight = Time.time;
			}
			if (Mouse.current.middleButton.wasPressedThisFrame && _placement.state == EditPlacement.State.Idle)
			{
				_startClickCenter = Time.time;
			}
			if (Mouse.current.middleButton.wasReleasedThisFrame && Time.time - _startClickCenter < _maxTimeBetweenClick && _placement.state == EditPlacement.State.Idle)
			{
				SelectObject(null);
			}
			if (Mouse.current.leftButton.wasReleasedThisFrame && Time.time - _startClickLeft < _maxTimeBetweenClick && !EventSystem.current.IsPointerOverGameObject())
			{
				if (showingPanel)
				{
					HideBehaviors();
					return;
				}
				if (_control.selectedEntity != null && !_control.selectedEntity.isPositioned)
				{
					return;
				}
				HideBehaviors();
				RaycastHit hitInfo;
				if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, float.PositiveInfinity, Layers.actionSelectionMask, QueryTriggerInteraction.Collide))
				{
					if (hitInfo.collider.gameObject.layer == Layers.uiLayer)
					{
						return;
					}
					_cursorPosition = hitInfo.point;
					EntityBase entityBase = FindEntity(hitInfo.transform);
					if (entityBase != null)
					{
						SelectObject(entityBase);
					}
				}
			}
			if (!Mouse.current.rightButton.wasReleasedThisFrame || !(Time.time - _startClickRight < _maxTimeBetweenClick) || EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			if (showingPanel)
			{
				HideBehaviors();
			}
			else
			{
				if (_control.selectedEntity != null && !_control.selectedEntity.isPositioned)
				{
					return;
				}
				HideBehaviors();
				RaycastHit hitInfo2;
				if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo2, float.PositiveInfinity, Layers.actionSelectionMask, QueryTriggerInteraction.Collide) && hitInfo2.collider.gameObject.layer != Layers.uiLayer)
				{
					_cursorPosition = hitInfo2.point;
					EntityBase entityBase2 = FindEntity(hitInfo2.transform);
					if (StateManager.Keyboard.Shift && _targetEntity != null)
					{
						SelectObject(entityBase2);
					}
					OpenCommandMenu(entityBase2);
				}
			}
		}

		private List<IBehavior> GetListPossibleBehaviors(EntityBase targetEntity)
		{
			if (_control.selectedEntity == null || !(_control.selectedEntity as Humanoid) || BehaviorLists.Instance == null)
			{
				return new List<IBehavior>();
			}
			return BehaviorLists.Instance.FindBehaviors(_control.selectedEntity, targetEntity).FindAll(_003C_003Ec._003C_003E9__24_0 ?? (_003C_003Ec._003C_003E9__24_0 = _003C_003Ec._003C_003E9._003CGetListPossibleBehaviors_003Eb__24_0));
		}

		private EntityBase FindEntity(Transform hitElement)
		{
			return hitElement.GetComponentInParent<EntityBase>();
		}

		public void OpenCommandMenu(EntityBase targetEntity)
		{
			if (!targetEntity || targetEntity.Initialized)
			{
				_targetEntity = targetEntity;
				List<IBehavior> listPossibleBehaviors = GetListPossibleBehaviors(targetEntity);
				_entries = LoadMenuEntries(listPossibleBehaviors, targetEntity);
				if (_entries.Menu.Count > 0)
				{
					ShowMenuEntries(_entries);
					KeepCommandMenuOnScreen();
				}
			}
		}

		public void KeepCommandMenuOnScreen()
		{
			viewPanel.transform.position = Input.mousePosition;
			viewPanel.SetActive(true);
			RectTransform rectTransform = (RectTransform)viewPanel.transform;
			Vector2 canvasReferenceResolution = GuiManager.Instance.GetCanvasReferenceResolution();
			Vector2 canvasScaleVector = GuiManager.Instance.GetCanvasScaleVector();
			float x = rectTransform.sizeDelta.x;
			float num = buttonLayout.cellSize.y * (float)_buttonCount + buttonLayout.spacing.y * (float)(_buttonCount - 1);
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			float num2 = anchoredPosition.y;
			float num3 = anchoredPosition.x;
			float num4 = canvasReferenceResolution.x * screenSafeZone.x * 0.5f * (canvasScaleVector.y / canvasScaleVector.x);
			float num5 = canvasReferenceResolution.y * screenSafeZone.x * 0.5f * (canvasScaleVector.x / canvasScaleVector.y);
			if (num3 - num4 < 0f)
			{
				num3 = num4;
			}
			else if (num3 + x + num4 >= canvasReferenceResolution.x)
			{
				num3 = canvasReferenceResolution.x - x - num4;
			}
			if (num < canvasReferenceResolution.y * (1f - screenSafeZone.y))
			{
				if (num2 - num < 0f)
				{
					num2 = num + num5;
				}
				else if (num2 + num5 > canvasReferenceResolution.y)
				{
					num2 = canvasReferenceResolution.y - num5;
				}
				Vector2 sizeDelta = rectTransform.sizeDelta;
				sizeDelta.y = num;
				rectTransform.sizeDelta = sizeDelta;
			}
			else
			{
				Vector2 sizeDelta2 = rectTransform.sizeDelta;
				sizeDelta2.y = canvasReferenceResolution.y * (1f - screenSafeZone.y);
				rectTransform.sizeDelta = sizeDelta2;
				num2 = canvasReferenceResolution.y - num5;
			}
			rectTransform.anchoredPosition = new Vector2(num3, num2);
		}

		private MenuEntryList LoadMenuEntries(List<IBehavior> listBehaviors, EntityBase targetEntity)
		{
			_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = new _003C_003Ec__DisplayClass28_0();
			_003C_003Ec__DisplayClass28_._003C_003E4__this = this;
			_003C_003Ec__DisplayClass28_.targetEntity = targetEntity;
			MenuEntryList menuEntryList = default(MenuEntryList);
			menuEntryList.Menu = new Dictionary<string, List<CommandAction>>();
			MenuEntryList result = menuEntryList;
			if (listBehaviors.Count == 0)
			{
				return result;
			}
			result.Menu[_rootDir] = new List<CommandAction>();
			int count = listBehaviors.Count;
			for (int i = 0; i < count; i++)
			{
				CommandAction item = default(CommandAction);
				string[] array = listBehaviors[i].GetText().Split('/');
				int num = array.Length;
				for (int j = 0; j < num; j++)
				{
					string key = ((j == 0) ? _rootDir : string.Join("/", array, 0, j));
					item.Text = array[j];
					if (j == num - 1)
					{
						_003C_003Ec__DisplayClass28_1 _003C_003Ec__DisplayClass28_2 = new _003C_003Ec__DisplayClass28_1();
						_003C_003Ec__DisplayClass28_2.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass28_;
						_003C_003Ec__DisplayClass28_2.behavior = listBehaviors[i];
						item.Action = _003C_003Ec__DisplayClass28_2._003CLoadMenuEntries_003Eb__0;
					}
					else
					{
						_003C_003Ec__DisplayClass28_2 _003C_003Ec__DisplayClass28_3 = new _003C_003Ec__DisplayClass28_2();
						_003C_003Ec__DisplayClass28_3.CS_0024_003C_003E8__locals2 = _003C_003Ec__DisplayClass28_;
						_003C_003Ec__DisplayClass28_3.subDir = string.Join("/", array, 0, j + 1);
						if (result.Menu.ContainsKey(_003C_003Ec__DisplayClass28_3.subDir))
						{
							continue;
						}
						item.Text += "...";
						item.Action = _003C_003Ec__DisplayClass28_3._003CLoadMenuEntries_003Eb__1;
					}
					if (!result.Menu.ContainsKey(key))
					{
						result.Menu[key] = new List<CommandAction>();
					}
					result.Menu[key].Add(item);
				}
			}
			if ((bool)_003C_003Ec__DisplayClass28_.targetEntity)
			{
				IPlayable component = _003C_003Ec__DisplayClass28_.targetEntity.GetComponent<IPlayable>();
				if (component != null)
				{
					if (!component.IsPlayerControlled)
					{
						result.Menu[_rootDir].Add(_playAsCommand);
					}
					else if (component.IsPlayerControlled && GameController.LocalClient.Player.Entity == _003C_003Ec__DisplayClass28_.targetEntity)
					{
						result.Menu[_rootDir].Add(_stopPlayingCommand);
					}
				}
			}
			result.Menu[_rootDir].Add(_cancelCommand);
			return result;
		}

		private void OpenSubDir(string subDir)
		{
			HideBehaviors();
			ShowMenuEntries(_entries, subDir);
			KeepCommandMenuOnScreen();
		}

		private void ShowMenuEntries(MenuEntryList entryList, string dir = "")
		{
			foreach (CommandAction item in entryList.Menu[dir])
			{
				AddButton(item);
			}
		}

		private void SelectObject(EntityBase entity)
		{
			_control.SetSelectedObject(entity);
			HideBehaviors();
		}

		private void PlayTargetEntity()
		{
			GameController.LocalClient.Player.PlayAs(_targetEntity.GetComponent<IPlayable>());
			EntityBase entity = GameController.LocalClient.Player.Entity;
			HideBehaviors();
			if (entity.isGiantess)
			{
				ObjectManager.UpdateGiantessSpeed(entity as Giantess);
			}
			else
			{
				ObjectManager.UpdateMicroSpeed(entity as Micro);
			}
		}

		private void StopPlayingEntity()
		{
			EntityBase entity = GameController.LocalClient.Player.Entity;
			GameController.LocalClient.Player.StopPlayingEntity();
			SelectObject(null);
			if (entity.isGiantess)
			{
				ObjectManager.UpdateGiantessSpeed(entity as Giantess);
			}
			else
			{
				ObjectManager.UpdateMicroSpeed(entity as Micro);
			}
		}

		public void HideBehaviors()
		{
			int num = (buttonPanel ? buttonPanel.transform.childCount : 0);
			for (int i = 0; i < num; i++)
			{
				Button component = buttonPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
				component.onClick.RemoveAllListeners();
				UnityEngine.Object.Destroy(component.gameObject);
			}
			_buttonCount = 0u;
			viewPanel.SetActive(false);
		}

		private void OnBehaviorClicked(IBehavior behavior, EntityBase targetEntity)
		{
			HideBehaviors();
			Humanoid humanoid = _control.selectedEntity as Humanoid;
			if ((bool)humanoid && humanoid.ai != null && humanoid.ActionManager != null)
			{
				if (!behavior.IsSecondary())
				{
					humanoid.ai.DisableAI();
				}
				if (StateManager.Keyboard.Shift)
				{
					humanoid.ai.ScheduleCommand(behavior, targetEntity, _cursorPosition);
				}
				else
				{
					humanoid.ai.ImmediateCommand(behavior, targetEntity, _cursorPosition);
				}
			}
		}

		private void OnDisable()
		{
			HideBehaviors();
		}

		private void AddButton(CommandAction commandAction)
		{
			UnityEngine.Object.Instantiate(actionButtonPrefab, buttonPanel.transform, false).AssignCommand(commandAction);
			_buttonCount++;
		}

		public CommandButton AddCustomButton(string str, Action action)
		{
			CommandButton commandButton = UnityEngine.Object.Instantiate(actionButtonPrefab, buttonPanel.transform, false);
			commandButton.AssignCommand(new CommandAction(str, action));
			_buttonCount++;
			return commandButton;
		}
	}
}
