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
    private Outline outline;  // 슬롯 테두리 효과

    public int index;
    public bool equipped;
    public int quantity;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        quantityText = GetComponentInChildren<TextMeshProUGUI>();

        if (quantityText == null)
        {
            Debug.LogError("[ItemSlot.Awake] quatityText null 상태");
        }
    }

    private void OnEnable()
    {
        outline.enabled = equipped;  // 장착된 경우 테두리 활성화
    }

    public void Set()
    {
        if (icon == null)
        {
            Debug.LogError("[ItemSlot.Set] icon이 null입니다!");
            return;
        }

        if (item == null)
        {
            Debug.LogError("[ItemSlot.Set] item이 null입니다!");
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
            Debug.LogError("[ItemSlot.Clear] icon이 null입니다!");

        if (quantityText != null)
            quantityText.text = string.Empty;
        else
            Debug.LogError("[ItemSlot.Clear] quatityText가 null입니다!");
    }


    public void OnClickButton()
    {
        inventory.SelectItem(index);  // 선택한 슬롯의 인덱스를 인벤에 전달
    }
}