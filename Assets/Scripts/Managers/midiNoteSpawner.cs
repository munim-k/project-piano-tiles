using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MidiNoteSpawner : MonoBehaviour
{
    public TextAsset midiFile; // Assign in the Unity editor
    public AudioSource audioSource;
    
    private List<float> noteTimings = new List<float>();
    private int noteIndex = 0;

    void Start()
    {
        midiFile = Resources.Load<TextAsset>("MidiFiles/levelMusic/Level1");
        if (midiFile == null)
        {
            Debug.LogError("MIDI file not found! Make sure it's in Resources/MidiFiles/");
            return;
        }

        LoadMidiFile(midiFile);
        StartCoroutine(SpawnNotesOnMidi());
        audioSource.Play();
    }

    void LoadMidiFile(TextAsset midiFile)
{
    try
    {
        using (var stream = new System.IO.MemoryStream(midiFile.bytes))
        {
            var midiFileInstance = MidiFile.Read(stream); // Correct way to read byte[]
            var tempoMap = midiFileInstance.GetTempoMap();
            var notes = midiFileInstance.GetNotes();

            foreach (var note in notes)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                float noteTime = (float)metricTimeSpan.TotalSeconds;
                noteTimings.Add(noteTime);
            }

            noteTimings.Sort(); // Ensures notes are in order
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"Error loading MIDI file: {e.Message}");
    }
}


    private IEnumerator SpawnNotesOnMidi()
    {
        while (noteIndex < noteTimings.Count)
        {
            float waitTime = noteIndex == 0 ? noteTimings[0] : noteTimings[noteIndex] - noteTimings[noteIndex - 1];
            waitTime = Mathf.Max(waitTime, 0); // Prevent negative wait times

            yield return new WaitForSeconds(waitTime);
            
            SpawnNotes();
            noteIndex++;
        }
    }

    private void SpawnNotes()
    {
        Debug.Log($"Spawning note at {Time.time}s");
        // Your existing note spawning logic here...
    }
}
