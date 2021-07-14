using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class EnviromentLighting : MonoBehaviour
    {
        public Material skybox;
        [Range(0, 2)]
        public float brightness;
        public Color skyColor = Color.black;
        public Color equatorColor = Color.black;
        public Color groundColor = Color.black;
        public bool fog;
        public Color fogColor = Color.white;

        void OnEnable()
        {
            UpdateLighting();   
        }

        public void UpdateLighting()
        {
            RenderSettings.skybox = skybox;
            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;
            RenderSettings.fogColor = fogColor;
        }
    }
}
