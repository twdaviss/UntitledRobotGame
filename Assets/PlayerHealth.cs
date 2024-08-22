using RobotGame.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int staggerHealthPercentage;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject pfOilSlick;
    [SerializeField] private float absorbRadius;
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
        if (isHurt)
        {
            if (oilSpillCooldown <= 0.0f)
            {
                DropOil();
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
        int layerMask = LayerMask.GetMask("Absorb");
        Collider2D[] oilSlicks = Physics2D.OverlapCircleAll(transform.position, absorbRadius, layerMask);

        if(oilSlicks.Length == 0) 
        {
            absorbTimer = 0.0f;
            return; 
        }

        if (absorbTimer < absorbTime)
        {
            absorbTimer += Time.deltaTime;
        }
        else
        {
            GameObject oil = null;
            for(int i = 0; i < oilSlicks.Length; i++)
            {
                if(oil == null)
                {
                    oil = oilSlicks[i].gameObject;
                }
                else if(Vector2.Distance(transform.position, oilSlicks[i].transform.position) < Vector2.Distance(transform.position, oil.transform.position))
                {
                    oil = oilSlicks[i].gameObject;
                }
            }
            if(oil != null)
            {
                Destroy(oil);
                AddHealth(10);
                absorbTimer = 0.0f;
            }
        }
    }

    public void UpdateHealthBar()
    {
        //healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void DealDamage(float damage)
    {
        if (invincibilityTime <= 0.0f)
        {
            currentHealth -= damage;
            currentStaggerHealth -= damage;
            if (currentStaggerHealth < 0.0f)
            {
                //playerController.TransitionState(new PlayerStaggered(playerController));
                currentStaggerHealth = staggerHealth;
            }
            UpdateHealthBar();
            invincibilityTime = 0.1f;
        }
    }

    public void AddHealth(float health)
    {
        currentHealth += health;
        UpdateHealthBar(); 
    }

    private void DropOil()
    {
        Instantiate(pfOilSlick, transform.position, Quaternion.identity);
    }
}
