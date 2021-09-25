using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject cameraObj;

    PlanetScript selectedPlanet;

    private void Start()
    {
        SetActiveInfoBox(false);
    }

    public void SetActiveInfoBox(bool _active)
    {
        infoBox.SetActive(_active);
    }

    public void UpdateInfoBox(PlanetScript _planetScript, Vector3 _pos)
    {
        SetActiveInfoBox(true);

        selectedPlanet = _planetScript;
        cameraObj.transform.position = _pos;
    }

    public void UpgradeProductionButtonPressed()
    {
        selectedPlanet.UpgradeProduction();
    }
    public void UpgradeStorageButtonPressed()
    {
        selectedPlanet.UpgradeStorage();
    }
}
