using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NXZ
{
    public class ParticleEnter : MonoBehaviour
    {
        public static event Action<int> EGetVP;

        public float flyspeed = 1;
        public int VPvalue = 80;
        private Transform player;
        Vector3 offset = new Vector3(0, 1, 0);
        // Start is called before the first frame update
        void Start()
        {
            player = FindObjectOfType<NXZPlayerState>().transform;
        }


        void OnParticleTrigger()
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();

            // particles
            List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

            // get
            int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);

            // iterate
            for (int i = 0; i < numInside; i++)
            {
                ParticleSystem.Particle p = inside[i];
                if ((player.position + offset - p.position).sqrMagnitude < 1f)
                {
                    p.remainingLifetime = 0;
                    inside[i] = p;
                    if (EGetVP != null)
                    {
                        EGetVP.Invoke(VPvalue);
                    }
                }
                else
                {
                    p.velocity = flyspeed *(player.position + offset - p.position).normalized;
                    inside[i] = p;
                }
            }
            // set
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        }
    }
}

