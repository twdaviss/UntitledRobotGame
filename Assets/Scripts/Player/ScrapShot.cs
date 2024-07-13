using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ScrapShot : MonoBehaviour
{
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float scrapSpeed;
    [SerializeField] private float scrapDamage;
    [SerializeField] private float scrapRange;

    private ObjectPool<GameObject> scrapPool;
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
        scrapPool = new ObjectPool<GameObject>(OnCreateScrap,OnPullFromPool, OnReturnToPool, OnDestroyPoolObject,true,maxAmmo, 10);
    }

    public void ShootScrap()
    {
        scrapPool.Get();
    }

    GameObject OnCreateScrap()
    {
        GameObject scrap = Instantiate(scrapPrefab);
        scrap.GetComponent<Scrap>().SetPool(scrapPool);
        scrap.GetComponent<Scrap>().moveSpeed = scrapSpeed;
        scrap.GetComponent<Scrap>().damage = scrapDamage;
        scrap.GetComponent<Scrap>().range = scrapRange;
        //Implement
        scrap.GetComponent<Scrap>().direction = playerController.GetMouseDirection();

        return scrap;
    }

    void OnPullFromPool(GameObject scrap)
    {
        scrap.SetActive(true);
        currentAmmo--;
    }

    void OnReturnToPool(GameObject scrap)
    {
        scrap.SetActive(false);
        currentAmmo++;
    }

    void OnDestroyPoolObject (GameObject scrap)
    {
        Destroy(scrap);
    }
}
