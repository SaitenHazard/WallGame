using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using UnityEngine;

interface IInteractable
{
    public void Interact(GameObject self);
}

delegate void Interaction(GameObject self);

public class Interactor : MonoBehaviour
{
    private void Awake()
    {
        Inputs.Interact += Interact;
    }

    private void OnDestroy()
    {
        Inputs.Interact -= Interact;
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
        _interact = _ => { print("No interactable nearby"); };
    }

    private void Interact()
    {
        print("INTERACTOR");
        _interact?.Invoke(gameObject);
    }
}
