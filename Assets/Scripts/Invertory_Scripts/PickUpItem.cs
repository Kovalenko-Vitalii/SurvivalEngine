using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Swapping_UI_Layers SUIL;
    public Invertory_logic inventoryLogic;
    private GameObject highlightedObject;

    [Header("Sounds")]

    public AudioSource audioSource;
    public AudioClip pickupSound;
    private void Update(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.gameObject != highlightedObject)
            {
                if (highlightedObject != null)
                {
                    Outline outline = highlightedObject.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                }

                highlightedObject = hit.collider.gameObject;

                if (highlightedObject.tag == "Item")
                {
                    SUIL.pickUpImage.SetActive(true);
                    Outline outline = highlightedObject.GetComponent<Outline>();
                    if (outline != null)
                    {
                        outline.enabled = true;
                    }
                }
                else
                {
                    SUIL.pickUpImage.SetActive(false);
                }
            }

            if (hit.collider.tag == "Item" && Input.GetKeyDown(KeyCode.F) && inventoryLogic._invertoryList.Count < 12)
            {
                inventoryLogic.pickUpItem(hit.collider.gameObject.GetComponent<Ite_so_Holder>().itemSO);
                Destroy(hit.collider.gameObject);
                highlightedObject = null;
                audioSource.PlayOneShot(pickupSound);
            }
        }
        else
        {
            if (highlightedObject != null)
            {
                Outline outline = highlightedObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }

            highlightedObject = null;
            SUIL.pickUpImage.SetActive(false);
        }
    }
}
