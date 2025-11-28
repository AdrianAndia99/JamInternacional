using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] protected bool isInvulnerable;
    [SerializeField] protected float currentHealth;

    protected float maxHealth;
    
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnTakeDamage;
    public UnityEvent<float> OnHeal;
    public UnityEvent OnDeath;

    private void OnEnable()
    {
        ResetHealth();
    }
    public virtual void SetUp(float _maxHealth)
    {
        maxHealth = _maxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnTakeDamage?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHeal?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
    protected void Die()
    {
        OnDeath?.Invoke();
    }
    public void SetInvulnerability(bool state)
    {
        isInvulnerable = state;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}