import cv2
import numpy as np
import mediapipe as mp
import os

# 1. Setup MediaPipe Holistic model
mp_holistic = mp.solutions.holistic

# 2. Define your exact folder paths
VIDEO_DIR = os.path.join('dataset', 'videos')
SAVE_DIR = os.path.join('dataset', 'keypoints')

# Create the keypoints directory if it doesn't exist
os.makedirs(SAVE_DIR, exist_ok=True)

# 3. Function to extract and flatten coordinates
def extract_keypoints(results):
    # Extract pose (33 landmarks x 4 values: x, y, z, visibility)
    pose = np.array([[res.x, res.y, res.z, res.visibility] for res in results.pose_landmarks.landmark]).flatten() if results.pose_landmarks else np.zeros(33*4)
    
    # Extract left hand (21 landmarks x 3 values: x, y, z)
    lh = np.array([[res.x, res.y, res.z] for res in results.left_hand_landmarks.landmark]).flatten() if results.left_hand_landmarks else np.zeros(21*3)
    
    # Extract right hand (21 landmarks x 3 values: x, y, z)
    rh = np.array([[res.x, res.y, res.z] for res in results.right_hand_landmarks.landmark]).flatten() if results.right_hand_landmarks else np.zeros(21*3)
    
    # Combine all into a single 1D array of 258 values
    return np.concatenate([pose, lh, rh])

# 4. Loop through every video in your folder
with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5) as holistic:
    for video_name in os.listdir(VIDEO_DIR):
        if not video_name.endswith('.mp4'):
            continue
            
        video_path = os.path.join(VIDEO_DIR, video_name)
        file_name_without_ext = video_name.split('.')[0] # Gets 'hello' from 'hello.mp4'
        
        cap = cv2.VideoCapture(video_path)
        video_keypoints = []
        
        print(f"Processing: {video_name}...")
        
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                break # Reached the end of the video
                
            # Convert the frame to RGB for MediaPipe
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False                  
            
            # Process the frame to find landmarks
            results = holistic.process(image)
            
            # Extract numbers and append to our list for this video
            keypoints = extract_keypoints(results)
            video_keypoints.append(keypoints)
            
        cap.release()
        
        # Save the collected data as a .npy file
        npy_save_path = os.path.join(SAVE_DIR, f"{file_name_without_ext}.npy")
        np.save(npy_save_path, video_keypoints)
        print(f"Saved: {npy_save_path}")

print("Extraction complete!")