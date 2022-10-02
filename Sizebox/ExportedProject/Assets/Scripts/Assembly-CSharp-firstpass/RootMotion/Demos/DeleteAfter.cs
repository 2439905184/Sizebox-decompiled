using System.Collections;
using UnityEngine;

namespace RootMotion.Demos
{
	public class DeleteAfter : MonoBehaviour
	{
		public float delay = 5f;

		private void Start()
		{
			StartCoroutine(Destruct());
		}

		private IEnumerator Destruct()
		{
			yield return new WaitForSeconds(delay);
			Object.Destroy(base.gameObject);
		}
	}
}
