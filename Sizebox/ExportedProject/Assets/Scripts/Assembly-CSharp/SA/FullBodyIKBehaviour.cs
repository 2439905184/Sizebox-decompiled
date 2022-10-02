using UnityEngine;

namespace SA
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class FullBodyIKBehaviour : FullBodyIKBehaviourBase
	{
		[SerializeField]
		private FullBodyIK _fullBodyIK;

		public override FullBodyIK fullBodyIK
		{
			get
			{
				if (_fullBodyIK == null)
				{
					_fullBodyIK = new FullBodyIK();
				}
				return _fullBodyIK;
			}
		}
	}
}
