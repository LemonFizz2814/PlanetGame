using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    [SerializeField] private PlanetScript[] planetObjects;
    private PlanetScript[] unlockedPlanetObjects;

    [SerializeField] private GameObject transportVessel;

    [SerializeField] private int maxAmountOfTransports;

    private void Start()
    {
        unlockedPlanetObjects = planetObjects;
    }

    public PlanetScript[] GetPlanetObjects()
    {
        return planetObjects;
    }
    public PlanetScript[] GetUnlockedPlanetObjects()
    {
        return unlockedPlanetObjects;
    }

    public void SpawnTransport(GameObject _planet, GameObject _previousPlanet, PlanetScript.ERESOURCES _resource, int _resourceAmount)
    {
        GameObject vesselObj = Instantiate(transportVessel, _planet.transform.position, Quaternion.identity);
        vesselObj.GetComponent<VesselScript>().MoveToPlanet(_planet, _previousPlanet, _resource, _resourceAmount);
    }
}
