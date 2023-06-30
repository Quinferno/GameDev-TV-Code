using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinemetics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        void Awake()
        {
            GetComponent<PlayableDirector>().played += DisableControl;//Runs disable code when attached director runs cutscene
            GetComponent<PlayableDirector>().stopped += EnableControl;//Flip of the above when cutscene ends
            player = GameObject.FindWithTag("Player");
        }
        void DisableControl(PlayableDirector playableDirector)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector playableDirector)
        {
            if(player != null)
            {
            player.GetComponent<PlayerController>().enabled = true;
            }
        }
    }
}
