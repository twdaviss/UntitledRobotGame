using RobotGame.States;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStun : MonoBehaviour
{
    [SerializeField] private GameObject stunBarBack;
    [SerializeField] private Image stunBar;
    [SerializeField] private float maxStun;
    private float currentStun;
    private EnemyController enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
        currentStun = 0;
    }
    private void Update()
    {
        if(currentStun <= 0)
        {
            stunBarBack.SetActive(false);
        }
        else if (currentStun > 0)
        {
            stunBarBack.SetActive(true);
        }
        if (currentStun >= maxStun)
        {
            //enemy.SetState(new EnemyStunned(enemy));
        }
        UpdateStunBar();
    }
    public void UpdateStunBar()
    {
        stunBar.fillAmount = currentStun/maxStun;
    }

    public void DealDamage(float stun)
    {
        if(enemy.invincibilityTime <= 0.0f)
        {
            currentStun -= stun;
            
            UpdateStunBar();
            enemy.invincibilityTime = 0.1f;
        }
    }
}
