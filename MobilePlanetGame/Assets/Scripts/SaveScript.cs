using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveScript : MonoBehaviour
{
    public void SavePlanetInfo(PlanetScript.PlanetInfo _planetInfo)
    {
        string planetNum = "Planet" + _planetInfo.planetNum;

        //save
        PlayerPrefs.SetInt(planetNum + "hasBeenUnlocked", BoolToInt(_planetInfo.hasBeenUnlocked));
        PlayerPrefs.SetInt(planetNum + "isDoubleSpeed", BoolToInt(_planetInfo.isDoubleSpeed));
        PlayerPrefs.SetInt(planetNum + "level", _planetInfo.level);

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
        _planetInfo.level = PlayerPrefs.GetInt(planetNum + "level");

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