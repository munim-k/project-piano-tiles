using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Existing variables
    public Transform lastSpawnedNote;
    private static float noteHeight;
    private static float noteWidth;
    public Note notePrefab;
    private Vector3 noteLocalScale;
    private float noteSpawnStartPosX;
    public float noteSpeed = 5f;
    public const int NotesToSpawn = 20;
    private int prevRandomIndex = -1;
    public static GameController Instance { get; private set; }
    public Transform noteContainer;
    public ReactiveProperty<bool> GameStarted { get; set; }
    public ReactiveProperty<bool> GameOver { get; set; }
    public ReactiveProperty<int> Score { get; set; }
    private int lastNoteId = 1;
    public int LastPlayedNoteId { get; set; } = 0;
    public AudioSource audioSource;
    private Coroutine playSongSegmentCoroutine;
    private float songSegmentLength = 0.8f;
    private bool lastNote = false;
    private bool lastSpawn = false;
    public ReactiveProperty<bool> ShowGameOverScreen { get; set; }
    public bool PlayerWon { get; set; } = false;

    // New variables for beat detection
    private float[] audioSamples;
    private int sampleRate;
    private int samplesPerBeat;
    private int nextBeatSample;
    private bool isBeatDetected;

    private void Awake()
    {
        Debug.Log("GameController: Awake called.");
        Instance = this;
        GameStarted = new ReactiveProperty<bool>();
        GameOver = new ReactiveProperty<bool>();
        Score = new ReactiveProperty<int>();
        ShowGameOverScreen = new ReactiveProperty<bool>();
    }

    void Start()
    {
        Debug.Log("GameController: Start called.");
        SetDataForNoteGeneration();
        InitializeBeatDetection();
        StartCoroutine(SpawnNotesOnBeat());
    }

    private void Update()
    {
        DetectNoteClicks();
        DetectStart();
    }

    private void DetectStart()
    {
        if (!GameController.Instance.GameStarted.Value && Input.GetMouseButtonDown(0))
        {
            Debug.Log("GameController: Game started.");
            GameController.Instance.GameStarted.Value = true;
            audioSource.Play();
        }
    }

    private void DetectNoteClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GameController: Mouse button clicked.");
            var origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(origin, Vector2.zero);
            if (hit)
            {
                Debug.Log("GameController: Raycast hit an object.");
                var gameObject = hit.collider.gameObject;
                if (gameObject.CompareTag("Note"))
                {
                    Debug.Log("GameController: Hit object is a Note.");
                    var note = gameObject.GetComponent<Note>();
                    note.Play();
                }
            }
        }
    }

    private void SetDataForNoteGeneration()
    {
        Debug.Log("GameController: Setting data for note generation.");
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
        var leftmostPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2));
        var leftmostPointPivot = leftmostPoint.x + noteWidth / 2;
        noteSpawnStartPosX = leftmostPointPivot;
    }

    private void InitializeBeatDetection()
    {
        Debug.Log("GameController: Initializing beat detection.");
        audioSamples = new float[audioSource.clip.samples * audioSource.clip.channels];
        audioSource.clip.GetData(audioSamples, 0);
        sampleRate = audioSource.clip.frequency;

        // Calculate samplesPerBeat based on BPM
        float bpm = 120f; // Replace with your audio clip's BPM
        samplesPerBeat = Mathf.FloorToInt((60f / bpm) * sampleRate);

        nextBeatSample = 0;
        isBeatDetected = false;
        Debug.Log($"GameController: Sample rate = {sampleRate}, Samples per beat = {samplesPerBeat}");
    }

    private IEnumerator SpawnNotesOnBeat()
    {
        Debug.Log("GameController: Starting SpawnNotesOnBeat coroutine.");
        while (true)
        {
            if (audioSource.isPlaying)
            {
                Debug.Log($"GameController: Audio source is playing = {audioSource.isPlaying}");
                int currentSample = (int)(audioSource.timeSamples % audioSource.clip.samples);
                Debug.Log($"GameController: Current sample = {currentSample}, Next beat sample = {nextBeatSample}");

                if (currentSample >= nextBeatSample && !isBeatDetected)
                {
                    Debug.Log("GameController: Beat detected, spawning notes.");
                    isBeatDetected = true;
                    SpawnNotes();
                    nextBeatSample += samplesPerBeat;
                }
                else if (currentSample < nextBeatSample - samplesPerBeat / 2)
                {
                    isBeatDetected = false;
                }
            }
            yield return null;
        }
    }

    public void SpawnNotes()
    {
        if (lastSpawn)
        {
            Debug.Log("GameController: Last spawn reached, skipping note spawn.");
            return;
        }

        Debug.Log("GameController: Spawning notes.");
        var noteSpawnStartPosY = lastSpawnedNote.position.y + noteHeight;
        Note note = null;
        var randomIndex = GetRandomIndex();
        for (int j = 0; j < 4; j++)
        {
            note = Instantiate(notePrefab, noteContainer.transform);
            note.transform.localScale = noteLocalScale;
            note.transform.position = new Vector2(noteSpawnStartPosX + noteWidth * j, noteSpawnStartPosY);
            note.Visible = (j == randomIndex);
            if (note.Visible)
            {
                note.Id = lastNoteId;
                lastNoteId++;
                Debug.Log($"GameController: Spawned note with ID {note.Id} at index {j}.");
            }
        }
        noteSpawnStartPosY += noteHeight;
        lastSpawnedNote = note.transform;
    }

    private int GetRandomIndex()
    {
        Debug.Log("GameController: Getting random index.");
        var randomIndex = Random.Range(0, 4);
        while (randomIndex == prevRandomIndex) randomIndex = Random.Range(0, 4);
        prevRandomIndex = randomIndex;
        Debug.Log($"GameController: Random index = {randomIndex}.");
        return randomIndex;
    }

    public void PlaySomeOfSong()
    {
        Debug.Log("GameController: PlaySomeOfSong called.");
        if (!audioSource.isPlaying && !lastNote)
        {
            Debug.Log("GameController: Starting audio playback.");
            audioSource.Play();
        }
        if (audioSource.clip.length - audioSource.time <= songSegmentLength)
        {
            Debug.Log("GameController: Last note reached.");
            lastNote = true;
        }
    }

    public void PlayAgain()
    {
        Debug.Log("GameController: Restarting game.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator EndGame()
    {
        Debug.Log("GameController: Ending game.");
        GameOver.Value = true;
        yield return new WaitForSeconds(1);
        ShowGameOverScreen.Value = true;
        Debug.Log("GameController: Game over screen shown.");
    }
}