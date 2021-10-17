using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    [SerializeField] private PlanetScript[] planetObjects;
    private PlanetScript[] unlockedPlanetObjects;

    [SerializeField] private GameObject transportVessel;

    [SerializeField] private int maxAmountOfTransports;

    [SerializeField] private float transportSpeed;

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
    public PlanetScript[] GetLockedPlanetObjects()
    {
        return unlockedPlanetObjects;
    }

    public void UpgradeMaxTransport(int _i)
    {
        maxAmountOfTransports += _i;
    }

    public void UpgradeTransportSpeed(int _i)
    {
        transportSpeed += _i;
    }

    public void SpawnTransport(GameObject _planet, GameObject _previousPlanet, PlanetScript.ERESOURCES _resource, int _resourceAmount)
    {
        if (GameObject.FindGameObjectsWithTag("Vessel").Length <= maxAmountOfTransports)
        {
            GameObject vesselObj = Instantiate(transportVessel, _planet.transform.position, Quaternion.identity);
            vesselObj.GetComponent<VesselScript>().MoveToPlanet(_planet, _previousPlanet, _resource, _resourceAmount, transportSpeed);
        }
    }
}
