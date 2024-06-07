using UnityEngine;

namespace AnimationCotrollers
{
    public class BoltController : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(0.4f, 0, 0);
        }
    }
}