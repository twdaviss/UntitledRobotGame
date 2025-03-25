using RobotGame.States;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private float staggerTime;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private float healRate;

    private PlayerController playerController;
    private AudioSource audioSource;
    private float currentHealth;
    private float staggerHealth;
    private float currentStaggerHealth;
    private float invincibilityTime;
    private float staggerCooldown = 5.0f;
    private float staggerTimer = 0.0f;
    private float healCooldownTime = 1.0f;
    private bool autoHeal = true;
    private float healCooldownTimer;

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

        staggerTimer -= Time.deltaTime;
        if(staggerTimer <= 0.0f)
        {
            currentStaggerHealth = staggerHealth;
        }

        if (!autoHeal) { return; }
        healCooldownTimer -= Time.deltaTime;
        if(healCooldownTimer <= 0.0f)
        {
            Heal(healRate);
            healCooldownTimer = healCooldownTime;
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

    public void Heal(float health)
    {
        currentHealth += health;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void EnableAutoHeal()
    {
        autoHeal = true;
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
