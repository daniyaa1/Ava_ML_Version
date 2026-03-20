using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void Speak(string text)
    {
        // Clean text and ensure we don't cut in the middle of a word
        string cleanText = text.Replace("*", "").Trim();
        if (cleanText.Length > 200) {
            int lastSpace = cleanText.Substring(0, 200).LastIndexOf(' ');
            cleanText = cleanText.Substring(0, lastSpace) + ".";
        }

        // 3. ENCODE: Ensure the text is properly formatted for a web URL
        string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&client=tw-ob&q=" + 
                     UnityWebRequest.EscapeURL(cleanText) + "&tl=en";

        StartCoroutine(DownloadAndPlayAudio(url));
    }

    IEnumerator DownloadAndPlayAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            // Crucial: Tells Google we are a "browser" to prevent blocking
            www.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) 
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null) 
                {
                    Debug.Log("[Voice Success] Playing audio now.");
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            } 
            else 
            {
                // This will now show the specific reason if it fails
                Debug.LogError($"[Voice Error] Code {www.responseCode}: {www.error}");
            }
        }
    }
}