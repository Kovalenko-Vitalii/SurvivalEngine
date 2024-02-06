using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHands : MonoBehaviour
{
    [SerializeField] GameObject blank;
    [SerializeField] Invertory_logic inventoryLogic;
    Transform position;
    GameObject inHandsItem;

    [SerializeField] float originalDistance;
    [SerializeField] float originalHeight;
    [SerializeField] float travelDistance;
    [SerializeField] float travelHeight;
    [SerializeField] float aimDuration = 0.5f; // Duration for aiming animation
    [SerializeField] float zoomFactor = 0.8f; // Factor by which to decrease the FOV when zooming

    [SerializeField] HeadBobScript headBob;

    [SerializeField] GameObject postFX;
    public float maxVingette;
    public PostProcessProfile postProcessProfile;

    float initialAmount;

    private bool isAiming = false; // Track if the player is aiming
    private Coroutine aimCoroutine; // Reference to the aiming coroutine
    private Coroutine resetCoroutine = null;

    private Camera mainCamera;
    private float originalFOV;

    private void Start()
    {
        mainCamera = Camera.main;
        originalFOV = mainCamera.fieldOfView;
        initialAmount = headBob.Amount;
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


        if (inventoryLogic.equipedItem_so == null)
        {
             Destroy(inHandsItem);
            inHandsItem = null;
        }
    }

    public void changeItemInHand(Item_so item)
    {
        position = blank.transform;

        if (inHandsItem != null)
        {
            Destroy(inHandsItem);
            inHandsItem = null;
        }

        if (item != null)
        {
            inHandsItem = Instantiate(item.FPSprefab);
            inHandsItem.transform.SetParent(position, false);
        }
    }

    IEnumerator AimCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = blank.transform.localPosition;
        Vector3 targetPosition = new Vector3(travelDistance, travelHeight, startPosition.z);
        float startFOV = mainCamera.fieldOfView;
        float targetFOV = originalFOV * zoomFactor;

        headBob.Amount = 0.001f;
        HeadBobScript.Smooth = HeadBobScript.Smooth / 2;

        postProcessProfile = postFX.GetComponent<PostProcessVolume>().profile;

        while (elapsedTime < aimDuration)
        {
            elapsedTime += Time.deltaTime;
            blank.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / aimDuration);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / aimDuration);

            if (postProcessProfile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.value = Mathf.Lerp(0f, maxVingette, elapsedTime / aimDuration);
            }

            yield return null;
        }

        blank.transform.localPosition = targetPosition; // Ensure final position is reached
        mainCamera.fieldOfView = targetFOV; // Ensure final FOV is reached
    }

    IEnumerator ResetAimCoroutine()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = blank.transform.localPosition;
        Vector3 targetPosition = new Vector3(originalDistance, originalHeight, startPosition.z);
        float startFOV = mainCamera.fieldOfView;
        float targetFOV = originalFOV;

        headBob.Amount = initialAmount;
        HeadBobScript.Smooth = HeadBobScript.Smooth * 2;

        postProcessProfile = postFX.GetComponent<PostProcessVolume>().profile;

        while (elapsedTime < aimDuration)
        {
            elapsedTime += Time.deltaTime;
            blank.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / aimDuration);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / aimDuration);

            if (postProcessProfile.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.value = Mathf.Lerp(maxVingette, 0f, elapsedTime / aimDuration);
            }

            yield return null;
        }

        blank.transform.localPosition = targetPosition; // Ensure final position is reached
        mainCamera.fieldOfView = targetFOV; // Ensure final FOV is reached
    }

    IEnumerator DelayedResetAimCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        resetCoroutine = null; // Reset the coroutine reference
        resetCoroutine = StartCoroutine(ResetAimCoroutine());
    }
}
