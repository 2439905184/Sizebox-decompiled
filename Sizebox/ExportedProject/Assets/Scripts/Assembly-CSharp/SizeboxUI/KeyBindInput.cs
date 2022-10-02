using UnityEngine;
using UnityEngine.UI;

namespace SizeboxUI
{
	public class KeyBindInput : MonoBehaviour
	{
		public string oldKeyName;

		private void OnGUI()
		{
			Event current = Event.current;
			if (current.isKey && current.keyCode != 0)
			{
				if (current.keyCode == KeyCode.Escape || current.keyCode == KeyCode.Backspace)
				{
					base.gameObject.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "'" + oldKeyName + "'";
					Object.Destroy(this);
				}
				else
				{
					base.gameObject.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text = "'" + ((current.shift || current.capsLock) ? current.keyCode.ToString().ToUpper() : current.keyCode.ToString().ToLower()) + "'";
					Object.Destroy(this);
				}
			}
		}
	}
}
