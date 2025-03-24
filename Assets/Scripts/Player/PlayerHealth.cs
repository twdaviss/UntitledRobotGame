using RobotGame.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private float staggerTime;
    [SerializeField] private GameObject pfOilSlick;
    [SerializeField] private float absorbShortRadius;
    [SerializeField] private float absorbLongRadius;
    [SerializeField] private float absorbTime;
    [SerializeField] private AudioClip hurtSound;

    private PlayerController playerController;
    private AudioSource audioSource;
    private float currentHealth;
    private EnemyController enemy;
    private float staggerHealth;
    private float currentStaggerHealth;
    private float invincibilityTime;
    private float oilSpillCooldown = 0.0f;
    private float staggerCooldown = 5.0f;
    private float staggerTimer = 0.0f;

    public bool isHurt = false;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        audioSource = GetComponentInParent<AudioSource>();
        currentHealth = maxHealth;
        staggerHealth = maxHealth * ((float)staggerHealthPercentage/100);
        currentStaggerHealth = staggerHealth;
    }

    private void Update()
    {
        if (invincibilityTime > 0.0f) { invincibilityTime -= Time.deltaTime; }
        else
        {
            invincibilityTime = 0.0f;
        }
        if (isHurt)
        {
            if (oilSpillCooldown <= 0.0f)
            {
                DealDamage(10);
            }
            
            if(staggerTimer <= 0.0f)
            {
                currentStaggerHealth = staggerHealth;
            }
            staggerTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        GameManager.Instance.UpdateHealthBar(currentHealth / maxHealth);
    }

    public void DealDamage(float damage)
    {
        if (invincibilityTime <= 0.0f)
        {
            currentHealth -= damage;
            currentStaggerHealth -= damage;
            staggerTimer = staggerCooldown;
            Debug.Log("Stagger Health: " + currentStaggerHealth);
            if (currentHealth < 0)
            {
                currentHealth = 0;
                PlayerDeath();
            }
            if (currentStaggerHealth < 0.0f)
            {
                playerController.TransitionState(new PlayerStaggered(playerController, staggerTime));
                currentStaggerHealth = staggerHealth;
                staggerTimer = 0.0f;
            }
            //DropOil();
            audioSource.PlayOneShot(hurtSound);
            invincibilityTime = 0.2f;
        }
    }

    public void AddHealth(float health)
    {
        currentHealth += health;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void PlayerDeath()
    {
        SceneManager.LoadScene(0);
    }
}
