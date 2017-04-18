using UnityEngine;
using System.Collections;

//***************************************************************
//  PlayerController.cs       Author: Glauz
//
//  This is my basic thirdperson Character Controller.
//  Lots of cool settings to adjust.
//  Still working on a better rotation Angle.
//
//  Youtube: https://www.youtube.com/GlauzCoding
//
//***************************************************************

namespace Glauz.Avatar
{
    [RequireComponent(typeof( CharacterController))]
    public class PlayerController : MonoBehaviour
    {

        private Animator anim;
        private CharacterController cc;

        [Header("General")]
        public float movementSpeed = 8f;    //Set to 0 since its using Root Motion
        public float rotationSpeed = 500f;
        public float rotationFastSpeed = 1000f; //Used when need to turn fast
        public float fallSpeed = 6.32f;

        private Vector3 gravity;
        private float yVelocity;

        [Header("Animation")]
        private GameObject cam; //For relative position
        public float animationTransition = 9f;
       // Quaternion targetRotation = Quaternion.identity;
        private bool rotationMet = true;

        
        Quaternion rotateHere;
        //private float rotationAngle;

        //private Vector3 targetMovement;


        [Header("Debug")]
        public bool DisableMove = false;
        public Vector2 lastInput, currentInput;
        public float YAnglePos,  YAngleLastPos; //These are used to know whether to skip rotation if angle is greater than 90f;

        public enum AvatarState
        {
            Idle,
            Walking,
            Running,
            Jumping,
            Falling
        }

        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            cam = Camera.main.gameObject;
            cc = GetComponent<CharacterController>();
            gravity = Physics.gravity;
        }


        void Update()
        {
            Debug.DrawRay(transform.position + Vector3.up * 5f, transform.position  - Camera.main.transform.position , Color.blue);

            Animate();

            Rotate();
            Move();

            Fall();
            //Jump();
            //Attack();
        }

        void LateUpdate()
        {

        }

        //Put more animations here based on states
        private void Animate()
        {
            if (!rotationMet) return;

            //Animation Mechnim with Movement
            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
                anim.SetFloat("Forward", Mathf.MoveTowards(anim.GetFloat("Forward"), 1f, animationTransition * Time.deltaTime));
            else
                anim.SetFloat("Forward", Mathf.MoveTowards(anim.GetFloat("Forward"), 0f, animationTransition * Time.deltaTime));
        }

        private void Move()
        {
            if (DisableMove || !rotationMet) return;

            if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
                cc.Move(gameObject.transform.forward * movementSpeed * Time.deltaTime);
        }

        void Rotate()
        {
            //Stop execution if no direction keys pressed
            if (isDirectionPressed() == false) return; 





            //Rotate to this angle (Will be used to target this rotation)
            //targetRotation = GetAngle();// new Quaternion (transform.rotation.x, Camera.main.transform.rotation.y, transform.rotation.z, 0f) ;



            //print(rotationAngle);

            //WASD place pointer from transform.forward on first button Input
            //Check if angle is greater than 90F
            //  if over then teleport rotation
            //  if not then slerp to rotation

            currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            //print("CurrentInput: " + currentInput);
            //print("LastInput: " + lastInput);


            

            //If input is different, then calculate rotation
            if (lastInput != currentInput)
            {
                //rotateHere = GetAngle();
                rotationMet = true; //Rest this
                YAngleLastPos = YAnglePos;
                lastInput = currentInput;
            }

            rotateHere = GetAngle();

            //float rotationAngle = 90f;

            if (YAnglePos == 0) YAnglePos = 360f;
            if (YAngleLastPos == 0) YAngleLastPos = 360f;


            //if (  ToDegree(YAngleLastPos + 90f, "1") <= YAnglePos || ToDegree(YAngleLastPos - 90f, "2") >= YAnglePos)

            //Check if +90f or -90f to last value is more then 90f away
            //if (DistanceBetweenValues(ToDegree(YAngleLastPos + 90f), YAnglePos) <= 90f ||
            //    DistanceBetweenValues(ToDegree(YAngleLastPos - 90f), YAnglePos) <= 90f)
            //if (lastInput != -currentInput)

            if (WithinRotationDistance(YAngleLastPos, YAnglePos))
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateHere, rotationSpeed * Time.deltaTime);

            else
            {
                transform.rotation = rotateHere;

                //rotationMet = false;
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateHere, rotationFastSpeed * Time.deltaTime);

                //if (transform.rotation == rotateHere)
                //    rotationMet = true;
            }




        }


        private Quaternion GetAngle()
        {
            Vector3 moveVector = Vector3.zero;
            Vector3 gameRotation;
            float YAngle;

            moveVector.x = (Input.GetAxisRaw("Horizontal"));
            moveVector.y = (Input.GetAxisRaw("Vertical"));

            float angle = Vector3.Angle(moveVector, Vector3.right);
            Vector3 cross = Vector3.Cross(moveVector, Vector3.right).normalized;

            //YAngleLastNeg = YAngleNeg;
            //YAngleLastPos = YAnglePos;

            YAngle = (((angle * -cross.z) < 0) ? (360f - angle * cross.z) : angle) * -1 + 90f;
            //Debug.Log("YAngle Before: " + YAngle);
            //YAngleNeg = YAngle;

            YAngle = (YAngle < 0) ? (360f + YAngle) : YAngle;  //Convert to positive Number so no -90 but instead 270
            //Debug.Log("YAnglePos: " + YAngle);
            YAnglePos = YAngle;


            gameRotation = new Vector3(0, YAngle + Camera.main.transform.eulerAngles.y, 0);            

            return (Quaternion.Euler(gameRotation));            
        }

        //private float DistanceBetweenValues(float value1, float value2)
        //{
        //    float value = value1 - value2;
        //    if (value < 0f) value = value * -1f;

        //    //print("Distance: " + value);

        //    return value;
        //}

        //If within 90 degrees on both sides
        private bool WithinRotationDistance(float value1, float value2)
        {
            float test1 = 0f, test2 = 0f;

            if (value1 > value2)
            {
                test1 = value1 - value2;
                test2 = (value2 + 360) - value1;
                //print("Value1 >= Value2");
            }

            else if (value2 > value1)
            {
                test1 = value2 - value1;
                test2 = (value1 + 360) - value2;
                //print("Value2 >= Value1");
            }

            else if (value1 == value2)
            {
                //print("ITS THE SAME!");
                return true;                
            }

           // print("Test1: " + test1);
            //print("Test2: " + test2);

            if (test1 <= 90f || test2 <= 90f)
                return true;

            else
                return false;
        }

        private float ToDegree(float value)
        {
            //print(derp);
            //print("ValueBefore: " + value);

            if (value == 0f)
                value = 360;

            if (value < 0f)
                value = value + 360f;

            if (value > 360f)
                value = value - 360f;
            //value = value % 360;

            print("ValueAfter: " + value);
            return value;
        }

        private bool isDirectionPressed()
        {
             return (Input.GetButton("Horizontal") == true || Input.GetButton("Vertical") == true) ? true : false;            
        }

        private void Fall()
        {
            if (!cc.isGrounded)
            {
                //print("IS NOT GROUNDED!");
                yVelocity += .0055f * Time.deltaTime;
                cc.Move(gameObject.transform.up + gravity * (fallSpeed * Time.deltaTime + yVelocity));
            }

            else
            {
                //print("IS GROUNDED!");
                //if (yVelocity != Vector3.zero)
                //    yVelocity = Vector3.zero;
                yVelocity = 0f;
            }
        }

    }//End Class
}//End Namespace