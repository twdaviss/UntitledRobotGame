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
    public void UpdateHealthBar(float newHealth, float maxHealth)
    {
        healthBar.fillAmount = newHealth/maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectiles") && collision.enabled)
        {
            currentHealth -= collision.GetComponentInParent<Scrap>().GetDamage();
            collision.GetComponentInParent<Scrap>().ClampVelocity();
            collision.enabled = false;
            UpdateHealthBar(currentHealth, maxHealth);
        }
    }
}
