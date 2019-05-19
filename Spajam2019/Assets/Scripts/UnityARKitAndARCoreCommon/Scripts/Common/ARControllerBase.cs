namespace ARKitAndARCoreCommon
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    // iOSで動くか要検証
    using GoogleARCore;

    public abstract class ARControllerBase : MonoBehaviour
    {
        [SerializeField] protected GameObject ARMainCameraPrefab;
        [SerializeField] protected GameObject DetectedPlanePrefab;
        [SerializeField] private GameObject PointCloudPrefab;

        [SerializeField] private GameObject newsPaperPrefab;

        protected GameObject pointCloudObj;
        protected Camera mainCamera;
        //protected List<NewsPaper> appearedNewsPaper = new List<NewsPaper>();
        protected NewsPaper appearingNewsPaper = null;

        protected virtual void Awake()
        {
            Camera defaultCamera = Camera.main;
            if (defaultCamera != null)
            {
                defaultCamera.enabled = false;
                defaultCamera.gameObject.SetActive(false);
            }
            GameObject mainCameraManager = GameObject.Find(ARMainCameraPrefab.name);
            if(mainCameraManager == null){
                mainCameraManager = Util.InstantiateTo(this.gameObject, ARMainCameraPrefab);
            }
            mainCamera = Util.FindCompomentInChildren<Camera>(mainCameraManager.transform);
            if (mainCamera == null)
            {
                mainCamera = mainCameraManager.GetComponent<Camera>();
            }
            mainCamera.enabled = true;
            mainCamera.gameObject.SetActive(true);
        }

        protected virtual void Update()
        {
        }

        protected virtual void AppearNewsPaper(GameObject root)
        {
            if(appearingNewsPaper == null)
            {
                appearingNewsPaper = Util.InstantiateTo<NewsPaper>(root, newsPaperPrefab);
                appearingNewsPaper.Scaleing();
            }
        }

        public static GameObject CreateAnchor(Vector3 pos, Quaternion rotate)
        {
            Pose pose = new Pose(pos, rotate);
            Anchor anchor = Session.CreateAnchor(pose);
            return anchor.gameObject;
        }
    }
}