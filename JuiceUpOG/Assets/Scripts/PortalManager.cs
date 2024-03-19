using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float EnemyPeriod;
    public GameObject PS;

    void Start()
    {
        InvokeRepeating("InvokeEnemy", 0, EnemyPeriod);
    }

    void InvokeEnemy()
    {
        Instantiate(EnemyPrefab, transform.position, transform.rotation);
        
        GameObject newPS = Instantiate(PS, transform.position, transform.rotation);
        Destroy(newPS.gameObject, 1);
    }
}