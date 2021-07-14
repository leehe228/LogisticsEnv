using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class WebGLHideMouse : MonoBehaviour
    {
        void Start()
        {
            Invoke("HideMouse", 0.5f);
        }

        void OnGUI()
        {
            if (EscapePressed()) {
                UnHideMouse();
            }
            if (PointerReleased()) {
                Invoke("HideMouse", 5f);
            }
        }

        private bool EscapePressed()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
        private bool PointerReleased()
        {
            return Input.GetKeyUp(KeyCode.Mouse0);
        }

        public void HideMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        public void UnHideMouse()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
