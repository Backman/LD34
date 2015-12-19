using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class CameraShakeSettings : ScriptableObject
{
    public AnimationCurve XCurve;
    public AnimationCurve YCurve;
    public AnimationCurve LifetimeCurve;
}
