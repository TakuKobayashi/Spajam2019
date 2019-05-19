using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> uipages;

    void Start()
    {
        ShowCurrentPage();
    }
    
    void Update()
    {
        
    }

    public void OnMovePageButtonClicked(int page)
    {
        int currentPage = GlobalController.Instance.CurrentMainMenuPage;
        currentPage = currentPage + page;
        currentPage = Mathf.Max(currentPage, 0);
        currentPage = Mathf.Min(currentPage, uipages.Count);
        GlobalController.Instance.CurrentMainMenuPage = currentPage;

        if (currentPage == 5)
        {
            SceneManager.LoadScene("ARScene");
        }
        else
        {
            ShowCurrentPage();
        }
    }

    private void ShowCurrentPage()
    {
        for(int i = 0;i < uipages.Count; ++i)
        {
            uipages[i].gameObject.SetActive(i == GlobalController.Instance.CurrentMainMenuPage);
        }
    }
}
