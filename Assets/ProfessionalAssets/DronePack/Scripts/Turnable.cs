using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnable : MonoBehaviour {

    private void Update()
    {
        if (MouseInputReceived())
        {
            RotateDronesUsingMouse();
        }
        if (GamepadInputReceived())
        {
            RotateDronesUsingGamepad();
        }
    }

    private bool MouseInputReceived()
    {
        return Input.GetMouseButton(0);
    }
    private bool GamepadInputReceived()
    {
        return Input.GetAxisRaw("GP SecondaryJoystick X") != 0;
    }

    private void RotateDronesUsingMouse()
    {
        transform.Rotate(0, -Input.GetAxis("Mouse X") * 10f, 0);
    }
    private void RotateDronesUsingGamepad()
    {
        transform.Rotate(0, -Input.GetAxis("GP SecondaryJoystick X") * 10f, 0);
    }
}
