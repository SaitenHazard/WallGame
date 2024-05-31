using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class Catapult : MonoBehaviour, IInteractable
{
    private bool _inCatapult = false;
    public Transform dropInPoint;
    public Transform dropOffPoint;
    public Transform targetPos;

    [SerializeField]
    [Tooltip("How far the mangonel will shoot wallther on the z axis")]
    public float maxZ = 10.0f;

    public bool ready = false;

    [SerializeField]
    [Tooltip("How many degrees per second can you rotate the Mangonel")]
    private float rotSpeed;

    [SerializeField]
    [Tooltip("How many meters per second can you change the Mangonel vertically")]
    private float aimUpDownSpeed;

    private Animator anim;

    private LineRenderer lineRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        //lineRenderer.transform.SetParent(null);
        //lineRenderer.transform.localScale = Vector3.one;
        //lineRenderer.transform.position = Vector3.zero;
        EventManager.OnEnterCatapult += StartAiming;
        EventManager.OnExitCatapult += StopAiming;
        EventManager.OnCatapultFire += OnFire;
    }

    void OnDestroy()
    {
        EventManager.OnEnterCatapult -= StartAiming;
        EventManager.OnExitCatapult -= StopAiming;
        EventManager.OnCatapultFire -= OnFire;
    }

    public void Interact()
    {
        if (_inCatapult)
        {
            _inCatapult = false;
            Vector3[] path = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(path);
            
            EventManager.RaiseCatapultFire(path, lineRenderer.positionCount);
        }
        else
        {
            if (ready)
            {
                _inCatapult = true;
                EventManager.RaiseEnterCatapult(dropInPoint);
            } else
            {
                Debug.Log("Catapult Not Ready Yet.");
            }
        }
    }

    public void StartAiming(Transform t)
    {
        Debug.Log("StartAiming");
        currentlyAiming = true;
        lineRenderer.enabled = true;
    }

    public void StopAiming(Transform t)
    {
        Debug.Log("StopAiming");
        currentlyAiming = false;
        lineRenderer.enabled = false;
    }

    private bool currentlyAiming = false;

    public float magicNum = 0.1f;

    public int bezierSmoothness = 5;

    public float minTargetY = 3.0f;
    public float maxTargetY = 6.0f;

    // Update is called once per frame
    void Update()
    {
        if (!currentlyAiming) return;

        //if (desiredAltitudeChange != 0)
        {
            desiredAltitude += desiredAltitudeChange * Time.deltaTime * aimUpDownSpeed;
            desiredAltitude = Mathf.Clamp(desiredAltitude, minTargetY, maxTargetY);
            targetPos.position = new Vector3(targetPos.position.x, 
                Mathf.Round(desiredAltitude / 1.5f) * 1.5f + 1.1f, 
                targetPos.position.z);

        }

        if (desiredRotationChange != 0)
        {
            //Debug.Log("Rotating by " + (targetRotation * Time.deltaTime));
            
            transform.Rotate(new Vector3(0, rotSpeed * desiredRotationChange * Time.deltaTime, 0));
            float unclampedRot = transform.rotation.eulerAngles.y;
            if (unclampedRot > 180) unclampedRot -= 360;
            unclampedRot = Mathf.Clamp(unclampedRot, -60, 60);
            transform.rotation = Quaternion.Euler(0, unclampedRot, 0);
        }
        float alpha = transform.rotation.eulerAngles.y;
        if (alpha > 180) alpha -= 360;
        targetPos.position = new Vector3(Mathf.Tan(alpha*magicNum) * maxZ , targetPos.position.y, maxZ);
        lineRenderer.SetPositions(new Vector3[] { dropInPoint.position, targetPos.position });
        SmoothBezier();
    }

    Vector3 startPoint = Vector3.zero;
    Vector3 endPoint = Vector3.zero;

    // Setting Interpolation Points
    Vector3 startHandle = Vector3.zero;
    Vector3 endHandle = Vector3.zero;

    private void OnDrawGizmosSelected()
    {
        Color darkRedColor = new Color(0.5f, 0.1f, 0.1f, 0.8f);
        Gizmos.color = darkRedColor;
        Gizmos.DrawLine(startPoint, startHandle);
        Gizmos.DrawLine(endPoint, endHandle);
    }

    private void SmoothBezier()
    {
        startPoint = lineRenderer.GetPosition(0)+new Vector3(0,1.6f,0);
        endPoint = lineRenderer.GetPosition(1);

        // Setting Interpolation Points
        startHandle = startPoint + new Vector3(0, 1, 0) * endPoint.y;
        endHandle = startHandle;

        lineRenderer.positionCount = bezierSmoothness + 1;
        for (int i = 0; i <= bezierSmoothness; i++)
        {
            float t = (float)i / (float)bezierSmoothness;
            lineRenderer.SetPosition(i, Vector3.Lerp(Vector3.Lerp(startPoint, startHandle, t), Vector3.Lerp(endHandle, endPoint, t), t));
        }

    }

    public void Aim(Vector2 input)
    {
        //Debug.Log("Aiming!" + input);
        float clamped = Mathf.Abs(input.x) > 0.1 ? input.x : 0;
        anim.SetFloat("Rotation", -1 * clamped);
        desiredRotationChange = clamped;
        desiredAltitudeChange = input.y;
    }

    public void ActionEvent_FullyRewound()
    {
        Debug.Log("Catapult ready");
        ready = true;
    }

    public void OnFire(Vector3[] path, int vertexCount)
    {
        StopAiming(transform);
        anim.SetTrigger("Shoot");
    }

    public float desiredAltitudeChange = 0;
    public float desiredAltitude = 3f;
    public float desiredRotationChange = 0;
}
