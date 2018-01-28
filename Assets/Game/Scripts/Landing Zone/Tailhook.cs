using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailhook : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> _pathSegements; 
    private Flightpath _landingPath;

    private int _activeCount = 0;

    /// <summary>
    /// Returns back the number of active directables
    /// that are currently using this tail hook. 
    /// </summary>
    public int activeCount
    {
        get { return _activeCount;}
    }

    private void Awake()
    {
        _landingPath = new Flightpath(transform.position);
        for(int i = 0; i < _pathSegements.Count; i++)
        {
            _landingPath.AddPosition(_pathSegements[i]);
        }
        _landingPath.drawPath = false;
        _landingPath.disposeOnComplete = false; 
        _landingPath.onPathExited += OnExitedPath;
        _landingPath.Finialized();
    }


    /// <summary>
    /// Takes a directable and forces it to land along
    /// the given path. 
    /// </summary>
    /// <param name="iDirectable">The thing you want to land</param>
    public void LandDirectable(IDirectable iDirectable)
    {
        iDirectable.SetFlightpath(_landingPath);
    }

    /// <summary>
    /// Invoked whenever we have had a plan exit it's path
    /// </summary>
    private void OnExitedPath(Flightpath flightPath, IDirectable IDirectable)
    {
        flightPath.drawPath = flightPath.occupancy > 0;
    }
}
