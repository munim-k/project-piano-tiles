using UnityEngine;

public class Note : MonoBehaviour
{
    Animator animator;

    [SerializeField] private bool visible;
    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (!visible) animator.Play("Invisible");
        }
    }

    public bool Played { get; set; }
    public int Id { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector2.down * GameController.Instance.noteSpeed * Time.deltaTime);
        }
    }

    public void Play()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            if (Visible)
            {
                if (!Played && GameController.Instance.LastPlayedNoteId == Id - 1)
                {
                    Played = true;
                    GameController.Instance.LastPlayedNoteId = Id;
                    GameController.Instance.Score.Value++;
                    animator.Play("Played");
                }
            }
            else
            {
                StartCoroutine(GameController.Instance.EndGame());
                animator.Play("Missed");
            }
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played)
        {
            StartCoroutine(GameController.Instance.EndGame());
            animator.Play("Missed");
        }
    }
}
