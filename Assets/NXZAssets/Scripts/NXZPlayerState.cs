using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NXZ
{
    public class NXZPlayerState : MonoBehaviour
    {
        [SerializeField]
        private int hp = 100, maxHP = 100;
        [SerializeField]
        private int vp = 1000, maxVP = 1000;
        [SerializeField]
        private float heartBeatTimer = 1;

        public List<NXZVirusCircle> virusCircles;

        [SerializeField]
        private float vpConsumeRate = 0;
        [SerializeField]
        private float vpProtect = 0;

        private bool ifInSafeCircle = false;

        public NXZPlayerInputController inputController;

        public int HP
        {
            get => hp;
            private set => hp = value;
        }
        public int MaxHP
        {
            get => maxHP;
            private set => maxHP = value;
        }
        public int VP
        {
            get => vp;
            private set => vp = value;
        }
        public int MaxVP
        {
            get => maxVP;
            private set => maxVP = value;
        }
        public float HeartBeatTimer
        {
            get => heartBeatTimer;
            set => heartBeatTimer = Mathf.Clamp(value,0.01f,10);
        }
        public float VpConsumeRate
        {
            get => vpConsumeRate;
            set => vpConsumeRate = value;
        }
        public float VpProtect
        {
            get => vpProtect;
            set => vpProtect = value;
        }
        public bool IfInSafeCircle
        {
            get => ifInSafeCircle;
            set => ifInSafeCircle = value;
        }

        public void AddHP(int add)
        {
            hp = Mathf.Clamp(hp + add, 0, maxHP);
        }

        public void AddVP(int add)
        {
            vp = Mathf.Clamp(vp + add, 0, maxVP);
        }

        public void HarmHP(int harm)
        {
            hp = Mathf.Clamp(hp - harm, 0, maxHP);
        }

        public void ConsumeVP(int consume)
        {
            if (vp > 0)
            {
                vp = Mathf.Clamp(vp - consume, 0, maxVP);
            }
            else
            {
                if (!IfInSafeCircle)
                {
                    HarmHP(consume);
                }
            }
        }

        public void SetMaxVP(int maxVP)
        {
            maxVP = Mathf.Clamp(maxVP, 0, 1000000);
        }

        void Start()
        {
            ParticleEnter.EGetVP += AddVP;

            if (!inputController)
            {
                inputController = GetComponent<NXZPlayerInputController>();
            }
            StartCoroutine(HeartBeat());
        }

        public void OnSafeCircleEnter()
        {
            for (int i = 0; i < virusCircles.Count; i++)
            {
                if (virusCircles[i].IsInside)
                {
                    virusCircles[i].UpdateVPConsume();
                }
            }
        }

        public void OnSafeCircleExit()
        {
            for (int i = 0; i < virusCircles.Count; i++)
            {
                if (virusCircles[i].IsInside)
                {
                    virusCircles[i].UpdateVPConsume();
                }
            }
        }

        IEnumerator HeartBeat()
        {
            while (true)
            {
                VpConsumeRate = Mathf.Clamp(VpConsumeRate, 0, 10000);
                //vpProtect = Mathf.Clamp(vpProtect, 0, 10000);

                //int vpconsume = (int)(VpConsumeRate - vpProtect);
               // vpconsume = Mathf.Clamp(vpconsume, 0, 10000);

                if (inputController.Sprint)
                {
                    yield return new WaitForSeconds(0.5f*HeartBeatTimer);
                    ConsumeVP((int)VpConsumeRate + 5);
                }
                else
                {
                    yield return new WaitForSeconds(HeartBeatTimer);
                    ConsumeVP((int)VpConsumeRate);
                }

            }
        }

    }
}

