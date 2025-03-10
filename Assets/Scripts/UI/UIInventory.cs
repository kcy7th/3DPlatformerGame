using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex = -1; // 기본값 설정

    private PlayerController controller;
    private PlayerCondition condition;

    private Coroutine activeEffectCoroutine; // 현재 효과의 Coroutine 저장

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory -= Toggle;
        controller.inventory += Toggle;

        CharacterManager.Instance.Player.addItem += AddItem;

        // 인벤토리 초기화
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen())
        {
            Debug.Log("인벤토리 닫힘");
            inventoryWindow.SetActive(false);
        }
        else
        {
            Debug.Log("인벤토리 열림");
            inventoryWindow.SetActive(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !slots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && slots[index].equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                }
            }
            RemoveSelctedItem();
        }
    }

    public void OnEquipButton()
    {
        if (selectedItem == null || selectedItem.item == null) return;

        if (curEquipIndex != -1)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;

        ApplyItemEffect(selectedItem.item);

        Debug.Log($"{selectedItem.item.displayName} 장착됨");

        equipButton.SetActive(false);
        unEquipButton.SetActive(true);

        UpdateUI();
    }
    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelctedItem();
    }

    public void UnEquip(int index)
    {
        if (index == -1 || slots[index].item == null || !slots[index].equipped) return;

        slots[index].equipped = false;
        curEquipIndex = -1;

        RemoveItemEffect(slots[index].item);

        Debug.Log($"{slots[index].item.displayName} 장착 해제됨");

        equipButton.SetActive(true);
        unEquipButton.SetActive(false);

        UpdateUI();
    }

    void RemoveSelctedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].equipped)
            {
                UnEquip(selectedItemIndex);
            }

            selectedItem.item = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    private void ApplyItemEffect(ItemData item)
    {
        if (activeEffectCoroutine != null)
        {
            StopCoroutine(activeEffectCoroutine);
        }
        activeEffectCoroutine = StartCoroutine(TemporaryEffect(item));
    }

    private IEnumerator TemporaryEffect(ItemData item)
    {
        Debug.Log($"{item.displayName} 효과 적용: {item.effectType} +{item.effectValue} ({item.duration}초 동안)");

        if (item.effectType == EffectType.DefenseBoost)
        {
            condition.currentDefense += item.effectValue;
        }
        else if (item.effectType == EffectType.AttackBoost)
        {
            condition.currentAttackPower += item.effectValue;
        }

        yield return new WaitForSeconds(item.duration);

        RemoveItemEffect(item);
        activeEffectCoroutine = null;
    }

    private void RemoveItemEffect(ItemData item)
    {
        Debug.Log($"{item.displayName} 효과 해제");

        if (item.effectType == EffectType.DefenseBoost)
        {
            condition.currentDefense -= item.effectValue;
        }
        else if (item.effectType == EffectType.AttackBoost)
        {
            condition.currentAttackPower -= item.effectValue;
        }
    }
}
