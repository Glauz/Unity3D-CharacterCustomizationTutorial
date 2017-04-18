using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//***************************************************************
//  CameraController.cs       Author: Glauz
//
//  Really Awesome Camera :P. Camera controls literally
//  everything.
//  Need to impliment using trig instead in the future.
//
//***************************************************************


namespace Glauz
{
    public class CameraController : MonoBehaviour
    {
        [Header("CAMERA TARGET")]
        public GameObject target;
        [HideInInspector]
        public bool overrideLookAt;
        [HideInInspector]
        public GameObject lookAtOveride;

        [Header("GENERAL SETTINGS")]
        public float offsetX;
        public float offsetY;
        public float offsetZ;
        public float camSpeed = 5f;
        public float mouseSensitivity = 50;
        public float gamePadSensitivity = 100;  //Has a bit of support for gamepad

        [Header("SCROLL SETTINGS")]
        public float scrollSpeed = 350f;
        public float scrollDist = 3f;
        public float scrollMax = 10f, scrollMin = 5f;

        [Header("CAMERA ROTATION Y-AXIS LIMIT SETTINGS")]
        [Range(0, 90f)]
        public float rotYMax = 90f;     //Camera limit of moving it up and down
        [Range(0, -90f)]
        public float rotYMin = -90f;
        private float currentAngleY = 0, currentAngleX = 0;

        [Header("MISC SETTINGS")]
        public float maxDistance = 180; //Camera Teleports to target if to far away

        //Doesn't work at the moment
        [Header("FADE CHARACTER SETTINGS (WIP)")]   //Fade materials if close to to camera
        public List<Material> materials = new List<Material>();
        public float fadeSpeed = 3f;        //Speed at which the camera fades
        public float fadeCameraDis = 2f;    //How close the camera has to be before fading


        Vector3 offset;
        private Ray ray;
        private RaycastHit hit;
        private float rayScrollDistance;
        private bool isColliding;

        void Update()
        {
            Rotation();
            ScrollCamera();
            FadeCharacter();
        }

        void LateUpdate()
        {
            if (target == null) return;


            LerpCamera();
            LookAtTarget();
            HitDetection();
        }

        //Scrolling manipulates the scrollDist Variable where LerpCamera applies the new variable.
        private void ScrollCamera()
        {
            if (!isColliding)
            {
                scrollDist -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
                scrollDist = Mathf.Clamp(scrollDist, scrollMin, scrollMax);
            }

        }

        //Simple Unity LookAt, to always look at target
        public void LookAtTarget()
        {
            if (!overrideLookAt)
                transform.LookAt(target.transform.position - offset);
            else
                transform.LookAt(lookAtOveride.transform.position - offset);
        }

        public void Rotation()
        {
            //Keyboard & Mouse
            if (Input.GetButton("Fire2"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                currentAngleY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                currentAngleX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                //Debug.Log ("CurrentAngleY: " + currentAngleY);         
            }

            else if (Cursor.visible == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            //Gamepad (Axis 5 & 4) *make sure inputs exist in Input Manager
            //currentAngleY += Input.GetAxis("JoystickRight Y") * gamePadSensitivity * Time.deltaTime;
            //currentAngleX += Input.GetAxis("JoystickRight X") * gamePadSensitivity * Time.deltaTime;

            currentAngleY = Mathf.Clamp(currentAngleY, rotYMin, rotYMax);


        }

        //Handles camera positioning and following of the player
        public void LerpCamera()
        {


            offset = -(Vector3.up * offsetY) + (Vector3.right * offsetX) + (Vector3.forward * offsetZ);

            Quaternion q = Quaternion.Euler(-currentAngleY, currentAngleX, 0);
            Vector3 direction = q * Vector3.forward;
            Vector3 targetPos = target.transform.position + -offset;

            //This is how the camera knows its place with no trig but just vectors (Will use trig in the future for this)
            if (transform.position == (targetPos - direction * scrollDist)) return;

            //If Camera To Far Then Teleport Camera
            if (CameraTooFar())
                transform.position = targetPos - direction * scrollDist;

            //Lerp Camera If with collision or not
            if (!isColliding)
                transform.position = Vector3.Lerp(transform.position, targetPos - direction * scrollDist, camSpeed * Time.deltaTime);
            else
                transform.position = Vector3.Lerp(transform.position, targetPos - direction * rayScrollDistance, camSpeed * Time.deltaTime);


        }

        //Overrides scroll distance when raycast hits a collider. Distance of camera goes to closest unit possible from collision
        public void HitDetection()
        {
            ray.origin = target.transform.position - offset;
            ray.direction = Camera.main.transform.position - ray.origin;

            Debug.DrawRay(ray.origin, ray.direction * scrollDist, Color.red);

            if (Physics.Raycast(ray, out hit, scrollDist))
            {
                rayScrollDistance = hit.distance;
                isColliding = true;

                //Trigger Fade Event if rayScrollDistance is to Close

            }
            else
                isColliding = false;
        }

        private void FadeCharacter()
        {
            //If Camera is to close to player, then fade the character out to prevent clipping.

            //if (rayScrollDistance )
        }

        //If Camera to far then teleport
        private bool CameraTooFar()
        {
            float dis = Vector3.Distance(gameObject.transform.position, target.transform.position);

            if (dis > maxDistance)
                return true;

            //Else Default to false
            return false;
        }

        //Override scroll distance
        public void HitDetection(GameObject target)
        {
            ray.origin = target.transform.position - offset;
            ray.direction = Camera.main.transform.position - ray.origin;

            Debug.DrawRay(ray.origin, transform.position, Color.red);

            if (Physics.Raycast(ray, out hit, Vector3.Distance(target.transform.position, transform.position)))
            {
                rayScrollDistance = hit.distance;
                isColliding = true;
            }
            else
                isColliding = false;
        }

        public void OnDisabled()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            enabled = false;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }
    }//End of Class

}