using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}

delegate void Interaction();

public class Interactor : MonoBehaviour
{
    private void OnEnable()
    {
        Inputs.Interact += Interact;
    }

    private Interaction _interact;    
    // Start is called before the first frame update
    
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            _interact = interactable.Interact;
        }
        
        /*
         * Possibly call to UI class to display Tooltip (Press E to Interact)
         */
    }

    private void OnTriggerExit(Collider other)
    {
        _interact = () => { print("No interactable nearby"); };
    }

    void Interact()
    {
        print("INTERACTOR");
        _interact.Invoke();
    }
}
