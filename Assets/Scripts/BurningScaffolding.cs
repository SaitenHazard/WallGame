using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

public class BurningScaffolding : MonoBehaviour
{
    public GameObject particlePrefab;
    public float simulationTime = 2.0f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ball")
        {
            StartCoroutine(InstantiateAndDeactivate());
        }
    }

    private IEnumerator InstantiateAndDeactivate()
    {
        GameObject particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        ParticleSystem particleSystem = particleInstance.GetComponent<ParticleSystem>();
        particleSystem.Simulate(simulationTime, true, true, true);
        particleSystem.Play();

        gameObject.SetActive(false);

        yield return null; // Optional: wait one frame if needed
    }
}
