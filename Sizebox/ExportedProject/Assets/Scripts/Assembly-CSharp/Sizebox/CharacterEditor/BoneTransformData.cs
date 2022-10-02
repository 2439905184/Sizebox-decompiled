using System;
using UnityEngine;

namespace Sizebox.CharacterEditor
{
	[Serializable]
	public struct BoneTransformData
	{
		public Vector3 position;

		public Vector3 scale;

		public Quaternion rotation;

		public static BoneTransformData Default = new BoneTransformData
		{
			position = Vector3.zero,
			rotation = Quaternion.identity,
			scale = Vector3.one
		};

		public static BoneTransformData Lerp(BoneTransformData data, BoneTransformData data2, float time)
		{
			BoneTransformData result = default(BoneTransformData);
			result.position = Vector3.Lerp(data.position, data2.position, time);
			result.rotation = Quaternion.Slerp(data.rotation, data2.rotation, time);
			result.scale = Vector3.Lerp(data.scale, data2.scale, time);
			return result;
		}

		public static BoneTransformData operator +(BoneTransformData data, BoneTransformData data2)
		{
			BoneTransformData result = default(BoneTransformData);
			result.position = data.position + data2.position;
			result.rotation = data.rotation * data2.rotation;
			result.scale = Vector3.Scale(data.scale, data2.scale);
			return result;
		}

		public static bool operator ==(BoneTransformData data, BoneTransformData data2)
		{
			if (data.position != data2.position || data.scale != data2.scale || data.rotation != data2.rotation)
			{
				return false;
			}
			return true;
		}

		public static bool operator !=(BoneTransformData data, BoneTransformData data2)
		{
			return !(data == data2);
		}

		public override bool Equals(object obj)
		{
			if (obj is BoneTransformData)
			{
				return this == (BoneTransformData)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			Vector3 vector = Vector3.Scale(rotation * position, scale) * 10.5f;
			return Mathf.CeilToInt(vector.x + vector.y + vector.z);
		}
	}
}
