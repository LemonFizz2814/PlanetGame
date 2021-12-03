using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetScript : MonoBehaviour
{
    public enum ERESOURCES
    {
        Water,
        Timber,
        Sulphur,
        Oil,
        Copper,
        Uranium,
        Gold,
        Nitrogen,
        Cosmic_Quartz,
        Space_Dust,
        None,
    };

    [Serializable]
    public class PlanetInfo
    {
        //basic info
        public string name;
        public Sprite sprite;
        public Sprite dullSprite;
        public int planetNum;
        public bool hasBeenUnlocked;
        public bool isDoubleSpeed;
        [NonSerialized] public float doubleSpeedTime;

        //production variables
        public ERESOURCES[] productionResources;
        public float[] productionTime;
        public float[] productionGain;
        public float StorageMultiply;
        //public float[] productionMax;
        [NonSerialized] public float[] resourceMax = { 400, 500, 500, 10000, 1250, 95000, 250, 7500, 1, 550 }; //set on very first start

        //level variables
        [NonSerialized] public int levelProd = 0;
        [NonSerialized] public int levelStor = 0;

        //lists
        [NonSerialized] public List<float> resourceValues = new List<float>();
        [NonSerialized] public List<float> time = new List<float>();

        //defense
        [NonSerialized] public int defensePoints;
        public int defenseMax;

        public ERESOURCES[] gainedResources;

        [TextArea(8, 10)] public string descriptionText;
    };

    [Serializable]
    public class UpgradeRequirements
    {
        public ERESOURCES[] upProdResource;
        public int[] upProdAmount;
        public int[] upProdIncrease;
        public ERESOURCES[] upStorResource;
        public int[] upStorAmount;
        public int[] upStorIncrease;
        [NonSerialized] public int maxUpgrades;
    };

    [Serializable]
    public class SpaceStationInfo
    {
        public bool isSpaceStation;
        public int unitGain;
        public int unitMax;
        public int unitMaxIncrease;
        public ERESOURCES[] unitBuyResource;
        public int[] unitBuyAmount;
        [NonSerialized] public int units;
    };

    [SerializeField] private PlanetInfo planetInfo;
    [SerializeField] private UpgradeRequirements upgradeReq;
    [SerializeField] private SpaceStationInfo spaceStationInfo;
    [SerializeField] private SaveScript saveScript;
    [SerializeField] private TextMesh nameText;
    [SerializeField] private TextMesh defensePointsText;
    [SerializeField] private GameObject collectedTextPrefab;
    [SerializeField] private GameObject productionTextPrefab;
    [SerializeField] private GameObject x2PointsOutline;
    [SerializeField] private GameObject levelUpPrefab;

    [SerializeField] private float collectedSpawnRand;
    [SerializeField] private int collectedDestroy;

    [SerializeField] private Transform productionTextsParent;

    [SerializeField] TransportManager transportManager;
    [SerializeField] SoundManager soundManager;

    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip levelUpSound;

    List<TextMesh> productionText = new List<TextMesh>();

    private UIManager uiManager;


    private int lengthOfAllResources;
    private int lengthOfProdRes;
    private int levelUpAmount = 5;
    private int levelUpDoubleSpeedTime = 10;

    private bool currentlyClicked = false;

    //debug variables
    private bool debugSaveActive = false;

    private void Start()
    {
        UpdateInfo();

        lengthOfAllResources = Enum.GetNames(typeof(ERESOURCES)).Length - 1;
        lengthOfProdRes = planetInfo.productionResources.Length;

        x2PointsOutline.SetActive(planetInfo.isDoubleSpeed);

        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        upgradeReq.maxUpgrades = upgradeReq.upProdResource.Length;

        nameText.text = planetInfo.name;
        GetComponent<SpriteRenderer>().sprite = (planetInfo.hasBeenUnlocked)? planetInfo.sprite : planetInfo.dullSprite;

        GetPlanetInfo().defensePoints = GetPlanetInfo().defenseMax;

        //add planet to transport manager locked or unlocked lists
        /*if (GetPlanetInfo().hasBeenUnlocked)
        {
            transportManager.AddToUnlockedPlanet(true, this);
        } else {
            transportManager.AddToLockedPlanet(true, this);
        }*/

        //debug check
        if (planetInfo.resourceMax.Length != lengthOfAllResources)
        {
            Debug.LogError("resourceMax isn't the same length as the amount of resources " + planetInfo.resourceMax.Length + "/" + lengthOfAllResources);
        }

        if(GetPlanetInfo().hasBeenUnlocked)
        {
            SetUpProductionText();
        }

        for (int i = 0; i < lengthOfAllResources; i++)
        {
            planetInfo.resourceValues.Add(0);
        }

        UpdateProductionText();
        UpdateDefensePointsText();
    }

    private void Update()
    {
        if (planetInfo.hasBeenUnlocked)
        {
            //loop through all resources
            for (int i = 0; i < lengthOfProdRes; i++)
            {
                planetInfo.time[i] -= Time.deltaTime;

                if (planetInfo.time[i] < 0)
                {
                    planetInfo.time[i] = planetInfo.productionTime[i];

                    if (planetInfo.isDoubleSpeed)
                    {
                        planetInfo.time[i] /= 2;
                    }

                    if (planetInfo.resourceValues[ResourceNum(i)] + planetInfo.productionGain[i] <= planetInfo.resourceMax[ResourceNum(i)])
                    {
                        GainResource(i, planetInfo.productionGain[i]);
                        SpawnCollectedText(ResourceNum(i), planetInfo.productionGain[i]);
                    }
                    else
                    {
                        GainResource(i, planetInfo.resourceMax[ResourceNum(i)] - planetInfo.resourceValues[ResourceNum(i)]);
                    }
                }
            }

            //if this palnet is on double speed
            if (planetInfo.isDoubleSpeed)
            {
                planetInfo.doubleSpeedTime -= Time.deltaTime;

                if (planetInfo.doubleSpeedTime < 0)
                {
                    x2PointsOutline.SetActive(false);
                    planetInfo.isDoubleSpeed = false;
                    print("double speed ended");
                }
            }
        }
    }

    public void SetDoubleSpeed(float _time)
    {
        print("double speed set to " + _time);
        planetInfo.isDoubleSpeed = true;
        planetInfo.doubleSpeedTime = _time;

        x2PointsOutline.SetActive(true);

        uiManager.AddNotificiton(planetInfo.name + " has double speed", planetInfo.sprite);
    }

    void GainResource(int _i, float _gain)
    {
        planetInfo.resourceValues[ResourceNum(_i)] += _gain;
        UpdateProductionText();
    }

    //spawn in collected text on planet of resource
    void SpawnCollectedText(int _resource, float _gain)
    {
        /*Vector2 pos = new Vector2(
            transform.position.x + UnityEngine.Random.Range(collectedSpawnRand, -collectedSpawnRand),
            transform.position.y + UnityEngine.Random.Range(collectedSpawnRand, -collectedSpawnRand));*/
        GameObject textObj = Instantiate(collectedTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        textObj.GetComponent<TextMesh>().text = "+" + _gain;
        if (planetInfo.productionResources.Length > 0) { textObj.transform.SetParent(productionTextsParent.GetChild(Array.IndexOf(planetInfo.productionResources, (ERESOURCES)_resource))); }
        //textObj.transform.GetChild(0).localPosition = new Vector3(0, -1.75f - (Array.IndexOf(uiManager.GetResourceIcon(), _resource) * 0.5f), 0);//new Vector3(4 + (_gain.ToString().Length * 1.2f), 0, -1);
        textObj.transform.localPosition = new Vector3(25, 0.5f, 0);
        textObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = uiManager.GetResourceIcon()[_resource];

        Destroy(textObj, collectedDestroy);
    }

    public void SetUpProductionText()
    {
        for (int i = 0; i < lengthOfProdRes; i++)
        {
            //add production time
            planetInfo.time.Add(planetInfo.productionTime[i]);
            GameObject productionTextObj = Instantiate(productionTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            productionText.Add(productionTextObj.GetComponent<TextMesh>());
            productionTextObj.transform.SetParent(productionTextsParent);
            productionTextObj.transform.localPosition = new Vector3(0, -1.75f - (i * 0.5f), 0);

            //set resource increase
            planetInfo.resourceMax[ResourceNum(i)] *= planetInfo.StorageMultiply;
        }
    }

    public bool GainResourceExternal(ERESOURCES _resource, int _amount)
    {
        if(_amount > 0)
        {
            uiManager.AddNotificiton(_amount + " of " + _resource.ToString() + " arrived at " + planetInfo.name, planetInfo.sprite);
            soundManager.PlaySound(pickUpSound);
        }
        else
        {
            uiManager.AddNotificiton(_amount + " of " + _resource.ToString() + " transported from " + planetInfo.name, planetInfo.sprite);
        }

        if (planetInfo.resourceValues[(int)_resource] + _amount <= planetInfo.resourceMax[(int)_resource])
        {
            //do gain here
            //SpawnCollectedText((int)_resource, _amount);
            planetInfo.resourceValues[(int)_resource] += _amount;
            UpdateProductionText();
            return false;
        }
        else
        {
            //float whatsLeft = (planetInfo.resourceValues[(int)_resource] + _amount) - planetInfo.resourceMax[(int)_resource];
            planetInfo.resourceValues[(int)_resource] = planetInfo.resourceMax[(int)_resource];
            //return whatsLeft;
            return true;
        }
    }

    int ResourceNum(int _i)
    {
        return (int)planetInfo.productionResources[_i];
    }

    //planet visable production text
    void UpdateProductionText()
    {
        if (GetPlanetInfo().hasBeenUnlocked)
        {
            for (int i = 0; i < lengthOfProdRes; i++)
            {
                productionText[i].text = "" + planetInfo.productionResources[i].ToString() + ": " +
                    planetInfo.resourceValues[ResourceNum(i)] + "/" + planetInfo.resourceMax[ResourceNum(i)] + " +" + planetInfo.productionTime[i] + "/s";
            }

            if (currentlyClicked)
            {
                uiManager.UpdateRequirementsText();
                uiManager.UpdateResourcesTab();
            }
        }
    }

    public bool UpgradeProduction()
    {
        int level = GetLevelProdTier() * 2;

        if (CanBuyProduction(level))
        {
            IncreaseLevelProd(1);

            planetInfo.resourceValues[(int)upgradeReq.upProdResource[level]] -= upgradeReq.upProdAmount[level];
            if (upgradeReq.upProdResource[level + 1] != ERESOURCES.None)
            { planetInfo.resourceValues[(int)upgradeReq.upProdResource[level + 1]] -= upgradeReq.upProdAmount[level + 1]; }

            for (int i = 0; i < lengthOfProdRes; i++)
            {
                planetInfo.productionGain[i] *= ((upgradeReq.upProdIncrease[GetLevelProdTier()] + 100) * 0.01f);
            }
            UpdateProductionText();
        }
        else
        {
            print("not enough resources");
            return false;
        }
        return true;
    }
    public bool UpgradeStorage()
    {
        int level = GetLevelStorTier() * 2;

        if (CanBuyStorage(level))
        {
            IncreaseLevelStor(1);

            planetInfo.resourceValues[(int)upgradeReq.upStorResource[level]] -= upgradeReq.upStorAmount[level];
            if (upgradeReq.upStorResource[level + 1] != ERESOURCES.None)
            { planetInfo.resourceValues[(int)upgradeReq.upStorResource[level + 1]] -= upgradeReq.upStorAmount[level + 1]; }

            spaceStationInfo.unitMax += spaceStationInfo.unitMaxIncrease;

            for (int i = 0; i < lengthOfProdRes; i++)
            {
                planetInfo.resourceMax[ResourceNum(i)] *= (upgradeReq.upStorIncrease[GetLevelStorTier()] + 100) * 0.01f;
            }
            UpdateProductionText();
        }
        else
        {
            print("not enough resources");
            return false;
        }
        return true;
    }
    public bool UnitPurchased()
    {
        if (planetInfo.resourceValues[(int)spaceStationInfo.unitBuyResource[0]] >= spaceStationInfo.unitBuyAmount[0]
            && (spaceStationInfo.units + spaceStationInfo.unitGain) <= spaceStationInfo.unitMax)
        {
            planetInfo.resourceValues[(int)spaceStationInfo.unitBuyResource[0]] -= spaceStationInfo.unitBuyAmount[0];

            spaceStationInfo.units += spaceStationInfo.unitGain;
        }
        else
        {
            print("not enough resources");
            return false;
        }
        return true;
    }

    public void IncreaseLevelProd(int _i)
    {
        planetInfo.levelProd += _i;
        uiManager.UpdateLevels();

        uiManager.UpdateRequirementsText();

        if (planetInfo.levelProd % levelUpAmount == 0)
        {
            uiManager.AddNotificiton(planetInfo.name + " leveled up production to level " + GetLevelProdTier(), planetInfo.sprite);
            Destroy(Instantiate(levelUpPrefab, transform.position, Quaternion.identity));
            soundManager.PlaySound(levelUpSound);
            SetDoubleSpeed(levelUpDoubleSpeedTime);
            UpdateProductionText();
        }
    }
    public void IncreaseLevelStor(int _i)
    {
        planetInfo.levelStor += _i;
        uiManager.UpdateLevels();

        uiManager.UpdateRequirementsText();

        if (planetInfo.levelStor % levelUpAmount == 0)
        {
            uiManager.AddNotificiton(planetInfo.name + " leveled up storage to level " + GetLevelStorTier(), planetInfo.sprite);
            Destroy(Instantiate(levelUpPrefab, transform.position, Quaternion.identity));
            soundManager.PlaySound(levelUpSound);
            SetDoubleSpeed(levelUpDoubleSpeedTime);
        }
    }

    public bool CanBuyProduction(int _level)
    {
        if(upgradeReq.upProdResource[_level + 1] == ERESOURCES.None)
        {
            return planetInfo.resourceValues[(int)upgradeReq.upProdResource[_level]] >= upgradeReq.upProdAmount[_level];
        }
        else
        {
            return planetInfo.resourceValues[(int)upgradeReq.upProdResource[_level]] >= upgradeReq.upProdAmount[_level]
                && planetInfo.resourceValues[(int)upgradeReq.upProdResource[_level + 1]] >= upgradeReq.upProdAmount[_level + 1];
        }
    }
    public bool CanBuyStorage(int _level)
    {
        if (upgradeReq.upStorResource[_level + 1] == ERESOURCES.None)
        {
            return planetInfo.resourceValues[(int)upgradeReq.upStorResource[_level]] >= upgradeReq.upStorAmount[_level];
        }
        else
        {
            return planetInfo.resourceValues[(int)upgradeReq.upStorResource[_level]] >= upgradeReq.upStorAmount[_level]
            && planetInfo.resourceValues[(int)upgradeReq.upStorResource[_level + 1]] >= upgradeReq.upStorAmount[_level + 1];
        }
    }

    void Save()
    {
        if(debugSaveActive)
        {
            saveScript.SavePlanetInfo(planetInfo);
        }
    }

    private void UpdateInfo()
    {
        if (debugSaveActive)
        {
            planetInfo = saveScript.GetSavedPlanetInfo(planetInfo);
        }
    }

    public void RemoveDefensePoints(int _amount)
    {
        GetPlanetInfo().defensePoints -= _amount;

        uiManager.AddNotificiton(planetInfo.name + " defenses reduced by " + _amount, planetInfo.dullSprite);

        //check if planet has been unlocked
        if (GetPlanetInfo().defensePoints <= 0)
        {
            GetPlanetInfo().hasBeenUnlocked = true;
            SetUpProductionText();
            GetComponent<SpriteRenderer>().sprite = planetInfo.sprite;

            uiManager.AddNotificiton("New Planet Discovered - " + planetInfo.name, planetInfo.sprite);
            uiManager.ShowPlanetPopUp(this);

            //add to unlocked and remove from locked
            transportManager.AddToLockedPlanet(false, this);
            transportManager.AddToUnlockedPlanet(true, this);

            for(int i = 0; i < planetInfo.gainedResources.Length; i++)
            {
                uiManager.SetUnlockedResources(planetInfo.gainedResources[i], true);
            }
        }

        UpdateDefensePointsText();
    }

    public void UpdateDefensePointsText()
    {
        defensePointsText.gameObject.SetActive(!GetPlanetInfo().hasBeenUnlocked);
        defensePointsText.text = "Defense " + GetPlanetInfo().defensePoints + "/" + GetPlanetInfo().defenseMax;
    }

    public int GetLevelProdTier()
    {
        return (int)Mathf.Floor(planetInfo.levelProd / levelUpAmount);
    }
    public int GetLevelStorTier()
    {
        return (int)Mathf.Floor(planetInfo.levelStor / levelUpAmount);
    }

    public UpgradeRequirements GetUpgradeRequirements()
    {
        return upgradeReq;
    }
    public PlanetInfo GetPlanetInfo()
    {
        return planetInfo;
    }
    public SpaceStationInfo GetSpaceStationInfo()
    {
        return spaceStationInfo;
    }

    void OnMouseDown()
    {
        if (!currentlyClicked)
        {
            ClickedOn();
        }
    }

    public void ClickedOn()
    {
        if (planetInfo.hasBeenUnlocked)
        {
            currentlyClicked = !currentlyClicked;

            if (currentlyClicked && uiManager.GetPlanetScript() == null)
            {
                uiManager.UpdateInfoBox(transform.GetComponent<PlanetScript>());
                uiManager.ShowTab(0);
            }
            else
            {
                //uiManager.SetActiveInfoBox(false);
            }
        }
        else
        {
            Debug.LogWarning("display message for planet not being unlocked");
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
