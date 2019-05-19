using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigaterController : MonoBehaviour
{
    [SerializeField] GameObject navigaterCharacter;
    [SerializeField] Vector3 characterPosition;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform cameraTransform = Camera.main.transform;
        navigaterCharacter.transform.position = cameraTransform.position - characterPosition;
        
        navigaterCharacter.transform.LookAt(new Vector3(cameraTransform.position.x, navigaterCharacter.transform.position.y, cameraTransform.position.z));
    }
}
