using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class SceneTreeEntryGui : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[Header("Colors")]
		[SerializeField]
		private Color selectedColor = new Color(0f, 0.88f, 1f, 0.88f);

		[SerializeField]
		private Color unselectedColor = new Color(1f, 1f, 1f, 0.39f);

		[SerializeField]
		private Color hoverColor = new Color(0.39f, 0.78f, 1f, 0.66f);

		[Header("Required References")]
		[SerializeField]
		private Text entryText;

		[SerializeField]
		private Image entryImage;

		[SerializeField]
		private Image entryBackground;

		private SceneTreeView stv;

		private SceneTreeViewCategory category;

		private bool renderersShown = true;

		private List<bool> defaultRendererStates = new List<bool>(10);

		public EntityBase entity { get; private set; }

		private void Awake()
		{
			stv = GetComponentInParent<SceneTreeView>();
		}

		public void Initialize(SceneTreeViewCategory category, EntityBase entity, Sprite sprite)
		{
			this.entity = entity;
			this.category = category;
			if (entity.isPlayer)
			{
				entryText.text = "Player";
			}
			else if (entity.asset != null)
			{
				entryText.text = entity.asset.AssetFullName;
			}
			else
			{
				entryText.text = entity.name;
			}
			entryImage.sprite = sprite;
		}

		public void Select()
		{
			entryBackground.color = selectedColor;
		}

		public void Deselect()
		{
			entryBackground.color = unselectedColor;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			CancelInvoke();
			ResetFlash();
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (eventData.clickCount == 1)
				{
					stv.OnLeftClick(this);
				}
				else
				{
					stv.OnLeftDoubleClick(this);
				}
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				stv.OnRightClick(this);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!entity)
			{
				Delete();
			}
			else if (!stv.IsEntrySelected(this))
			{
				entryBackground.color = hoverColor;
				if (!entity.GetComponent<CityBuilder>())
				{
					InvokeRepeating("Flash", 0f, 0.4f);
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!stv.IsEntrySelected(this))
			{
				entryBackground.color = unselectedColor;
				CancelInvoke();
				ResetFlash();
			}
		}

		private void ResetFlash()
		{
			if (!renderersShown)
			{
				Flash();
			}
		}

		private void Flash()
		{
			Renderer[] componentsInChildren = entity.GetComponentsInChildren<Renderer>();
			renderersShown = !renderersShown;
			if (!base.gameObject.activeInHierarchy && renderersShown)
			{
				CancelInvoke();
			}
			if (!renderersShown)
			{
				defaultRendererStates.Clear();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					defaultRendererStates.Add(componentsInChildren[i].enabled);
				}
			}
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].enabled = renderersShown && defaultRendererStates[j];
			}
		}

		public void Delete()
		{
			if ((bool)category)
			{
				category.OnDelete();
			}
			if ((bool)this)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
