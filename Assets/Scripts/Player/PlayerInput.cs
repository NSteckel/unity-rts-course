using UnityEngine;
using UnityEngine.InputSystem;

namespace Nate.RTS
{ 

    public class PlayerInput : MonoBehaviour
    {
        // move this transform based on keyboard input from player
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float keyboardPanSpeed = 5f;


        void Update()
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