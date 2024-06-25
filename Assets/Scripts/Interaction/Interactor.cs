using Input;
using Player;
using UnityEngine;

namespace Interaction
{
    internal interface IInteractable
    {
        public void Interact(ThirdPersonController player);
    }

    internal delegate void Interaction(ThirdPersonController player);

    [RequireComponent(typeof(ThirdPersonController))]
    public class Interactor : MonoBehaviour
    {
        private ThirdPersonController _controller;

        private Interaction _interact;

        private void Awake()
        {
            _controller = GetComponent<ThirdPersonController>();

            Inputs.Interact += Interact;
        }

        private void OnDestroy()
        {
            Inputs.Interact -= Interact;
        }
        // Start is called before the first frame update

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.gameObject.GetComponent<IInteractable>();

            if (interactable != null) _interact = interactable.Interact;

            /*
             * Possibly call to UI class to display Tooltip (Press E to Interact)
             */
        }

        private void OnTriggerExit(Collider other)
        {
            _interact = _ => { print("No interactable nearby"); };
        }

        private void Interact()
        {
            _interact?.Invoke(_controller);
        }
    }
}