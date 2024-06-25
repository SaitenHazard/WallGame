using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class Footsoldier : MonoBehaviour
    {
        public float vibing = 1;

        public float ecstasy = 1;

        public float moveSpeed = 0.1f;

        private bool _dead;

        private float _drift;
        private float _offsetMove;

        private Vector3 _startPosition;

        private void Start()
        {
            _offsetMove = Random.Range(0, 6.28f);
            _startPosition = transform.position;
            _drift = Random.Range(-0.05f, 0.05f);
        }

        private void Update()
        {
            if (!_dead)
                transform.position = _startPosition + new Vector3(_drift * 3,
                    vibing + vibing * Mathf.Sin(ecstasy * (_offsetMove + Time.time)), -Time.time * moveSpeed * 3);
        }

        public void SetUp(float vibing, float ecstasy, float moveSpeed)
        {
            this.vibing = vibing;
            this.ecstasy = ecstasy;
            this.moveSpeed = moveSpeed;
        }

        public IEnumerator Die()
        {
            _dead = true;
            for (var i = 0; i < 44; i++)
            {
                transform.Rotate(2, 0, 0);
                yield return null;
            }
            //Destroy(gameObject);
        }
    }
}