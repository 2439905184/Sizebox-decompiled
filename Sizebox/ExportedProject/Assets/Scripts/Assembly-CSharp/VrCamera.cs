using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class VrCamera : MonoBehaviour
{
	private enum Mode
	{
		None = 0,
		Split = 1,
		Stereo = 2,
		OpenVR = 3,
		Oculus = 4,
		Unknown = 5
	}

	private Mode currentMode;

	public bool vrSupported;

	public GameObject VRcamera;

	public GameObject farCamera;

	public XRRig xrRig;

	private void Start()
	{
		if (!VRcamera)
		{
			VRcamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		if ((bool)VRcamera && !farCamera)
		{
			farCamera = GameObject.Find("Far Camera");
		}
		vrSupported = true;
	}

	private IEnumerator LoadDevice(string newDevice)
	{
		XRSettings.LoadDeviceByName(newDevice);
		yield return null;
		XRSettings.enabled = true;
		yield return null;
	}

	private void EnableMode(bool on, string modeName, Mode mode = Mode.Unknown)
	{
		if (on && (!XRSettings.enabled || currentMode != mode))
		{
			currentMode = mode;
			StartCoroutine(LoadDevice(modeName));
		}
		else if (!on && mode == currentMode)
		{
			XRSettings.LoadDeviceByName("");
		}
		if ((bool)xrRig)
		{
			xrRig.enabled = on;
		}
	}

	public void ReparentFarCamera(bool value)
	{
		if (VRcamera != null && farCamera != null)
		{
			if (value)
			{
				farCamera.transform.parent = VRcamera.transform.parent;
			}
			if (!value)
			{
				farCamera.transform.parent = VRcamera.transform;
			}
		}
		else
		{
			Debug.LogError("Missing camera refrence!");
		}
	}

	public void SelectXR(int sel)
	{
		string text = XRSettings.supportedDevices[sel];
		switch (text)
		{
		case "None":
			EnableMode(false, "None", currentMode);
			ReparentFarCamera(false);
			break;
		case "split":
			EnableSplit(true);
			break;
		case "stereo":
			EnableStereo(true);
			break;
		case "Oculus":
			EnableOculus(true);
			break;
		case "OpenVR":
			EnableOpenVR(true);
			break;
		default:
			EnableMode(true, text);
			ReparentFarCamera(true);
			break;
		}
	}

	public void EnableSplit(bool on)
	{
		EnableMode(on, "Split", Mode.Split);
		ReparentFarCamera(on);
	}

	public void EnableStereo(bool on)
	{
		EnableMode(on, "Stereo", Mode.Stereo);
	}

	public void EnableOculus(bool on)
	{
		EnableMode(on, "Oculus", Mode.Oculus);
		ReparentFarCamera(on);
	}

	public void EnableOpenVR(bool on)
	{
		EnableMode(on, "OpenVR", Mode.OpenVR);
		ReparentFarCamera(on);
	}

	public bool GetSplit()
	{
		if (XRSettings.enabled)
		{
			return currentMode == Mode.Split;
		}
		return false;
	}

	public bool GetStereo()
	{
		if (XRSettings.enabled)
		{
			return currentMode == Mode.Stereo;
		}
		return false;
	}

	public bool GetOpenVR()
	{
		if (XRSettings.enabled)
		{
			return currentMode == Mode.OpenVR;
		}
		return false;
	}

	public bool GetOculus()
	{
		if (XRSettings.enabled)
		{
			return currentMode == Mode.Oculus;
		}
		return false;
	}

	public bool IsInVR()
	{
		if (currentMode != Mode.OpenVR)
		{
			return currentMode == Mode.Oculus;
		}
		return true;
	}
}
