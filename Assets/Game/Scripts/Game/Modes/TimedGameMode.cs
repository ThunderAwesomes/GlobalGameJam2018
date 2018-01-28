using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game Modes/Timed")]
public class TimedGameMode : GameMode
{
	[SerializeField]
	public PlaneSpawner _spawnerPrefab;
	[SerializeField]
	public float _gameTime;

	private PlaneSpawner _spawnerInstace;

	public override Type GameType
	{
		get { return Type.Timed; }
	}

	public override void OnGameStarted()
	{
		base.OnGameStarted();
		_spawnerInstace = GameObject.Instantiate(_spawnerPrefab);
		_spawnerInstace.transform.position = Vector3.zero;
	}

	public override void OnGameEnded()
	{
		base.OnGameEnded();
		if(_spawnerInstace != null)
		{
			Destroy(_spawnerInstace.gameObject);
			_spawnerInstace = null;
		}
	}

	public override void Update()
	{
		_gameTime -= Time.deltaTime;

		if(_gameTime <= 0f)
		{
			OnGameEnded();
		}
	}
}
