using UnityEngine;

public class flashlight : MonoBehaviour
{
    private Light flashlightLight;

    void Start()
    {
        // Get the Light component attached to the game object
        flashlightLight = transform.GetChild(0).GetComponent<Light>();

        // Start with the light turned off
        flashlightLight.enabled = false;
    }

    void Update()
    {
        // Check if the left mouse button is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            // Toggle the light on or off based on its current state
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }
}
