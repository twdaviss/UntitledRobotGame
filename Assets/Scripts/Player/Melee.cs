using RobotGame.States;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float stun;
    [SerializeField] private float knockBack;
    [SerializeField] private float duration;
    [SerializeField] private AudioClip swoosh;

    private PlayerController playerController;
    private float meleeCooldownTime = 1.0f;
    private float meleeCooldownTimer;
    private bool isDealingDamage = false;

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
        if(isDealingDamage)
        {
            int layerMask = LayerMask.GetMask("Enemies");

            Collider2D[] targets = Physics2D.OverlapCircleAll(playerController.transform.position, radius, layerMask);
            foreach (Collider2D target in targets)
            {
                if (target.gameObject.GetComponent<EnemyController>() != null)
                {
                    target.gameObject.GetComponent<EnemyController>().Damage(damage, stun, knockBack, (target.transform.position - playerController.transform.position).normalized);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        GameManager.Instance.SetMeleeCooldownUI(meleeCooldownTimer/ meleeCooldownTime);
    }

    public void StartDealDamage()
    {
        isDealingDamage = true;
    }

    public void StopDealDamage()
    {
        isDealingDamage = false;
    }

    public void TryAttack()
    {
        if (playerController.GetCurrentState() == "PlayerGrappling")
        {
            return;
        }
        Attack();
    }

    public void Attack()
    {
        if (meleeCooldownTimer >= meleeCooldownTime)
        {
            meleeCooldownTimer = 0.0f;
            GetComponentInParent<AudioSource>().PlayOneShot(swoosh);
            playerController.TransitionState(new PlayerMelee(playerController, radius, damage, stun, knockBack, duration));
            playerController.playerAnimator.SetBool("isMeleeing", true);
        }
    }

}
