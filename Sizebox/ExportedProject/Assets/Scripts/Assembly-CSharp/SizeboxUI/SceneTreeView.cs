using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SizeboxUI
{
	public class SceneTreeView : MonoBehaviour
	{
		[Header("Required References")]
		[SerializeField]
		private SceneTreeViewCategory giantessCategory;

		[SerializeField]
		private SceneTreeViewCategory microCategory;

		[SerializeField]
		private SceneTreeViewCategory objectCategory;

		private Dictionary<EntityBase, SceneTreeEntryGui> entries = new Dictionary<EntityBase, SceneTreeEntryGui>();

		private Dictionary<EntityBase, SceneTreeEntryGui> selectedEntries = new Dictionary<EntityBase, SceneTreeEntryGui>();

		private CommandView cView;

		private InterfaceControl control;

		private void Awake()
		{
			cView = GetComponentInParent<EditView>().CommandView;
			control = GetComponentInParent<EditView>().control;
			ObjectManager instance = ObjectManager.Instance;
			RegisterEntries(objectCategory.InitializeEntries(instance.objectList));
			RegisterEntries(giantessCategory.InitializeEntries(instance.giantessList));
			RegisterEntries(microCategory.InitializeEntries(instance.microList));
			instance.OnEntityAdd += OnAdd;
			instance.OnEntityRemove += OnRemove;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				DeleteSelectedEntities();
			}
		}

		private void Select(SceneTreeEntryGui entry)
		{
			EntityBase entity = entry.entity;
			if ((bool)entry && (bool)entity)
			{
				entry.Select();
				if (GlobalPreferences.SlSelObjOnEntryClick.value)
				{
					control.SetSelectedObject(entry.entity);
				}
				if (!selectedEntries.ContainsKey(entity))
				{
					selectedEntries.Add(entity, entry);
				}
			}
		}

		private void Deselect(SceneTreeEntryGui entry)
		{
			EntityBase entity = entry.entity;
			if ((bool)entry && (bool)entity)
			{
				entry.Deselect();
				if (selectedEntries.ContainsKey(entity))
				{
					selectedEntries.Remove(entity);
				}
			}
		}

		private void ToggleSelected(SceneTreeEntryGui entry)
		{
			if (!selectedEntries.ContainsKey(entry.entity))
			{
				Select(entry);
			}
			else
			{
				Deselect(entry);
			}
		}

		public bool IsEntrySelected(SceneTreeEntryGui entry)
		{
			return selectedEntries.ContainsValue(entry);
		}

		public void ClearSelected()
		{
			SceneTreeEntryGui[] array = new SceneTreeEntryGui[selectedEntries.Count];
			int num = 0;
			foreach (KeyValuePair<EntityBase, SceneTreeEntryGui> selectedEntry in selectedEntries)
			{
				array[num] = selectedEntry.Value;
				num++;
			}
			SceneTreeEntryGui[] array2 = array;
			foreach (SceneTreeEntryGui entry in array2)
			{
				Deselect(entry);
			}
		}

		private void DeleteSelectedEntities()
		{
			EntityBase[] array = new EntityBase[selectedEntries.Count];
			int num = 0;
			foreach (SceneTreeEntryGui value in selectedEntries.Values)
			{
				array[num] = value.entity;
				num++;
			}
			EntityBase[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].DestroyObject();
			}
		}

		private void RegisterEntries(List<SceneTreeEntryGui> newEntries)
		{
			foreach (SceneTreeEntryGui newEntry in newEntries)
			{
				entries.Add(newEntry.entity, newEntry);
			}
		}

		private void OnAdd(EntityBase entity)
		{
			SceneTreeEntryGui value = (entity.GetComponent<Giantess>() ? giantessCategory.CreateEntry(entity) : ((!entity.GetComponent<Micro>()) ? objectCategory.CreateEntry(entity) : microCategory.CreateEntry(entity)));
			entries.Add(entity, value);
		}

		private void OnRemove(EntityBase entity)
		{
			if (entries.ContainsKey(entity))
			{
				DeleteEntry(entries[entity]);
			}
		}

		private void DeleteEntry(SceneTreeEntryGui entry)
		{
			Deselect(entry);
			if (entries.ContainsKey(entry.entity))
			{
				entries.Remove(entry.entity);
			}
			entry.Delete();
		}

		public void OnLeftDoubleClick(SceneTreeEntryGui clickedEntry)
		{
			CenterView(clickedEntry.entity);
		}

		public void OnLeftClick(SceneTreeEntryGui clickedEntry)
		{
			if (StateManager.Keyboard.Shift)
			{
				ToggleSelected(clickedEntry);
			}
			else if (!IsEntrySelected(clickedEntry) || selectedEntries.Count > 1)
			{
				ClearSelected();
				Select(clickedEntry);
			}
			else
			{
				Deselect(clickedEntry);
			}
		}

		public void OnRightClick(SceneTreeEntryGui clickedEntry)
		{
			cView.HideBehaviors();
			EntityBase entity = clickedEntry.entity;
			if (StateManager.Keyboard.Shift)
			{
				ToggleSelected(clickedEntry);
				return;
			}
			bool flag = selectedEntries.ContainsKey(entity);
			if (selectedEntries.Count == 0)
			{
				Select(clickedEntry);
			}
			else if (!flag)
			{
				ClearSelected();
				Select(clickedEntry);
			}
			if (!control.selectedEntity)
			{
				control.SetSelectedObject(entity);
			}
			if (selectedEntries.Count == 1)
			{
				cView.OpenCommandMenu(entity);
			}
			else if (selectedEntries.Count > 1)
			{
				cView.AddCustomButton("Delete", _003COnRightClick_003Eb__21_0);
				cView.AddCustomButton("Cancel", _003COnRightClick_003Eb__21_1);
			}
			cView.KeepCommandMenuOnScreen();
		}

		private void CenterView(EntityBase entity)
		{
			Camera main = Camera.main;
			float num = entity.transform.lossyScale.y * entity.meshHeight;
			main.transform.parent.SetPositionAndRotation(entity.transform.position + new Vector3(0f, num / 2f, 0f) - main.transform.forward.normalized * (num * 1.4f), main.transform.rotation);
		}

		[CompilerGenerated]
		private void _003COnRightClick_003Eb__21_0()
		{
			DeleteSelectedEntities();
			cView.HideBehaviors();
		}

		[CompilerGenerated]
		private void _003COnRightClick_003Eb__21_1()
		{
			cView.HideBehaviors();
		}
	}
}
