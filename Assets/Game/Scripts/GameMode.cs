using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private OVRManager _ovrManagerPrefab;


    private OVRManager _ovrManager;

    private void Awake()
    {
        StartMode();
        transform.position = Vector3.zero;
    }

    public void StartMode()
    {
        CreateInputManager();
    }

    public void EndMode()
    {
        if(_ovrManager != null)
        {
            Destroy(_ovrManager);
        }
    }

    private void CreateInputManager()
    {
        _ovrManager = Instantiate<OVRManager>(_ovrManagerPrefab);
        _ovrManager.transform.SetParent(transform);
        _ovrManager.name = _ovrManagerPrefab.name;
        _ovrManager.transform.localPosition = Vector3.zero;
    }
}
