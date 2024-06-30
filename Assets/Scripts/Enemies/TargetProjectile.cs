using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class TargetProjectile : MonoBehaviour
    {
        public float parabolaHeight = 50;

        [SerializeField]
        [Tooltip(
            "This MeshRenderer is deactivated on impact. The invisible GameObject will linger for 1 second and wait for the trail to finish.")]
        private MeshRenderer meshRenderer;

        public bool spawnParticlesOnHit;

        [SerializeField] private GameObject onHitParticles;

        public bool particlesSpawned;
        private float _arrivalTime;

        private Vector3 _destination;
        private Vector3 _lastPosition;
        private Vector3 _releasePoint;

        private float _startOfLife = -1;

        public void Start()
        {
            _releasePoint = transform.position;
            _lastPosition = transform.position - transform.forward;
        }

        public void Update()
        {
            if (Mathf.Approximately(_startOfLife, -1)) return;

            var t = (Time.time - _startOfLife) / (_arrivalTime - _startOfLife);
            if (t >= 1)
            {
                meshRenderer.enabled = false;
                if (spawnParticlesOnHit && !particlesSpawned)
                {
                    GameObject particles = Instantiate(onHitParticles, _destination, Quaternion.identity);
                    particlesSpawned = true;
                }
                Destroy(gameObject); // Wait 1 sec for the trail to disappear
            }
            else
            {
                transform.position = Vector3.Lerp(_releasePoint, _destination, t) +
                                     new Vector3(0, parabolaHeight * (-Mathf.Pow(2 * t - 1, 2) + 1), 0);
                transform.LookAt(transform.position + (transform.position - _lastPosition));
                //transform.Rotate(Time.deltaTime, Time.deltaTime, 0);
            }

            _lastPosition = transform.position;
        }

        public void SetDestination(Vector3 position)
        {
            _destination = position;
        }

        public void SetFlightTime(float flightTime)
        {
            _startOfLife = Time.time;
            _arrivalTime = _startOfLife + flightTime;
        }

        public void SetSettings(ProjectileSettings settings)
        {
            SetFlightTime(settings.flightTime);
            SetParabolaHeight(settings.parabolaHeight);
        }

        private void SetParabolaHeight(float parabolaHeight)
        {
            this.parabolaHeight = parabolaHeight;
        }
    }
}