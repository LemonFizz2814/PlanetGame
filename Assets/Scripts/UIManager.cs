using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI unitsText;
    [SerializeField] private TextMeshProUGUI currencyText;

    [SerializeField] private Slider levelTierSliderProduction;
    [SerializeField] private Slider levelTierSliderStorage;
    [SerializeField] private Slider levelSliderProduction;
    [SerializeField] private Slider levelSliderStorage;

    [SerializeField] private TransportManager transportManager;
    [SerializeField] private CameraScript cameraScript;
    [SerializeField] private SaveScript saveScript;

    [SerializeField] private GameObject planetInfoScreen;
    [SerializeField] private GameObject defaultScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject buyCurrencyScreen;

    [SerializeField] private GameObject resourceContent;
    [SerializeField] private GameObject resourceText;
    [SerializeField] private GameObject resourceDeliveryScreen;
    [SerializeField] private GameObject resourceDeliveryContent;

    [SerializeField] private GameObject attackPlanetScreen;
    [SerializeField] private GameObject attackPlanetContent;

    [SerializeField] private GameObject planetDeliveryButton;
    [SerializeField] private GameObject planetAttackButton;

    [SerializeField] private GameObject notificationBar;
    [SerializeField] private GameObject planetUnlockedPopUp;

    [SerializeField] private GameObject buttonPurchase;
    [SerializeField] private GameObject transitionObject;

    [SerializeField] private Image muteImage;

    [SerializeField] private GameObject[] tabObjects;

    [SerializeField] private Image[] upgradeButtons;

    [SerializeField] private Sprite[] resourceIcons;
    [SerializeField] private Sprite[] buttonPurchaseSprites;
    [SerializeField] private Sprite[] buttonReadySprites;
    [SerializeField] private Sprite[] muteSprites;

    [SerializeField] private Sprite notificationIcon;
    [SerializeField] private Sprite crossIcon;

    [SerializeField] private SoundManager soundManager;

    [SerializeField] private AudioClip buttonPressSound;
    [SerializeField] private AudioClip denySound;
    [SerializeField] private AudioClip acceptSound;
    [SerializeField] private AudioClip notificationSound;
    [SerializeField] private AudioClip planetSelectSound;
    [SerializeField] private AudioClip closeSound;

    [SerializeField] int adReward;
    [SerializeField] int followReward;

    private List<PlanetScript.ERESOURCES> unlockedResources = new List<PlanetScript.ERESOURCES>();

    private PlanetScript selectedPlanet;

    private int resourceLength;

    private int currency;

    private bool isMute = false;

    private List<Sprite> notificationImageList = new List<Sprite>();
    private List<string> notificationTextList = new List<string>();
    private List<float> notificationTimeList = new List<float>();

    enum EADTYPES
    {
        DoubleSpeed,
        Currency,
    };

    private EADTYPES adType;

    private void Start()
    {
        Advertisement.Initialize("4490264", true);

        transitionObject.SetActive(true);

        SetActiveInfoBox(false);
        SettingButtonPressed(false);
        ShowBuyCurrencyScreen(false);
        //notificationBar.SetActive(true);
        resourceDeliveryScreen.SetActive(false);

        currency = PlayerPrefs.GetInt("Currency");

        UpdateCurrencyText(0);

        resourceLength = Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length - 1;

        for(int i = 0; i < GetUnlockedResources().Count; i++)
        {
            AddNewResourceToContent(i);
        }

        //getting saved unlocked resources
        for (int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
        {
            if (PlayerPrefs.GetFloat("resourceUnlocked" + i) == 1)
            {
                unlockedResources.Add((PlanetScript.ERESOURCES)i);
            }
        }

        //debug check
        if (resourceIcons.Length != resourceLength)
        {
            Debug.LogError("resourceIcons length not same as ERESOURCES " + resourceIcons.Length + "/" + resourceLength);
        }

        AddNotificiton("Welcome", notificationIcon, 2.5f);
        SetPlanetPopUpActive(false);
    }

    public void SetActiveInfoBox(bool _active)
    {
        planetInfoScreen.SetActive(_active);
        defaultScreen.SetActive(!_active);
    }

    public void ClosePressed()
    {
        SetActiveInfoBox(false);
        selectedPlanet.ClickedOn();
        selectedPlanet = null;
        cameraScript.SetCanMove(true);
        soundManager.PlaySound(closeSound);
    }

    public void UpdateInfoBox(PlanetScript _planetScript)
    {
        cameraScript.SetCanMove(false);

        SetActiveInfoBox(true);

        selectedPlanet = _planetScript;
        cameraScript.MoveCamera(new Vector3(selectedPlanet.transform.position.x, selectedPlanet.transform.position.y - 2.8f, -10));
        cameraScript.SetCamZoom(cameraScript.GetStartingZoom());

        soundManager.PlaySound(planetSelectSound);

        UpdateUnitsText();
        UpdateLevels();
        UpdateRequirementsText();
        CancelResourceDelivery();

        //clear old content
        /*foreach (Transform child in resourceContent.transform)
        {
            Destroy(child.gameObject);
        }

        print("children 1 " + resourceContent.transform.childCount);

        //add resource text in resource tab
        for (int i = 0; i < unlockedResources.Count; i++)
        {
            GameObject resourceTextObj = Instantiate(resourceText, new Vector3(0, 0, 0), Quaternion.identity);
            resourceTextObj.transform.SetParent(resourceContent.transform);
            resourceTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(14, -20 - (35 * i));
            resourceTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 32);
            resourceTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            resourceTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            resourceTextObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            resourceTextObj.transform.GetChild(1).GetComponent<Image>().sprite = resourceIcons[i];

            int x = i;
            resourceTextObj.transform.GetComponent<Button>().onClick.AddListener(delegate { TransferPressed((PlanetScript.ERESOURCES)x); });
        }*/

        print("children 2 " + resourceContent.transform.childCount);
        UpdateResourcesTab();

        resourceContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40 * resourceLength);
    }

    void AddNewResourceToContent(int i)
    {
        GameObject resourceTextObj = Instantiate(resourceText, new Vector3(0, 0, 0), Quaternion.identity);
        resourceTextObj.transform.SetParent(resourceContent.transform);
        resourceTextObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(14, -20 - (35 * i));
        resourceTextObj.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 32);
        resourceTextObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        resourceTextObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        resourceTextObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

        resourceTextObj.transform.GetChild(1).GetComponent<Image>().sprite = resourceIcons[i];

        int x = i;
        resourceTextObj.transform.GetComponent<Button>().onClick.AddListener(delegate { TransferPressed((PlanetScript.ERESOURCES)x); });
    }

    //update upgrade requirements text
    public void UpdateRequirementsText()
    {
        int levelTierProd = selectedPlanet.GetLevelProdTier() * 2;
        int levelTierStor = selectedPlanet.GetLevelStorTier() * 2;

        //productionReqText.text = selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd].ToString() + ": " + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTierProd];
        RequirementsText(0).text = selectedPlanet.GetPlanetInfo().resourceValues[(int)selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd]] + "/" + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTierProd];
        GetUpgradeButton(0).transform.GetChild(2).GetComponent<Image>().sprite = resourceIcons[(int)selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd]];
        GetUpgradeButton(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Production\n" + "+" + selectedPlanet.GetUpgradeRequirements().upProdIncrease[levelTierProd] + "%";

        RequirementsText(1).text = selectedPlanet.GetPlanetInfo().resourceValues[(int)selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor]] + "/" + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTierStor];
        GetUpgradeButton(1).transform.GetChild(2).GetComponent<Image>().sprite = resourceIcons[(int)selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor]];
        GetUpgradeButton(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Storage\n" + "+" + selectedPlanet.GetUpgradeRequirements().upStorIncrease[levelTierStor] + "%";

        //second requirement for production
        if (selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd + 1] != PlanetScript.ERESOURCES.None)
        {
            RequirementsText(0).text += "\n" + selectedPlanet.GetPlanetInfo().resourceValues[(int)selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd + 1]] + "/" + selectedPlanet.GetUpgradeRequirements().upProdAmount[levelTierProd + 1];
            GetUpgradeButton(0).transform.GetChild(3).gameObject.SetActive(true);
            GetUpgradeButton(0).transform.GetChild(3).GetComponent<Image>().sprite = resourceIcons[(int)selectedPlanet.GetUpgradeRequirements().upProdResource[levelTierProd + 1]];
        }
        else { GetUpgradeButton(0).transform.GetChild(3).gameObject.SetActive(false); }

        //second requirement for storage
        if (selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor + 1] != PlanetScript.ERESOURCES.None)
        {
            RequirementsText(1).text += "\n" + selectedPlanet.GetPlanetInfo().resourceValues[(int)selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor + 1]] + "/" + selectedPlanet.GetUpgradeRequirements().upStorAmount[levelTierStor + 1];
            GetUpgradeButton(1).transform.GetChild(3).gameObject.SetActive(true);
            GetUpgradeButton(1).transform.GetChild(3).GetComponent<Image>().sprite = resourceIcons[(int)selectedPlanet.GetUpgradeRequirements().upStorResource[levelTierStor + 1]];
        }
        else { GetUpgradeButton(1).transform.GetChild(3).gameObject.SetActive(false); }

        descriptionText.text = selectedPlanet.GetPlanetInfo().descriptionText;

        //upgrade button sprites
        //GetUpgradeButton(0).sprite = buttonReadySprites[Convert.ToInt32(selectedPlanet.CanBuyProduction(selectedPlanet.GetLevelProdTier() * 2) && CheckUnitLimit())];
        //GetUpgradeButton(1).sprite = buttonReadySprites[Convert.ToInt32(selectedPlanet.CanBuyStorage(selectedPlanet.GetLevelStorTier() * 2) && CheckUnitLimit())];
        bool acceptProd = selectedPlanet.CanBuyProduction(selectedPlanet.GetLevelProdTier() * 2) && CheckUnitLimit();
        bool acceptStor = selectedPlanet.CanBuyStorage(selectedPlanet.GetLevelStorTier() * 2) && CheckUnitLimit();

        GetUpgradeButton(0).GetComponent<Animator>().SetBool("Accept", acceptProd);
        GetUpgradeButton(0).GetComponent<Animator>().SetBool("Deny", !acceptProd);
        GetUpgradeButton(1).GetComponent<Animator>().SetBool("Accept", acceptStor);
        GetUpgradeButton(1).GetComponent<Animator>().SetBool("Deny", !acceptStor);
    }

    public TextMeshProUGUI RequirementsText(int _i)
    {
        return GetUpgradeButton(_i).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public Image GetUpgradeButton(int _i)
    {
        if(selectedPlanet.GetSpaceStationInfo().isSpaceStation)
        {
            return upgradeButtons[_i + 2];
        }
        else
        {
            return upgradeButtons[_i];
        }
    }

    public void UpdateResourcesTab()
    {
        //print("children " + resourceContent.transform.childCount);

        for(int i = 0; i < resourceContent.transform.childCount; i++)
        {
            //print("i " + i + " resource " + ((PlanetScript.ERESOURCES)i).ToString());
            resourceContent.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = ((PlanetScript.ERESOURCES)i).ToString() + ": " + (int)selectedPlanet.GetPlanetInfo().resourceValues[i] + "/" + (int)selectedPlanet.GetPlanetInfo().resourceMax[i];
        }
    }

    public void UpgradeProductionButtonPressed(GameObject _obj)
    {
        bool canBuy = selectedPlanet.UpgradeProduction();
        soundManager.PlaySound((canBuy) ? acceptSound : denySound);
        ButtonPurchaseSpawn(canBuy, _obj.GetComponent<RectTransform>().anchoredPosition);
    }
    public void UpgradeStorageButtonPressed(GameObject _obj)
    {
        bool canBuy = selectedPlanet.UpgradeStorage();
        soundManager.PlaySound((canBuy) ? acceptSound : denySound);
        ButtonPurchaseSpawn(canBuy, _obj.GetComponent<RectTransform>().anchoredPosition);
    }

    public void UpgradeMaxTransportPressed()
    {
        //update values -----------
        transportManager.UpgradeMaxTransport(1);
    }
    public void UpgradeTransportSpeedPressed()
    {
        //update values
        transportManager.UpgradeTransportSpeed(1);
    }

    public void PurchaseUnitsPressed(GameObject _obj)
    {
        ButtonPurchaseSpawn(selectedPlanet.UnitPurchased(), _obj.GetComponent<RectTransform>().anchoredPosition);
        UpdateUnitsText();
        UpdateRequirementsText();
    }

    public void UpdateUnitsText()
    {
        unitsText.text = "" + selectedPlanet.GetSpaceStationInfo().units + "/" + selectedPlanet.GetSpaceStationInfo().unitMax;
    }

    bool CheckUnitLimit()
    {
        return (selectedPlanet.GetSpaceStationInfo().units < selectedPlanet.GetSpaceStationInfo().unitMax || selectedPlanet.GetSpaceStationInfo().units == 0);
    }

    public Sprite[] GetResourceIcon()
    {
        return resourceIcons;
    }

    public void SetUnlockedResources(PlanetScript.ERESOURCES _resource, bool _notifity)
    {
        //make sure resource isn't already contained in list
        if (!unlockedResources.Contains(_resource))
        {
            unlockedResources.Add(_resource);
            AddNewResourceToContent(GetUnlockedResources().Count - 1);

            if (_notifity) { AddNotificiton("New resource unlocked: " + _resource.ToString(), resourceIcons[(int)_resource], 3.5f); }
        }
    }
    public List<PlanetScript.ERESOURCES> GetUnlockedResources()
    {
        return unlockedResources;
    }

    public void AddNotificiton(string _text, Sprite _image, float _time)
    {
        notificationTextList.Add(_text);
        notificationImageList.Add(_image);
        notificationTimeList.Add(_time);

        //if there aren't already notifications 
        if (notificationTextList.Count <= 1)
        {
            StartCoroutine(ShowNotification());
        }
    }

    //show notification
    public IEnumerator ShowNotification()
    {
        //loop time all notifications are done
        if (notificationTextList.Count >= 1)
        {
            soundManager.PlaySound(notificationSound);

            notificationBar.GetComponent<Animator>().SetBool("Open", true);
            notificationBar.transform.GetChild(0).GetComponent<Image>().sprite = notificationImageList[0]; //image of notification bar
            notificationBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = notificationTextList[0]; //text of notification bar

            yield return new WaitForSeconds(notificationTimeList[0]);

            notificationImageList.RemoveAt(0);
            notificationTextList.RemoveAt(0);

            if (notificationTextList.Count >= 1) { notificationBar.GetComponent<Animator>().SetTrigger("Update"); }

            StartCoroutine(ShowNotification());
        }
        else
        {
            notificationBar.GetComponent<Animator>().SetBool("Open", false);
            notificationBar.GetComponent<Animator>().SetTrigger("Close");
            //notificationBar.SetActive(false);
        }
    }

    public void ShowTab(int _i)
    {

        //if space station and click on upgrade then set tab to space station upgrade tab instead
        if (selectedPlanet.GetSpaceStationInfo().isSpaceStation && _i == 0)
        {
            _i = 3;
        }

        for(int i = 0; i < tabObjects.Length; i++)
        {
            tabObjects[i].SetActive(false);
        }

        tabObjects[_i].SetActive(true);

        UpdateRequirementsText();
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

        List<PlanetScript> planetList = new List<PlanetScript>();
        planetList.AddRange(transportManager.GetUnlockedPlanetObjects());

        //remove selected planet from the list
        for (int i = 0; i < planetList.Count; i++)
        {
            if (planetList[i] == selectedPlanet)
            {
                planetList.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < planetList.Count; i++)
        {
            GameObject planetDeliverObj = Instantiate(planetDeliveryButton, new Vector3(0, 0, 0), Quaternion.identity);
            planetDeliverObj.transform.SetParent(resourceDeliveryContent.transform);
            planetDeliverObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(3, -30 - (50 * i));
            planetDeliverObj.GetComponent<RectTransform>().sizeDelta = new Vector2(260, 50);
            planetDeliverObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            planetDeliverObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            planetDeliverObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            int x = i;
            planetDeliverObj.transform.GetComponent<Button>().onClick.AddListener(delegate { PlanetDeliveryPressed(planetList[x].gameObject, _resource); });

            planetDeliverObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = planetList[i].GetPlanetInfo().name;
            planetDeliverObj.transform.GetChild(0).GetComponent<Image>().sprite = planetList[i].GetPlanetInfo().sprite;
        }
        resourceDeliveryContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (50 * planetList.Count) + 5);
    }
    public void DeployPressed()
    {
        attackPlanetScreen.SetActive(true);

        //clear old content
        foreach (Transform child in attackPlanetContent.transform)
        {
            Destroy(child.gameObject);
        }

        //populate new content
        for (int i = 0; i < transportManager.GetLockedPlanetObjects().Length; i++)
        {
            GameObject planetAttackObj = Instantiate(planetAttackButton, new Vector3(0, 0, 0), Quaternion.identity);
            planetAttackObj.transform.SetParent(attackPlanetContent.transform);
            planetAttackObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(3, -30 - (50 * i));
            planetAttackObj.GetComponent<RectTransform>().sizeDelta = new Vector2(270, 50);
            planetAttackObj.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            planetAttackObj.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            planetAttackObj.GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            int x = i;
            planetAttackObj.transform.GetComponent<Button>().onClick.AddListener(delegate { AttackPlanetPressed(x); });

            planetAttackObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().defensePoints + "/\n" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().defenseMax;
            planetAttackObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().name;
            planetAttackObj.transform.GetChild(0).GetComponent<Image>().sprite = transportManager.GetLockedPlanetObjects()[i].GetPlanetInfo().sprite;
        }

        attackPlanetContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (50 * transportManager.GetLockedPlanetObjects().Length) + 5);
    }

    public void UpdateLevels()
    {
        //level tier
        levelTierSliderProduction.value = selectedPlanet.GetPlanetInfo().levelProd % 5;
        levelTierSliderProduction.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + selectedPlanet.GetPlanetInfo().levelProd;
        levelTierSliderStorage.value = selectedPlanet.GetPlanetInfo().levelStor % 5;
        levelTierSliderStorage.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "" + selectedPlanet.GetPlanetInfo().levelStor;

        //level 
        levelSliderProduction.value = selectedPlanet.GetLevelProdTier();
        levelSliderStorage.value = selectedPlanet.GetLevelStorTier();
    }

    public void DoubleSpeedPressed(float _time)
    {
        selectedPlanet.SetDoubleSpeed(_time);
    }

    public void PlanetDeliveryPressed(GameObject _planetObj, PlanetScript.ERESOURCES _resource)
    {
        if(!transportManager.SpawnTransport(
            _planetObj,
            transportManager.GetPlanetObjects()[selectedPlanet.GetPlanetInfo().planetNum].gameObject,
            _resource,
            (int)selectedPlanet.GetPlanetInfo().resourceValues[(int)_resource]))
        {
            AddNotificiton("Can't have more than " + transportManager.GetMaxTransport() + " transport vessels active", crossIcon, 2.5f);
        }
        else
        {
            selectedPlanet.GainResourceExternal(_resource, -(int)selectedPlanet.GetPlanetInfo().resourceValues[(int)_resource]);
        }

        CancelResourceDelivery();
    }

    public void SetPlanetPopUpActive(bool _active)
    {
        planetUnlockedPopUp.SetActive(_active);
    }

    public void ShowPlanetPopUp(PlanetScript _planet)
    {
        SetPlanetPopUpActive(true);
        planetUnlockedPopUp.transform.GetChild(1).GetComponent<Image>().sprite = _planet.GetPlanetInfo().sprite;
        planetUnlockedPopUp.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _planet.GetPlanetInfo().name;
    }

    public void AttackPlanetPressed(int _planet)
    {
        //work here
        print("planet selected " + _planet);
        transportManager.GetLockedPlanetObjects()[_planet].RemoveDefensePoints(selectedPlanet.GetSpaceStationInfo().units);
        selectedPlanet.GetSpaceStationInfo().units = 0;
        UpdateUnitsText();
        CancelResourceDelivery();
    }

    public PlanetScript GetPlanetScript()
    {
        return selectedPlanet;
    }

    public void SettingButtonPressed(bool _active)
    {
        settingsScreen.SetActive(_active);
    }

    public void MutePressed()
    {
        isMute = !isMute;
        muteImage.sprite = muteSprites[Convert.ToInt32(isMute)];
        soundManager.SetVolume(Convert.ToInt32(!isMute));
    }

    public void ShowAdReward(int _adType)
    {
        adType = (EADTYPES)_adType;

        //play ad

        const string placementId = "rewardedVideo";

        if (Advertisement.IsReady())
        {
            print("is ready!");
            Advertisement.Show(placementId, new ShowOptions() { resultCallback = adViewResult });
        }
        else
        {
            print("is not ready!");
        }
    }

    public void ShowAdVideo(int _adType)
    {
        adType = (EADTYPES)_adType;

        //play ad

        const string placementId = "video";

        if (Advertisement.IsReady())
        {
            Advertisement.Show(placementId, new ShowOptions() { resultCallback = adViewResult });
        }
    }
    private void adViewResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                switch(adType)
                {
                    case EADTYPES.DoubleSpeed:
                        break;
                    case EADTYPES.Currency:
                        UpdateCurrencyText(adReward);
                        break;
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");

                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");

                break;
        }
    }

    public void TwitterButtonPressed()
    {
        if (PlayerPrefs.GetInt("twitterPressed") == 0)
        {
            PlayerPrefs.SetInt("twitterPressed", 1);
            UpdateCurrencyText(followReward);
        }

        Application.OpenURL("https://twitter.com/AppleSeedGames");
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
        UpdateRequirementsText();
    }

    public void UpdateCurrencyText(int _i)
    {
        currency += _i;
        currencyText.text = "" + currency;
        PlayerPrefs.SetInt("Currency", currency);
    }

    public void PlayButtonPressSound()
    {
        soundManager.PlaySound(buttonPressSound);
    }

    public void ShowBuyCurrencyScreen(bool _active)
    {
        buyCurrencyScreen.SetActive(_active);
    }
}
