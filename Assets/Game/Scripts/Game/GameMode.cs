using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : ScriptableObject
{
	private int _planesLanded;
	private int _planesLost;
	private bool _isActive;

	public enum Type
	{
		Undefined,
		Timed,
	}

	public bool isActive
	{
		get { return _isActive; }
	}
	

	public abstract Type GameType
	{
		get;
	}

	public int planesLost
	{
		get { return _planesLost; }
	}

	public int planesLanded
	{
		get { return _planesLanded; }
	}
	

	public virtual void OnPlaneDestroyed()
	{
		_planesLost++;
	}

	public virtual void OnPlaneLanded()
	{
		_planesLanded++;
	}

	public virtual void OnGameStarted()
	{
		_isActive = true;
	}

	public virtual void OnGameEnded()
	{
		_isActive = false;
	}

	public virtual void Update()
	{

	}
}
