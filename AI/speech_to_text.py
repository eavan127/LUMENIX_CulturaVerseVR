import whisper
import sys

model = whisper.load_model("base")

audio_file = sys.argv[1]

result = model.transcribe(audio_file)

print(result["text"])