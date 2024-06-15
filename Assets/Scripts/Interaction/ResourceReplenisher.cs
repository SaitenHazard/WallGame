using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Interaction
{
    public class ResourceReplenisher : MonoBehaviour, IInteractable
    {
        [SerializeField] private int replenishAmount = 3;
        
        public enum Resource
        {
            Wood,
            Stone
        }

        public Resource resource;
        public void Interact(ThirdPersonController interactor)
        {
            if (resource == Resource.Wood)
            {
                EventManager.RaiseOnReplenishWood(replenishAmount);
            }
            else
            {
                EventManager.RaiseOnReplenishStone(replenishAmount);
            }
        }
    }
}
