using UltimateFracturing;
using UnityEngine;

public class UltimateFracturingFPS : MonoBehaviour
{
	public enum Mode
	{
		ShootObjects = 0,
		ExplodeRaycast = 1
	}

	public Mode ShootMode = Mode.ExplodeRaycast;

	public float MouseSpeed = 0.3f;

	public Texture HUDTexture;

	public float HUDSize = 0.03f;

	public Color HUDColorNormal;

	public Color HUDColorRaycast;

	public Transform Weapon;

	public AudioClip AudioWeaponShot;

	public float WeaponShotVolume = 1f;

	public float ExplosionForce = 1f;

	public float ExplosionRadius = 0.4f;

	public float RecoilDuration = 0.2f;

	public float RecoilIntensity = 0.05f;

	public GameObject ObjectToShoot;

	public float InitialObjectSpeed = 1f;

	public float ObjectScale = 1f;

	public float ObjectMass = 1f;

	public float ObjectLife = 10f;

	private Vector3 m_v3MousePosition;

	private bool m_bRaycastFound;

	private float m_fRecoilTimer;

	private Vector3 m_v3InitialWeaponPos;

	private Quaternion m_qInitialWeaponRot;

	private void Start()
	{
		m_v3MousePosition = Input.mousePosition;
		m_bRaycastFound = false;
		m_fRecoilTimer = 0f;
		if ((bool)Weapon)
		{
			m_v3InitialWeaponPos = Weapon.localPosition;
			m_qInitialWeaponRot = Weapon.localRotation;
		}
	}

	private void OnGUI()
	{
		Color color = GUI.color;
		if (ShootMode == Mode.ExplodeRaycast)
		{
			int num = Mathf.RoundToInt((float)Screen.width * HUDSize * 0.5f);
			Rect position = new Rect(Screen.width / 2 - num, Screen.height / 2 - num, num * 2, num * 2);
			GUI.color = (m_bRaycastFound ? HUDColorRaycast : HUDColorNormal);
			GUI.DrawTexture(position, HUDTexture, ScaleMode.StretchToFill, true);
			GUI.color = color;
		}
		GUI.color = color;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W))
		{
			ShootMode = ((ShootMode != Mode.ExplodeRaycast) ? Mode.ExplodeRaycast : Mode.ShootObjects);
		}
		if (ObjectToShoot != null && ShootMode == Mode.ShootObjects)
		{
			if ((bool)Weapon)
			{
				Weapon.GetComponent<Renderer>().enabled = false;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				GameObject obj = Object.Instantiate(ObjectToShoot);
				obj.transform.position = base.transform.position;
				obj.transform.localScale = new Vector3(ObjectScale, ObjectScale, ObjectScale);
				obj.GetComponent<Rigidbody>().mass = ObjectMass;
				obj.GetComponent<Rigidbody>().solverIterations = 255;
				obj.GetComponent<Rigidbody>().AddForce(base.transform.forward * InitialObjectSpeed, ForceMode.VelocityChange);
				obj.AddComponent<DieTimer>().SecondsToDie = ObjectLife;
			}
		}
		if (ShootMode == Mode.ExplodeRaycast)
		{
			if ((bool)Weapon)
			{
				Weapon.GetComponent<Renderer>().enabled = true;
			}
			bool keyDown = Input.GetKeyDown(KeyCode.Space);
			if (keyDown)
			{
				m_fRecoilTimer = RecoilDuration;
				if ((bool)AudioWeaponShot)
				{
					AudioSource.PlayClipAtPoint(AudioWeaponShot, base.transform.position, WeaponShotVolume);
				}
			}
			m_bRaycastFound = false;
			RaycastHit hitInfo;
			FracturedChunk fracturedChunk = FracturedChunk.ChunkRaycast(base.transform.position, base.transform.forward, out hitInfo);
			if ((bool)fracturedChunk)
			{
				m_bRaycastFound = true;
				if (keyDown)
				{
					fracturedChunk.Impact(hitInfo.point, ExplosionForce, ExplosionRadius, true);
				}
			}
		}
		if (m_fRecoilTimer > 0f)
		{
			if ((bool)Weapon)
			{
				Weapon.transform.localPosition = m_v3InitialWeaponPos + new Vector3(0f, 0f, (0f - m_fRecoilTimer) / RecoilDuration * RecoilIntensity);
				Weapon.transform.localRotation = m_qInitialWeaponRot * Quaternion.Euler(new Vector3(m_fRecoilTimer / RecoilDuration * 360f * RecoilIntensity * 0.1f, 0f, 0f));
			}
			m_fRecoilTimer -= Time.deltaTime;
		}
		else if ((bool)Weapon)
		{
			Weapon.transform.localPosition = m_v3InitialWeaponPos;
			Weapon.transform.localRotation = m_qInitialWeaponRot;
		}
		if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
		{
			base.transform.Rotate((0f - (Input.mousePosition.y - m_v3MousePosition.y)) * MouseSpeed, 0f, 0f);
			base.transform.RotateAround(base.transform.position, Vector3.up, (Input.mousePosition.x - m_v3MousePosition.x) * MouseSpeed);
		}
		m_v3MousePosition = Input.mousePosition;
	}
}
