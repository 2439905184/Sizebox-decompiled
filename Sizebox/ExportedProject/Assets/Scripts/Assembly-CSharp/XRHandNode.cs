using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

public class XRHandNode : MonoBehaviour
{
	public XRNode nodeType;

	public GameObject containerToDisable;

	[Tooltip("If left null, will attempt to use GetComponent on self")]
	public TrackedPoseDriver tpd;

	private Transform m_Transform;

	private void Start()
	{
		if (tpd == null)
		{
			tpd = GetComponent<TrackedPoseDriver>();
		}
		m_Transform = GetComponent<Transform>();
	}

	private void Update()
	{
		ShowOrHide();
		if (XRSettings.loadedDeviceName == "daydream")
		{
			tpd.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
			m_Transform.localPosition = new Vector3(0f, -1f, 0.5f);
		}
		else
		{
			tpd.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
		}
	}

	private void ShowOrHide()
	{
		List<XRNodeState> list = new List<XRNodeState>();
		InputTracking.GetNodeStates(list);
		bool active = false;
		foreach (XRNodeState item in list)
		{
			if (item.nodeType == nodeType)
			{
				active = true;
			}
		}
		containerToDisable.SetActive(active);
	}
}
