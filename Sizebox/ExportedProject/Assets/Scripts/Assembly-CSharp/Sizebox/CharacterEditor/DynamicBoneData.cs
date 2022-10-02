using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct DynamicBoneData
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{
			public CharacterEditor characterEditor;

			internal string _003C_002Ector_003Eb__0(Transform t)
			{
				return characterEditor.GetTransformKey(t);
			}
		}

		public string boneKey;

		public List<string> exclusions;

		public DynamicBoneSettings settings;

		public DynamicBoneData(DynamicBone bone, CharacterEditor characterEditor)
		{
			_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_ = new _003C_003Ec__DisplayClass3_0();
			_003C_003Ec__DisplayClass3_.characterEditor = characterEditor;
			boneKey = _003C_003Ec__DisplayClass3_.characterEditor.GetTransformKey(bone.transform);
			exclusions = bone.m_Exclusions.ToList().ConvertAll<string>(_003C_003Ec__DisplayClass3_._003C_002Ector_003Eb__0);
			settings = new DynamicBoneSettings(bone);
		}
	}
}
