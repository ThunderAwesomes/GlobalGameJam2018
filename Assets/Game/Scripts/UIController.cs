using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _planesLanded;
	[SerializeField]
	private TextMeshPro _planesLost;
	[SerializeField]
	TextMeshPro _timeLeft; 

	private TimedGameMode _instance;

	private void Start()
	{
		_instance = Game.Instance.Mode as TimedGameMode;
	}
	// Use this for initialization
	private void Update ()
	{
		_planesLanded.text = _instance.planesLanded.ToString();
		_planesLost.text = _instance.planesLost.ToString();
		int minutes = Mathf.FloorToInt(_instance.timeLeft / 60f);
		int seconds = Mathf.FloorToInt(_instance.timeLeft % 60);
		_timeLeft.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
	}
}
