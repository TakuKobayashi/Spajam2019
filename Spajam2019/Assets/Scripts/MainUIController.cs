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
        int currentPage = PlayerPrefs.GetInt("page", 0);
        currentPage = currentPage + page;
        currentPage = Mathf.Max(currentPage, 0);
        if(currentPage >= uipages.Count)
        {
            currentPage = 0;
        }
        PlayerPrefs.SetInt("page", currentPage);
        PlayerPrefs.Save();

        if (currentPage == 4)
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
            uipages[i].gameObject.SetActive(i == PlayerPrefs.GetInt("page", 0));
        }
    }
}
