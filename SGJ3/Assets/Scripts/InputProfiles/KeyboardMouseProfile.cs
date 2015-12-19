using UnityEngine;
using System.Collections;
using InControl;

public class KeyboardMouseProfile : UnityInputDeviceProfile
{
    public KeyboardMouseProfile()
    {
        Name = "Keyboard/Mouse";
        Meta = "Keyboard and mouse profile";

        // This profile only works on desktops.
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
				Handle = "Water Canon Attack",
				Target = InputControlType.Action2,
				Source = KeyCodeButton (KeyCode.E, KeyCode.Mouse1)
            },
			new InputControlMapping
            {
				Handle = "Rake Attack",
				Target = InputControlType.Action3,
				Source = KeyCodeButton (KeyCode.Q, KeyCode.Mouse0)
            }
        };

        AnalogMappings = new[]
        {
			new InputControlMapping
			{
				Handle = "Move X",
				Target = InputControlType.LeftStickX,
				// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
				Source = KeyCodeAxis( KeyCode.A, KeyCode.D )
			},
			new InputControlMapping
			{
				Handle = "Move X Alternate",
				Target = InputControlType.LeftStickX,
				Source = KeyCodeAxis( KeyCode.LeftArrow, KeyCode.RightArrow )
			},
			new InputControlMapping
			{
				Handle = "Mouse Y",
				Target = InputControlType.RightStickY,
				Source = MouseYAxis,
				Raw    = true,
				Scale  = 0.1f
            },
			new InputControlMapping
			{
				Handle = "Mouse X",
				Target = InputControlType.RightStickX,
				Source = MouseXAxis,
				Raw    = true,
				Scale  = 0.1f
            }
        };
    }
}