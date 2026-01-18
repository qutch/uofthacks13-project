using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class ElevenLabsSTTManager : MonoBehaviour
{
    private const string STT_URL = "https://api.elevenlabs.io/v1/speech-to-text";

    [SerializeField] private string _elevenLabsApiKey = "";
    [SerializeField] private int sampleRate = 16000;
    [SerializeField] private int maxRecordSeconds = 10;

    private AudioClip recordingClip;
    private string micDevice;
    private bool isRecording;

    // Start microphone recording (optional: pass device name)
    public void StartRecording(string device = null)
    {
        if (isRecording) return;
        micDevice = device;
        recordingClip = Microphone.Start(micDevice, false, maxRecordSeconds, sampleRate);
        isRecording = true;
    }

    // Stop recording and send to ElevenLabs STT
    public void StopRecordingAndTranscribe(Action<string> onComplete, Action<string> onError)
    {
        if (!isRecording)
        {
            onError?.Invoke("Not recording.");
            return;
        }

        Microphone.End(micDevice);
        isRecording = false;

        if (recordingClip == null || recordingClip.samples == 0)
        {
            onError?.Invoke("No audio captured.");
            return;
        }

        byte[] wav = AudioClipToWav(recordingClip);
        StartCoroutine(SendSpeechToTextCoroutine(wav, onComplete, onError));
    }

    private IEnumerator SendSpeechToTextCoroutine(byte[] wavData, Action<string> onComplete, Action<string> onError)
    {
        var form = new WWWForm();
        form.AddField("model_id", "scribe_v1");
        form.AddBinaryData("file", wavData, "speech.wav", "audio/wav");

        using (UnityWebRequest request = UnityWebRequest.Post(STT_URL, form))
        {
            request.SetRequestHeader("xi-api-key", _elevenLabsApiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Network Error: {request.error}\n{request.downloadHandler.text}");
                yield break;
            }

            string json = request.downloadHandler.text;

            // Try to parse a top-level "text" field
            try
            {
                // Response helper for simple { "text": "..." } responses
                var resp = JsonUtility.FromJson<TextResponse>(json);
                if (!string.IsNullOrEmpty(resp.text))
                {
                    onComplete?.Invoke(resp.text);
                    yield break;
                }

                // Fallback: attempt to find "text" value by simple search
                const string key = "\"text\":";
                int idx = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    int start = json.IndexOf('"', idx + key.Length);
                    if (start >= 0)
                    {
                        start += 1;
                        int end = json.IndexOf('"', start);
                        if (end > start)
                        {
                            string txt = json.Substring(start, end - start);
                            onComplete?.Invoke(txt);
                            yield break;
                        }
                    }
                }

                onError?.Invoke("Could not extract transcribed text from response:\n" + json);
            }
            catch (Exception e)
            {
                onError?.Invoke("Parse error: " + e.Message);
            }
        }
    }

    [Serializable]
    private class TextResponse
    {
        public string text;
    }

    // Convert AudioClip (PCM float samples) to WAV (16-bit PCM) bytes
    private byte[] AudioClipToWav(AudioClip clip)
    {
        int channels = clip.channels;
        int samples = clip.samples * channels;
        float[] floatData = new float[samples];
        clip.GetData(floatData, 0);

        using (var ms = new MemoryStream())
        using (var bw = new BinaryWriter(ms))
        {
            // RIFF header
            bw.Write(Encoding.UTF8.GetBytes("RIFF"));
            bw.Write((int)0); // placeholder for file size
            bw.Write(Encoding.UTF8.GetBytes("WAVE"));

            // fmt chunk
            bw.Write(Encoding.UTF8.GetBytes("fmt "));
            bw.Write(16); // subchunk1 size
            bw.Write((short)1); // audio format = PCM
            bw.Write((short)channels);
            bw.Write(sampleRate);
            int byteRate = sampleRate * channels * 2;
            bw.Write(byteRate);
            short blockAlign = (short)(channels * 2);
            bw.Write(blockAlign);
            bw.Write((short)16); // bits per sample

            // data chunk
            bw.Write(Encoding.UTF8.GetBytes("data"));
            int dataSize = samples * 2;
            bw.Write(dataSize);

            // write PCM16 samples
            for (int i = 0; i < floatData.Length; i++)
            {
                float f = Mathf.Clamp(floatData[i], -1f, 1f);
                short s = (short)(f * short.MaxValue);
                bw.Write(s);
            }

            // fill file size
            bw.Seek(4, SeekOrigin.Begin);
            bw.Write((int)(ms.Length - 8));

            return ms.ToArray();
        }
    }
}
