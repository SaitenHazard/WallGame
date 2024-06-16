using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;

public class BurningScaffolding : MonoBehaviour
{
    public GameObject particlePrefab;
    private GameObject particleInstance;
    private ParticleSystem fireParticleSystem;
    public float simulationTime = 2.0f;
    public AnimationCurve intensityCurve;
    public float duration = 10f;
    private float timer = 0f;
    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        fireParticleSystem = particleInstance.GetComponent<ParticleSystem>();
        fireParticleSystem.Simulate(simulationTime, true, true, true);
        fireParticleSystem.Play();

        gameObject.SetActive(false);

        emissionModule = fireParticleSystem.emission;
    }

    void Update()
    {
        // timer += Time.deltaTime;
        // float normalizedTime = timer / duration;

        // if (normalizedTime > 1f)
        // {
        //     normalizedTime = 1f;
        // }

        // float intensity = intensityCurve.Evaluate(normalizedTime);
        // emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(intensity);

        // if (normalizedTime >= 1f)
        // {
        //     fireParticleSystem.Stop();
        // }
    }
}