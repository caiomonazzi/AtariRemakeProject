using Berzerk;
using UnityEngine;

public class LevelMusicZoneTrigger : MonoBehaviour
{
    public AudioClip music;

    public void OnTriggerEnter2D(Collider2D collision) //If player entered in trigger
    {
        if (collision.gameObject.tag == "Player") //if its player
        {
            Player.Instance.PlayMusic(music);
        }
    }

    public void OnTriggerExit2D(Collider2D collision) //If player entered in trigger
    {
        if (collision.gameObject.tag == "Player") //if its player
        {
            Player.Instance.StopMusic();
        }
    }
}
