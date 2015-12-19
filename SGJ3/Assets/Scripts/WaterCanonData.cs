using UnityEngine;

[CreateAssetMenu]
public class WaterCanonData : ScriptableObject
{
    public WaterBallon BallonPrefab;
    public int StartAmmoCount;
    public int MaxAmmoCount;
    public float AimAngle = 60;
    public float Knockback;
    public float HitKnockback;
    public float StunTime;
    public float AttackForce;
    public float ShakeForce;
}