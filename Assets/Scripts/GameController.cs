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
    // public midiFIlesContainer midiFilesContainer;
    
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

    float songSegmentLength = 0.8f;

    public AudioSource audioSource;
    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; } = false;
   // public float noteSpawnStartPosY

    // MIDI Variables
    public TextAsset midiFile;  // Assign MIDI file in the Unity Editor
    private List<float> noteTimings = new List<float>();
    private int noteIndex = 0;
    public float playAudioWaitTime = 0.5f;

    float waitTimeThreshold;

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
        // midiFilesContainer = FindAnyObjectByType<midiFIlesContainer>();
        
        SetDataForNoteGeneration();
        // LoadMidiFromBytes(midiFilesContainer.midiData1);

        int level = FirebaseLevelManager.Instance.level;

        if (level == 0) {
            LoadMidiFromBytes(midiFIlesContainer.midiData8); // 1st Song - Naruto - sadness and sorrow

            // Short length clip for testing, also change level index 0 in levelSelect to Street Melancholy
            //  LoadMidiFromBytes(midiFIlesContainer.midiData7);
        } else if (level == 1) {
            LoadMidiFromBytes(midiFIlesContainer.midiData5); // Imagine dragons - Radioactive
        } else if (level == 2) {
            LoadMidiFromBytes(midiFIlesContainer.midiData4); // Naruto Shippuden - Naruto Shpippuuden Opening 9
        } else if (level == 3) {
            LoadMidiFromBytes(midiFIlesContainer.midiData2); // rihanna-diamonds
        } else if (level == 4) {
            LoadMidiFromBytes(midiFIlesContainer.midiData3); // David Guetta - Titanium (Alesso Remix)
        } else if (level == 5) {
            LoadMidiFromBytes(midiFIlesContainer.midiData6); // sia-chandelier (2)
        }

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
        Invoke(nameof(PlayAudio), playAudioWaitTime);
        
    }
    private void PlayAudio()
    {
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

    [SerializeField] Transform bgTransform;
    private void SetDataForNoteGeneration()
    {
        var backgroundWidth = bgTransform.GetComponent<SpriteRenderer>().bounds.size.x;
        var backgroundHeight = bgTransform.GetComponent<SpriteRenderer>().bounds.size.y;

        var topRight = new Vector3(Screen.width, Screen.height, 0);
        var topRightWorldPoint = Camera.main.ScreenToWorldPoint(topRight);
        var bottomLeftWorldPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var screenWidth = topRightWorldPoint.x - bottomLeftWorldPoint.x;
        var screenHeight = topRightWorldPoint.y - bottomLeftWorldPoint.y;
        noteHeight = screenHeight / 4;
        noteWidth = backgroundWidth / 4;
        var noteSpriteRenderer = notePrefab.GetComponent<SpriteRenderer>();
        noteLocalScale = new Vector3(
            noteWidth / noteSpriteRenderer.bounds.size.x * noteSpriteRenderer.transform.localScale.x,
            noteHeight / noteSpriteRenderer.bounds.size.y * noteSpriteRenderer.transform.localScale.y, 1);
        // noteLocalScale.x = 2;
        var leftmostPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2));
        leftmostPoint.x = backgroundWidth / 2 * -1;
        var leftmostPointPivot = leftmostPoint.x + noteWidth / 2;
        noteSpawnStartPosX = leftmostPointPivot;
        noteSpawnStartPosY = topRightWorldPoint.y+noteHeight;  
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
                   // Debug.Log($"Note: {note.NoteName}, Time: {note.Time}");
                    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                    float noteTime = (float)metricTimeSpan.TotalSeconds;
                    noteTimings.Add(noteTime);

                    // Assign column based on MIDI note
                    int column = GetColumnFromMidiNote(note.NoteNumber);
                    noteColumns.Add(column);
                }

                noteTimings.Sort();

                // Extract the first tempo event from the MIDI file using LINQ.
                // Ensure you have "using System.Linq;" at the top of your file.
                var tempo = tempoMap.GetTempoAtTime(new MidiTimeSpan(0));

                // Convert microseconds per quarter note to BPM
                float bpm = 60000000f / tempo.MicrosecondsPerQuarterNote;

                // Map BPM to noteSpeed (adjust this divisor to fine-tune)
                noteSpeed = bpm / 30f;

                Debug.Log($"Calculated BPM: {bpm} -> noteSpeed: {noteSpeed}");

                waitTimeThreshold = ( 60f / bpm ) * 1.25f;
                Debug.Log("Wait Time Threshold: " + waitTimeThreshold);

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
        bool gameEnded = false;
        bool hasStartedPlaying = false;

        while (!gameEnded)
        {
            if (noteIndex < noteTimings.Count) {
                float waitTime = noteIndex == 0 ? noteTimings[0] : noteTimings[noteIndex] - noteTimings[noteIndex - 1];
                waitTime = Mathf.Max(waitTime, 0); // Prevent negative wait times
                if(waitTime ==0)
                {
                    noteIndex++;
                    continue;
                }
                hasStartedPlaying = true;

                if (waitTime < waitTimeThreshold) {
                    waitTime = waitTimeThreshold; // Prevent very short wait times
                    // noteIndex++;
                    // continue;
                }

                yield return new WaitForSeconds(waitTime);
                
                SpawnNotes();
                noteIndex++;
            }

            // Only check end game conditions after audio has started playing at least once
            if (hasStartedPlaying)
            {
                bool isNearEnd = audioSource.clip.length - audioSource.time <= songSegmentLength;
                bool hasStoppedPlaying = !audioSource.isPlaying;
                bool allNotesPlayed = LastPlayedNoteId >= lastNoteId - 1;

                Debug.Log("LastPlayedNoteId: " + LastPlayedNoteId);
                Debug.Log("lastNoteId: " + lastNoteId);

                Debug.Log("Length: " + audioSource.clip.length);
                Debug.Log("Time: " + audioSource.time);

                Debug.Log($"isNearEnd: {isNearEnd}, hasStoppedPlaying: {hasStoppedPlaying}, allNotesPlayed: {allNotesPlayed}");

                if ((isNearEnd || hasStoppedPlaying) && allNotesPlayed)
                {
                    Debug.Log("Ending game with victory condition");
                    GameOver.Value = true;
                    PlayerWon = true;
                    audioSource.Stop();
                    ShowGameOverScreen.Value = true;
                    gameEnded = true;
                    yield break;
                }
            }

            yield return null;
        }
    }

    public void SpawnNotes()
{
    

    // Get the correct column from the noteColumns list
    int column = noteColumns[noteIndex]; 

    for (int i = 0; i < 4; i++) {
        Note note = Instantiate(notePrefab, noteContainer.transform);
        note.transform.localScale = noteLocalScale;

        note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * i, noteSpawnStartPosY);

        if (i == column) {
            note.Visible = true;

            note.Id = lastNoteId;
            lastNoteId++;
            lastSpawnedNote = note.transform;
        } else {
            note.Visible = false;
        }

    }
    
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
