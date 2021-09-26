using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject cameraObj;

    [SerializeField] private TextMeshProUGUI productionReqText;
    [SerializeField] private TextMeshProUGUI storageReqText;

    [SerializeField] private GameObject resourceContent;
    [SerializeField] private GameObject resourceText;

    private List<PlanetScript.ERESOURCES> unlockedResources = new List<PlanetScript.ERESOURCES>();

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

        int levelTier = selectedPlanet.GetPlanetInfo().levelTier;

        productionReqText.text = selectedPlanet.GetUpgradeRequirements().upProdResource[levelTier].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTier];
        storageReqText.text = selectedPlanet.GetUpgradeRequirements().upStorResource[levelTier].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTier];

        //clear old content
        foreach(Transform child in resourceContent.transform)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
        {
            //print("add");
            GameObject resourceTextObj = Instantiate(resourceText, new Vector3(0, 0, 0), Quaternion.identity);
            resourceTextObj.transform.SetParent(resourceContent.transform);
            resourceTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(25, 120 - (40 * i));
        }
    }

    public void UpgradeProductionButtonPressed()
    {
        selectedPlanet.UpgradeProduction();
        //UPDATE INFO BOX 
    }
    public void UpgradeStorageButtonPressed()
    {
        selectedPlanet.UpgradeStorage();
    }
}
