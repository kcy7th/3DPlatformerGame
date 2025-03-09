using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRadius = 2f;
    public LayerMask layerMask;  

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;

    public TextMeshProUGUI promptText;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, layerMask);

        if (hitColliders.Length > 0)
        {
            if (curInteractGameObject != hitColliders[0].gameObject)
            {
                curInteractGameObject = hitColliders[0].gameObject;
                curInteractable = curInteractGameObject.GetComponent<IInteractable>();
                ShowPrompt(); 
            }
        }
        else
        {
            HidePrompt(); 
        }
    }

    private void ShowPrompt()
    {
        if (curInteractable != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = curInteractable.GetInteractPrompt();
        }
    }

    private void HidePrompt()
    {
        curInteractGameObject = null;
        curInteractable = null;
        promptText.gameObject.SetActive(false);
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && curInteractable != null)
        {
            curInteractable.OnInteract();
            HidePrompt();  
        }
    }
}
