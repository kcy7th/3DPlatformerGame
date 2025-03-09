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
        currentDefense = baseDefense;
        currentAttackPower = baseAttackPower;
    }

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

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
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        float finalDamage = damageAmount - currentDefense;
        if (finalDamage < 0) finalDamage = 0;

        health.Subtract(finalDamage);
        onTakeDamage?.Invoke();
    }

    public void ApplyEffect(ItemData item)
    {
        if (item.effectType == EffectType.None) return;

        Debug.Log($"{item.displayName} 장착됨. 효과: {item.effectType} +{item.effectValue} ({item.duration}초)");

        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense += item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower += item.effectValue;
                break;
        }

        if (activeEffectCoroutine != null)
        {
            StopCoroutine(activeEffectCoroutine);
        }
        activeEffectCoroutine = StartCoroutine(RemoveEffectAfterDuration(item));
    }

    private IEnumerator RemoveEffectAfterDuration(ItemData item)
    {
        yield return new WaitForSeconds(item.duration);

        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense -= item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower -= item.effectValue;
                break;
        }

        Debug.Log($"{item.displayName} 효과 끝");
        activeEffectCoroutine = null;
    }

    public void RemoveItemEffect(ItemData item)
    {
        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense -= item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower -= item.effectValue;
                break;
        }
    }

    public float GetDefense() => currentDefense;
    public float GetAttackPower() => currentAttackPower;
}
