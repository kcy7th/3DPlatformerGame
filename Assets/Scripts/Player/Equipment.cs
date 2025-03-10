using UnityEngine;

public class Equipment : MonoBehaviour
{
    private ItemData currentItem;

    public void EquipNew(ItemSlot itemSlot)
    {
        if (currentItem != null)
        {
            UnEquip(); // 기존 장착 아이템 해제
        }

        currentItem = itemSlot.item;
        CharacterManager.Instance.Player.condition.ApplyEffect(currentItem); // 효과 적용

        Debug.Log($"{currentItem.displayName} 장착됨!");
    }

    public void UnEquip()
    {
        if (currentItem != null)
        {
            CharacterManager.Instance.Player.condition.RemoveItemEffect(currentItem); // 즉시 효과 제거
            Debug.Log($"{currentItem.displayName} 장착 해제됨!");
            currentItem = null;
        }
    }
}
