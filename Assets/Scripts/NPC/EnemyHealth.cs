using RobotGame.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    private float currentHealth;
    private EnemyController enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (currentHealth <= 0)
        {
            enemy.SetState(new EnemyDying(enemy));
        }
    }
    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth/maxHealth;
    }

    public void DealDamage(float damage)
    {
        if(enemy.invincibilityTime <= 0.0f)
        {
            currentHealth -= damage;
            UpdateHealthBar();
            enemy.invincibilityTime = 0.1f;
        }
    }
}
