using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IDeselectHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!EventSystem.current.alreadySelecting)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		GetComponent<Selectable>().OnPointerExit(null);
	}
}
