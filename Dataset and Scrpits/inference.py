import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.utils import to_categorical

# 1. Load the trained AI and the vocabulary
print("Loading AI model...")
model = load_model('sign_language_model.keras')
classes = np.load('label_classes.npy', allow_pickle=True)

print(f"Ready! Model knows {len(classes)} words.\n")

# 2. Ask for text input
target_word = input("Enter a word to generate sign language for: ").lower().strip()

if target_word not in classes:
    print(f"\nError: The model hasn't learned '{target_word}' yet.")
    print(f"Available words: {classes}")
else:
    # 3. Convert the text into the mathematical one-hot format
    word_index = np.where(classes == target_word)[0][0]
    word_one_hot = to_categorical(word_index, num_classes=len(classes))
    
    # Add a "batch" dimension because neural networks expect data in batches (even if it's just 1)
    word_one_hot = np.expand_dims(word_one_hot, axis=0) 
    
    # 4. Generate the pose sequence
    print(f"\nGenerating motion sequence for '{target_word}'...")
    generated_poses = model.predict(word_one_hot)
    
    # Remove the batch dimension so we are left with just the (134, 258) matrix
    generated_poses = generated_poses[0]
    
    print(f"\nSuccess! Generated pose matrix shape: {generated_poses.shape}")
    
    # 5. Save the generated sequence for Unity
    save_path = f"generated_{target_word}.npy"
    np.save(save_path, generated_poses)
    # Save as CSV for Unity
    csv_save_path = f"generated_{target_word}.csv"
    np.savetxt(csv_save_path, generated_poses, delimiter=",")
    print(f"Saved Unity-ready CSV to: {csv_save_path}")
    print(f"Saved generated animation to: {save_path}")