using UnityEngine;


[System.Serializable]
public class Song
{
    public string songName;
    public AudioClip songClip;
    public float bpm;
    public float offset;
    public float[] notes;
}

[CreateAssetMenu(menuName = "Piano-Tiles/SongsSO")]
public class SongsSO : ScriptableObject
{
    public Song[] songs;
}