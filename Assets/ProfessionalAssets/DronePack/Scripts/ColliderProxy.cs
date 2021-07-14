using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsPlayground
{
    public class ColliderProxy : MonoBehaviour {

        public GameObject host;

        void OnTriggerEnter(Collider col) { host.SendMessage("OnTriggerEnter", col, SendMessageOptions.DontRequireReceiver); }
        void OnTriggerStay(Collider col) { host.SendMessage("OnTriggerStay", col, SendMessageOptions.DontRequireReceiver); }
        void OnTriggerExit(Collider col) { host.SendMessage("OnTriggerExit", col, SendMessageOptions.DontRequireReceiver); }

        void OnCollisionEnter(Collision col) { host.SendMessage("OnCollisionEnter", col, SendMessageOptions.DontRequireReceiver); }
        void OnCollisionStay(Collision col) { host.SendMessage("OnCollisionStay", col, SendMessageOptions.DontRequireReceiver); }
        void OnCollisionExit(Collision col) { host.SendMessage("OnCollisionExit", col, SendMessageOptions.DontRequireReceiver); }
    }
}
