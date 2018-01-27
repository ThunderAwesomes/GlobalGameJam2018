using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private VRInputManager _inputManagerPrefab;


    private VRInputManager _inputManager;

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
        if(_inputManager != null)
        {
            Destroy(_inputManager);
        }
    }

    private void CreateInputManager()
    {
        _inputManager = Instantiate<VRInputManager>(_inputManagerPrefab);
        _inputManager.transform.SetParent(transform);
        _inputManager.name = _inputManagerPrefab.name;
        _inputManager.transform.localPosition = Vector3.zero;
    }
}
