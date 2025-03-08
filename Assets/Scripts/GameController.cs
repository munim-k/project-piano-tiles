using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public midiFIlesContainer midiFilesContainer;
    
    public Transform lastSpawnedNote;
    private int lastNoteId = 1;

    public Note notePrefab;
    private static float noteHeight;
    private static float noteWidth;

    private Vector3 noteLocalScale;
    private float noteSpawnStartPosX;
    private float noteSpawnStartPosY;

    public float noteSpeed = 5f;
    public Transform noteContainer;

    public ReactiveProperty<bool> GameStarted { get; set; }
    public ReactiveProperty<bool> GameOver { get; set; }
    public ReactiveProperty<int> Score { get; set; }
    public int LastPlayedNoteId { get; set; } = 0;

    public AudioSource audioSource;
    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; } = false;

    // MIDI Variables
    public TextAsset midiFile;  // Assign MIDI file in the Unity Editor
    private List<float> noteTimings = new List<float>();
    private int noteIndex = 0;

    private void Awake()
    {
        Instance = this;
        GameStarted = new ReactiveProperty<bool>();
        GameOver = new ReactiveProperty<bool>();
        Score = new ReactiveProperty<int>();
        ShowGameOverScreen = new ReactiveProperty<bool>();
        audioSource = GameObject.Find("levelMusic").GetComponent<AudioSource>();
    }

    void Start()
    {
        midiFilesContainer = FindAnyObjectByType<midiFIlesContainer>();
        
        SetDataForNoteGeneration();
        LoadMidiFromBytes(midiFilesContainer.midiData1);
        StartCoroutine(SpawnNotesOnMidi());
        StartGame();
    }

    private void Update()
    {
        DetectNoteClicks();
    }

    public void GoHome()
    {
        SceneManager.LoadScene("Start");
    }

    private void StartGame()
    {
        GameController.Instance.GameStarted.Value = true;
        audioSource.Play();
    }

    private void DetectNoteClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (hit)
            {
                var gameObject = hit.collider.gameObject;
                if (gameObject.CompareTag("Note"))
                {
                    var note = gameObject.GetComponent<Note>();
                    note.Play();
                }
            }
        }
    }

    private void SetDataForNoteGeneration()
    {
        var topRight = new Vector3(Screen.width, Screen.height, 0);
        var topRightWorldPoint = Camera.main.ScreenToWorldPoint(topRight);
        var bottomLeftWorldPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var screenWidth = topRightWorldPoint.x - bottomLeftWorldPoint.x;
        var screenHeight = topRightWorldPoint.y - bottomLeftWorldPoint.y;
        noteHeight = screenHeight / 4;
        noteWidth = screenWidth / 4;
        var noteSpriteRenderer = notePrefab.GetComponent<SpriteRenderer>();
        noteLocalScale = new Vector3(
            noteWidth / noteSpriteRenderer.bounds.size.x * noteSpriteRenderer.transform.localScale.x,
            noteHeight / noteSpriteRenderer.bounds.size.y * noteSpriteRenderer.transform.localScale.y, 1);
        noteLocalScale.x = 2;
        var leftmostPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2));
        var leftmostPointPivot = leftmostPoint.x + noteWidth / 2;
        noteSpawnStartPosX = leftmostPointPivot;
    }

    private List<int> noteColumns = new List<int>(); // Stores the column of each note


void LoadMidiFromBytes(byte[] midiBytes)
    {
        try
        {
            using (var stream = new MemoryStream(midiBytes))
            {
                var midiFileInstance = MidiFile.Read(stream);
                var tempoMap = midiFileInstance.GetTempoMap();
                var notes = midiFileInstance.GetNotes();

                foreach (var note in notes)
                {
                    Debug.Log($"Note: {note.NoteName}, Time: {note.Time}");
                    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                    float noteTime = (float)metricTimeSpan.TotalSeconds;
                    noteTimings.Add(noteTime);

                    // Assign column based on MIDI note
                    int column = GetColumnFromMidiNote(note.NoteNumber);
                    noteColumns.Add(column);
                }

                noteTimings.Sort();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading MIDI file: {e.Message}");
        }
    }


private int GetColumnFromMidiNote(int midiNoteNumber)
{
    return midiNoteNumber % 4; // Maps MIDI notes to columns 0-3
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

    public void SpawnNotes()
{
    noteSpawnStartPosY = lastSpawnedNote != null ? lastSpawnedNote.position.y + noteHeight : 0;
    Note note = Instantiate(notePrefab, noteContainer.transform);
    note.transform.localScale = noteLocalScale;

    // Get the correct column from the noteColumns list
    int column = noteColumns[noteIndex]; 
    note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * column, noteSpawnStartPosY);

    note.Id = lastNoteId;
    lastNoteId++;
    lastSpawnedNote = note.transform;
}


    public IEnumerator EndGame(bool won = false)
    {
        GameOver.Value = true;
        if (won) PlayerWon = true;
        audioSource.Stop();
        FirebaseCurrencyManager.Instance.AddTokens(Score.Value);
        yield return new WaitForSeconds(1); // Short delay before showing the game over screen
        ShowGameOverScreen.Value = true;
    }
}
