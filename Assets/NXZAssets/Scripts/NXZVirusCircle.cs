using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NXZ
{

    public class NXZVirusCircle : MonoBehaviour
    {

        public NXZPlayerState playerState;

        [SerializeField]
        private int virusStartValue = 25, virusMax = 75, currValue = 25;

        private int tempValue = 25;

        [SerializeField]
        private int vpConsume = 5;

        [SerializeField]
        private int vpProtectPercent = 0;

        [SerializeField]
        private float spreadSpeed = 1f;

        [SerializeField]
        private bool isInside = false, ifSpread = false, ifSafeCircle = false;

        private float radias;

        public int VirusStartValue
        {
            get => virusStartValue;
            private set => virusStartValue = value;
        }
        public int CurrValue
        {
            get => currValue;
            private set => currValue = value;
        }
        public bool IsInside
        {
            get => isInside;
            private set => isInside = value;
        }

        void Start()
        {
            currValue = 0;
            tempValue = 0;
            playerState = FindObjectOfType<NXZPlayerState>();
            radias = 0.5f * transform.localScale.x;
            //StartCoroutine(Consuming());
        }

        void Update()
        {
            if (ifSpread)
            {
                radias += spreadSpeed * Time.deltaTime;
                transform.localScale = new Vector3(2 * radias, 2 * radias, 2 * radias);
            }
        }

        //IEnumerator Consuming()
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(consumTimer);
        //        if (isInside)
        //        {
        //            playerState.ConsumeVP(vpConsume);
        //        }
        //    }
        //}
        void OnTriggerStay(Collider col)
        {
            if (!ifSafeCircle)
            {
                if (col.CompareTag("Player"))
                {
                    if (Random.Range(0, 50) == 0)
                    {
                        UpdateVPConsume();
                    }
                }
            }
            else
            {
                if (col.CompareTag("Player"))
                {
                    playerState.AddHP(1);
                }
            }
        }

        public void UpdateVPConsume()
        {
            CalculateValue();
            PlayerVPConsumeUpdate();
        }

        void CalculateValue()
        {
            float distance = Vector3.Distance(playerState.transform.position, transform.position);
            radias = 0.5f * transform.localScale.x;

            currValue = (int)(((radias - distance) / radias) * (virusMax - virusStartValue) + virusStartValue);
            currValue += Random.Range(-1, 1);
            currValue -= (int)playerState.VpProtect;
            currValue = Mathf.Clamp(currValue, 0, virusMax);
        }

        void PlayerVPConsumeUpdate()
        {
            playerState.VpConsumeRate += 0.01f * (currValue - tempValue) * vpConsume;
            tempValue = currValue;
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                IsInside = true;
                //playerState.VpConsumeRate += vpConsume;
                if (!ifSafeCircle&&!playerState.virusCircles.Contains(this))
                {
                    playerState.virusCircles.Add(this);
                }

                if (ifSafeCircle)
                {
                    playerState.VpProtect += vpProtectPercent;
                    playerState.OnSafeCircleEnter();
                    playerState.IfInSafeCircle = true;
                }
                else
                {
                    UpdateVPConsume();
                }
            }
        }
        void OnTriggerExit(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                IsInside = false;
                //playerState.VpConsumeRate -= vpConsume;
                //playerState.virusCircles.Remove(this);
                if (ifSafeCircle)
                {
                    playerState.IfInSafeCircle = false;
                    playerState.VpProtect -= vpProtectPercent;
                    playerState.OnSafeCircleExit();
                }
                else
                {
                    currValue = 0;
                    PlayerVPConsumeUpdate();
                }
            }
        }
    }

}

