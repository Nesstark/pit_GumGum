using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint; // The object where the laser starts (e.g., gun muzzle)
    public Transform laserRayOrigin; // The origin of the raycast (e.g., the camera)
    private LineRenderer raycastLineRenderer; // The LineRenderer for visualizing the laser
    private Color bloodRed = new Color(0.6f, 0.0f, 0.0f, 1f); // Blood red color

    void Start()
    {
        // Get or add a LineRenderer to the object
        raycastLineRenderer = GetComponent<LineRenderer>();
        if (raycastLineRenderer == null)
        {
            raycastLineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        raycastLineRenderer.startWidth = 0.06f;  // Width of the line at the start
        raycastLineRenderer.endWidth = 0.01f;    // Width of the line at the end
        raycastLineRenderer.enabled = true;      // Enable the laser line

        // Set up an unlit material for consistent coloring
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = bloodRed;
        raycastLineRenderer.material = lineMaterial;
    }

    void FixedUpdate()
    {
        if (laserStartPoint == null || laserRayOrigin == null)
        {
            Debug.LogError("Laser Start Point or Ray Origin is not assigned!");
            return;
        }

        RaycastHit hit; // To store information about the raycast hit
        Vector3 rayDirection = laserRayOrigin.forward; // Ray direction from the ray origin

        // Perform the raycast
        if (Physics.Raycast(laserRayOrigin.position, rayDirection, out hit, 100f))
        {
            Debug.DrawRay(laserRayOrigin.position, rayDirection * hit.distance, bloodRed);

            // Update the LineRenderer positions
            raycastLineRenderer.SetPosition(0, laserStartPoint.position); // Laser starts at the chosen object
            raycastLineRenderer.SetPosition(1, hit.point); // Laser ends where the ray hits
        }
        else
        {
            // If the ray doesn't hit anything, extend it 100 units forward
            raycastLineRenderer.SetPosition(0, laserStartPoint.position); // Laser starts at the chosen object
            raycastLineRenderer.SetPosition(1, laserRayOrigin.position + rayDirection * 100f); // Laser ends forward
        }

        // Ensure the laser maintains its color
        raycastLineRenderer.startColor = bloodRed;
        raycastLineRenderer.endColor = bloodRed;
    }
}
