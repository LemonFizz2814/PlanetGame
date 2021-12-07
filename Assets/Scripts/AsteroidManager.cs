using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject cameraObj;
    [Space]
    [SerializeField] private float spawnWaitMax;
    [SerializeField] private float spawnWaitMin;
    [Space]
    [SerializeField] private float spawnPosX;
    [SerializeField] private float spawnPosY;

    private void Start()
    {
        StartCoroutine(SpawnAsteroid());
    }

    private IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(Random.Range(spawnWaitMin, spawnWaitMax));
        GameObject _asteroid = Instantiate(asteroidPrefab, PickPosition(), Quaternion.identity);
        _asteroid.GetComponent<AsteroidScript>().SetLookAt(cameraObj);
        StartCoroutine(SpawnAsteroid());
    }

    Vector3 PickPosition()
    {
        return new Vector3(spawnPosX * -Random.Range(0, 1), Random.Range(-spawnPosY, spawnPosY), 0);
    }
}
