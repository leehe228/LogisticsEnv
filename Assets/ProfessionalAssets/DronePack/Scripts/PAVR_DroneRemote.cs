using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace PA_DronePack
{
    public class PAVR_DroneRemote : MonoBehaviour
    {
        List<XRNodeState> nodes = new List<XRNodeState>();
        Vector3 rightHandPos;
        Quaternion rightHandRot;

        void Update()
        {
            InputTracking.GetNodeStates(nodes);
            foreach (XRNodeState node in nodes)
            {
                if (node.nodeType == XRNode.RightHand)
                {
                    node.TryGetPosition(out rightHandPos);
                    node.TryGetRotation(out rightHandRot);
                }
            }
            transform.localPosition = rightHandPos;
            transform.localRotation = rightHandRot;
        }
    }
}