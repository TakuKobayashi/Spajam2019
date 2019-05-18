using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tochikuru;

public class LocationLoader : SingletonBehaviour<LocationLoader>
{
    public Action<Compass, LocationService> OnLoadLocation = null;

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (OnLoadLocation != null) OnLoadLocation(Input.compass, Input.location);
    }

    void OnDestroy()
    {
        Input.compass.enabled = false;
        Input.location.Stop();
    }
}
