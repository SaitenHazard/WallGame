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

        public delegate void OnInput(InputValue value);

        public delegate void OnInputNoParams();

        public static event OnInputNoParams Jump;
        public static event OnInputNoParams Interact;

        public static event OnInput Select;

        private Catapult catapult;

        void Start()
        {
            catapult = FindObjectOfType<Catapult>();
            if (catapult == null)
            {
                Debug.LogWarning("Inputs.cs could not find a Catapult.cs script in the scene and is now deactivated. Make sure to include a Catapult prefab in the scene and activate its Catapult.cs component!");
                this.enabled = false;
            }
        }

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                Jump?.Invoke();
            }
        }

        public void OnInteract(InputValue value)
        {
            if (value.isPressed)
            {
                Interact?.Invoke();
            }
        }

        public void OnSelect(InputValue value)
        {
            Select?.Invoke(value);
        }

        public void OnAim(InputValue input)
        {
            if (input != null)
            {
                catapult.Aim(input.Get<Vector2>());
            }
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