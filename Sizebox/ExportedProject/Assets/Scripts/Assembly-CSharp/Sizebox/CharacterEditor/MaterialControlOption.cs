using System.Collections.Generic;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	public abstract class MaterialControlOption : MonoBehaviour
	{
		protected MaterialEntryGui selectedGui;

		protected List<MaterialEntryGui> selectedGuis;

		public virtual void ValidateOption(MaterialEntryGui newGui, List<MaterialEntryGui> newGuis)
		{
			selectedGui = newGui;
			selectedGuis = newGuis;
		}
	}
}
