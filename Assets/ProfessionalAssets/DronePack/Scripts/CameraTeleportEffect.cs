using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class CameraTeleportEffect : MonoBehaviour
    {
        public List<ParticleSystem> particleEffects;

        void Start()
        {
            StartEffects();
            StopEffects();
        }

        public void StartEffects()
        {
            foreach (ParticleSystem ps in particleEffects) { if (!ps.isPlaying) { ps.Play(); } }
        }

        public void StopEffects()
        {
            foreach (ParticleSystem ps in particleEffects) { if (ps.isPlaying) { ps.Stop(); } }
        }
    }
}
