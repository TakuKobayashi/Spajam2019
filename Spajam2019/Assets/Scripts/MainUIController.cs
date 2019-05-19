using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> uipages;

    private int currentPage = 0;

    void Start()
    {
        ShowCurrentPage();
    }
    
    void Update()
    {
        
    }

    public void OnMovePageButtonClicked(int page)
    {
        currentPage = currentPage + page;
        currentPage = Mathf.Max(currentPage, 0);
        currentPage = Mathf.Min(currentPage, uipages.Count);
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        for(int i = 0;i < uipages.Count; ++i)
        {
            uipages[i].gameObject.SetActive(i == currentPage);
        }
    }
}
