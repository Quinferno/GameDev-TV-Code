using System.Threading;
using System;
using System.Collections;
using RPG.Control;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Gate : MonoBehaviour//Like portal but interactable rather than auto. uses rigid body and collider
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 0.5f;
        [SerializeField] float fadeWaitTime = 0.2f;

        private void OnTriggerStay(Collider other) 
        {
            if (other.tag == "Player")
            {
                if (Input.GetKey(KeyCode.E))
                {
                    StartCoroutine(Transition());
                } 
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            
            yield return fader.FadeOut(fadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            savingWrapper.Load();
            
            Gate otherGate = GetOtherGate();
            UpdatePlayer(otherGate);

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            savingWrapper.Save();
            Destroy(gameObject);
        }

        private void UpdatePlayer(Gate otherGate)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherGate.spawnPoint.position;
            player.transform.rotation = otherGate.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Gate GetOtherGate()
        {
            foreach (Gate gate in FindObjectsOfType<Gate>())
            {
                if (gate == this) continue;
                if (gate.destination != destination) continue;

                return gate;
            }

            return null;
        }
    }
}