using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Zooming : MonoBehaviour
{
    [SerializeField] float aimDuration = 0.5f; // Duration for aiming animation
    [SerializeField] float zoomFactor = 0.8f; // Factor by which to decrease the FOV when zooming


    [SerializeField] GameObject postFX;
    public float maxVingette;
    PostProcessProfile postProcessProfile;

    private bool isAiming = false; // Track if the player is aiming
    private Coroutine aimCoroutine; // Reference to the aiming coroutine
    private Coroutine resetCoroutine = null;

    public CinemachineVirtualCamera cinemachine;
    private float originalFOV;
    private void Start()
    {
        originalFOV = cinemachine.m_Lens.FieldOfView;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAiming)
            {
                isAiming = true; // Set isAiming to true when RMB is pressed
                if (resetCoroutine != null)
                    StopCoroutine(resetCoroutine); // Stop the reset coroutine if it's running
                aimCoroutine = StartCoroutine(AimCoroutine());
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (isAiming)
            {
                isAiming = false; // Set isAiming to false when RMB is released
                if (aimCoroutine != null)
                    StopCoroutine(aimCoroutine); // Stop the aiming coroutine if it's running
                resetCoroutine = StartCoroutine(DelayedResetAimCoroutine());
            }
        }
    }

    IEnumerator AimCoroutine()
    {
        float elapsedTime = 0f;
        float startFOV = cinemachine.m_Lens.FieldOfView;
        float targetFOV = originalFOV * zoomFactor;

        postProcessProfile = postFX.GetComponent<PostProcessVolume>().profile;

        while (elapsedTime < aimDuration)
        {
            elapsedTime += Time.deltaTime;
            cinemachine.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / aimDuration);

            if (postProcessProfile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.value = Mathf.Lerp(0f, maxVingette, elapsedTime / aimDuration);
            }

            yield return null;
        }
        cinemachine.m_Lens.FieldOfView = targetFOV; // Ensure final FOV is reached
    }

    IEnumerator ResetAimCoroutine()
    {
        float elapsedTime = 0f;
        float startFOV = cinemachine.m_Lens.FieldOfView;
        float targetFOV = originalFOV;

        postProcessProfile = postFX.GetComponent<PostProcessVolume>().profile;

        while (elapsedTime < aimDuration)
        {
            elapsedTime += Time.deltaTime;
            cinemachine.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / aimDuration);

            if (postProcessProfile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.value = Mathf.Lerp(maxVingette, 0f, elapsedTime / aimDuration);
            }

            yield return null;
        }
        cinemachine.m_Lens.FieldOfView = targetFOV; // Ensure final FOV is reached
    }

    IEnumerator DelayedResetAimCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        resetCoroutine = null; // Reset the coroutine reference
        resetCoroutine = StartCoroutine(ResetAimCoroutine());
    }

}
