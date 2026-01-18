using UnityEngine;
using TMPro;

public class LanguagePreference : MonoBehaviour
{
    public TMP_InputField languagePreference; //The name entered in the input field is the name stored in variable playerUserName

    public void Language()
    {
        string language = languagePreference.text; //the text is now stored in a public string variable named name
        PlayerPrefs.SetString("LanguageChosen", language); //built in unity command to store player's information in database
        PlayerPrefs.Save(); //saves user's preferences in database
    }

}
