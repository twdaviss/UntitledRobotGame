using RobotGame.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject pfOilSlick;
    [SerializeField] private float absorbShortRadius;
    [SerializeField] private float absorbLongRadius;
    [SerializeField] private float absorbTime;

    private PlayerController playerController;
    public float currentHealth;
    private EnemyController enemy;
    private float staggerHealth;
    private float currentStaggerHealth;
    private float invincibilityTime;
    private float oilSpillDuration = 1.0f;
    private int oilSpillRate = 4;
    private float oilSpillCooldown = 0.0f;
    private float oilSpillTimer = 0.0f;
    private float absorbTimer = 0.0f;

    public bool isHurt = false;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (invincibilityTime > 0.0f) { invincibilityTime -= Time.deltaTime; }
        if (isHurt)
        {
            if (oilSpillCooldown <= 0.0f)
            {
                DealDamage(10);
                oilSpillCooldown = oilSpillDuration / oilSpillRate;
            }
            oilSpillCooldown -= Time.deltaTime;

            oilSpillTimer += Time.deltaTime;
            if(oilSpillTimer > oilSpillDuration)
            {
                oilSpillTimer = 0;
                isHurt = false;
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateHealthBar();
        AbsorbOil();
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
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            if (currentStaggerHealth < 0.0f)
            {
                //playerController.TransitionState(new PlayerStaggered(playerController));
                currentStaggerHealth = staggerHealth;
            }
            DropOil();
            invincibilityTime = 0.1f;
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

    private void AbsorbOil()
    {
        if(maxHealth - currentHealth <= 0) { return; }
        int layerMask = LayerMask.GetMask("Absorb");
        Vector3 position = transform.position;
        position.y -= 1.0f;
        Collider2D[] oilSlicks = Physics2D.OverlapCircleAll(position, absorbLongRadius, layerMask);

        if (oilSlicks.Length == 0)
        {
            absorbTimer = 0.0f;
            return;
        }
        GameObject oil = oilSlicks[0].gameObject;

        for (int i = 0; i < oilSlicks.Length; i++)
        {
            float distToPlayer = Vector2.Distance(position, oilSlicks[i].transform.position);
            float closestDistance = Vector2.Distance(position, oil.transform.position);
            if (distToPlayer < closestDistance)
            {
                oil = oilSlicks[i].gameObject;
            }
            if (absorbTimer > absorbTime && oil != null)
            {
                Destroy(oil.transform.parent.gameObject);
                AddHealth(10);
                absorbTimer = 0.0f;
                continue;
            }
            absorbTimer += Time.deltaTime;
        }
        
    }
    private void DropOil()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.x += Random.Range(-0.25f, 0.25f);
        spawnPos.y += Random.Range(-1.25f, -0.25f);
        Instantiate(pfOilSlick, spawnPos, Quaternion.identity);
    }
}
