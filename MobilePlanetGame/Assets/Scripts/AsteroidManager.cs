using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float spawnWaitMax;
    [SerializeField] private float spawnWaitMin;

    private void Start()
    {
        StartCoroutine(SpawnAsteroid());
    }

    private IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(Random.Range(spawnWaitMin, spawnWaitMax));
        Instantiate(asteroidPrefab, PickPosition(), Quaternion.identity);
    }

    Vector3 PickPosition()
    {
        return Vector3.zero;
    }
}
