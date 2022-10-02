using System;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct DynamicBoneSettings
	{
		public float dampening;

		public float stiffness;

		public float elasticity;

		public float inertia;

		public DynamicBoneSettings(DynamicBone bone)
		{
			dampening = bone.m_Damping;
			stiffness = bone.m_Stiffness;
			elasticity = bone.m_Elasticity;
			inertia = bone.m_Inert;
		}
	}
}
