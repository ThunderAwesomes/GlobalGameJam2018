using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField]
	private OVRManager _ovrManagerPrefab;
	[SerializeField]
	private GameMode.Type _selectedGameModeType;
	[SerializeField]
	private GameMode[] _gameModes;


	private GameMode _gameModeInstance;
	private OVRManager _ovrManager;
	private static Game _instance;

	public static Game Instance
	{
		get { return _instance; }
	}

	public GameMode Mode
	{
		get { return _gameModeInstance; }
	}

	private void Awake()
	{
		StartMode();
		CreateInputManager();
		transform.position = Vector3.zero;
		_instance = this;
	}

	[ContextMenu("Start")]
	public void StartMode()
	{
		for (int i = 0; i < _gameModes.Length; i++)
		{
			if (_gameModes[i].GameType == _selectedGameModeType)
			{
				_gameModeInstance = Instantiate(_gameModes[i]);
				break;
			}
		}
		_gameModeInstance.OnGameStarted();
	}

	[ContextMenu("End")]
	public void EndMode()
	{
		if (_gameModeInstance != null)
		{
			_gameModeInstance.OnGameEnded();
			_gameModeInstance = null;
		}
	}

	private void Update()
	{
		if (_gameModeInstance != null)
		{
			if (_gameModeInstance.isActive)
			{
				_gameModeInstance.Update();
			}
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
