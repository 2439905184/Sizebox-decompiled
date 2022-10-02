using System;
using UnityEngine.UI;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public class ViewConfig
	{
		public delegate void ViewConfigEvent();

		public BaseView view;

		public Selectable activator;

		public bool activatorIsToggle;

		public bool disableInteractionOnClick;

		public string name
		{
			get
			{
				if ((bool)view)
				{
					return view.name;
				}
				return "SubView";
			}
		}

		public event ViewConfigEvent OnViewOpen;

		public void _OpenEvent()
		{
			if (this.OnViewOpen != null)
			{
				this.OnViewOpen();
			}
		}
	}
}
