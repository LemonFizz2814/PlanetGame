using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselScript : MonoBehaviour
{
    private float speed;
    private float t = 0;
    private bool firstDelivery;

    private GameObject planetHeadingTo;
    private GameObject planetHeadingFrom;
    [SerializeField] private LineRenderer dottedLine;

    private PlanetScript.ERESOURCES resource;
    private int resourceAmount;

    public void MoveToPlanet(GameObject _planetPos, GameObject _previousPlanetPos, PlanetScript.ERESOURCES _resource, int _resourceAmount, float _speed)
    {
        speed = _speed;
        planetHeadingTo = _planetPos;
        planetHeadingFrom = _previousPlanetPos;
        resource = _resource;
        resourceAmount = _resourceAmount;

        firstDelivery = true;

        //spawn in dotted line
        LineRenderer _lineObj = Instantiate(dottedLine, new Vector2(0, 0), Quaternion.identity);
        _lineObj.SetPosition(0, planetHeadingTo.transform.position);
        _lineObj.SetPosition(1, planetHeadingFrom.transform.position);
        _lineObj.transform.SetParent(transform);
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
