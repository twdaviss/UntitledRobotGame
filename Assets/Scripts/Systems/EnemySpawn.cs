using RobotGame.States;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private int numToSpawn;
    [SerializeField] private float spawnTime;

    private bool isTriggerd = false;
    private float spawnTimer = 0.0f;

    private void Start()
    {
        spawnTimer = spawnTime;
    }
    void Update()
    {
        if (!isTriggerd || numToSpawn <= 0)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if(spawnTimer < spawnTime)
        {
            return;
        }

        int index = Random.Range(0, enemies.Length);
        Vector2 direction = Random.insideUnitCircle;
        Vector3 spawnPosition = transform.position;
        spawnPosition.x += direction.x * 10.0f;
        spawnPosition.y += direction.y * 10.0f;
        GameObject enemyObject = Instantiate(enemies[index], transform.position, enemies[index].transform.rotation);
        EnemyController enemy = enemyObject.GetComponent<EnemyController>();
        enemy.TransitionState(new EnemyKnockback(enemy, 40, direction));
        spawnTimer = 0.0f;
        numToSpawn--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            isTriggerd = true;
        }
    }
}
