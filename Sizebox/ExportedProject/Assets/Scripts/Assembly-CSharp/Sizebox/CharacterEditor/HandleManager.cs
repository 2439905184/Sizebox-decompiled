using System.Collections.Generic;
using RuntimeGizmos;
using SizeboxUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class HandleManager : MonoBehaviour
	{
		public delegate void OnHandleSelect(SkeletonEditHandle handle);

		[Header("Prefabs")]
		[SerializeField]
		private SkeletonEditHandle handlePrefab;

		[Header("Required References")]
		[SerializeField]
		private Text selectionText;

		private Color SELECT_COLOR = Color.white;

		private Color DESELECT_COLOR = Color.grey;

		public OnHandleSelect onHandleSelection;

		public OnHandleSelect onHandleDeselection;

		private Camera mainCamera;

		private InterfaceControl uiController;

		private EditPlacement placement;

		private TransformGizmo gizmo;

		private SkeletonEditOptions skeletonEditOptions = new SkeletonEditOptions();

		private List<SkeletonEditHandle> boneHandles = new List<SkeletonEditHandle>();

		private bool handlesEnabled;

		private string ActiveDataId;

		private TransformType previousType;

		private float cycleTimer;

		private int previousCount;

		private int previousIndex = -1;

		private const float SELECTION_CYCLING_SECONDS = 2f;

		public CharacterEditor TargetEditor { get; private set; }

		public SkeletonEditHandle TargetHandle { get; private set; }

		public List<SkeletonEditHandle> TargetHandles { get; private set; }

		public Dictionary<string, SkeletonEditHandle> HandleKeyMap { get; private set; }

		public Dictionary<Transform, SkeletonEditHandle> HandleTransformMap { get; private set; }

		public Dictionary<SkeletonEditHandle, string> KeyMap { get; private set; }

		private void Awake()
		{
			TargetHandles = new List<SkeletonEditHandle>();
			HandleKeyMap = new Dictionary<string, SkeletonEditHandle>();
			HandleTransformMap = new Dictionary<Transform, SkeletonEditHandle>();
			KeyMap = new Dictionary<SkeletonEditHandle, string>();
			mainCamera = Camera.main;
			gizmo = GuiManager.Instance.TransformGizmo;
			uiController = GuiManager.Instance.InterfaceControl;
			placement = GuiManager.Instance.EditPlacement;
		}

		private void OnEnable()
		{
			TargetHandle = null;
			ClearTargetHandles();
		}

		private void LateUpdate()
		{
			UpdateHandles();
		}

		private void UpdateHandles()
		{
			if (!handlesEnabled || skeletonEditOptions == null)
			{
				return;
			}
			UpdateTargetHandle();
			if (gizmo.type != previousType)
			{
				previousType = gizmo.type;
				foreach (SkeletonEditHandle targetHandle in TargetHandles)
				{
					targetHandle.UpdatePosition(gizmo.type);
				}
			}
			if ((bool)TargetHandle && ActiveDataId != null)
			{
				TransformationData data = TargetHandle.UpdateHandle(ActiveDataId, gizmo.type, skeletonEditOptions);
				foreach (SkeletonEditHandle targetHandle2 in TargetHandles)
				{
					if (targetHandle2 != TargetHandle)
					{
						targetHandle2.UpdateHandle(ActiveDataId, data, gizmo.type, skeletonEditOptions);
					}
				}
			}
			foreach (SkeletonEditHandle boneHandle in boneHandles)
			{
				if (boneHandle.gameObject.activeInHierarchy)
				{
					boneHandle.UpdatePosition(gizmo.type);
				}
			}
		}

		private void UpdateTargetHandle()
		{
			bool shift = StateManager.Keyboard.Shift;
			bool ctrl = StateManager.Keyboard.Ctrl;
			if (Mouse.current.middleButton.wasPressedThisFrame)
			{
				gizmo.SetTarget(null);
				TargetHandle = null;
				ClearTargetHandles();
			}
			else
			{
				if (!Mouse.current.leftButton.wasPressedThisFrame)
				{
					return;
				}
				if (ctrl)
				{
					RaycastHit[] array = Physics.RaycastAll(mainCamera.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, Layers.auxMask);
					if (array.Length == 0)
					{
						return;
					}
					RaycastHit[] array2 = array;
					foreach (RaycastHit raycastHit in array2)
					{
						SkeletonEditHandle componentInParent = raycastHit.collider.transform.GetComponentInParent<SkeletonEditHandle>();
						if (DeselectHandle(componentInParent))
						{
							break;
						}
					}
				}
				else
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
					RaycastHit[] array3 = Physics.RaycastAll(mainCamera.ScreenPointToRay(Input.mousePosition), float.PositiveInfinity, Layers.auxMask);
					if (array3.Length != 0)
					{
						if (previousCount == array3.Length && cycleTimer > Time.time)
						{
							if (previousIndex + 1 == array3.Length)
							{
								previousIndex = -1;
							}
							for (int j = previousIndex + 1; j < array3.Length; j++)
							{
								SkeletonEditHandle componentInParent2 = array3[j].collider.transform.GetComponentInParent<SkeletonEditHandle>();
								if (SelectHandle(componentInParent2, shift))
								{
									previousIndex = j;
									break;
								}
							}
						}
						else
						{
							previousIndex = -1;
							RaycastHit[] array2 = array3;
							foreach (RaycastHit raycastHit2 in array2)
							{
								SkeletonEditHandle componentInParent3 = raycastHit2.collider.transform.GetComponentInParent<SkeletonEditHandle>();
								if (SelectHandle(componentInParent3, shift))
								{
									break;
								}
							}
						}
					}
					previousCount = array3.Length;
					cycleTimer = Time.time + 2f;
				}
			}
		}

		public void SetTarget(CharacterEditor target)
		{
			TargetEditor = target;
		}

		public void DeleteTargets(bool deleteBone = false)
		{
			for (int i = 0; i < TargetHandles.Count; i++)
			{
				EditBone editBone = TargetHandles[i].EditBone;
				if ((bool)editBone)
				{
					Transform realTransform = editBone.RealTransform;
					editBone.Disable();
					Object.Destroy(editBone.gameObject);
					if (deleteBone)
					{
						TargetEditor.DeleteTransform(editBone.Key);
					}
				}
			}
			ClearTargetHandles();
		}

		private bool SelectHandle(SkeletonEditHandle handle, bool multiSelect)
		{
			if (!handle || TargetHandles.Contains(handle))
			{
				return false;
			}
			if (multiSelect)
			{
				TargetHandles.Add(handle);
			}
			else
			{
				ClearTargetHandles();
				TargetHandles.Add(handle);
			}
			gizmo.SetTarget(handle.transform);
			TargetHandle = handle;
			handle.Selected = true;
			if (onHandleSelection != null)
			{
				onHandleSelection(handle);
			}
			if ((bool)handle.EditBone.RealTransform)
			{
				if (TargetHandles.Count == 1)
				{
					selectionText.text = handle.EditBone.RealTransform.name;
				}
				else
				{
					selectionText.text = handle.EditBone.RealTransform.name + " (+" + (TargetHandles.Count - 1) + ")";
				}
			}
			selectionText.color = SELECT_COLOR;
			return true;
		}

		private bool DeselectHandle(SkeletonEditHandle handle)
		{
			if (!handle || !TargetHandles.Contains(handle))
			{
				return false;
			}
			handle.Selected = false;
			TargetHandles.Remove(handle);
			selectionText.text = handle.EditBone.RealTransform.name;
			selectionText.color = DESELECT_COLOR;
			return true;
		}

		private void ClearTargetHandles()
		{
			foreach (SkeletonEditHandle targetHandle in TargetHandles)
			{
				if ((bool)targetHandle)
				{
					targetHandle.Selected = false;
					if (onHandleDeselection != null)
					{
						onHandleDeselection(targetHandle);
					}
				}
			}
			TargetHandles.Clear();
			selectionText.text = "";
		}

		public void EnableHandlesMode(string id, List<string> keys = null)
		{
			if (!TargetEditor)
			{
				return;
			}
			SkeletonEdit skeletonEdit = TargetEditor.SkeletonEdit;
			ActiveDataId = id;
			EnsureHandles(skeletonEdit);
			HandleKeyMap.Clear();
			HandleTransformMap.Clear();
			KeyMap.Clear();
			if (keys != null)
			{
				int num = 0;
				EntityBase component = TargetEditor.GetComponent<EntityBase>();
				foreach (string key in keys)
				{
					if (skeletonEdit.BoneMap.ContainsKey(key))
					{
						SkeletonEditHandle skeletonEditHandle = boneHandles[num];
						EditBone editBone = skeletonEdit.BoneMap[key];
						skeletonEditHandle.gameObject.SetActive(true);
						skeletonEditHandle.AssignBone(editBone, component);
						KeyMap.Add(skeletonEditHandle, key);
						HandleKeyMap.Add(key, skeletonEditHandle);
						HandleTransformMap.Add(editBone.RealTransform, skeletonEditHandle);
						num++;
					}
				}
			}
			uiController.commandEnabled = false;
			placement.updateHandles = false;
			gizmo.overrideSpace = true;
			gizmo.space = TransformSpace.Local;
			handlesEnabled = true;
		}

		public void EnableHandles(ICollection<string> keys)
		{
			if (!handlesEnabled)
			{
				return;
			}
			foreach (string key in keys)
			{
				if (HandleKeyMap.ContainsKey(key))
				{
					HandleKeyMap[key].gameObject.SetActive(true);
				}
			}
		}

		public void DisableHandles(ICollection<string> keys)
		{
			foreach (string key in keys)
			{
				if (HandleKeyMap.ContainsKey(key))
				{
					HandleKeyMap[key].Disable(base.transform);
				}
			}
		}

		public void DisableHandlesMode()
		{
			uiController.commandEnabled = true;
			placement.updateHandles = true;
			gizmo.overrideSpace = false;
			ClearTargetHandles();
			foreach (SkeletonEditHandle boneHandle in boneHandles)
			{
				boneHandle.Disable(base.transform);
			}
			if ((bool)TargetEditor)
			{
				gizmo.SetTarget(TargetEditor.transform);
			}
			ActiveDataId = null;
			handlesEnabled = false;
		}

		public void SetHandleStyle(SkeletonEditHandle.HandleStyle style, ICollection<string> keys, bool oppositeStyleOthers = false)
		{
			if (oppositeStyleOthers)
			{
				SkeletonEditHandle.HandleStyle style2 = ((style == SkeletonEditHandle.HandleStyle.Active) ? SkeletonEditHandle.HandleStyle.Inactive : SkeletonEditHandle.HandleStyle.Active);
				foreach (SkeletonEditHandle value in HandleKeyMap.Values)
				{
					value.SetStyle(style2);
				}
			}
			foreach (string key in keys)
			{
				if (HandleKeyMap.ContainsKey(key))
				{
					HandleKeyMap[key].SetStyle(style);
				}
			}
		}

		public void SetHandleStyle(SkeletonEditHandle.HandleStyle style, Transform bone)
		{
			if (HandleTransformMap.ContainsKey(bone))
			{
				HandleTransformMap[bone].SetStyle(style);
			}
		}

		public void SetHandleSize(float sizeMult)
		{
			foreach (SkeletonEditHandle boneHandle in boneHandles)
			{
				boneHandle.SetSize(sizeMult);
			}
		}

		private void EnsureHandles(SkeletonEdit skeletonEdit)
		{
			for (int i = 0; i < boneHandles.Count; i++)
			{
				if (boneHandles[i] == null)
				{
					boneHandles.RemoveAt(i);
					i--;
				}
			}
			int count = skeletonEdit.BoneMap.Count;
			for (int j = boneHandles.Count; j < count; j++)
			{
				SkeletonEditHandle item = Object.Instantiate(handlePrefab, base.transform);
				boneHandles.Add(item);
			}
		}

		public void ResetAll(string id)
		{
			if (!TargetEditor)
			{
				return;
			}
			foreach (EditBone value in TargetEditor.SkeletonEdit.BoneMap.Values)
			{
				value.ResetAll(id);
			}
		}

		public void ResetTarget(string id, SkeletonEditOptions options)
		{
			if (!TargetEditor || !TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in TargetHandles)
			{
				targetHandle.ResetBone(id, options);
				targetHandle.UpdatePosition(gizmo.type);
			}
		}

		public void Reset(string id, SkeletonEditOptions options)
		{
			if (!TargetEditor || !TargetHandle)
			{
				return;
			}
			foreach (SkeletonEditHandle targetHandle in TargetHandles)
			{
				targetHandle.Reset(id, gizmo.type, options);
				targetHandle.UpdatePosition(gizmo.type);
			}
		}

		public void SetOptions(SkeletonEditOptions options)
		{
			skeletonEditOptions = options;
		}

		public SkeletonEditHandle GetHandle(string key)
		{
			if (HandleKeyMap.ContainsKey(key))
			{
				return HandleKeyMap[key];
			}
			return null;
		}

		public string GetHandleKey(SkeletonEditHandle handle = null)
		{
			if (handle == null)
			{
				handle = TargetHandle;
			}
			if (KeyMap.ContainsKey(handle))
			{
				return KeyMap[handle];
			}
			return null;
		}

		public void PairBones()
		{
			if (TargetHandles.Count == 2)
			{
				EditBone editBone = TargetHandles[0].EditBone;
				EditBone editBone2 = TargetHandles[1].EditBone;
				if (!editBone.SiblingBone && !editBone2.SiblingBone)
				{
					TargetEditor.SkeletonEdit.CreatePair(editBone, editBone2);
				}
				else if (editBone.SiblingBone == editBone2)
				{
					TargetEditor.SkeletonEdit.RemovePair(editBone, editBone2);
				}
			}
		}
	}
}
