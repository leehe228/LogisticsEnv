using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class NightLights : MonoBehaviour
    {
        public List<Material> materials;
        public List<Light> lights;

        void Awake()
        {
            LightsOff();
        }

        void OnApplicationQuit()
        {
            LightsOff();
        }

        public void LightsOn()
        {
            foreach (Material mat in materials) { mat.EnableKeyword("_EMISSION"); }
            foreach (Light light in lights) { light.enabled = true; }
        }
        public void LightsOff()
        {
            foreach (Material mat in materials) { mat.DisableKeyword("_EMISSION"); }
            foreach (Light light in lights) { light.enabled = false; }
        }
    }
}
