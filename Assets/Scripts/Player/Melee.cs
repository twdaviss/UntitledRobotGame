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

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void Attack()
    {
        playerController.SetState(new PlayerMelee(playerController, radius, damage, knockBack, duration, staggerTime));
    }
}
