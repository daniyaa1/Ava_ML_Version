import numpy as np
from sklearn.preprocessing import LabelEncoder
from tensorflow.keras.utils import to_categorical
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, RepeatVector, TimeDistributed

# 1. Load the prepared data
poses = np.load('X_data.npy')    # Shape: (49, 134, 258)
words = np.load('y_labels.npy')  # Shape: (49,)

print("Data loaded successfully.")

# 2. Convert text labels into One-Hot Encoded numbers
# The neural network can't read "drink", it needs a binary array.
label_encoder = LabelEncoder()
word_indices = label_encoder.fit_transform(words)
num_classes = len(np.unique(words))
words_one_hot = to_categorical(word_indices, num_classes=num_classes)

print(f"Total unique signs to learn: {num_classes}")

# 3. Build the Text-to-Pose Neural Network
model = Sequential()

# Input: The one-hot encoded word
model.add(Dense(64, activation='relu', input_shape=(num_classes,)))

# Stretch the single word across the 134 frames
model.add(RepeatVector(134))

# LSTM layers to learn the fluid motion over time
model.add(LSTM(128, return_sequences=True))
model.add(LSTM(256, return_sequences=True))

# Output: 258 coordinates for every single frame
model.add(TimeDistributed(Dense(258, activation='linear')))

# Compile the model using Mean Squared Error (since we are predicting continuous coordinate numbers)
model.compile(optimizer='adam', loss='mse', metrics=['mae'])

print("\nModel Architecture:")
model.summary()

# 4. Train the Model
print("\nStarting the training process...")

# In Text-to-Pose, 'x' is the text (one-hot encoded), and 'y' is the target video coordinates (poses)
# We will train for 200 epochs to allow the model to learn the complex coordinate mappings
history = model.fit(words_one_hot, poses, epochs=200, batch_size=8)

print("\nTraining complete!")

# 5. Save the trained AI model
model.save('sign_language_model.keras')
print("Model saved as 'sign_language_model.keras'")

# 6. Save the vocabulary mapping
# We must save the label encoder's classes so our inference script and Unity know which index belongs to which word.
np.save('label_classes.npy', label_encoder.classes_)
print("Vocabulary mapping saved as 'label_classes.npy'")