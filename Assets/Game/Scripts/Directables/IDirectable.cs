using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectable 
{
    void OnSelected();
    void SetFlightPath(Flightpath flightPath);
    void OnDeselected();
}
