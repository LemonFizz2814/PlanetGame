using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    [SerializeField] private GameObject[] tabObjects;

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
            GameObject resourceTextObj = Instantiate(resourceText, new Vector3(0, 0, 0), Quaternion.identity);
            resourceTextObj.transform.SetParent(resourceContent.transform);
            resourceTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-25, -20 - (40 * i));
            resourceTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 40);
            resourceTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            resourceTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);

            int x = i;
            resourceTextObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { TransferPressed((PlanetScript.ERESOURCES)x); });

            resourceTextObj.GetComponent<TextMeshProUGUI>().text = ((PlanetScript.ERESOURCES)i).ToString() + ": " + selectedPlanet.GetPlanetInfo().resourceValues[i] + "/" + 100;
        }

        resourceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40 * Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length);
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

    public void ShowTab(int _i)
    {
        for(int i = 0; i < tabObjects.Length; i++)
        {
            tabObjects[i].SetActive(false);
        }

        tabObjects[_i].SetActive(true);
    }

    public void TransferPressed(PlanetScript.ERESOURCES _resource)
    {
        print("transferring " + _resource.ToString());
    }
}
