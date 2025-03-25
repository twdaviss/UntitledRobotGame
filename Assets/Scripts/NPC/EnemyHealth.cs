using RobotGame.States;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private AudioClip[] metalSounds;

    private float currentHealth;
    private EnemyController enemy;
    private float staggerHealth;
    private float invincibilityTime;
    private float currentStaggerHealth;

    public bool isGrappled = false;
    public float grappledTime = 1.0f;
    public float grappledTimer = 0.0f;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        currentHealth = maxHealth;
        staggerHealth = maxHealth * staggerHealthPercentage / 100;
        currentStaggerHealth = staggerHealth;
    }
    private void Update()
    {
        if (invincibilityTime > 0.0f) { invincibilityTime -= Time.deltaTime; }
        else
        {
            invincibilityTime = 0.0f;
        }

        if (isGrappled )
        {
            grappledTimer += Time.deltaTime;
            if(grappledTimer > grappledTime)
            {
                isGrappled = false;
                grappledTimer = 0;
            }
        }

        if (currentHealth <= 0)
        {
            enemy.SetState(new EnemyDying(enemy));
        }
    }
    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth/maxHealth;
    }

    public void DealDamage(float damage, float knockBack, Vector2 direction)
    {
        if (invincibilityTime <= 0.0f)
        {
            if(isGrappled)
            {
                damage *= 1.5f;
            }
            if(enemy.isStunned)
            {
                damage *= 2;
            }
            Debug.Log("Damage Dealt: " + damage);

            currentHealth -= damage;
            currentStaggerHealth -= damage;

            UpdateHealthBar();

            int randIndex = Random.Range(0, metalSounds.Length);
            float randVolume = Random.Range(0.3f, 0.5f);
            GetComponentInParent<AudioSource>().PlayOneShot(metalSounds[randIndex], randVolume);

            if (knockBack > 0)
            {
                enemy.TransitionState(new EnemyKnockback(enemy, knockBack, direction));
            }

            else if (currentStaggerHealth < 0.0f)
            {
                currentStaggerHealth = staggerHealth;

                Stagger(0.4f);
            }
            else
            {
                Stagger();
            }
            invincibilityTime = 0.2f;
        }
    }

    public void Stagger(float duration = 0.1f)
    {
        enemy.TransitionState(new EnemyStaggered(enemy, duration));
    }
}
