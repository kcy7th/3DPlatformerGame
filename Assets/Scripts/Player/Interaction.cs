using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRadius = 2f;  // 상호작용 가능한 거리
    public LayerMask layerMask;  

    private GameObject curInteractGameObject;  // 감지된 오브젝트
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
        else  // 상호작용 가능한 오브젝트가 없을 경우
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

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
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
