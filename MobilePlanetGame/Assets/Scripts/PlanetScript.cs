using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetScript : MonoBehaviour
{
    public enum ERESOURCES
    {
        Water,
        Nitrogen,
        Uranium,
        Sulphur,
        Coal,
        Iron,
        Gold
    };

    [Serializable]
    public class PlanetInfo
    {
        public string name;
        public Sprite sprite;
        public ERESOURCES[] productionResources;
        public float[] productionTime;
        public float[] productionGain;
        public float[] productionMax;

        public float[] upgradeGain;
        public float[] upgradeStorage;

        [NonSerialized] public List<float> resourceValues = new List<float>();
        [NonSerialized] public List<float> time = new List<float>();
    };

    [SerializeField] PlanetInfo planetInfo;
    [SerializeField] TextMesh nameText;
    [SerializeField] UIManager uiManager;

    List<TextMesh> productionText = new List<TextMesh>();

    public Transform productionTextsParent;

    int lengthOfAllResources;
    int lengthOfProdRes;

    private void Start()
    {
        lengthOfAllResources = Enum.GetNames(typeof(ERESOURCES)).Length;
        lengthOfProdRes = planetInfo.productionResources.Length;

        nameText.text = planetInfo.name;
        GetComponent<SpriteRenderer>().sprite = planetInfo.sprite;

        foreach (Transform child in productionTextsParent)
        {
            productionText.Add(child.GetComponent<TextMesh>());
        }

        for (int i = 0; i < lengthOfProdRes; i++)
        {
            planetInfo.time.Add(planetInfo.productionTime[i]);
        }

        for (int i = 0; i < lengthOfAllResources; i++)
        {
            planetInfo.resourceValues.Add(0);
        }

        UpdateProductionText();
    }

    private void Update()
    {
        for (int i = 0; i < lengthOfProdRes; i++)
        {
            planetInfo.time[i] -= Time.deltaTime;

            if (planetInfo.time[i] < 0)
            {
                planetInfo.time[i] = planetInfo.productionTime[i];

                if (planetInfo.resourceValues[ResourceNum(i)] + planetInfo.productionGain[i] <= planetInfo.productionMax[i])
                {
                    planetInfo.resourceValues[ResourceNum(i)] += planetInfo.productionGain[i];
                    UpdateProductionText();
                }
            }
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
                planetInfo.resourceValues[ResourceNum(i)] + "/" + planetInfo.productionMax[i] + " +" + planetInfo.productionTime[i] + "/s";
        }
    }

    public void UpgradeProduction()
    {
        for (int i = 0; i < lengthOfProdRes; i++)
        {
            planetInfo.productionGain[i] += planetInfo.upgradeGain[i];
        }
        UpdateProductionText();
    }
    public void UpgradeStorage()
    {
        for (int i = 0; i < lengthOfProdRes; i++)
        {
            planetInfo.productionMax[i] += planetInfo.upgradeStorage[i];
        }
        UpdateProductionText();
    }

    void OnMouseDown()
    {
        print("clicked");
        uiManager.UpdateInfoBox(transform.GetComponent<PlanetScript>(), transform.position);
    }
}
