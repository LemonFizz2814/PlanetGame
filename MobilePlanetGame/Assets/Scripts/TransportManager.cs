using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    [SerializeField] private PlanetScript[] planetObjects;
    private List<PlanetScript> unlockedPlanetObjects = new List<PlanetScript>();
    private List<PlanetScript> lockedPlanetObjects = new List<PlanetScript>();

    [SerializeField] private GameObject transportVessel;

    [SerializeField] private int maxAmountOfTransports;

    [SerializeField] private float transportSpeed;

    private void Start()
    {
        //unlockedPlanetObjects = planetObjects;
        //lockedPlanetObjects = planetObjects;
    }

    public PlanetScript[] GetPlanetObjects()
    {
        return planetObjects;
    }
    public PlanetScript[] GetUnlockedPlanetObjects()
    {
        return unlockedPlanetObjects.ToArray();
    }
    public PlanetScript[] GetLockedPlanetObjects()
    {
        return unlockedPlanetObjects.ToArray();
    }

    public void AddToUnlockedPlanet(bool _add, PlanetScript _planet)
    {
        if(_add)
        {
            unlockedPlanetObjects.Add(_planet);
        } else {
            unlockedPlanetObjects.Remove(_planet);
        }
    }
    public void AddToLockedPlanet(bool _add, PlanetScript _planet)
    {
        if(_add)
        {
            lockedPlanetObjects.Add(_planet);
        } else {
            lockedPlanetObjects.Remove(_planet);
        }
    }

    public void UpgradeMaxTransport(int _i)
    {
        /*if ()
        {
            maxAmountOfTransports += _i;
        }
        else
        {
            return false;
        }
        return true;*/
    }

    public void UpgradeTransportSpeed(int _i)
    {
        //transportSpeed += _i;
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
