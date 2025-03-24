using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ScrapShot : MonoBehaviour
{
    public int currentAmmo; 
    [SerializeField] public int maxAmmo;

    [SerializeField] private Scrap scrapPrefab;
    [SerializeField] private float scrapSpeed;
    [SerializeField] private float scrapDamage;
    [SerializeField] private float scrapStun;
    [SerializeField] private float scrapRange;
    [SerializeField] private float scrapCoolDownTime;
    [SerializeField] private float magnetizeRadius;
    [SerializeField] private AudioClip projectile;
    
    private float coolDownTimer;
    private ObjectPool<Scrap> scrapPool;
    private PlayerController playerController;
    private AudioSource audioSource;

    public bool canRicochet = false;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        coolDownTimer = scrapCoolDownTime;
        currentAmmo = maxAmmo;
        GeneratePool();
    }

    private void Update()
    {
        coolDownTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        GameManager.Instance.SetAmmoCountUI(currentAmmo, maxAmmo);
        GameManager.Instance.SetScrapCooldownUI(coolDownTimer/scrapCoolDownTime);
    }
    private void GeneratePool()
    {
        scrapPool = new ObjectPool<Scrap>(OnCreateScrap,OnPullFromPool, OnReturnToPool, OnDestroyPoolObject,true, maxAmmo, 10);
    }

    public void ShootScrap()
    {
        if(coolDownTimer >= scrapCoolDownTime && !playerController.GetComponentInChildren<Grapple>().isAimingGrapple)
        {
            if (currentAmmo > 0)
            {
                scrapPool.Get();
                audioSource.pitch = 1;
                audioSource.PlayOneShot(projectile);
                coolDownTimer = 0;
            }
        }
    }

    private void Magnetize()
    {
        if (coolDownTimer < scrapCoolDownTime)
        {
            return;
        }

        if (currentAmmo < maxAmmo)
        {
            audioSource.clip = projectile;
            audioSource.pitch = -1;
            audioSource.timeSamples = projectile.samples - 1;
            audioSource.Play();
        }

        InputManager.Instance.MagnetizeScrap();
        coolDownTimer = 0;
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
        Vector3 spawnPosition = transform.position;
        spawnPosition.z -= 1;
        Vector2 aimDirection = InputManager.Instance.GetAimDirection(transform.position); 
        scrap.SetParameters(scrapSpeed, scrapDamage, scrapStun, scrapRange, aimDirection, playerController.gameObject);
        scrap.transform.position = spawnPosition;
        scrap.gameObject.SetActive(true);
        scrap.inert = false;
        scrap.canRicochet = canRicochet;
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

    public void PlayMagnetizeAudio()
    {
        if(currentAmmo < maxAmmo)
        {
            audioSource.clip = projectile;
            audioSource.pitch = -1;
            audioSource.timeSamples = projectile.samples - 1;
            audioSource.Play();
        }
    }

    private void OnEnable()
    {
        InputManager.onScrapShot += ShootScrap;
        InputManager.onMagnetize += Magnetize;
    }

    private void OnDestroy()
    {
        InputManager.onScrapShot -= ShootScrap;
        InputManager.onMagnetize -= Magnetize;
    }
}
