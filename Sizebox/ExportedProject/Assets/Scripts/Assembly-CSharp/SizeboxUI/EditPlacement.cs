using System;
using System.Runtime.CompilerServices;
using RuntimeGizmos;
using SaveDataStructures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SizeboxUI
{
	public class EditPlacement : MonoBehaviour
	{
		public enum State
		{
			Idle = 0,
			Move = 1,
			Ctrl = 2
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass40_0
		{
			public SavableData data;

			internal void _003CPasteFromClipboard_003Eb__0(EntityBase b)
			{
				b.Load(data, false);
			}
		}

		public static EditPlacement Instance;

		[Header("Required Reference")]
		[SerializeField]
		private GameObject cursorPrefab;

		public bool updateHandles = true;

		private TransformGizmo gizmo;

		public static bool showHandles = true;

		private Vector3 cursorPosition = Vector3.zero;

		private Quaternion cursorRotation = Quaternion.identity;

		private bool recreateMesh;

		private GuiManager view;

		private CtrlDisplay ctrlDisplay;

		private EntityBase hoverTarget;

		private AssetDescription clipboardAsset;

		private EntityType clipboardType;

		private SavableData clipboardSave;

		public TransformGizmo Gizmo
		{
			get
			{
				return gizmo;
			}
		}

		public Camera cam { get; private set; }

		public GameObject cursor { get; private set; }

		public State state { get; set; }

		public InterfaceControl control { get; private set; }

		public bool IsMoving()
		{
			return state == State.Move;
		}

		private void Awake()
		{
			Instance = this;
			state = State.Idle;
			control = GuiManager.Instance.InterfaceControl;
			gizmo = GuiManager.Instance.TransformGizmo;
			cam = Camera.main;
		}

		private void Start()
		{
			view = GetComponent<GuiManager>();
			ctrlDisplay = view.EditMode.CtrlDisplay;
			cursor = UnityEngine.Object.Instantiate(cursorPrefab, view.EditMode.transform);
			Show3DCursor(false);
		}

		private void OnEnable()
		{
			gizmo.enabled = true;
		}

		private void OnDisable()
		{
			if ((bool)cursor)
			{
				cursor.gameObject.SetActive(false);
			}
			gizmo.enabled = false;
			if (state == State.Move)
			{
				PlaceObject();
			}
			state = State.Idle;
		}

		private void LateUpdate()
		{
			bool flag = false;
			if (control == null)
			{
				control = GetComponent<InterfaceControl>();
			}
			Vector3 mousePosition = Input.mousePosition;
			Ray ray = cam.ScreenPointToRay(mousePosition);
			LayerMask layerMask = Layers.placementMask;
			if (state == State.Ctrl)
			{
				layerMask = Layers.ctrlMask;
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layerMask))
			{
				cursor.transform.localScale = Vector3.one * hitInfo.distance / 50f;
				cursorPosition = hitInfo.point;
				cursorRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
				cursor.transform.position = cursorPosition;
				cursor.transform.rotation = cursorRotation;
			}
			if (!StateManager.Keyboard.userIsTyping)
			{
				switch (state)
				{
				case State.Idle:
					flag = true;
					if (StateManager.Keyboard.Ctrl)
					{
						Show3DCursor(true);
						state = State.Ctrl;
					}
					break;
				case State.Move:
					if ((bool)control.selectedEntity)
					{
						if (control.selectedEntity.isPositioned)
						{
							control.selectedEntity.isPositioned = false;
						}
						control.selectedEntity.Move(cursorPosition);
						if (hitInfo.transform == null || hitInfo.transform.gameObject.layer == Layers.mapLayer || hitInfo.transform.gameObject.layer == Layers.defaultLayer)
						{
							control.selectedEntity.transform.SetParent(null);
						}
						else
						{
							control.selectedEntity.transform.SetParent(hitInfo.transform, true);
						}
						if (!control.lockRotation && control.selectedEntity.rotationEnabled)
						{
							control.selectedEntity.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(control.selectedEntity.transform.forward, hitInfo.normal), hitInfo.normal);
						}
						if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
						{
							PlaceObject();
						}
					}
					else
					{
						state = State.Idle;
					}
					if (state == State.Idle)
					{
						control.selectedEntity.Place();
					}
					break;
				case State.Ctrl:
					if (!StateManager.Keyboard.Ctrl)
					{
						state = State.Idle;
						Show3DCursor(false);
					}
					else if ((bool)hitInfo.collider)
					{
						EntityBase componentInParent = hitInfo.collider.GetComponentInParent<EntityBase>();
						UpdateClipboardAndTarget(componentInParent);
						if (Keyboard.current.vKey.wasPressedThisFrame)
						{
							PasteFromClipboard();
						}
					}
					break;
				}
			}
			if (flag && showHandles && (bool)control.selectedEntity)
			{
				gizmo.enabled = true;
				if (updateHandles)
				{
					gizmo.SetTarget(control.selectedEntity.transform);
				}
			}
			else
			{
				gizmo.enabled = false;
			}
		}

		private void UpdateClipboardAndTarget(EntityBase target)
		{
			if ((bool)target)
			{
				hoverTarget = target;
				if (Keyboard.current.cKey.wasPressedThisFrame)
				{
					clipboardAsset = hoverTarget.asset;
					clipboardType = hoverTarget.EntityType;
					clipboardSave = hoverTarget.Save();
				}
			}
			else
			{
				hoverTarget = null;
			}
			ctrlDisplay.SetHoverDisplay(target);
			ctrlDisplay.SetClipboardDisplay(clipboardAsset, clipboardType);
		}

		private void PasteFromClipboard()
		{
			_003C_003Ec__DisplayClass40_0 _003C_003Ec__DisplayClass40_ = new _003C_003Ec__DisplayClass40_0();
			if (clipboardAsset != null)
			{
				_003C_003Ec__DisplayClass40_.data = clipboardSave;
				Action<EntityBase> callback = _003C_003Ec__DisplayClass40_._003CPasteFromClipboard_003Eb__0;
				switch (clipboardType)
				{
				case EntityType.OBJECT:
				case EntityType.VEHICLE:
					LocalClient.Instance.SpawnObject(clipboardAsset, cursorPosition, Quaternion.identity, 1f, callback);
					break;
				case EntityType.MICRO:
					LocalClient.Instance.SpawnMicro(clipboardAsset, cursorPosition, Quaternion.identity, 1f, callback);
					break;
				case EntityType.MACRO:
					LocalClient.Instance.SpawnGiantess(clipboardAsset, cursorPosition, Quaternion.identity, 0.05f, callback);
					break;
				}
			}
		}

		private void PlaceObject()
		{
			if ((bool)control.selectedEntity && state == State.Move)
			{
				control.selectedEntity.SetColliderActive(true);
				if (control.giantess != null && recreateMesh)
				{
					control.giantess.ForceColliderUpdate();
				}
				control.selectedEntity.isPositioned = true;
				state = State.Idle;
				if (control.selectedEntity.isPlayer)
				{
					CenterOrigin.Instance.ForceUpdate();
				}
			}
		}

		public void AddGameObject(AssetDescription asset)
		{
			GameObject gameObject = LocalClient.Instance.SpawnObject(asset, cursorPosition, Quaternion.identity, control.lastMicroScale);
			if ((bool)gameObject)
			{
				OnObjectAuthoritySpawned(gameObject.GetComponent<EntityBase>());
			}
		}

		public void OnObjectAuthoritySpawned(EntityBase entity)
		{
			if (!(control == null))
			{
				control.SetSelectedObject(entity);
				entity.SetColliderActive(false);
				state = State.Move;
			}
		}

		public void AddGiantess(AssetDescription gtsAsset, bool playAs)
		{
			GameObject gameObject = LocalClient.Instance.SpawnGiantess(gtsAsset, cursorPosition, Quaternion.identity, control.lastMacroScale);
			if ((bool)gameObject)
			{
				if (playAs)
				{
					LocalClient.Instance.Player.PlayAs(gameObject.GetComponent<IPlayable>());
				}
				OnGiantessSpawned(gameObject.GetComponent<Giantess>());
			}
		}

		public void AddMicro(AssetDescription microAsset, bool playAs)
		{
			GameObject gameObject = LocalClient.Instance.SpawnMicro(microAsset, cursorPosition, Quaternion.identity, control.lastMicroScale);
			if ((bool)gameObject)
			{
				if (playAs)
				{
					LocalClient.Instance.Player.PlayAs(gameObject.GetComponent<IPlayable>());
				}
				OnMicroSpawned(gameObject.GetComponent<Micro>());
			}
		}

		public void OnGiantessSpawned(Giantess gts)
		{
			if (!(control == null))
			{
				control.SetSelectedObject(gts);
				gts.SetColliderActive(false);
				recreateMesh = true;
				state = State.Move;
			}
		}

		public void OnMicroSpawned(Micro micro)
		{
			if (!(control == null))
			{
				control.SetSelectedObject(micro);
				recreateMesh = true;
				state = State.Move;
			}
		}

		private void Show3DCursor(bool show)
		{
			cursor.SetActive(show);
			ctrlDisplay.gameObject.SetActive(show);
		}

		public void MoveCurrentGO()
		{
			if ((bool)control.selectedEntity)
			{
				control.selectedEntity.SetColliderActive(false);
				state = State.Move;
			}
		}

		public void Delete()
		{
			control.DeleteObject();
			state = State.Idle;
		}
	}
}
