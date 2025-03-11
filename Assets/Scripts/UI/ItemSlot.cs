using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;

    public UIInventory inventory;
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private Outline outline;  // ���� �׵θ� ȿ��

    public int index;
    public bool equipped;
    public int quantity;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        quantityText = GetComponentInChildren<TextMeshProUGUI>();

        if (quantityText == null)
        {
            Debug.LogError("[ItemSlot.Awake] quatityText null ����");
        }
    }

    private void OnEnable()
    {
        outline.enabled = equipped;  // ������ ��� �׵θ� Ȱ��ȭ
    }

    public void Set()
    {
        if (icon == null)
        {
            Debug.LogError("[ItemSlot.Set] icon�� null�Դϴ�!");
            return;
        }

        if (item == null)
        {
            Debug.LogError("[ItemSlot.Set] item�� null�Դϴ�!");
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;

        if (icon != null)
            icon.gameObject.SetActive(false);
        else
            Debug.LogError("[ItemSlot.Clear] icon�� null�Դϴ�!");

        if (quantityText != null)
            quantityText.text = string.Empty;
        else
            Debug.LogError("[ItemSlot.Clear] quatityText�� null�Դϴ�!");
    }


    public void OnClickButton()
    {
        inventory.SelectItem(index);  // ������ ������ �ε����� �κ��� ����
    }
}