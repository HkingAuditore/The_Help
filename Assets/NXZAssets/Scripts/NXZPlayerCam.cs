using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NXZ
{
    public class NXZPlayerCam : MonoBehaviour
    {
        public Transform target;
        public Transform targetBody;
        public float targetHeight = 1.2f;
        public float distance = 4.0f;
        private float currdistance = 4.0f;
        public float maxDistance = 6;
        public float minDistance = 1.0f;
        public float xSpeed = 250.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -10;
        public float yMaxLimit = 70;
        public float zoomRate = 80;
        public float rotationDampening = 3.0f;
        private float x = 20.0f;
        private float y = 0.0f;
        public Quaternion aim;
        public float aimAngle = 8;
        public bool lockOn = false;
        RaycastHit hit;
        public GameObject aimStick; //For Mobile

        void Start()
        {
            transform.SetParent(null);
            if (!target)
            {
                target = GameObject.FindWithTag("Player").transform;
            }
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
            //Screen.lockCursor = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            if (!target)
                return;
            if (!targetBody)
            {
                targetBody = target;
            }

            //if (Time.timeScale == 0.0f || GlobalConditionC.freezeAll){
            //	return;
            //}

            x += Mathf.Clamp(Input.GetAxis("Mouse X"),-2,2) * xSpeed * 0.02f;
            y -= Mathf.Clamp(Input.GetAxis("Mouse Y"),-2,2) * ySpeed * 0.02f;

            if (aimStick)
            {
                //float aimHorizontal = aimStick.GetComponent<MobileJoyStickC>().position.x;
                //float aimVertical = aimStick.GetComponent<MobileJoyStickC>().position.y;
                //if(aimHorizontal != 0 || aimVertical != 0){
                //	x += aimHorizontal * xSpeed * 0.02f;
                //	y -= aimVertical * ySpeed * 0.02f;
                //}
            }


            distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            currdistance = Mathf.Lerp(currdistance, distance, 0.25f);

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            // Rotate Camera
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Quaternion temprot = transform.rotation;
            temprot = Quaternion.Lerp(temprot, rotation, 0.25f);

            transform.rotation = temprot;
            //transform.rotation = rotation;

            aim = Quaternion.Euler(y - aimAngle, x, 0);

            //Rotate Target
            //if(!GlobalConditionC.freezePlayer){
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || lockOn)  //Input.GetButton("Fire1") || Input.GetButton("Fire2") || 
            {

                Quaternion bodytemprot = targetBody.transform.rotation;
                bodytemprot = Quaternion.Lerp(bodytemprot, Quaternion.Euler(0, x, 0), 0.1f);
                targetBody.transform.rotation = bodytemprot;

            }
            //}

            //Camera Position

            Vector3 positiona = target.position - (temprot * Vector3.forward * currdistance + new Vector3(0.0f, -targetHeight, 0.0f));

            transform.position = positiona;

            Vector3 trueTargetPosition = target.transform.position - new Vector3(0.0f, -targetHeight, 0.0f);

            if (Physics.Linecast(trueTargetPosition, transform.position, out hit, ~(1<<8)))
            {
                if (hit.transform.tag == "Wall" || hit.transform.tag == "Ground")
                {
                    float tempDistance = Vector3.Distance(trueTargetPosition, hit.point) - 0.28f;

                    positiona = target.position - (temprot * Vector3.forward * tempDistance + new Vector3(0, -targetHeight, 0));
                    transform.position = positiona;
                }

            }
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);

        }
    }
}
