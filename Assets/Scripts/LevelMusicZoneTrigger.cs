using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Berzerk
{
    public class LevelMusicZoneTrigger : MonoBehaviour
    {
        public AudioClip music;

        public void OnTriggerEnter2D(Collider2D collision) //If player entered in trigger
        {
            if (collision.gameObject.tag == "Player") //if its player
            {
                // AudioManager.Instance.PlayMusic(music);
            }
        }

        public void OnTriggerExit2D(Collider2D collision) //If player entered in trigger
        {
            if (collision.gameObject.tag == "Player") //if its player
            {
                // AudioManager.Instance.PlayMusic(AudioManager.Instance.music);
            }
        }
    }
}