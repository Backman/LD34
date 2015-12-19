using UnityEngine;
using System.Collections;
using InControl;

public class CharacterInput : MonoBehaviour
{
    private InputDevice _inputDevice;
	
    private WaterCanon _cannon;
    private Movement _movement;

    private void Start()
    {
        _inputDevice = InputManager.ActiveDevice;
        _movement = GetComponent<Movement>();
        _cannon = GetComponent<WaterCanon>();
    }

    private void Update()
    {
        var move = _inputDevice.LeftStickX;
        var jump = _inputDevice.Action1.IsPressed;
        var attack = _inputDevice.Action2.IsPressed;
        var rake = _inputDevice.Action3.WasPressed;

        var movementData = new MovementData() {
			XMove = move,
			Jump = jump
        };
		
        _movement.SetInput(movementData);

        if (attack)
        {
            var mousePos = Input.mousePosition;
            var camera = Camera.main;
            mousePos = camera.ScreenToWorldPoint(mousePos);
            var dir = mousePos - transform.position;
        }
        if (rake)
        {
            
        }
    }
}
