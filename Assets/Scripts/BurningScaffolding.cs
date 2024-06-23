using UnityEngine;

public class BurningScaffolding : MonoBehaviour
{
    public GameObject particlePrefab;
    public float simulationTime = 2.0f;
    public AnimationCurve intensityCurve;
    public float duration = 10f;
    private ParticleSystem.EmissionModule _emissionModule;
    private ParticleSystem _fireParticleSystem;
    private GameObject _particleInstance;
    private float _timer = 0f;

    private void Start()
    {
        _particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);

        _fireParticleSystem = _particleInstance.GetComponent<ParticleSystem>();
        _fireParticleSystem.Simulate(simulationTime, true, true, true);
        _fireParticleSystem.Play();

        gameObject.SetActive(false);

        _emissionModule = _fireParticleSystem.emission;
    }

    private void Update()
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