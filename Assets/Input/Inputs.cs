using Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using static EventManager;

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
        public static event OnInputNoParams RepairWood;
        public static event OnInputNoParams RepairStone;
        public static event OnInputNoParams YPressed;
        public static event OnInputVector2 Select;

        private Catapult _catapult;

        void Start()
        {
            _catapult = FindObjectOfType<Catapult>();
            if (_catapult == null)
            {
                Debug.LogWarning(
                    "Inputs.cs could not find a Catapult.cs script in the scene and is now deactivated. Make sure to include a Catapult prefab in the scene and activate its Catapult.cs component!");
                enabled = false;
            }
        }

        public void OnYPressed(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                YPressed?.Invoke();
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
                Interact?.Invoke();
            }
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            Select?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRepairWood(InputAction.CallbackContext context)
        {
            if (context.started) RepairWood?.Invoke();
        }

        public void OnRepairStone(InputAction.CallbackContext context)
        {
            if (context.started) RepairStone?.Invoke();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            _catapult.Aim(context.ReadValue<Vector2>());
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