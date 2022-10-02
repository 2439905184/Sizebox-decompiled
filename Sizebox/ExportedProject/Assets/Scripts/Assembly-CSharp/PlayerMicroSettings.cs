using UnityEngine;

public class PlayerMicroSettings : MonoBehaviour
{
	[Header("Settings")]
	[Header("Walking")]
	public float baseSpeed = 4f;

	public float walkSpeed = 0.3f;

	public float runSpeed = 1f;

	public float sprintSpeed = 2f;

	public float movementSmoothing = 4f;

	public float turnSmoothing = 4f;

	[Header("Jumping")]
	public float jumpHeight = 7f;

	public float jumpCooldown = 1f;

	public float airControlMult = 0.1f;

	public float splatLength = 3.5f;

	public float rollingSpeed = 2.5f;

	public bool isSplatEnabled = true;

	public float controlRestoreDelay = 0.5f;

	[Header("Flying")]
	public float flySpeed = 12f;

	public float superSpeed = 500f;

	public float flySprintMultiplier = 3f;

	public AnimationCurve flyingSpeedSmoothingCurve = new AnimationCurve();

	public float flyingRotationSmoothing = 6f;

	[Space]
	public float floatingPunchVelocity = 4f;

	[Header("Climbing")]
	public float climbSpeed = 4f;

	public float climbSprintMultiplier = 1.35f;

	[Space]
	public float climbTurnSpeed = 1.5f;

	public float climbRotationSmooth = 4f;

	public float alignWithSurfaceRotationSmooth = 0.3f;

	[Space]
	public float climbingGravityScale = 1.5f;

	public float climbingGravitySmoothing = 0.8f;

	[Space]
	public int numberOfBaseClimbingRayCasts = 4;

	public int numberOfForwardClimbingRayCasts = 4;

	public float baseClimbingRaycastRadius = 0.65f;

	public float baseClimbingRaycastLength = 1f;

	[Space]
	public float climbingAnimationSpeed = 2f;

	public AnimationCurve climbingGroundingCurve = new AnimationCurve();

	[Header("Aiming")]
	public float baseZeroing = 100f;

	public float maxAimAngle = 105f;

	public float responseAngle = 150f;

	[Header("Combat")]
	public float punchingPower = 8f;

	[Header("Animation")]
	public float walkAnimationSpeed = 1f;

	public float runAnimationSpeed = 2.5f;

	public float sprintAnimationSpeed = 4f;

	public float speedDampTime = 0.4f;

	public float rollLength = 0.7f;

	[Space]
	public float jumpBoolTime = 0.15f;

	public float hardLandingLength = 3.5f;

	public float fallDownLength = 2f;

	public float standUpLength = 1f;

	public float splatStandUpLength = 2.5f;

	public float flyingPunchStartLength = 0.3f;

	public float flyingPunchEndLength = 0.35f;

	public float force = 50f;

	public float maxSpeed = 8f;

	public AnimationCurve groundingCurve = new AnimationCurve();

	public float rollFallDistanceThreshold = 7.5f;

	public float rollVelocityThreshold = 12.5f;

	public float splatFallDistanceThreshold = 37.5f;

	public float splatVelocityThreshold = 55f;

	public float fallOverAngleThreshold = 45f;

	public float fallOverVelocityThreshold = 0.75f;
}
