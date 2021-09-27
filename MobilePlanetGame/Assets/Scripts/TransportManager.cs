using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    [SerializeField] private PlanetScript[] planetObjects;
    [SerializeField] private GameObject transportVessel;

    public PlanetScript[] GetPlanetObjects()
    {
        return planetObjects;
    }

    public void SpawnTransport(Vector3 _planetPos)
    {
        Instantiate(transportVessel, _planetPos, Quaternion.identity);
    }
}
