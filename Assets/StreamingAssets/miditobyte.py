import base64
import keyboard
import pyperclip

# Read the MIDI file as bytes
with open("level1.mid", "rb") as midi_file:
    midi_bytes = midi_file.read()

# Convert bytes to a C# byte array string
byte_array_str = "byte[] midiData = new byte[] {\n    " + ', '.join(str(b) for b in midi_bytes) + "\n};"

# Print the byte array
print(byte_array_str)

# Wait for spacebar press to copy to clipboard
print("\nPress SPACEBAR to copy the byte array to the clipboard...")

keyboard.wait("space")  # Wait for spacebar press
pyperclip.copy(byte_array_str)  # Copy the string to the clipboard

print("✅ Copied to clipboard!")
