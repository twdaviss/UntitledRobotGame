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
        if(damage > maxHealth/10)
        {
            //DropOil();
        }
        if(currentStaggerHealth < 0.0f)
        {
            Stagger();
            currentStaggerHealth = staggerHealth;
        }
        if(Random.Range(0,4) == 0)
        {
            enemy.ReleaseSparks();
        }
        UpdateHealthBar();

        int randIndex = Random.Range(0,metalSounds.Length);
        float randVolume = Random.Range(0.3f, 0.5f);
        GetComponentInParent<AudioSource>().PlayOneShot(metalSounds[randIndex],randVolume);

        if(enemy.enemyType == EnemyType.Shy)
        {
            enemy.SetState(new EnemyFlee(enemy, enemy.fleeTime));
        }
    }

    public void Stagger()
    {
        enemy.TransitionState(new EnemyStaggered(enemy));
    }

    private void DropOil()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.x += Random.Range(-0.25f, 0.25f);
        spawnPos.y += Random.Range(-1.25f, -0.25f);
        Instantiate(pfOilSlick, spawnPos, Quaternion.identity);
    }
}
