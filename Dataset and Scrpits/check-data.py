import numpy as np
import os

KEYPOINT_DIR = os.path.join('dataset', 'keypoints')
lengths = []

# 1. Inspect a single file
sample_file = os.path.join(KEYPOINT_DIR, 'drink.npy')
if os.path.exists(sample_file):
    sample_data = np.load(sample_file)
    print(f"--- Single File Inspection ---")
    print(f"Shape of drink.npy: {sample_data.shape}")
    print(f"This means: {sample_data.shape[0]} frames, and {sample_data.shape[1]} coordinates per frame.\n")

# 2. Analyze the whole dataset for sequence length
for file in os.listdir(KEYPOINT_DIR):
    if file.endswith('.npy'):
        data = np.load(os.path.join(KEYPOINT_DIR, file))
        lengths.append(data.shape[0]) # Get the number of frames

print(f"--- Full Dataset Analysis ---")
print(f"Total videos processed: {len(lengths)}")
print(f"Shortest video (frames): {min(lengths)}")
print(f"Longest video (frames): {max(lengths)}")
print(f"Average frames per video: {int(sum(lengths)/len(lengths))}")