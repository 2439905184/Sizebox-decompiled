using UnityEngine;
using UnityEngine.Serialization;

[AddComponentMenu("SDK/Map Settings")]
public class MapSettings : MonoBehaviour
{
	[Tooltip("Maximum size of a Macro")]
	public float maxGtsSize = 1000f;

	[Tooltip("Minimum size of a Macro")]
	public float minGtsSize = 0.001f;

	[Tooltip("The default spawning size of a Macro")]
	public float gtsStartingScale = 1f;

	[Tooltip("Maximum size of a Micro")]
	public float maxPlayerSize = 1000f;

	[Tooltip("Minimum size of a Micro")]
	public float minPlayerSize = 0.001f;

	[Tooltip("The default spawning size of a Micro")]
	public float startingSize = 1f;

	[Tooltip("Scale ratio between a Unity and a meter")]
	public float scale = 1f;

	[Tooltip("Should fog be allowed to render on this map? You may want to disable fog if the map is a room interior or in space")]
	[FormerlySerializedAs("macro")]
	public bool enableFog;
}
