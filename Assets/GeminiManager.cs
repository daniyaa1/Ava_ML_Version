using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;
using UnityEngine.UI;
using System.IO;

[Serializable]
public class GeminiRequest { public Content[] contents; }
[Serializable]
public class Content { public Part[] parts; }
[Serializable]
public class Part { public string text; }
[Serializable]
public class GeminiResponse { public Candidate[] candidates; }
[Serializable]
public class Candidate { public Content content; }

[Serializable]
public class GeminiKeyData
{
    public string GEMINI_API_KEY;
}

public class GeminiManager : MonoBehaviour
{
    [Header("Gemini Settings")]
    public string modelName = "gemini-2.5-flash";

    private string apiKey;

    [Header("Connections")]
    public VoiceManager voice;

    [Header("UI Connections")]
    public TMP_InputField chatInput;
    public Button sendButton;

    [Header("Debug Info")]
    public string lastResponse;

    private string systemInstruction = "Your name is Eva. You are a helpful AI avatar. " +
                                       "IMPORTANT: Always provide very short, concise replies (maximum 20 words). " +
                                       "Never mention being a large language model or trained by Google. " +
                                       "Always complete your final sentence. Stay in character as Eva.";

    void Awake()
    {
        LoadApiKey();
    }

    void Start()
    {
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(OnSendButtonClick);
        }
    }

void LoadApiKey()
{
    string path = Path.Combine(Application.streamingAssetsPath, "gemini_key.json");

    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        GeminiKeyData keyData = JsonUtility.FromJson<GeminiKeyData>(json);
        apiKey = keyData.GEMINI_API_KEY;

        Debug.Log("Gemini API key loaded successfully.");
    }
    else
    {
        Debug.LogError("gemini_key.json not found in StreamingAssets.");
    }
}
    public void OnSendButtonClick()
    {
        if (chatInput != null && !string.IsNullOrEmpty(chatInput.text))
        {
            StartCoroutine(SendRequestToGemini(chatInput.text));
            chatInput.text = "";
        }
    }

    public IEnumerator SendRequestToGemini(string userMessage)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("Gemini API key is missing.");
            yield break;
        }

        string url = $"https://generativelanguage.googleapis.com/v1/models/{modelName}:generateContent?key={apiKey}";

        string combinedPrompt = $"{systemInstruction}\n\nUser says: {userMessage}";

        GeminiRequest requestData = new GeminiRequest();
        requestData.contents = new Content[] { new Content { parts = new Part[] { new Part { text = combinedPrompt } } } };
        string jsonBody = JsonUtility.ToJson(requestData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[Gemini] Sending: {userMessage}...");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Gemini Error] {webRequest.error}: {webRequest.downloadHandler.text}");
            }
            else
            {
                GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(webRequest.downloadHandler.text);
                if (response != null && response.candidates != null && response.candidates.Length > 0)
                {
                    string reply = response.candidates[0].content.parts[0].text;
                    lastResponse = reply;
                    Debug.Log($"[Gemini Reply]: {reply}");

                    if (voice != null)
                    {
                        voice.Speak(reply);
                    }
                }
            }
        }
    }
}