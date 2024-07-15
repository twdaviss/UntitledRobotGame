using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ScrapShot : MonoBehaviour
{
    [SerializeField] private Scrap scrapPrefab;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float scrapSpeed;
    [SerializeField] private float scrapDamage;
    [SerializeField] private float scrapRange;

    private ObjectPool<Scrap> scrapPool;
    private PlayerController playerController;

    private int currentAmmo; 

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        currentAmmo = maxAmmo;
        GeneratePool();
    }
    private void GeneratePool()
    {
        scrapPool = new ObjectPool<Scrap>(OnCreateScrap,OnPullFromPool, OnReturnToPool, OnDestroyPoolObject,true,maxAmmo, 10);
    }

    public void ShootScrap()
    {
        scrapPool.Get();
    }

    private Scrap OnCreateScrap()
    {
        Scrap scrap = Instantiate(scrapPrefab, this.transform);
        scrap.gameObject.SetActive(false);
        scrap.SetPool(scrapPool);
        
        return scrap;
    }

    private void OnPullFromPool(Scrap scrap)
    {
        scrap.SetParameters(scrapSpeed, scrapDamage, scrapRange, playerController.GetMouseDirection());
        scrap.transform.position = transform.position;
        scrap.gameObject.SetActive(true);
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
}
