using UnityEngine;
using Whisper;
using Whisper.Utils; // Needed for AudioChunk
using TMPro;

public class VoiceInputHandler : MonoBehaviour
{
    public GeminiManager gemini;
    public WhisperManager whisperManager;
    public MicrophoneRecord microphoneRecord;
    public TextMeshProUGUI buttonText;
    
    private bool isRecording = false;

    void Awake()
    {
        // Subscribe to the event that triggers when recording is actually finished
        microphoneRecord.OnRecordStop += OnMicrophoneStopped;
    }

    public void ToggleRecording() 
    {
        if (!isRecording) 
        {
            isRecording = true;
            if (buttonText != null) buttonText.text = "STOP"; 
            microphoneRecord.StartRecord();
            Debug.Log("[Whisper] Recording started...");
        } 
        else 
        {
            isRecording = false;
            if (buttonText != null) buttonText.text = "LISTEN";
            
            // This method returns void, so just call it without 'await'
            microphoneRecord.StopRecord(); 
        }
    }

    // This handles the transcription once the audio data is ready
    private async void OnMicrophoneStopped(AudioChunk recordedAudio)
    {
        Debug.Log("[Whisper] Transcribing...");
        
        // Use the raw audio data from the event for local processing
        var result = await whisperManager.GetTextAsync(recordedAudio.Data, 
                                                       recordedAudio.Frequency, 
                                                       recordedAudio.Channels);
        
        if (result != null && !string.IsNullOrEmpty(result.Result))
        {
            Debug.Log("[Whisper] Result: " + result.Result);
            StartCoroutine(gemini.SendRequestToGemini(result.Result));
        }
    }
}