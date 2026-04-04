using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nate.RTS
{ 

    public class PlayerInput : MonoBehaviour
    {
        // move this transform based on keyboard input from player
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        [SerializeField] private float keyboardPanSpeed = 5f;
        [SerializeField] private float zoomSpeed = 1f;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private float minZoomDistance = 7.5f;

        private CinemachineFollow cinemachineFollow;

        private float zoomStartTime;    // time when player presses button to start zooming in camera
        private float rotationStartTime;
        private Vector3 startingFollowOffset;   // position (offset) of camera before zooming in
        private float maxRotationAmount;


        void Awake()
        {
            // here 'TryGetComponent' is similar to 'GetComponent.' Will return 'true' if component is found, 'false' if not
            // if 'cinemachineCamera' can not find/get the 'cinemachineFollow' component, print error message
            // if found, attaches 'cinemachineFollow' to CinemachineFollow component of 'cinemachineCamera'
            if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))  
            {
                Debug.LogError("Cinemachine camera did not have CinemachineFollow. Zoom functionality will not work.");
            }

            // set to original camera offset
            startingFollowOffset = cinemachineFollow.FollowOffset;
            maxRotationAmount = Mathf.Abs(cinemachineFollow.FollowOffset.z);
        }



        void Update()
        {
            HandlePanning();
            HandleZooming();
            HandleRotation();
        }

        private void HandleRotation()
        {
            
            if (ShouldSetRotationStartTime())
            {
                rotationStartTime = Time.time;
            }

            float rotationTime = Mathf.Clamp01((Time.time - rotationStartTime) * rotationSpeed);

            Vector3 targetFollowOffset;

            if (Keyboard.current.zKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    maxRotationAmount,
                    cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else if (Keyboard.current.aKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                    -maxRotationAmount,
                    cinemachineFollow.FollowOffset.y,
                    0
                );
            }
            else
            {
                targetFollowOffset = new Vector3(
                    startingFollowOffset.x,
                    cinemachineFollow.FollowOffset.y,
                    startingFollowOffset.z
                );
            }

            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset,
                targetFollowOffset,
                rotationTime
            );
        }


        private bool ShouldSetRotationStartTime()
        {
            // tutorial uses 'page up' and 'page down' keys, which I don't have on my keyboard. 
            // I'm using the 'A' and 'Z' keys as a replacement.
            return Keyboard.current.aKey.wasPressedThisFrame 
            || Keyboard.current.zKey.wasPressedThisFrame 
            || Keyboard.current.aKey.wasReleasedThisFrame 
            || Keyboard.current.zKey.wasReleasedThisFrame;
            
        }

        private void HandleZooming()
        {
            if (shouldSetZoomStartTime())
            {
                // stored as variable and reset everytime we start pressing or release the Key
                zoomStartTime = Time.time;
            }



            // time it has been (time.time) since we started pressing or released the zoom key (zoomStartTime) 
            // also will be modified (sped up or down) by zoomSpeed;
            float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * zoomSpeed);
            
            Vector3 targetFollowOffset; // where we need to move the camera to
            // same functionality as 'targetFollowOffset = new Vector3(x, y, z);' just different formatting
            

            // originally had 'targetFollowOffset' x and z values as 'startingFollowOffset.' those need to be 
            // 'cinemachineFollow.FollowOffset' or it will affect the rotation
            if (Keyboard.current.qKey.isPressed)
            {
                targetFollowOffset = new Vector3(
                cinemachineFollow.FollowOffset.x,
                minZoomDistance, 
                cinemachineFollow.FollowOffset.z);
            }

            else
            {
                targetFollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    startingFollowOffset.y,
                    cinemachineFollow.FollowOffset.z);
            }


                // using 'Slerp' instead of 'Lerp.' does almost the same thing and is written the same way, 
                // but 'Slerp' interpolates on an arc (spherically) instead of a line (Lerp)
            // slerp from wherever the cinemachine currently is (cinemachineFollow.FollowOffset) 
            // to the 'targetFollowOffset' which is different depending on if the zoom key is pressed or released
            cinemachineFollow.FollowOffset = Vector3.Slerp(
                cinemachineFollow.FollowOffset, 
                targetFollowOffset, 
                zoomTime);
            

        }

        private bool shouldSetZoomStartTime()
        {
            // tutorial uses 'End' key, which I don't have on my keyboard. I'm using 'Q' key as a replacement.
            // check if Q ('qKey') was pressed OR released
            return Keyboard.current.qKey.wasPressedThisFrame 
            || Keyboard.current.qKey.wasReleasedThisFrame;
        }

        private void HandlePanning()
        {
            // moveAmount is how much we want the keyboard to be moved by as a vector 
            // we only use the x and the z of the vector because we don't want it to move on the y axis (higher / lower)
            
            // camera movement based on keyboard input
            Vector2 moveAmount = Vector2.zero;

            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += keyboardPanSpeed;
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= keyboardPanSpeed;
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= keyboardPanSpeed;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += keyboardPanSpeed;
            }

            moveAmount *= Time.deltaTime;
            cameraTarget.position += new Vector3(moveAmount.x, 0, moveAmount.y);
        }
    }

}