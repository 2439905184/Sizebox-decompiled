using UnityEngine;

namespace RootMotion.Dynamics
{
	public static class PuppetMasterTools
	{
		public static void PositionRagdoll(PuppetMaster puppetMaster)
		{
			Rigidbody[] componentsInChildren = puppetMaster.transform.GetComponentsInChildren<Rigidbody>();
			if (componentsInChildren.Length == 0)
			{
				return;
			}
			Muscle[] muscles = puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				if (muscle.joint == null || muscle.target == null)
				{
					return;
				}
			}
			Vector3[] array = new Vector3[componentsInChildren.Length];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].transform.childCount == 1)
				{
					array[j] = componentsInChildren[j].transform.InverseTransformDirection(componentsInChildren[j].transform.GetChild(0).position - componentsInChildren[j].transform.position);
				}
			}
			Rigidbody[] array2 = componentsInChildren;
			foreach (Rigidbody rigidbody in array2)
			{
				muscles = puppetMaster.muscles;
				foreach (Muscle muscle2 in muscles)
				{
					if (muscle2.joint.GetComponent<Rigidbody>() == rigidbody)
					{
						rigidbody.transform.position = muscle2.target.position;
					}
				}
			}
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				if (componentsInChildren[l].transform.childCount == 1)
				{
					Vector3 position = componentsInChildren[l].transform.GetChild(0).position;
					componentsInChildren[l].transform.rotation = Quaternion.FromToRotation(componentsInChildren[l].transform.rotation * array[l], position - componentsInChildren[l].transform.position) * componentsInChildren[l].transform.rotation;
					componentsInChildren[l].transform.GetChild(0).position = position;
				}
			}
		}

		public static void RealignRagdoll(PuppetMaster puppetMaster)
		{
			Muscle[] muscles = puppetMaster.muscles;
			foreach (Muscle muscle in muscles)
			{
				if (muscle.joint == null || muscle.joint.transform == null || muscle.target == null)
				{
					Debug.LogWarning("Muscles incomplete, can not realign ragdoll.");
					return;
				}
			}
			muscles = puppetMaster.muscles;
			foreach (Muscle muscle2 in muscles)
			{
				if (muscle2.target != null)
				{
					Transform[] array = new Transform[muscle2.joint.transform.childCount];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = muscle2.joint.transform.GetChild(j);
					}
					Transform[] array2 = array;
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k].parent = null;
					}
					BoxCollider component = muscle2.joint.GetComponent<BoxCollider>();
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = Vector3.zero;
					if (component != null)
					{
						vector = component.transform.TransformVector(component.size);
						vector2 = component.transform.TransformVector(component.center);
					}
					CapsuleCollider component2 = muscle2.joint.GetComponent<CapsuleCollider>();
					Vector3 vector3 = Vector3.zero;
					Vector3 direction = Vector3.zero;
					if (component2 != null)
					{
						vector3 = component2.transform.TransformVector(component2.center);
						direction = component2.transform.TransformVector(DirectionIntToVector3(component2.direction));
					}
					SphereCollider component3 = muscle2.joint.GetComponent<SphereCollider>();
					Vector3 vector4 = Vector3.zero;
					if (component3 != null)
					{
						vector4 = component3.transform.TransformVector(component3.center);
					}
					Vector3 vector5 = muscle2.joint.transform.TransformVector(muscle2.joint.axis);
					Vector3 vector6 = muscle2.joint.transform.TransformVector(muscle2.joint.secondaryAxis);
					muscle2.joint.transform.rotation = muscle2.target.rotation;
					if (component != null)
					{
						component.size = component.transform.InverseTransformVector(vector);
						component.center = component.transform.InverseTransformVector(vector2);
					}
					if (component2 != null)
					{
						component2.center = component2.transform.InverseTransformVector(vector3);
						Vector3 dir = component2.transform.InverseTransformDirection(direction);
						component2.direction = DirectionVector3ToInt(dir);
					}
					if (component3 != null)
					{
						component3.center = component3.transform.InverseTransformVector(vector4);
					}
					muscle2.joint.axis = muscle2.joint.transform.InverseTransformVector(vector5);
					muscle2.joint.secondaryAxis = muscle2.joint.transform.InverseTransformVector(vector6);
					array2 = array;
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k].parent = muscle2.joint.transform;
					}
				}
			}
		}

		private static Vector3 DirectionIntToVector3(int dir)
		{
			switch (dir)
			{
			case 0:
				return Vector3.right;
			case 1:
				return Vector3.up;
			default:
				return Vector3.forward;
			}
		}

		private static int DirectionVector3ToInt(Vector3 dir)
		{
			float f = Vector3.Dot(dir, Vector3.right);
			float f2 = Vector3.Dot(dir, Vector3.up);
			float f3 = Vector3.Dot(dir, Vector3.forward);
			float num = Mathf.Abs(f);
			float num2 = Mathf.Abs(f2);
			float num3 = Mathf.Abs(f3);
			int result = 0;
			if (num2 > num && num2 > num3)
			{
				result = 1;
			}
			if (num3 > num && num3 > num2)
			{
				result = 2;
			}
			return result;
		}
	}
}
