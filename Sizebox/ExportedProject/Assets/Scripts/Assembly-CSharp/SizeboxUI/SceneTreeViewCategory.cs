using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class SceneTreeViewCategory : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[Header("Prefabs")]
		[SerializeField]
		private SceneTreeEntryGui entryPrefab;

		[Header("Sprites")]
		[SerializeField]
		private Sprite playerSprite;

		[SerializeField]
		private Sprite categorySprite;

		[Header("Colors")]
		[SerializeField]
		private Color openColor = Color.blue;

		[SerializeField]
		private Color closedColor = Color.red;

		[Header("Required References")]
		[SerializeField]
		private Image background;

		[SerializeField]
		private RectTransform contentTransform;

		[SerializeField]
		private Text countText;

		private bool isOpen;

		private bool openedBefore;

		private int count;

		public List<SceneTreeEntryGui> InitializeEntries(ICollection<EntityBase> entities)
		{
			List<SceneTreeEntryGui> list = new List<SceneTreeEntryGui>();
			foreach (EntityBase entity in entities)
			{
				list.Add(CreateEntry(entity));
			}
			if (contentTransform.childCount == 0)
			{
				Close();
			}
			return list;
		}

		public List<SceneTreeEntryGui> InitializeEntries(ICollection<Giantess> entities)
		{
			List<SceneTreeEntryGui> list = new List<SceneTreeEntryGui>();
			foreach (Giantess entity in entities)
			{
				list.Add(CreateEntry(entity));
			}
			if (contentTransform.childCount == 0)
			{
				Close();
			}
			return list;
		}

		public List<SceneTreeEntryGui> InitializeEntries(ICollection<Micro> entities)
		{
			List<SceneTreeEntryGui> list = new List<SceneTreeEntryGui>();
			foreach (Micro entity in entities)
			{
				list.Add(CreateEntry(entity));
			}
			if (contentTransform.childCount == 0)
			{
				Close();
			}
			return list;
		}

		public SceneTreeEntryGui CreateEntry(EntityBase entity)
		{
			SceneTreeEntryGui sceneTreeEntryGui = Object.Instantiate(entryPrefab, contentTransform);
			Sprite sprite = categorySprite;
			if (entity.isPlayer)
			{
				sprite = playerSprite;
			}
			sceneTreeEntryGui.Initialize(this, entity, sprite);
			OnAdd();
			return sceneTreeEntryGui;
		}

		private void OnAdd()
		{
			count++;
			UpdateCountText();
			if (!openedBefore)
			{
				Open();
			}
		}

		public void OnDelete()
		{
			count--;
			UpdateCountText();
			if (count <= 0)
			{
				Close();
			}
		}

		private void UpdateCountText()
		{
			countText.text = "( " + count + " )";
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				ToggleOpen();
			}
		}

		private void ToggleOpen()
		{
			if (isOpen || contentTransform.childCount == 0)
			{
				Close();
			}
			else
			{
				Open();
			}
		}

		private void Open()
		{
			openedBefore = true;
			contentTransform.gameObject.SetActive(true);
			isOpen = true;
			background.color = openColor;
		}

		private void Close()
		{
			contentTransform.gameObject.SetActive(false);
			isOpen = false;
			background.color = closedColor;
		}
	}
}
