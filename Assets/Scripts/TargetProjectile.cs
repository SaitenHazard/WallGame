using UnityEngine;
using UnityEngine.UIElements;

public class TargetProjectile : MonoBehaviour{
    public float parabolaHeight = 50;

    private float startOfLife = -1;
    private float arrivalTime;

    private Vector3 destination;
    private Vector3 releasePoint;

    private MeshRenderer meshRenderer;

    public void Start()
    {
        releasePoint = transform.position;
        lastPosition = transform.position - transform.forward;
    }
    private Vector3 lastPosition;
    public void Update()
    {
        if (startOfLife == -1) return;
        
        float t = (Time.time - startOfLife) / (arrivalTime - startOfLife);
        if (t >= 1)
        {
            Destroy(gameObject, 1); // Wait 1 sec for the trail to disappear
        }
        else
        {
            transform.position = Vector3.Lerp(releasePoint, destination, t) + new Vector3(0, parabolaHeight * (-Mathf.Pow((2 * t - 1), 2) + 1), 0);
            transform.LookAt(transform.position + (transform.position - lastPosition));
            //transform.Rotate(Time.deltaTime, Time.deltaTime, 0);
        }
        lastPosition = transform.position;
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
    }

    public void SetFlightTime(float flightTime)
    {
        startOfLife = Time.time;
        this.arrivalTime = startOfLife + flightTime;
    }
}