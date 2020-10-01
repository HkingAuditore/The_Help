using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NXZ
{
    public class NXZPlayerAnim : MonoBehaviour
    {
        public Animator playerAnim;
        public CharacterController controller;
        public NXZCharacterMotor playermotor;
        private float maxForwardspeed, maxRightspeed;

        private bool tempGround = true;

        private float xv, yv, zv;
        // Start is called before the first frame update
        void Start()
        {
            if (!controller)
            {
                controller = GetComponent<CharacterController>();
            }
            if (!playermotor)
            {
                playermotor = GetComponent<NXZCharacterMotor>();
            }
            maxForwardspeed = playermotor.movement.maxForwardSpeed;
            maxRightspeed = playermotor.movement.maxSidewaysSpeed;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 move = transform.InverseTransformVector(controller.velocity);

            xv = Mathf.Lerp(xv, move.x, 0.5f);
            yv = Mathf.Lerp(yv, move.y, 0.5f);
            zv = Mathf.Lerp(zv, move.z, 0.5f);

            float ax = Mathf.Abs(xv);
            float az = Mathf.Abs(zv);
            float ay = Mathf.Abs(yv);

            playerAnim.SetFloat("Forward", Mathf.Sign(zv) * (az * (1 + ay / (ax + az)) / maxForwardspeed));

            playerAnim.SetFloat("Right", Mathf.Sign(xv) * (ax * (1 + ay / (ax + az)) / maxRightspeed));

            if (controller.isGrounded)
            {
                playerAnim.SetBool("OnGround", true);
            }
            else
            {
                playerAnim.SetBool("OnGround", false);
            }
            playerAnim.SetFloat("Jump", move.y);
            if (!tempGround && controller.isGrounded)
            {
                TouchGround();
            }
            tempGround = controller.isGrounded;
        }

        void TouchGround()
        {

        }
    }


}
