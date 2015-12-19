using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class MovementSettings : ScriptableObject
{
    public AnimationCurve Deceleration;
    public float DecelerationScale;
    public float DecelerationCurveMax;
    public AnimationCurve Acceleration;
    public float AccelerationScale;
    public float MaxSpeed;
    public float MinJumpForce;
    public float MaxJumpForce;
    public float DownGravity;
    public float UpGravity;
    public float JumpCooldown = 0.1f;
    public float JumpDelay = 0.20f;
    public float MinLandingDelay = 0.05f;
    public float MaxLandingDelay = 0.05f;
    public float MinFallImpact = 10f;
    public float MaxFallImpact = 18f;
    public float FallImpactForce = 18f;
    public float GroundingCheckDistance = 0.2f;

}
