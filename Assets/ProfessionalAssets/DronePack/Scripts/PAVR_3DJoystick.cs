using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PA_DronePack
{
    // you can enable/disable this script when gripping down on the helicopters yoke
    public class PAVR_3DJoystick : MonoBehaviour
    {
        #region Classes
        [System.Serializable]
        public class PA_ValueEvent : UnityEvent<float> { };
        #endregion

        // use this transorm's position as the cyclic handle
        public Transform handle;

        // set how far the handle needs to go before reaching the edge.
        public float radius = 0.08f;

        // these unity events control what each axis of the cyclic does, as default the z axis should control forward/backward movment, and the x axis will turn/rotate the drone.
        public PA_ValueEvent cyclicZAxis;
        public PA_ValueEvent cyclicXAxis;

        // holds the actual data we send with the unity events
        [HideInInspector]
        public float zInput, xInput;

        void Update()
        {
            // caluclate how far Ahead/Behind our handle is from the base relative to set radius and current direction (clamped between -1 and +1 to mimic input axis)
            zInput = Mathf.Clamp(Vector3.Dot(transform.forward, handle.position - transform.position) / radius, -1, 1);
            // caluclate how far Right/left our handle is from the base relative to set radius and current direction (clamped between -1 and +1 to mimic input axis)
            xInput = Mathf.Clamp(Vector3.Dot(transform.right, handle.position - transform.position) / radius, -1, 1);

            // activate the unity events and send input data along with it
            cyclicZAxis.Invoke(zInput);
            cyclicXAxis.Invoke(xInput);
        }

        // draws lines/circles to help with setup (you can remove this entire function if you don't need it)
        void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, radius);
            #endif
            Gizmos.DrawLine(transform.position, handle.position);
        }
    }
}