using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class MaterialEntryGui : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[Header("Required References")]
		[SerializeField]
		private Image textBackground;

		[SerializeField]
		private Text nameText;

		[Space]
		[Header("Colors")]
		[SerializeField]
		private Color selectedColor = Color.white;

		[SerializeField]
		private Color normalColor = Color.white;

		[SerializeField]
		private Color selectedTextColor = Color.white;

		[SerializeField]
		private Color normalTextColor = Color.white;

		private MaterialEditView _materialView;

		public bool Selected { get; private set; }

		public MaterialWrapper MatWrapper { get; private set; }

		public void RegisterMaterial(MaterialWrapper wrapper, MaterialEditView materialView)
		{
			MatWrapper = wrapper;
			_materialView = materialView;
			nameText.text = wrapper.Material.name;
			nameText.text = nameText.text.Replace("(Instance)", "");
		}

		public void Select()
		{
			textBackground.color = selectedColor;
			nameText.color = selectedTextColor;
			Selected = true;
		}

		public void Deselect()
		{
			textBackground.color = normalColor;
			nameText.color = normalTextColor;
			Selected = false;
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if ((bool)_materialView)
			{
				_materialView.GetControls().ProcessClickOnMaterial(this, eventData);
			}
		}
	}
}
