using Player;
using UnityEngine;

namespace Interaction
{
    public class ResourceReplenisher : MonoBehaviour, IInteractable
    {
        [SerializeField] private int replenishAmountWood = 3;
        [SerializeField] private int replenishAmountStone = 3;
        
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
                EventManager.RaiseOnReplenishWood(replenishAmountWood);
            }
            else
            {
                EventManager.RaiseOnReplenishStone(replenishAmountStone);
            }
        }
    }
}
