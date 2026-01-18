using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    
    [SerializeField] private GameObject JournalScreen;
    public void OnJournalButtonClicked()
    {
        OpenJournal();
    }

    public void OpenJournal()
    {
        JournalScreen.SetActive(true);
    }
    
    public void CloseJournal()
    {
        JournalScreen.SetActive(false);
    }
}
