using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenLabsManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] [Range(0f, 1f)] private float stability = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float similarityBoost = 0.75f;
    
    private const string TTS_URL = "https://api.elevenlabs.io/v1/text-to-speech/";
    private const string STT_URL = "https://api.elevenlabs.io/v1/speech-to-text";

    private String _elevenLabsApiKey = "";
    
    // REQUESTS CLASSES
    [System.Serializable]
    public class RequestBody
    {
        public String text;
        public String model;
        public String outputForm;
    }

    public void GenerateVoiceOver(String prompt, String voiceID)
    {
        StartCoroutine(TextToSpeechCoroutine(prompt, voiceID,() =>
            {
                Debug.Log("success");
            }, (String error) =>
            {
                Debug.Log("error: " + error);
            }
        ));
    }

    private string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
    private String CreateRequestBody(String text)
    {
        return $@"{{
          ""text"": ""{EscapeJson(text)}"",
          ""model_id"": ""eleven_monolingual_v1"",
          ""voice_settings"": {{
            ""stability"": {stability.ToString(System.Globalization.CultureInfo.InvariantCulture)},
            ""similarity_boost"": {similarityBoost.ToString(System.Globalization.CultureInfo.InvariantCulture)}
          }}
        }}";
    }

    private IEnumerator TextToSpeechCoroutine(String text, String voiceID, Action onComplete, Action<string> onError)
    {
        
        string jsonData = CreateRequestBody(text);

        using (UnityWebRequest request = new UnityWebRequest(TTS_URL + voiceID, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(TTS_URL + voiceID, AudioType.MPEG);
            
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("xi-api-key", _elevenLabsApiKey);

            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"TTS Error: {request.error}");
                onError?.Invoke(request.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.clip = clip;
                audioSource.Play();
                
                Debug.Log("Text-to-Speech successful!");
                onComplete?.Invoke();
            }
        }
    }
}