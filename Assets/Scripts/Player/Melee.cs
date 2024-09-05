using RobotGame.States;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    [SerializeField] private float duration;
    [SerializeField] private float staggerTime;

    private PlayerController playerController;
    private float meleeCooldownTime = 1.0f;
    private float meleeCooldownTimer;

    private void Awake()
    {
        meleeCooldownTimer = meleeCooldownTime;
    }
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        meleeCooldownTimer += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        GameManager.Instance.SetMeleeCooldownUI(meleeCooldownTimer/ meleeCooldownTime);
    }

    public void Attack()
    {
        if(meleeCooldownTimer >= meleeCooldownTime)
        {
            playerController.SetState(new PlayerMelee(playerController, radius, damage, knockBack, duration, staggerTime));
            meleeCooldownTimer = 0.0f;
        }
    }

    private void OnEnable()
    {
        InputManager.onMelee += Attack;
    }

    private void OnDestroy()
    {
        InputManager.onMelee -= Attack;
    }
}
