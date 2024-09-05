using UnityEngine;
using UnityEngine.Pool;

public class ScrapShot : MonoBehaviour
{
    public int currentAmmo; 
    [SerializeField] public int maxAmmo;

    [SerializeField] private Scrap scrapPrefab;
    [SerializeField] private float scrapSpeed;
    [SerializeField] private float scrapDamage;
    [SerializeField] private float scrapRange;
    [SerializeField] private float magnetizeRadius;

    private ObjectPool<Scrap> scrapPool;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        currentAmmo = maxAmmo;
        GeneratePool();
    }
    private void GeneratePool()
    {
        scrapPool = new ObjectPool<Scrap>(OnCreateScrap,OnPullFromPool, OnReturnToPool, OnDestroyPoolObject,true, maxAmmo, 10);
    }

    public void ShootScrap()
    {
        if (currentAmmo > 0)
        {
            scrapPool.Get();
        }
    }

    private Scrap OnCreateScrap()
    {
        Scrap scrap = Instantiate(scrapPrefab);
        scrap.gameObject.SetActive(false);
        scrap.SetPool(scrapPool);
        
        return scrap;
    }

    private void OnPullFromPool(Scrap scrap)
    {
        scrap.SetParameters(scrapSpeed, scrapDamage, scrapRange, playerController.GetMouseDirection());
        scrap.transform.position = transform.position;
        scrap.gameObject.SetActive(true);
        scrap.inert = false;
        currentAmmo--;
    }

    private void OnReturnToPool(Scrap scrap)
    {
        scrap.gameObject.SetActive(false);
        currentAmmo++;
    }

    private void OnDestroyPoolObject (Scrap scrap)
    {
        Destroy(scrap);
    }

    public void MagnetizeScrap()
    {
        if(playerController.grappleCooldownTimer < playerController.grappleCooldownTime)
        {
            return;
        }
        playerController.grappleCooldownTimer = 0.0f;
        LayerMask layerMask = LayerMask.GetMask("Magnetic");
        Collider2D[] scraps = Physics2D.OverlapCircleAll(transform.position, magnetizeRadius, layerMask);

        foreach(Collider2D scrap in scraps)
        {
            scrap.GetComponentInParent<Scrap>().Magnetize(playerController.gameObject);
        }
    }
}
