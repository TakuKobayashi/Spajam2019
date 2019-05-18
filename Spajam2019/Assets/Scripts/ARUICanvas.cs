using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARUICanvas : MonoBehaviour
{
    [SerializeField] private Text debugText;
    // Start is called before the first frame update
    void Start()
    {
        LocationLoader.Instance.OnLoadLocation = OnLocationLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLocationLoaded(Compass compass, LocationService locationService)
    {
        debugText.text = "aaaaaaaa";
        /*
        debugText.text = string.Format("headingAccuracy:{0}\n magneticHeading:{1}\n rawVector:{2}\n timestamp:{3}\n trueHeading:{4}\n altitude:{5}\n latitude:{6}\n longitude:{7}\n horizontalAccuracy:{8}\n verticalAccuracy:{9}\n timestamp:{10}\n status:{11}",
            compass.headingAccuracy,
            compass.magneticHeading,
            compass.rawVector.ToString(),
            compass.timestamp,
            compass.trueHeading,
            locationService.lastData.altitude,
            locationService.lastData.latitude,
            locationService.lastData.longitude,
            locationService.lastData.horizontalAccuracy,
            locationService.lastData.verticalAccuracy,
            locationService.lastData.timestamp,
            locationService.status.ToString()
            );
            */
    }
}
