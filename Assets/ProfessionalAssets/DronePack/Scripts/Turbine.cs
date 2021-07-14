using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsPlayground
{
    public class Turbine : MonoBehaviour
    {
        public bool active;
        public float power = 400;
        private Transform fanSwitch;
        private Transform motor;
        private Transform fan;
        private ParticleSystem particles;

        void Start()
        {
            CacheComponents();
        }

        void Update()
        {
            if(SwitchedOn()) {
                TurnOnFan();
            }

            if(SwitchedOff()) {
                TurnOffFan();
            }
        }

        void OnTriggerStay(Collider _other)
        {
            Rigidbody oRigidbody = _other.GetComponent<Rigidbody>();
            if (active) {
                if (oRigidbody) {
                    oRigidbody.AddForce(transform.forward * Mathf.Abs(power / Vector3.Distance(transform.position, _other.transform.position)), ForceMode.Force);
                }
            }
        }

        private bool SwitchedOn()
        {
            return fanSwitch.transform.localRotation.eulerAngles.y < 10;
        }
        private bool SwitchedOff()
        {
            return fanSwitch.transform.localRotation.eulerAngles.y > 70;
        }

        private void CacheComponents()
        {
            fanSwitch = transform.Find("FanSwitch");
            motor = transform.Find("Pole").transform.Find("Motor");
            fan = motor.transform.Find("Fan");
            particles = motor.GetComponent<ParticleSystem>();
        }

        public void TurnOnFan()
        {
            if (active) { return; }
            fanSwitch.GetComponent<MeshRenderer>().material.color = Color.green;
            fanSwitch.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(0, 0.5f, 0, 1));
            fan.GetComponent<HingeJoint>().useMotor = true;
            if (!particles.isPlaying) { particles.Play(); }
            active = true;
        }
        public void TurnOffFan()
        {
            if (!active) { return; }
            fanSwitch.GetComponent<MeshRenderer>().material.color = Color.red;
            fanSwitch.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(0.5f, 0, 0, 1));
            fan.GetComponent<HingeJoint>().useMotor = false;
            if (particles.isPlaying) { particles.Stop(); }
            active = false;
        }
    }
}
