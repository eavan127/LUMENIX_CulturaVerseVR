using UnityEngine;
using System.IO;
using System.Diagnostics;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip clip;
    private bool recording = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!recording)
                StartRecording();
            else
                StopRecording();
        }
    }

    void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
        UnityEngine.Debug.Log("Recording...");
    }

    void StopRecording()
    {
        Microphone.End(null);
        recording = false;

        string path = Application.dataPath + "/question.wav";
        SaveWav(path, clip);

        UnityEngine.Debug.Log("Saved to: " + path);

        RunSpeechToText(); // run whisper
    }

    void RunSpeechToText()
    {
        string python = "python";
        string script = "C:/AI/speech_to_text.py";
        string audio = Application.dataPath + "/question.wav";

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = python;
        start.Arguments = $"-u \"{script}\" \"{audio}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        Process process = Process.Start(start);

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("PYTHON OUTPUT:\n" + output);
        UnityEngine.Debug.Log("PYTHON ERROR:\n" + error);

        string userText = "";

        foreach (string line in output.Split('\n'))
        {
            if (line.StartsWith("TRANSCRIPTION:"))
            {
                userText = line.Replace("TRANSCRIPTION:", "").Trim();
                break;
            }
        }

        UnityEngine.Debug.Log("User said: " + userText);

        if (!string.IsNullOrEmpty(userText))
        {
            StartCoroutine(SendToOllama(userText));
        }
    }

    IEnumerator SendToOllama(string question)
    {
        string url = "http://localhost:11434/api/generate";

        string prompt = "You are Yue Fei (岳飞), a loyal Song Dynasty general. Answer the player question: " + question;

        string json = "{\"model\":\"gemma3:4b\",\"prompt\":\"" + prompt + "\",\"stream\":false}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] body = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string responseJson = request.downloadHandler.text;

        int start = responseJson.IndexOf("\"response\":\"") + 12;
        int end = responseJson.IndexOf("\",", start);

        string aiText = responseJson.Substring(start, end - start);
    }

    void SaveWav(string filepath, AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        using (FileStream fs = new FileStream(filepath, FileMode.Create))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            int sampleCount = samples.Length;
            int frequency = clip.frequency;

            bw.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            bw.Write(36 + sampleCount * 2);
            bw.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            bw.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(frequency);
            bw.Write(frequency * 2);
            bw.Write((short)2);
            bw.Write((short)16);
            bw.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            bw.Write(sampleCount * 2);

            foreach (var sample in samples)
            {
                short value = (short)(sample * short.MaxValue);
                bw.Write(value);
            }
        }
    }
}