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
        Copper,
        Oil,
        Uranium,
        Nitrogen,
        Gold,
        Cosmic_Quartz,
        Space_Dust,
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
        public float[] productionMax;
        [NonSerialized] public float[] resourceMax = { 120, 72, 100, 100, 100, 100, 100, 100, 100, 100 }; //set on very first start

        //level variables
        [NonSerialized] public int level = 0;

        //lists
        [NonSerialized] public List<float> resourceValues = new List<float>();
        [NonSerialized] public List<float> time = new List<float>();

        //defense
        [NonSerialized] public int defensePoints;
        public int defenseMax;

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
    private UIManager uiManager;

    [SerializeField] private float collectedSpawnRand;
    [SerializeField] private int collectedDestroy;

    List<TextMesh> productionText = new List<TextMesh>();

    [SerializeField] private Transform productionTextsParent;

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

        lengthOfAllResources = Enum.GetNames(typeof(ERESOURCES)).Length;
        lengthOfProdRes = planetInfo.productionResources.Length;

        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        upgradeReq.maxUpgrades = upgradeReq.upProdResource.Length;

        nameText.text = planetInfo.name;
        GetComponent<SpriteRenderer>().sprite = (planetInfo.hasBeenUnlocked)? planetInfo.sprite : planetInfo.dullSprite;

        GetPlanetInfo().defensePoints = GetPlanetInfo().defenseMax;

        for (int i = 0; i < lengthOfProdRes; i++)
        {
            planetInfo.time.Add(planetInfo.productionTime[i]);
            GameObject productionTextObj = Instantiate(productionTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            productionText.Add(productionTextObj.GetComponent<TextMesh>());
            productionTextObj.transform.SetParent(productionTextsParent);
            productionTextObj.transform.localPosition = new Vector3(0, -1.75f - (i * 0.5f), 0);

            //set resource max
            //planetInfo.resourceMax[ResourceNum(i)] *= ((planetInfo.productionMax[i] + 100) * 0.01f);
            planetInfo.resourceMax[ResourceNum(i)] = planetInfo.productionMax[i];
        }

        for (int i = 0; i < lengthOfAllResources; i++)
        {
            planetInfo.resourceValues.Add(0);
        }

        //debug check
        if(planetInfo.resourceMax.Length != lengthOfAllResources)
        {
            Debug.LogError("resourceMax isn't the same length as the amount of resources " + planetInfo.resourceMax.Length + "/" + lengthOfAllResources);
        }

        UpdateProductionText();
        UpdateDefensePointsText();
    }

    private void Update()
    {
        if (planetInfo.hasBeenUnlocked)
        {
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

            if (planetInfo.isDoubleSpeed)
            {
                planetInfo.doubleSpeedTime -= Time.deltaTime;

                if (planetInfo.doubleSpeedTime < 0)
                {
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
    }

    void GainResource(int _i, float _gain)
    {
        planetInfo.resourceValues[ResourceNum(_i)] += _gain;
        UpdateProductionText();
    }

    void SpawnCollectedText(int _resource, float _gain)
    {
        Vector2 pos = new Vector2(
            transform.position.x + UnityEngine.Random.Range(collectedSpawnRand, -collectedSpawnRand),
            transform.position.y + UnityEngine.Random.Range(collectedSpawnRand, -collectedSpawnRand));
        GameObject textObj = Instantiate(collectedTextPrefab, pos, Quaternion.identity);
        textObj.GetComponent<TextMesh>().text = "+" + _gain;
        textObj.transform.GetChild(0).localPosition = new Vector3(4 + (_gain.ToString().Length * 1.2f), 0, -1);
        textObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = uiManager.GetResourceIcon()[_resource];

        Destroy(textObj, collectedDestroy);
    }

    public bool GainResourceExternal(ERESOURCES _resource, int _amount)
    {
        if (planetInfo.resourceValues[(int)_resource] + _amount <= planetInfo.resourceMax[(int)_resource])
        {
            //do gain here
            SpawnCollectedText((int)_resource, _amount);
            planetInfo.resourceValues[(int)_resource] += _amount;
            UpdateProductionText();
            return true;
        }
        else
        {
            return false;
        }
    }

    int ResourceNum(int _i)
    {
        return (int)planetInfo.productionResources[_i];
    }

    void UpdateProductionText()
    {
        for (int i = 0; i < lengthOfProdRes; i++)
        {
            productionText[i].text = "" + planetInfo.productionResources[i].ToString() + ": " +
                planetInfo.resourceValues[ResourceNum(i)] + "/" + planetInfo.resourceMax[ResourceNum(i)] + " +" + planetInfo.productionTime[i] + "/s";
        }

        if (currentlyClicked)
        {
            uiManager.UpdateResourcesTab();
        }
    }

    public void UpgradeProduction()
    {
        if (planetInfo.resourceValues[(int)upgradeReq.upProdResource[GetLevelTier()]] >= upgradeReq.upProdAmount[GetLevelTier()])
        {
            IncreaseLevel(1);

            planetInfo.resourceValues[(int)upgradeReq.upProdResource[GetLevelTier()]] -= upgradeReq.upProdAmount[GetLevelTier()];

            for (int i = 0; i < lengthOfProdRes; i++)
            {
                planetInfo.productionGain[i] *= (upgradeReq.upProdIncrease[GetLevelTier()] + 100) * 0.01f;
            }
            UpdateProductionText();
        }
        else
        {
            print("not enough resources");
        }
    }
    public void UpgradeStorage()
    {
        if (planetInfo.resourceValues[(int)upgradeReq.upStorResource[GetLevelTier()]] >= upgradeReq.upStorAmount[GetLevelTier()])
        {
            IncreaseLevel(1);

            planetInfo.resourceValues[(int)upgradeReq.upStorResource[GetLevelTier()]] -= upgradeReq.upStorAmount[GetLevelTier()];

            spaceStationInfo.unitMax += spaceStationInfo.unitMaxIncrease;

            for (int i = 0; i < lengthOfProdRes; i++)
            {
                planetInfo.resourceMax[ResourceNum(i)] *= (upgradeReq.upStorIncrease[GetLevelTier()] + 100) *0.01f;
            }
            UpdateProductionText();
        }
        else
        {
            print("not enough resources");
        }
    }
    public void UnitPurchased()
    {
        if (planetInfo.resourceValues[(int)spaceStationInfo.unitBuyResource[GetLevelTier()]] >= spaceStationInfo.unitBuyAmount[GetLevelTier()]
            && (spaceStationInfo.units + spaceStationInfo.unitGain) <= spaceStationInfo.unitMax)
        {
            planetInfo.resourceValues[(int)spaceStationInfo.unitBuyResource[GetLevelTier()]] -= spaceStationInfo.unitBuyAmount[GetLevelTier()];

            spaceStationInfo.units += spaceStationInfo.unitGain;
            uiManager.UpdateUnitsText();
        }
        else
        {
            print("not enough resources");
        }
    }

    public void IncreaseLevel(int _i)
    {
        planetInfo.level += _i;

        if(planetInfo.level % levelUpAmount == 0)
        {
            SetDoubleSpeed(levelUpDoubleSpeedTime);
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

    public void UpdateDefensePointsText()
    {
        defensePointsText.gameObject.SetActive(!GetPlanetInfo().hasBeenUnlocked);
        defensePointsText.text = "Defense " + GetPlanetInfo().defensePoints + "/" + GetPlanetInfo().defenseMax;
    }

    public int GetLevelTier()
    {
        return (int)Mathf.Floor(planetInfo.level / levelUpAmount);
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

            if (currentlyClicked)
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
