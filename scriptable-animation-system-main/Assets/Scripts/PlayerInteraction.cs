using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 5f;
    public LayerMask interactionLayer;
    public KeyCode pickupKey = KeyCode.F;
    public GameObject pickupUI;
    //public Inventory inventory;
    private Camera mainCamera;
    private bool canInteract;
    private Interactable currentInteractable;

    private void Start()
    {
        mainCamera = Camera.main;
        HidePickupUI();
    }

    private void Update()
    {
        CheckInteraction();

        if (canInteract && Input.GetKeyDown(pickupKey))
        {
            PickUpItem();
        }
    }

    private void CheckInteraction()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                if (!canInteract)
                {
                    ShowPickupUI();
                }

                canInteract = true;
                currentInteractable = interactable;
                
            }
        }
        else
        {
            if (canInteract)
            {
                HidePickupUI();
            }

            canInteract = false;
            currentInteractable = null;
        }
    }

    private void ShowPickupUI()
    {
        pickupUI.SetActive(true);
    }

    private void HidePickupUI()
    {
        pickupUI.SetActive(false);
    }

    // Pick up the item
    void PickUpItem()
    {
        Item item = currentInteractable.GetItem();
        Debug.Log("Picking up " + item.name);
        bool wasPickedUp = Inventory.instance.Add(item);    // Add to inventory

        // If successfully picked up
        if (wasPickedUp)
            Destroy(gameObject);    // Destroy item from scene
    }
}