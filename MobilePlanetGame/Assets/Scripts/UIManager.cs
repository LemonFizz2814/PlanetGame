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
    [SerializeField] private TextMeshProUGUI unitsText;

    [SerializeField] private Slider levelSliderProduction;
    [SerializeField] private Slider levelSliderStorage;

    [SerializeField] private TransportManager transportManager;
    [SerializeField] private CameraScript cameraScript;

    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject resourceContent;
    [SerializeField] private GameObject resourceText;
    [SerializeField] private GameObject resourceDeliveryScreen;
    [SerializeField] private GameObject resourceDeliveryContent;
    [SerializeField] private GameObject attackPlanetScreen;
    [SerializeField] private GameObject attackPlanetContent;
    [SerializeField] private GameObject planetDeliveryButton;
    [SerializeField] private GameObject planetAttackButton;
    [SerializeField] private GameObject buttonPurchase;

    [SerializeField] private GameObject[] tabObjects;

    [SerializeField] private Sprite[] resourceIcons;
    [SerializeField] private Sprite[] buttonPurchaseSprites;

    private List<PlanetScript.ERESOURCES> unlockedResources = new List<PlanetScript.ERESOURCES>();

    private PlanetScript selectedPlanet;

    private int resourceLength;

    private void Start()
    {
        SetActiveInfoBox(false);
        resourceDeliveryScreen.SetActive(false);

        resourceLength = Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length - 1;

        //debug check
        if (resourceIcons.Length != resourceLength)
        {
            Debug.LogError("resourceIcons length not same as ERESOURCES " + resourceIcons.Length + "/" + resourceLength);
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
        cameraScript.SetCanMove(true);
    }

    public void UpdateInfoBox(PlanetScript _planetScript)
    {
        cameraScript.SetCanMove(false);

        SetActiveInfoBox(true);

        selectedPlanet = _planetScript;
        cameraScript.MoveCamera(new Vector3(selectedPlanet.transform.position.x, selectedPlanet.transform.position.y, -10));
        cameraScript.SetCamZoom(cameraScript.GetStartingZoom());

        int levelTierProd = selectedPlanet.GetLevelProdTier() * 2;
        int levelTierStor = selectedPlanet.GetLevelStorTier() * 2;

        UpdateUnitsText();
        CancelResourceDelivery();

        productionReqText.text = selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTierProd];
        storageReqText.text = selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTierStor];

        if (selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd + 1] != PlanetScript.ERESOURCES.None)
        { productionReqText.text += "\n" + selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd + 1].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTierProd + 1]; }

        if (selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor + 1] != PlanetScript.ERESOURCES.None)
        { storageReqText.text += "\n" + selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor + 1].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTierStor + 1]; }

        descriptionText.text = selectedPlanet.GetPlanetInfo().descriptionText;

        UpdateLevels();

        //clear old content
        foreach (Transform child in resourceContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < resourceLength; i++)
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

        resourceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40 * resourceLength);
    }

    public void UpdateResourcesTab()
    {
        for(int i = 0; i < resourceContent.transform.childCount; i++)
        {
            resourceContent.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>().text = ((PlanetScript.ERESOURCES)i).ToString() + ": " + (int)selectedPlanet.GetPlanetInfo().resourceValues[i] + "/" + (int)selectedPlanet.GetPlanetInfo().resourceMax[i];
        }
    }

    public void UpgradeProductionButtonPressed(GameObject _obj)
    {
        ButtonPurchaseSpawn(selectedPlanet.UpgradeProduction(), _obj.GetComponent<RectTransform>().anchoredPosition);
    }
    public void UpgradeStorageButtonPressed(GameObject _obj)
    {
        ButtonPurchaseSpawn(selectedPlanet.UpgradeStorage(), _obj.GetComponent<RectTransform>().anchoredPosition);
    }

    public void UpgradeMaxTransportPressed()
    {
        //update values -----------
        transportManager.UpgradeMaxTransport(1);
    }
    public void UpgradeTransportSpeedPressed()
    {
        //update values -----------
        transportManager.UpgradeTransportSpeed(1);
    }

    public void PurchaseUnitsPressed(GameObject _obj)
    {
        ButtonPurchaseSpawn(selectedPlanet.UnitPurchased(), _obj.GetComponent<RectTransform>().anchoredPosition);
        UpdateUnitsText();
    }

    public void UpdateUnitsText()
    {
        unitsText.text = "Units:\n" + selectedPlanet.GetSpaceStationInfo().units + "/" + selectedPlanet.GetSpaceStationInfo().unitMax;
    }

    public Sprite[] GetResourceIcon()
    {
        return resourceIcons;
    }

    public void ShowTab(int _i)
    {
        if(selectedPlanet.GetSpaceStationInfo().isSpaceStation)
        {
            _i = 3;
        }

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
    public void DeployPressed()
    {
        //work here
        
        attackPlanetScreen.SetActive(true);

        //clear old content
        foreach (Transform child in attackPlanetContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < transportManager.GetLockedPlanetObjects().Length; i++)
        {
            GameObject planetAttackObj = Instantiate(planetAttackButton, new Vector3(0, 0, 0), Quaternion.identity);
            planetAttackObj.transform.SetParent(attackPlanetContent.transform);
            planetAttackObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(3, -30 - (50 * i));
            planetAttackObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50);
            planetAttackObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            planetAttackObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            planetAttackObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            int x = i;
            planetAttackObj.transform.GetComponent<Button>().onClick.AddListener(delegate { AttackPlanetPressed(x); });

            planetAttackObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().defensePoints + "/" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().defenseMax;
            planetAttackObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().name;
            planetAttackObj.transform.GetChild(0).GetComponent<Image>().sprite = transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().sprite;
        }

        attackPlanetContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (50 * transportManager.GetLockedPlanetObjects().Length) + 5);
    }

    public void UpdateLevels()
    {
        levelSliderProduction.value = selectedPlanet.GetPlanetInfo().levelProd % 5;
        levelSliderProduction.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + selectedPlanet.GetPlanetInfo().levelProd;
        levelSliderStorage.value = selectedPlanet.GetPlanetInfo().levelStor % 5;
        levelSliderStorage.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + selectedPlanet.GetPlanetInfo().levelStor;
    }

    public void DoubleSpeedPressed(float _time)
    {
        selectedPlanet.SetDoubleSpeed(_time);
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

        CancelResourceDelivery();
    }

    public void AttackPlanetPressed(int _planet)
    {
        //work here
        print("planet selected " + _planet);
        transportManager.GetPlanetObjects()[_planet].RemoveDefensePoints(selectedPlanet.GetSpaceStationInfo().units);
        selectedPlanet.GetSpaceStationInfo().units = 0;
        CancelResourceDelivery();
    }


    void ButtonPurchaseSpawn(bool _worked, Vector3 _pos)
    {
        GameObject buttonPurchaseObj = Instantiate(buttonPurchase, new Vector3(0, 0, 0), Quaternion.identity);
        buttonPurchaseObj.transform.SetParent(gameObject.transform);
        buttonPurchaseObj.GetComponent<RectTransform>().anchoredPosition = _pos;
        buttonPurchaseObj.GetComponent<Image>().sprite = buttonPurchaseSprites[Convert.ToInt32(_worked)];
        Destroy(buttonPurchaseObj, 0.5f);
    }
    public void CancelResourceDelivery()
    {
        resourceDeliveryScreen.SetActive(false);
        attackPlanetScreen.SetActive(false);
    }
}
