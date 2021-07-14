using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class ProximityCannon : MonoBehaviour
    {
        public GameObject projectile = null;
        public float projectileLife = 5f;

        private Transform target = null;
        private Transform shaft = null;
        private Transform head = null;
        private Transform spawn = null;
        private AudioSource[] audioSources = new AudioSource[0];
        private float delay = 0;

        void Awake()
        {
            CacheComponents();
        }

        private void OnTriggerEnter(Collider _other)
        {
            PA_DroneController drone = _other.GetComponent<PA_DroneController>();
            if(drone) {
                audioSources[0].PlayOneShot(audioSources[0].clip, 1f);
                target = drone.transform;
                delay = Time.time + 3f;
            }
        }

        private void OnTriggerExit(Collider _other)
        {
            if (_other.transform == target) {
                target = null;
            }
        }

        void Update()
        {
            if (!target) { return; }
            LookAtTarget();
            FireAtTarget();
        }

        private void CacheComponents()
        {
            shaft = transform.Find("Base").Find("Shaft");
            head = shaft.Find("Head");
            spawn = head.Find("Spawn");
            audioSources = GetComponents<AudioSource>();
        }

        private void LookAtTarget()
        {
            Quaternion lookRot = Quaternion.LookRotation(target.transform.position - head.transform.position);
            shaft.rotation = Quaternion.Slerp(shaft.rotation, Quaternion.Euler(0, lookRot.eulerAngles.y, 0), Time.deltaTime * 10f);
            head.rotation = Quaternion.Slerp(head.rotation, Quaternion.Euler(lookRot.eulerAngles.x, head.rotation.eulerAngles.y, 0), Time.deltaTime * 5f);
        }

        private void FireAtTarget()
        {
            if (Time.time < delay) { return; }
            GameObject newProjectile = Instantiate(projectile, spawn.transform.position, Quaternion.identity);
            newProjectile.GetComponent<Rigidbody>().AddForce(spawn.transform.forward * 100, ForceMode.VelocityChange);
            audioSources[1].PlayOneShot(audioSources[1].clip, 1f);
            Destroy(newProjectile, projectileLife);
            delay = Time.time + 2f;
        }

        public void SwapCannonBall(GameObject newPrefab)
        {
            projectile = newPrefab;
        }
    }
}
