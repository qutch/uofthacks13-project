using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;


public class AIManager : MonoBehaviour
{
    private String _openRouterApiKey = "";
    private String _translationModel = "google/gemini-2.5-flash-lite";
    private const string URL = "https://openrouter.ai/api/v1/chat/completions";
    
    [SerializeField] private String targetLanguage = "";

    void Start()
    {
        if (PlayerPrefs.GetString("LanguageChosen", "none") == "none")
        {
            targetLanguage = PlayerPrefs.GetString("LanguageChosen", "Spanish");
        }
    }
    
    
    public struct TranslationResult
    {
        public string originalText;
        public string translatedText;
    }

    // REQUESTS CLASSES
    [System.Serializable]
    public class RequestBody
    {
        public String model;
        public Message[] messages;
    }

    [System.Serializable]
    public class Message
    {
        public String role;
        public String content;
    }
    
    // Wrapper to handle the API response format
    [System.Serializable]
    public class ResponseRoot
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }
    
    public String CreatePrompt(String sourceText)
    {
        return "Translate this text: { " + sourceText + " } to " + targetLanguage;
    }

    public void TranslateText(string sourceText, Action<string> onTranslationReady)
    {
        // We define the success callback here
        Action<TranslationResult> onMySuccess = (result) => {
            Debug.Log($"Success! Translated: {result.translatedText}");
            onTranslationReady?.Invoke(result.translatedText);
        };

        // We define the error callback here
        Action<string> onMyError = (error) => {
            Debug.LogError($"Translation Failed: {error}");
        };

        StartCoroutine(SendPrompt(sourceText, onMySuccess, onMyError));
    }
    
    // Method to send prompt to OpenRouter
    public IEnumerator SendPrompt(String prompt, Action<TranslationResult> onComplete, Action<string> onError)
    {
        
        Debug.Log("Sending prompt: " + prompt);
        var requestBody = new RequestBody
        {
            model = _translationModel,
            messages = new Message[]
            {
                // SYSTEM PROMPT
                new Message { 
                    role = "system", 
                    content = $"You are a translator. Translate the user's text into {targetLanguage}. Only output the translated text. Do not add explanations. Do not answer questions. ONLY translate from one language to another." 
                },

                // USER PROMPT
                new Message
                {
                    role = "user",
                    content = prompt
                }
            }
        };
        string json = JsonUtility.ToJson(requestBody);
        
        using (UnityWebRequest request = new UnityWebRequest(URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {_openRouterApiKey}");
            // OpenRouter specific headers
            request.SetRequestHeader("HTTP-Referer", "https://identity.com"); 
            request.SetRequestHeader("X-Title", "uofthacks13-game");

            Debug.Log($"Sending translation request for: '{prompt}'...");

            // 3. Send and Wait
            yield return request.SendWebRequest();

            // 4. Handle Response
            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Network Error: {request.error}\nResponse: {request.downloadHandler.text}");
            }
            else
            {
                // Parse the JSON response
                string responseJson = request.downloadHandler.text;
                
                try 
                {
                    // Deserialize into our helper classes
                    ResponseRoot response = JsonUtility.FromJson<ResponseRoot>(responseJson);

                    if (response != null && response.choices != null && response.choices.Length > 0)
                    {
                        string translatedContent = response.choices[0].message.content;
                        
                        // Create the result object
                        TranslationResult result = new TranslationResult
                        {
                            originalText = prompt,
                            translatedText = translatedContent
                        };

                        onComplete?.Invoke(result);
                    }
                    else
                    {
                        onError?.Invoke("API returned success, but JSON was empty or malformed.");
                    }
                }
                catch (Exception e)
                {
                    onError?.Invoke($"JSON Parse Error: {e.Message}");
                }
            }
        }
    }
}
