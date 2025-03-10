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
        // 기본 방어력 및 공격력
        currentDefense = baseDefense;
        currentAttackPower = baseAttackPower;
    }

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        // 배고픔이 0일 경우 체력 감소
        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        // 체력이 0 되면 플레이어 사망
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
        float finalDamage = damageAmount - currentDefense;  // 방어력이 데미지를 초과하지 않게

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

        // 기존 효과 있을 시 중지 후 새 효과 적용
        if (activeEffectCoroutine != null)
        {
            StopCoroutine(activeEffectCoroutine);
        }
        activeEffectCoroutine = StartCoroutine(RemoveEffectAfterDuration(item));
    }

    private IEnumerator RemoveEffectAfterDuration(ItemData item)
    {
        yield return new WaitForSeconds(item.duration);

        // 효과 해제
        RemoveItemEffect(item);
        activeEffectCoroutine = null;
    }

    public void RemoveItemEffect(ItemData item)
    {
        if (item == null) return; // 예외 처리: 아이템이 null이면 실행하지 않음

        // 효과 해제
        switch (item.effectType)
        {
            case EffectType.DefenseBoost:
                currentDefense -= item.effectValue;
                break;
            case EffectType.AttackBoost:
                currentAttackPower -= item.effectValue;
                break;
        }

        Debug.Log($"{item.displayName} 효과 해제됨!");
    }

    public float GetDefense() => currentDefense; // 현재 방어력 반환
    public float GetAttackPower() => currentAttackPower; // 현재 공격력 반환
}
