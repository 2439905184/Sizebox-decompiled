using UnityEngine;

public class Layers : MonoBehaviour
{
	public static int defaultLayer;

	public static int mapLayer;

	public static int gtsBodyLayer;

	public static int playerLayer;

	public static int microLayer;

	public static int objectLayer;

	public static int buildingLayer;

	public static int uiLayer;

	public static int gtsCapsuleLayer;

	public static int auxLayer;

	public static int destroyerLayer;

	public static int vehicelsLayer;

	public static int ignoreWheelcastLayer;

	public static int detachableLayer;

	public static int cloudProximityLayer;

	public static int debrisLayer;

	public static int debrisSimpleLayer;

	public static LayerMask mapMask;

	public static LayerMask gtsBodyMask;

	public static LayerMask gtsCapsuleMask;

	public static LayerMask buildingMask;

	public static LayerMask auxMask;

	public static LayerMask placementMask;

	public static LayerMask ctrlMask;

	public static LayerMask microCameraCollisionMask;

	public static LayerMask giantessCameraCollisionMask;

	public static LayerMask vehicleCameraCollisionMask;

	public static LayerMask gtsCollisionCheckMask;

	public static LayerMask walkableMask;

	public static LayerMask crushableMask;

	public static LayerMask gtsWalkableMask;

	public static LayerMask actionSelectionMask;

	public static LayerMask visibilityMask;

	public static LayerMask pathfindingMask;

	public static LayerMask reflectionMask;

	public static LayerMask stompingMask;

	public static LayerMask raygunAimMask;

	private static bool initialized;

	public static void Initialize()
	{
		if (!initialized)
		{
			string text = "Default";
			string text2 = "Map";
			string text3 = "GTSBody";
			string text4 = "Player";
			string text5 = "Micro";
			string text6 = "Object";
			string text7 = "Building";
			string text8 = "UI";
			string text9 = "Aux";
			string text10 = "GTSCapsule";
			string text11 = "Vehicles";
			string layerName = "Ignore Wheel Cast";
			string layerName2 = "Detachable Part";
			string layerName3 = "CloudProximity";
			string text12 = "Debris";
			string layerName4 = "DebrisSimple";
			defaultLayer = LayerMask.NameToLayer(text);
			mapLayer = LayerMask.NameToLayer(text2);
			gtsBodyLayer = LayerMask.NameToLayer(text3);
			playerLayer = LayerMask.NameToLayer(text4);
			microLayer = LayerMask.NameToLayer(text5);
			objectLayer = LayerMask.NameToLayer(text6);
			buildingLayer = LayerMask.NameToLayer(text7);
			uiLayer = LayerMask.NameToLayer(text8);
			gtsCapsuleLayer = LayerMask.NameToLayer(text10);
			auxLayer = LayerMask.NameToLayer(text9);
			destroyerLayer = LayerMask.NameToLayer("Destroyer");
			vehicelsLayer = LayerMask.NameToLayer(text11);
			ignoreWheelcastLayer = LayerMask.NameToLayer(layerName);
			detachableLayer = LayerMask.NameToLayer(layerName2);
			cloudProximityLayer = LayerMask.NameToLayer(layerName3);
			debrisLayer = LayerMask.NameToLayer(text12);
			debrisSimpleLayer = LayerMask.NameToLayer(layerName4);
			mapMask = LayerMask.GetMask(text2);
			gtsBodyMask = LayerMask.GetMask(text3);
			gtsCapsuleMask = LayerMask.GetMask(text10);
			buildingMask = LayerMask.GetMask(text7);
			auxMask = LayerMask.GetMask(text9);
			placementMask = LayerMask.GetMask(text, text2, text3, text6);
			ctrlMask = LayerMask.GetMask(text, text2, text3, text6, text5, text11);
			microCameraCollisionMask = LayerMask.GetMask(text, text2, text3, text6, text7);
			giantessCameraCollisionMask = LayerMask.GetMask(text, text2);
			vehicleCameraCollisionMask = LayerMask.GetMask(text, text2, text3, text7, text6);
			gtsCollisionCheckMask = LayerMask.GetMask(text2, text6, text3, text);
			walkableMask = LayerMask.GetMask(text, text2, text3, text6, text7, text12);
			gtsWalkableMask = LayerMask.GetMask(text2, text6, text);
			crushableMask = LayerMask.GetMask(text4, text5);
			actionSelectionMask = LayerMask.GetMask(text, text2, text3, text4, text5, text6, text7, text8, text11);
			visibilityMask = LayerMask.GetMask(text2, text6, text5, text4, text10, text7, text);
			pathfindingMask = LayerMask.GetMask(text2, text6, text7, text);
			reflectionMask = LayerMask.GetMask(text2, text, text4, text3, text7, text6);
			stompingMask = LayerMask.GetMask(text2, text, text6);
			raygunAimMask = LayerMask.GetMask(text5, text3, text6, text2);
			initialized = true;
		}
	}
}
