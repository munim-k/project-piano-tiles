using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public Transform lastSpawnedNote;
    private float prevRandomIndex = -1;
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

    [SerializeField] float bpm = 120f;

    [SerializeField] float seed; // For generating consistent random numbers

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
        SetDataForNoteGeneration();
        InitializeBeatDetection();
        StartGame();
    }

    private void Update()
    {
        DetectNoteClicks();
    }

    public void GoHome() {
        SceneManager.LoadScene("Start");
    }

    private void StartGame()
    {
        // if (!GameController.Instance.GameStarted.Value && Input.GetMouseButtonDown(0))
        // {
            GameController.Instance.GameStarted.Value = true;

            StartCoroutine(SpawnNotesOnBeat());

            audioSource.Play();
        // }
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
                    // Debug.Log("GameController: Hit object is a Note.");
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
        var leftmostPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2));
        var leftmostPointPivot = leftmostPoint.x + noteWidth / 2;
        noteSpawnStartPosX = leftmostPointPivot;
    }

    private void InitializeBeatDetection()
    {
        audioSamples = new float[audioSource.clip.samples * audioSource.clip.channels];

        audioSource.clip.GetData(audioSamples, 0);
        sampleRate = audioSource.clip.frequency;

        samplesPerBeat = Mathf.FloorToInt((60f / bpm) * sampleRate);

        noteSpeed = noteHeight / samplesPerBeat * sampleRate;

        nextBeatSample = 0;
        isBeatDetected = false;

        seed = audioSamples.Length % 1000;
    }

    private IEnumerator SpawnNotesOnBeat()
    {
        while (true)
        {
            if (audioSource.isPlaying)
            {
                int currentSample = (int)(audioSource.timeSamples % audioSource.clip.samples);

                if (currentSample >= nextBeatSample && !isBeatDetected)
                {
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
            return;
        }
        
        noteSpawnStartPosY = lastSpawnedNote.position.y + noteHeight;
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
            }
        }
        noteSpawnStartPosY += noteHeight;
        lastSpawnedNote = note.transform;
    }

    private int GetRandomIndex()
    {
        var randomIndex = ( prevRandomIndex + seed ) % 4;
        seed *= 1.5f;
        if (seed > 10000) seed = (seed * 0.5f) % 5;
        if (randomIndex == prevRandomIndex)
        {
            randomIndex *= Mathf.Pow(-1, (int) seed);
        }
        prevRandomIndex = randomIndex;
        return (int) randomIndex;
    }

    public void PlaySomeOfSong()
    {
        if (!audioSource.isPlaying && !lastNote)
        {
            audioSource.Play();
        }
        if (audioSource.clip.length - audioSource.time <= songSegmentLength)
        {
            lastNote = true;
        }
    }

    public IEnumerator EndGame()
    {
        GameOver.Value = true;
        StopCoroutine(SpawnNotesOnBeat());
        audioSource.Stop();
        yield return new WaitForSeconds(1);
        ShowGameOverScreen.Value = true;
    }
}