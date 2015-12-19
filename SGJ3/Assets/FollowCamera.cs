using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraShakes
{
    struct CameraShakeData
    {
        public CameraShakeSettings Settings;
        public Vector2 Direction;
        public float StartTime;
        public float Force;
    }

    [NonSerialized]
    List<CameraShakeData> _ActiveShakes = new List<CameraShakeData>();

    public void Shake(CameraShakeSettings settings, float force, Vector2 direction = default(Vector2))
    {
        CameraShakeData data;
        data.Force = force;
        data.Settings = settings;
        data.StartTime = Time.unscaledTime;
        while (direction == Vector2.zero)
            direction = UnityEngine.Random.insideUnitCircle;
        direction.Normalize();
        data.Direction = direction;
        _ActiveShakes.Add(data);
    }

    public Vector2 GetOffset()
    {
        Vector2 offset = Vector2.zero;
        for (int i = _ActiveShakes.Count - 1; i >= 0; i--)
        {
            var shake = _ActiveShakes[i];
            float time = Time.unscaledTime - shake.StartTime;
            float duration = shake.Settings.LifetimeCurve.Evaluate(shake.Force);
            if (time > duration)
            {
                _ActiveShakes.RemoveAt(i);
                continue;
            }
            float t = time / duration;
            float angle = Mathf.Atan2(shake.Direction.y, shake.Direction.x);
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg));
            Vector2 add = new Vector2(shake.Settings.XCurve.Evaluate(t), shake.Settings.YCurve.Evaluate(t)) * shake.Force;
            add = (Vector2)(rot * (Vector3)add);
            offset += add;
        }
        return offset;
    }
}

public class FollowCamera : MonoBehaviour
{
    public BoxCollider2D ConstraintBox;
    public CameraShakes Shake;
    public Rect MoveRect;
    public float LerpFactor;
    public Transform Target;
    public CameraShakeSettings Test;
    public float MinForce;
    public float MaxForce;


    public float YDistanceFactor;
    public float XDistanceFactor;

    private Vector2 _LookAt;
    private Camera _Camera;

    void Awake()
    {
        _Camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector2 size = ConstraintBox.size;
        float ySize = _Camera.orthographicSize;
        float xSize = ySize * _Camera.aspect;
        size -= new Vector2(xSize, ySize) * 2f;
        Vector2 offset = ConstraintBox.offset + (Vector2)ConstraintBox.transform.position - size * 0.5f;
        Rect constraintRect = new Rect(offset, size);




        if (Input.GetKeyDown(KeyCode.K))
            Shake.Shake(Test, UnityEngine.Random.Range(MinForce, MaxForce), Vector2.left);
        Vector2 pos = Target.position;
        Rect rect = new Rect(_LookAt, MoveRect.size);
        rect.center += MoveRect.position;
        float xMod = 0f;
        if (pos.x < rect.xMin)
            xMod = Mathf.Abs(rect.xMin - pos.x);
        if (pos.x > rect.xMax)
            xMod = Mathf.Abs(pos.x - rect.xMax);

        xMod = Mathf.Clamp01(xMod * XDistanceFactor);

        float yMod = 0f;
        if (pos.y < rect.yMin)
            yMod = Mathf.Abs(rect.yMin - pos.y);
        if (pos.y > rect.yMax)
            yMod = Mathf.Abs(pos.y - rect.yMax);

        yMod = Mathf.Clamp01(yMod * YDistanceFactor);

        Vector2 newPos;
        newPos.x = Mathf.Lerp(_LookAt.x, pos.x, LerpFactor * xMod * Time.deltaTime);
        newPos.y = Mathf.Lerp(_LookAt.y, pos.y, LerpFactor * yMod * Time.deltaTime);

        newPos.x = Mathf.Clamp(newPos.x, constraintRect.xMin, constraintRect.xMax);
        newPos.y = Mathf.Clamp(newPos.y, constraintRect.yMin, constraintRect.yMax);

        Vector2 screenShake = Shake.GetOffset();
        if (newPos != _LookAt || screenShake != Vector2.zero)
        {
            Vector2 lookPos = newPos + screenShake;
            transform.position = new Vector3(lookPos.x, lookPos.y, transform.position.z);
            _LookAt = newPos;
        }
    }
}
