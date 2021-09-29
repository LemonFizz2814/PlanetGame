using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselScript : MonoBehaviour
{
    [SerializeField] private float speed;
    private float t = 0;
    private bool firstDelivery;

    private GameObject planetHeadingTo;
    private GameObject planetHeadingFrom;

    private PlanetScript.ERESOURCES resource;
    private int resourceAmount;

    public void MoveToPlanet(GameObject _planetPos, GameObject _previousPlanetPos, PlanetScript.ERESOURCES _resource, int _resourceAmount)
    {
        planetHeadingTo = _planetPos;
        planetHeadingFrom = _previousPlanetPos;
        resource = _resource;
        resourceAmount = _resourceAmount;

        firstDelivery = true;
    }

    private void Update()
    {
        t += speed * Time.deltaTime;

        transform.position = Vector3.Lerp(planetHeadingFrom.transform.position, planetHeadingTo.transform.position, t);

        if(t > 1)
        {
            if (firstDelivery)
            {
                firstDelivery = false;

                planetHeadingTo.GetComponent<PlanetScript>().GainResourceExternal(resource, resourceAmount);

                //planetHeadingTo = previousPlanet;
                Destroy(gameObject);
            }
        }
    }
}
