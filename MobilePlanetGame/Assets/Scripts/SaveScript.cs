using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveScript : MonoBehaviour
{
    UIManager uiManager;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        PlayerPrefs.SetInt("FirstTime", 0);

        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);

            for (int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
            {
                PlayerPrefs.SetFloat("resourceUnlocked" + i, 0);
            }

            //set unlocked resources
            uiManager.SetUnlockedResources(PlanetScript.ERESOURCES.Timber);
            uiManager.SetUnlockedResources(PlanetScript.ERESOURCES.Water);
            uiManager.SetUnlockedResources(PlanetScript.ERESOURCES.Sulphur);
        }
    }

    public void SaveGeneralInfo()
    {
        //set unlocked resources
        for(int i = 0; i < uiManager.GetUnlockedResources().Count; i++)
        {
            PlayerPrefs.SetFloat("resourceUnlocked" + i, 1);
        }
    }

    public void SavePlanetInfo(PlanetScript.PlanetInfo _planetInfo)
    {
        string planetNum = "Planet" + _planetInfo.planetNum;

        //save
        PlayerPrefs.SetInt(planetNum + "hasBeenUnlocked", BoolToInt(_planetInfo.hasBeenUnlocked));
        PlayerPrefs.SetInt(planetNum + "isDoubleSpeed", BoolToInt(_planetInfo.isDoubleSpeed));
        PlayerPrefs.SetInt(planetNum + "levelProd", _planetInfo.levelProd);
        PlayerPrefs.SetInt(planetNum + "levelStor", _planetInfo.levelStor);

        for(int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
        {
            PlayerPrefs.SetFloat(planetNum + "resource" + i, _planetInfo.resourceValues[i]);
        }
    }

    public PlanetScript.PlanetInfo GetSavedPlanetInfo(PlanetScript.PlanetInfo _planetInfo)
    {
        string planetNum = "Planet" + _planetInfo.planetNum;

        _planetInfo.hasBeenUnlocked = IntToBool(PlayerPrefs.GetInt(planetNum + "hasBeenUnlocked"));
        _planetInfo.isDoubleSpeed = IntToBool(PlayerPrefs.GetInt(planetNum + "isDoubleSpeed"));
        _planetInfo.levelProd = PlayerPrefs.GetInt(planetNum + "levelProd");
        _planetInfo.levelStor = PlayerPrefs.GetInt(planetNum + "levelStor");

        for (int i = 0; i < Enum.GetNames(typeof(PlanetScript.ERESOURCES)).Length; i++)
        {
            _planetInfo.resourceValues[i] = PlayerPrefs.GetFloat(planetNum + "resource" + i);
        }

        return _planetInfo;
    }

    int BoolToInt(bool _b)
    {
        return (_b)? 1 : 0;
    }
    bool IntToBool(int _i)
    {
        return _i == 1;
    }
}
