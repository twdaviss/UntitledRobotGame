using RobotGame.States;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStun : MonoBehaviour
{
    [SerializeField] private GameObject stunBarBack;
    [SerializeField] private Image stunBar;
    [SerializeField] private float maxStun;
    [SerializeField] private float stunDecayRate;
    [SerializeField] private float stunDecayDelay;
    private float currentStun;
    private float stunDecayTimer = 0;
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
            if(stunDecayTimer >= stunDecayDelay)
            {
                currentStun -= stunDecayRate * Time.deltaTime;
            }
            else
            {
                stunDecayTimer += Time.deltaTime;
            }
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
        currentStun += stun;
        stunDecayTimer = 0;
        UpdateStunBar();
    }
}
