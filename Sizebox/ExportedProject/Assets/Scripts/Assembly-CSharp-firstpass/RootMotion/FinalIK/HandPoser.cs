using UnityEngine;

namespace RootMotion.FinalIK
{
	public class HandPoser : Poser
	{
		private Transform _poseRoot;

		private Transform[] children;

		private Transform[] poseChildren;

		private Vector3[] defaultLocalPositions;

		private Quaternion[] defaultLocalRotations;

		public override void AutoMapping()
		{
			if (poseRoot == null)
			{
				poseChildren = new Transform[0];
			}
			else
			{
				poseChildren = poseRoot.GetComponentsInChildren<Transform>();
			}
			_poseRoot = poseRoot;
		}

		protected override void InitiatePoser()
		{
			children = GetComponentsInChildren<Transform>();
			StoreDefaultState();
		}

		protected override void FixPoserTransforms()
		{
			for (int i = 0; i < children.Length; i++)
			{
				children[i].localPosition = defaultLocalPositions[i];
				children[i].localRotation = defaultLocalRotations[i];
			}
		}

		protected override void UpdatePoser()
		{
			if (weight <= 0f || (localPositionWeight <= 0f && localRotationWeight <= 0f))
			{
				return;
			}
			if (_poseRoot != poseRoot)
			{
				AutoMapping();
			}
			if (poseRoot == null)
			{
				return;
			}
			if (children.Length != poseChildren.Length)
			{
				Warning.Log("Number of children does not match with the pose", base.transform);
				return;
			}
			float t = localRotationWeight * weight;
			float t2 = localPositionWeight * weight;
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i] != base.transform)
				{
					children[i].localRotation = Quaternion.Lerp(children[i].localRotation, poseChildren[i].localRotation, t);
					children[i].localPosition = Vector3.Lerp(children[i].localPosition, poseChildren[i].localPosition, t2);
				}
			}
		}

		private void StoreDefaultState()
		{
			defaultLocalPositions = new Vector3[children.Length];
			defaultLocalRotations = new Quaternion[children.Length];
			for (int i = 0; i < children.Length; i++)
			{
				defaultLocalPositions[i] = children[i].localPosition;
				defaultLocalRotations[i] = children[i].localRotation;
			}
		}
	}
}
