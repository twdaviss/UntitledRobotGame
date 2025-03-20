using RobotGame.States;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject pfOilSlick;
    [SerializeField] private AudioClip[] metalSounds;

    private float currentHealth;
    private EnemyController enemy;
    private float staggerHealth;
    private float currentStaggerHealth;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        currentHealth = maxHealth;
        staggerHealth = maxHealth * staggerHealthPercentage / 100;
        currentStaggerHealth = staggerHealth;
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
        currentHealth -= damage;
        currentStaggerHealth -= damage;
       
        if (Random.Range(0,4) == 0)
        {
            enemy.ReleaseSparks();
        }
        UpdateHealthBar();

        int randIndex = Random.Range(0,metalSounds.Length);
        float randVolume = Random.Range(0.3f, 0.5f);
        GetComponentInParent<AudioSource>().PlayOneShot(metalSounds[randIndex],randVolume);

        if (currentStaggerHealth < 0.0f)
        {
            Stagger(0.4f);

            currentStaggerHealth = staggerHealth;
        }
        else
        {
            Stagger();
        }
    }

    public void Stagger(float duration = 0.05f)
    {
        enemy.TransitionState(new EnemyStaggered(enemy, duration));
    }
}
