using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tochikuru;

public class GlobalController : SingletonBehaviour<GlobalController>
{
    [SerializeField] GameObject locationLoaderPrefab;

    public int CurrentMainMenuPage = 0;

    // Start is called before the first frame update
    void Awake()
    {
        Util.InstantiateTo(this.gameObject, locationLoaderPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
