using UnityEngine;

namespace Interaction
{
    public class ResourceReplenisher : MonoBehaviour, IInteractable
    {
        public enum Resource
        {
            Wood,
            Stone
        }

        public Resource resource;
        public void Interact(GameObject interactor)
        {
            if (resource == Resource.Wood)
            {
                EventManager.RaiseOnReplenishResource(3);
            }
        }
    }
}
