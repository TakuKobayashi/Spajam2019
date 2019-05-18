using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tochikuru;

public class GlobalController : SingletonBehaviour<GlobalController>
{
    [SerializeField] GameObject locationLoaderPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Util.InstantiateTo(this.gameObject, locationLoaderPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
