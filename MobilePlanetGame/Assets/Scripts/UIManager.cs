using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject infoBox;
    [SerializeField] private GameObject cameraObj;

    private void Start()
    {
        SetActiveInfoBox(false);
    }

    public void SetActiveInfoBox(bool _active)
    {
        infoBox.SetActive(_active);
    }

    public void UpdateInfoBox(Vector3 _pos)
    {
        SetActiveInfoBox(true);
        cameraObj.transform.position = _pos;
    }
}
