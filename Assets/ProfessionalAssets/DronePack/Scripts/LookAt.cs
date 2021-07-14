using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class LookAt : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            transform.LookAt(target);
        }
    }
}
