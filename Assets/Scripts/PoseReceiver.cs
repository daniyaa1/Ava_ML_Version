using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoseReceiver : MonoBehaviour
{
    [Header("Data Source & Settings")]
    public TextAsset poseDataCSV; 
    public float frameRate = 30f; 
    
    [Header("Alignment Tools (Use these to fit the wireframe to Ava)")]
    public float motionScale = 8.0f; // Increased to match your successful test
    public Vector3 positionOffset = new Vector3(0, 1.4f, 0.5f); // Lifted to shoulder height and pushed forward

    [Header("IK Targets & Hints")]
    public Transform leftHandTarget;
    public Transform leftElbowHint;
    public Transform rightHandTarget;
    public Transform rightElbowHint;

    private List<float[]> frames = new List<float[]>();

    void Start()
    {
        if (poseDataCSV != null)
        {
            LoadCSVData();
            StartCoroutine(StreamPoseData());
        }
    }

    void LoadCSVData()
    {
        string[] lines = poseDataCSV.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; 
            string[] values = line.Split(',');
            float[] frameData = new float[values.Length];
            for (int i = 0; i < values.Length; i++) { float.TryParse(values[i], out frameData[i]); }
            frames.Add(frameData);
        }
    }

    // NEW: This calculates raw ML points without world offsets first
    Vector3 GetRawMLPoint(float[] currentFrame, int mediaPipeIndex)
    {
        int arrayStartIndex = mediaPipeIndex * 4;
        float x = currentFrame[arrayStartIndex] - 0.5f;
        float y = (currentFrame[arrayStartIndex + 1] - 0.5f) * -1f; 
        float z = currentFrame[arrayStartIndex + 2]; // Removed -1f to keep hands in front
        
        return new Vector3(x, y, z) * motionScale;
    }

    IEnumerator StreamPoseData()
    {
        while (true) 
        {
            foreach (float[] frame in frames)
            {
                // 1. Get raw points for shoulders to find the center
                Vector3 rawLShoulder = GetRawMLPoint(frame, 11);
                Vector3 rawRShoulder = GetRawMLPoint(frame, 12);
                Vector3 mlShoulderCenter = (rawLShoulder + rawRShoulder) / 2f;

                // 2. Calculate Final Positions (Centered on PoseManager + Offset)
                System.Func<int, Vector3> GetFinalPos = (index) => {
                    Vector3 raw = GetRawMLPoint(frame, index);
                    // Subtract mlShoulderCenter to make the shoulders the (0,0,0) point of the data
                    return transform.position + (raw - mlShoulderCenter) + positionOffset;
                };

                Vector3 lShoulder = GetFinalPos(11);
                Vector3 rShoulder = GetFinalPos(12);
                Vector3 lElbow = GetFinalPos(13);
                Vector3 rElbow = GetFinalPos(14);
                Vector3 lHand = GetFinalPos(15);
                Vector3 rHand = GetFinalPos(16);

                // 3. Snap the IK Targets
                if (leftHandTarget != null)
                {
                    leftHandTarget.position = lHand;
                    leftElbowHint.position = lElbow; 
                    rightHandTarget.position = rHand;
                    rightElbowHint.position = rElbow;
                }

                // 4. Draw the wireframe
                Debug.DrawLine(lShoulder, lElbow, Color.green, 1f/frameRate);
                Debug.DrawLine(lElbow, lHand, Color.cyan, 1f/frameRate);
                Debug.DrawLine(rShoulder, rElbow, Color.green, 1f/frameRate);
                Debug.DrawLine(rElbow, rHand, Color.cyan, 1f/frameRate);
                Debug.DrawLine(lShoulder, rShoulder, Color.red, 1f/frameRate); 

                yield return new WaitForSeconds(1f / frameRate);
            }
        }
    }
}