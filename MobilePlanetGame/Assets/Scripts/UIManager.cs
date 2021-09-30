using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI productionReqText;
    [SerializeField] private TextMeshProUGUI storageReqText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private TransportManager transportManager;

    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject cameraObj;
    [SerializeField] private GameObject resourceContent;
    [SerializeField] private GameObject resourceText;
    [SerializeField] private GameObject resourceDeliveryScreen;
    [SerializeField] private GameObject resourceDeliveryContent;
    [SerializeField] private GameObject planetDeliveryButton;

    [SerializeField] private GameObject[] tabObjects;

    [SerializeField] private Sprite[] resourceIcons;

    private List<PlanetScript.ERESOURCES> unlockedResources = new List<PlanetScript.ERESOURCES>();

    private PlanetScript selectedPlanet;

    private void Start()
    {
        SetActiveInfoBox(false);
        resourceDeliveryScreen.SetActive(false);

        //debug check
        if(resourceIcons.Length != Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length)
        {
            Debug.LogError("resourceIcons length not same as ERESOURCES");
        }
    }

    public void SetActiveInfoBox(bool _active)
    {
        infoBox.SetActive(_active);
    }

    public void ClosePressed()
    {
        SetActiveInfoBox(false);
        selectedPlanet.ClickedOn();
    }

    public void UpdateInfoBox(PlanetScript _planetScript, Vector3 _pos)
    {
        SetActiveInfoBox(true);

        selectedPlanet = _planetScript;
        cameraObj.transform.position = _pos;

        int levelTier = selectedPlanet.GetLevelTier();

        productionReqText.text = selectedPlanet.GetUpgradeRequirements().upProdResource[levelTier].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTier];
        storageReqText.text = selectedPlanet.GetUpgradeRequirements().upStorResource[levelTier].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTier];
        descriptionText.text = selectedPlanet.GetPlanetInfo().descriptionText;

        //clear old content
        foreach (Transform child in resourceContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
        {
            GameObject resourceTextObj = Instantiate(resourceText, new Vector3(0, 0, 0), Quaternion.identity);
            resourceTextObj.transform.SetParent(resourceContent.transform);
            resourceTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-10, -20 - (35 * i));
            resourceTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 32);
            resourceTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            resourceTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            resourceTextObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            resourceTextObj.transform.GetChild(0).GetComponent<Image>().sprite = resourceIcons[i];

            int x = i;
            resourceTextObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { TransferPressed((PlanetScript.ERESOURCES)x); });

            resourceTextObj.GetComponent<TextMeshProUGUI>().text = ((PlanetScript.ERESOURCES)i).ToString() + ": " + selectedPlanet.GetPlanetInfo().resourceValues[i] + "/" + selectedPlanet.GetPlanetInfo().resourceMax[i];
        }

        resourceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40 * Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length);
    }

    public void UpdateResourcesTab()
    {
        for(int i = 0; i < resourceContent.transform.childCount; i++)
        {
            resourceContent.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().text = ((PlanetScript.ERESOURCES)i).ToString() + ": " + selectedPlanet.GetPlanetInfo().resourceValues[i] + "/" + selectedPlanet.GetPlanetInfo().resourceMax[i];
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

    public Sprite[] GetResourceIcon()
    {
        return resourceIcons;
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
        resourceDeliveryScreen.SetActive(true);

        //clear old content
        foreach (Transform child in resourceDeliveryContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < transportManager.GetUnlockedPlanetObjects().Length; i++)
        {
            GameObject planetDeliverObj = Instantiate(planetDeliveryButton, new Vector3(0, 0, 0), Quaternion.identity);
            planetDeliverObj.transform.SetParent(resourceDeliveryContent.transform);
            planetDeliverObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(3, -30 - (50 * i));
            planetDeliverObj.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 50);
            planetDeliverObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            planetDeliverObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            planetDeliverObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            int x = i;
            planetDeliverObj.transform.GetComponent<Button>().onClick.AddListener(delegate { PlanetDeliveryPressed(x, _resource); });

            planetDeliverObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + ((i != selectedPlanet.GetPlanetInfo().planetNum)? transportManager.GetUnlockedPlanetObjects()[i].GetPlanetInfo().name : "-------");
            planetDeliverObj.transform.GetChild(0).GetComponent<Image>().sprite = transportManager.GetUnlockedPlanetObjects()[i].GetPlanetInfo().sprite;
        }

        resourceDeliveryContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (50 * transportManager.GetUnlockedPlanetObjects().Length) + 5);
    }

    public void PlanetDeliveryPressed(int _planet, PlanetScript.ERESOURCES _resource)
    {
        print("planet selected " + _planet);
        transportManager.SpawnTransport(
            transportManager.GetPlanetObjects()[_planet].gameObject,
            transportManager.GetPlanetObjects()[selectedPlanet.GetPlanetInfo().planetNum].gameObject,
            _resource,
            (int)selectedPlanet.GetPlanetInfo().resourceValues[selectedPlanet.GetPlanetInfo().planetNum]);

        selectedPlanet.GainResourceExternal(_resource, -(int)selectedPlanet.GetPlanetInfo().resourceValues[selectedPlanet.GetPlanetInfo().planetNum]);

        resourceDeliveryScreen.SetActive(false);
    }

    public void CancelResourceDelivery()
    {
        resourceDeliveryScreen.SetActive(false);
    }
}
