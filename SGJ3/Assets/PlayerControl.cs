using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerControl : MonoBehaviour
{
    Movement _movement;

    void Awake()
    {
        _movement = GetComponent<Movement>();
    }

    void Update()
    {
        float direction;
        if (Input.GetKey(KeyCode.A))
            direction = -1;
        else if (Input.GetKey(KeyCode.D))
            direction = 1;
        else
            direction = 0;

        _movement.SetInput(new MovementData()
        {
            Jump = Input.GetKey(KeyCode.Space),
            XMove = direction,
        });
    }
}
