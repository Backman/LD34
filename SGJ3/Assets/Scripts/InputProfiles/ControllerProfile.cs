using UnityEngine;
using System.Collections;
using InControl;

public class ControllerProfile : UnityInputDeviceProfile
{
    public ControllerProfile()
    {
        Name = "Controller";
        Meta = "Controller profile";

        SupportedPlatforms = new[]
        {
			"Windows",
			"Mac",
			"Linux"
        };

        Sensitivity = 1.0f;
        LowerDeadZone = 0.0f;
        UpperDeadZone = 1.0f;

        ButtonMappings = new[]
        {
            new InputControlMapping
            {
                Handle = "Jump",
                Target = InputControlType.Action1,
                Source = KeyCodeButton( KeyCode.Space )
            },
			new InputControlMapping
            {
				Handle = "Attack",
				Target = InputControlType.Action2,
				Source = KeyCodeButton (KeyCode.E, KeyCode.Mouse0)
            }
        };

        AnalogMappings = new[]
        {
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX
			},
			new InputControlMapping
			{
				Handle = "Aim Y",
				Target = InputControlType.RightStickY
            },
			new InputControlMapping
			{
				Handle = "Aim X",
				Target = InputControlType.RightStickX
            }
        };
    }
}
