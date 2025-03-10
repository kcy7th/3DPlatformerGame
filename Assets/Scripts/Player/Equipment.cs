using UnityEngine;

public class Equipment : MonoBehaviour
{
    private ItemData currentItem;

    public void EquipNew(ItemSlot itemSlot)
    {
        if (currentItem != null)
        {
            UnEquip();
        }

        currentItem = itemSlot.item;
        CharacterManager.Instance.Player.condition.ApplyEffect(currentItem);

        Debug.Log($"{currentItem.displayName} ������!");
    }

    public void UnEquip()
    {
        if (currentItem != null)
        {
            CharacterManager.Instance.Player.condition.RemoveItemEffect(currentItem);
            Debug.Log($"{currentItem.displayName} ���� ������!");
            currentItem = null;
        }
    }
}
