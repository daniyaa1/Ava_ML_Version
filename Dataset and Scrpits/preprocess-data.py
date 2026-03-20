import numpy as np
import os

KEYPOINT_DIR = os.path.join('dataset', 'keypoints')
MAX_LENGTH = 134
NUM_FEATURES = 258

sequences = []
labels = []

print("Starting padding and label extraction...")

for file in os.listdir(KEYPOINT_DIR):
    if file.endswith('.npy'):
        # 1. Load the raw extracted frames
        data = np.load(os.path.join(KEYPOINT_DIR, file))
        
        # 2. Create a blank template matrix of exactly (134, 258) filled with zeros
        padded_data = np.zeros((MAX_LENGTH, NUM_FEATURES))
        
        # 3. Paste the actual video data into the beginning of the blank matrix
        padded_data[:data.shape[0], :] = data
        
        # 4. Store the padded matrix
        sequences.append(padded_data)
        
        # 5. Extract the label (e.g., 'hello.npy' becomes 'hello')
        label = file.split('.')[0]
        labels.append(label)

# Convert lists into final NumPy arrays
X = np.array(sequences)
y = np.array(labels)

print(f"Feature matrix (X) shape: {X.shape}")
print(f"Labels (y) shape: {y.shape}")

# Save the final structured data to the main folder
np.save('X_data.npy', X)
np.save('y_labels.npy', y)

print("Success! Final dataset saved as X_data.npy and y_labels.npy.")