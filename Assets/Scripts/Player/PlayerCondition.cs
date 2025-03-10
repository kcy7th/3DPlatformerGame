using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;

    public float baseDefense = 0f;
    public float baseAttackPower = 1f;

    public float currentDefense;
    public float currentAttackPower;

    private Coroutine activeEffectCoroutine;

    private void Start()
    {
        // �⺻ ���� �� ���ݷ�
        currentDefense = baseDefense;
        currentAttackPower = baseAttackPower;
    }

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        // ������� 0�� ��� ü�� ����
        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        // ü���� 0 �Ǹ� �÷��̾� ���
        if (health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("�÷��̾ �׾���.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        float finalDamage = damageAmount - currentDefense;  // ������ �������� �ʰ����� �ʰ�

        if (finalDamage < 0) finalDamage = 0;

        health.Subtract(finalDamage);
        onTakeDamage?.Invoke();
    }

    public void ApplyEffect(ItemData item)
    {
        if (item.effectType == EffectType.None) return;

        Debug.Log($"{item.displayName} ������. ȿ��: {item.effectType} +{item.effectValue} ({item.duration}��)");

        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense += item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower += item.effectValue;
                break;
        }

        // ���� ȿ�� ���� �� ���� �� �� ȿ�� ����
        if (activeEffectCoroutine != null)
        {
            StopCoroutine(activeEffectCoroutine);
        }
        activeEffectCoroutine = StartCoroutine(RemoveEffectAfterDuration(item));
    }

    private IEnumerator RemoveEffectAfterDuration(ItemData item)
    {
        yield return new WaitForSeconds(item.duration);

        // ȿ�� ����
        RemoveItemEffect(item);
        activeEffectCoroutine = null;
    }

    public void RemoveItemEffect(ItemData item)
    {
        if (item == null) return; // ���� ó��: �������� null�̸� �������� ����

        // ȿ�� ����
        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense -= item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower -= item.effectValue;
                break;
        }

        Debug.Log($"{item.displayName} ȿ�� ������!");
    }

    public float GetDefense() => currentDefense; // ���� ���� ��ȯ
    public float GetAttackPower() => currentAttackPower; // ���� ���ݷ� ��ȯ
}
