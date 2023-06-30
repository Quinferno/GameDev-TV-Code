using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinemetics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable//Remember that colliders can block raycasts unless you put them on IgnoreRaycast layer
    {
        bool wasTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag=="Player" && wasTriggered == false)//Make sure Player has player tag
            {
            GetComponent<PlayableDirector>().Play();//Will play Director's cutscene on entering associated collider
            wasTriggered = true;
            }
        }

        public object CaptureState()//Added this myself but it does seem like it should work based on how health saving works
        {
            return wasTriggered;
        }

        public void RestoreState(object state)
        {
            wasTriggered = (bool)state;
        }

    }
}
