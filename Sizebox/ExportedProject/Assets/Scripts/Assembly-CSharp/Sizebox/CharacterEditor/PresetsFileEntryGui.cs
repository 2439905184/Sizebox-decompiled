using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	public class PresetsFileEntryGui : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private Text text;

		[SerializeField]
		private Image background;

		[SerializeField]
		private Color selectedColor = Color.white;

		[SerializeField]
		private Color unselectedColor = Color.white;

		private const string PREFIX = "-  ";

		private bool _Selected;

		public string FilePath { get; private set; }

		public bool Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
				if (_Selected)
				{
					background.color = selectedColor;
				}
				else
				{
					background.color = unselectedColor;
				}
			}
		}

		private void Awake()
		{
			background.color = unselectedColor;
		}

		public void Initialize(string filePath)
		{
			FilePath = filePath;
			SetName(Path.GetFileName(filePath));
		}

		private void SetName(string name)
		{
			text.text = "-  " + name;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			GetComponentInParent<BasePresetsView>().SetTarget(this);
		}
	}
}
