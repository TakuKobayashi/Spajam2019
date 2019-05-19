using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tochikuru;

public class GlobalController : SingletonBehaviour<GlobalController>
{
    [SerializeField] GameObject locationLoaderPrefab;
    //[SerializeField] GameObject stockControllerPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        //Util.InstantiateTo(this.gameObject, locationLoaderPrefab);
        //Util.InstantiateTo(this.gameObject, stockControllerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
