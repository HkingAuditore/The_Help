using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NXZ
{
    public class NXZPlayerUI : MonoBehaviour
    {
        public NXZPlayerInputController inputController;
        public NXZPlayerState playerState;

        public Image SP, VP, HP;
        
        public List<Text> virusValues;

        private int showValue;
        // Start is called before the first frame update
        void Start()
        {
            if (!inputController)
            {
                inputController = FindObjectOfType<NXZPlayerInputController>();
            }
            if (!playerState)
            {
                playerState = FindObjectOfType<NXZPlayerState>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            SP.fillAmount = inputController.stamina / inputController.maxStamina;
            VP.fillAmount = (float)playerState.VP / playerState.MaxVP;
            HP.fillAmount = (float)playerState.HP / playerState.MaxHP;

            for (int i = 0; i < virusValues.Count; i++)
            {
                if (playerState.virusCircles.Count>i)   // temp hard code
                {
                    showValue = int.Parse(virusValues[i].text.Replace("%", ""));
                    if ((int)showValue < playerState.virusCircles[i].CurrValue)
                    {
                        showValue += 1;
                    }
                    if ((int)showValue > playerState.virusCircles[i].CurrValue)
                    {
                        showValue -= 1;
                    }
                    virusValues[i].text = (int)showValue + "%";
                }
            }

        }
    }
}


