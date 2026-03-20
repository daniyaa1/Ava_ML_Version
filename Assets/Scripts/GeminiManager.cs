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
    public AnimationController avaBody; 

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

    private string systemInstruction = "Your name is Ava. You are a helpful AI avatar. " +
                                       "IMPORTANT: Always provide very short, concise replies (maximum 20 words). " +
                                       "Never mention being a large language model. Stay in character as Eva. " +
                                       "When you want to perform a physical action, you MUST include exactly one of these specific tags in your response: " +
                                       "[CLAP], [RAISE_HAND], [NOD], [TURN_LEFT], [TURN_RIGHT], [POINT], [SHAKE_YES], [SHAKE_NO], [SIT], [STAND], [IDLE], [WALK], or [WAVE]. " +
                                       "Use only one tag per response. Example: 'I can certainly turn left for you! [TURN_LEFT]'";

    void Awake()
    {
        LoadApiKey();
    }

    void Start()
    {
        avaBody = FindObjectOfType<AnimationController>();
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
        if (string.IsNullOrEmpty(apiKey)) yield break;

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

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(webRequest.downloadHandler.text);
                if (response != null && response.candidates != null && response.candidates.Length > 0)
                {
                    string rawReply = response.candidates[0].content.parts[0].text;
                    string cleanReply = ProcessAnimationTags(rawReply);
                    lastResponse = cleanReply;
                    if (voice != null) voice.Speak(cleanReply);
                }
            }
        }
    }

    // FIXED: Now correctly handles all 12 animations
    private string ProcessAnimationTags(string aiResponse)
    {
        if (avaBody == null) return aiResponse;

        // Using a temporary string to check for tags without case sensitivity issues
        string checkText = aiResponse.ToUpper();

        if (checkText.Contains("[WAVE]")) { avaBody.PlayWave(); aiResponse = aiResponse.Replace("[WAVE]", ""); }
        else if (checkText.Contains("[CLAP]")) { avaBody.PlayClap(); aiResponse = aiResponse.Replace("[CLAP]", ""); }
        else if (checkText.Contains("[RAISE_HAND]")) { avaBody.PlayRaiseHand(); aiResponse = aiResponse.Replace("[RAISE_HAND]", ""); }
        else if (checkText.Contains("[NOD]") || checkText.Contains("[SHAKE_YES]")) { avaBody.PlayHeadYes(); aiResponse = aiResponse.Replace("[NOD]", "").Replace("[SHAKE_YES]", ""); }
        else if (checkText.Contains("[SHAKE_NO]")) { avaBody.PlayHeadNo(); aiResponse = aiResponse.Replace("[SHAKE_NO]", ""); }
        else if (checkText.Contains("[TURN_LEFT]")) { avaBody.PlayTurnLeft(); aiResponse = aiResponse.Replace("[TURN_LEFT]", ""); }
        else if (checkText.Contains("[TURN_RIGHT]")) { avaBody.PlayTurnRight(); aiResponse = aiResponse.Replace("[TURN_RIGHT]", ""); }
        else if (checkText.Contains("[POINT]")) { avaBody.PlayPoint(); aiResponse = aiResponse.Replace("[POINT]", ""); }
        else if (checkText.Contains("[SIT]")) { avaBody.PlaySit(); aiResponse = aiResponse.Replace("[SIT]", ""); }
        else if (checkText.Contains("[STAND]")) { avaBody.PlayStandUp(); aiResponse = aiResponse.Replace("[STAND]", ""); }
        else if (checkText.Contains("[WALK]")) { avaBody.PlayWalk(); aiResponse = aiResponse.Replace("[WALK]", ""); }
        else if (checkText.Contains("[IDLE]")) { avaBody.PlayIdle(); aiResponse = aiResponse.Replace("[IDLE]", ""); }

        return aiResponse.Trim(); 
    }
}