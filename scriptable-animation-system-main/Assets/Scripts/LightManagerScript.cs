using System.Collections.Generic;
using UnityEngine;

public class LightManagerScript : MonoBehaviour
{
    public GameObject player;
    public float maxDistance = 10f; // Maximum distance for a light to be considered visible
    public float updateInterval = 0.5f; // Time interval to check light visibility
    public LayerMask lightLayer; // LayerMask for the lights

    [SerializeField] private List<Light> lights;
    private float updateTimer;

    private void Start()
    {
        // Get all the lights in the scene with the "Light" tag
        lights = GetLightsWithTag("Light");
    }

    private void Update()
    {
        // Update the timer
        updateTimer += Time.deltaTime;

        // Check light visibility at regular intervals
        if (updateTimer >= updateInterval)
        {
            CheckLightVisibility();
            updateTimer = 0f;
        }
    }

    private void CheckLightVisibility()
    {
        foreach (Light light in lights)
        {
            // Calculate the distance between the light and the player
            float distance = Vector3.Distance(light.transform.position, player.transform.position);

            // Disable the light if it's beyond the maximum distance or not visible to the player
            if (distance < maxDistance || IsLightVisibleToPlayer(light))
            {
                light.enabled = true;
            }
            else
            {
                light.enabled = false;
            }
        }
    }

    private bool IsLightVisibleToPlayer(Light light)
    {
        // Get the direction from the player to the light
        Vector3 direction = light.transform.position - player.transform.position;

        // Cast a ray from the player towards the light
        if (Physics.Raycast(player.transform.position, direction, out RaycastHit hit, maxDistance))
        {
            // Check if the hit object has a collider associated with the light
            if (hit.collider != null && hit.collider.GetComponent<Light>() == light)
            {
                return true;
            }
        }

        return false;
    }

    private List<Light> GetLightsWithTag(string tag)
    {
        // Find all objects with the specified tag
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        // Create a list to store the lights
        List<Light> lightList = new List<Light>();

        // Iterate through the objects and add the lights to the list
        foreach (GameObject obj in objects)
        {
            Light light = obj.GetComponent<Light>();
            if (light != null)
            {
                lightList.Add(light);
            }
        }

        return lightList;
    }
}