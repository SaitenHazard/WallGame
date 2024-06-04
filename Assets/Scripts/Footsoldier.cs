using System.Collections;
using UnityEngine;

public class Footsoldier : MonoBehaviour
{
    private float offsetMove = 0.0f;

    private Vector3 startPosition;

    public float _vibing = 1;

    public float _ecstasy = 1;

    public float _moveSpeed = 0.1f;

    private void Start()
    {
        offsetMove = Random.Range(0, 6.28f);
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.position = startPosition + new Vector3(0, _vibing+_vibing * Mathf.Sin(_ecstasy * (offsetMove + Time.time)), -Time.time * _moveSpeed);
    }

    public void SetUp(float vibing, float ecstasy, float moveSpeed)
    {
        _vibing = vibing;
        _ecstasy = ecstasy;
        _moveSpeed = moveSpeed;
    }

    public IEnumerator Die()
    {
        transform.parent = null;
        for (int i = 0; i < 90; i++)
        {
            transform.Rotate(1, 0, 0);
            yield return null;
        }
        Destroy(gameObject);
    }
}