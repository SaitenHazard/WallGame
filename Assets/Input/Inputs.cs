using Interaction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class Inputs : MonoBehaviour
    {
        [Header("Character Input Values")] public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")] public bool analogMovement;

        [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        public delegate void OnInputVector2(Vector2 value);

        public delegate void OnInputNoParams();

        public static event OnInputNoParams Jump;
        public static event OnInputNoParams Interact;
        public static event OnInputNoParams Fire;

        public static event OnInputVector2 Select;

        private Catapult catapult;

        void Start()
        {
            catapult = FindObjectOfType<Catapult>();
            if (catapult == null)
            {
                Debug.LogWarning(
                    "Inputs.cs could not find a Catapult.cs script in the scene and is now deactivated. Make sure to include a Catapult prefab in the scene and activate its Catapult.cs component!");
                enabled = false;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Jump?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                print("Interacting" + context);
                Interact?.Invoke();
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            Select?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            catapult.Aim(context.ReadValue<Vector2>());
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            Fire?.Invoke();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}