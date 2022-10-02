using UnityEngine;

namespace SA
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class FullBodyIKUnityChanBehaviour : FullBodyIKBehaviourBase
	{
		[SerializeField]
		private FullBodyIKUnityChan _fullBodyIK;

		public override FullBodyIK fullBodyIK
		{
			get
			{
				if (_fullBodyIK == null)
				{
					_fullBodyIK = new FullBodyIKUnityChan();
				}
				return _fullBodyIK;
			}
		}
	}
}
