using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    
    [SerializeField] private GameObject JournalScreen;
    public void OnJournalButtonClicked()
    {
        Debug.Log("Clicked journal button");
        OpenJournal();
    }

    public void OpenJournal()
    {
        Debug.Log("Open Journa");
        JournalScreen.SetActive(true);
    }
    
    public void CloseJournal()
    {
        JournalScreen.SetActive(false);
    }
}
