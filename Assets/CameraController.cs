using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    
    // Define the boundaries of the player's movement
    public Vector3 playerMinPos;  // Minimum x, y, and z position of the player
    public Vector3 playerMaxPos;  // Maximum x, y, and z position of the player
    
    // Define the boundaries of the camera's movement
    public Vector3 cameraMinPos;  // Minimum x, y, and z position of the camera
    public Vector3 cameraMaxPos;  // Maximum x, y, and z position of the camera

    // Damping factor
    public float damping = 0.1f;
    
    void LateUpdate()
    {
        // Get player's current position
        Vector3 playerPos = playerTransform.position;

        // Calculate target camera position using Lerp
        float targetPosX = Mathf.Lerp(cameraMinPos.x, cameraMaxPos.x, Mathf.InverseLerp(playerMinPos.x, playerMaxPos.x, playerPos.x));
        float targetPosY = Mathf.Lerp(cameraMinPos.y, cameraMaxPos.y, Mathf.InverseLerp(playerMinPos.y, playerMaxPos.y, playerPos.y));
        float targetPosZ = Mathf.Lerp(cameraMinPos.z, cameraMaxPos.z, Mathf.InverseLerp(playerMinPos.z, playerMaxPos.z, playerPos.z));

        Vector3 targetPos = new Vector3(targetPosX, targetPosY, targetPosZ);

        // Calculate the distance to the target position
        float distance = Vector3.Distance(transform.position, targetPos);

        // Calculate the speed based on the distance and damping factor
        float speed = distance * (damping * 10f);

        // Smoothly move the camera towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
