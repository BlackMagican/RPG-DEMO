﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Portal : MonoBehaviour
    {

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        /// <summary>
        /// Trigger the portal.
        /// </summary>
        /// <param name="other">player</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        /// <summary>
        /// Load the other scene.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.Log("Scene to load not set.");
                yield break;
            }
            /*
             Save this portal at the first.
             Then use this portal to find which portal the
             player should be teleported to.
             */
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            // Save current level.
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // Load current level.
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            // Resave player's infomation when changed scene.
            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);

        }

        /// <summary>
        /// Find which portal should the player be teleported to.
        /// </summary>
        /// 
        /// <returns>
        /// Can't find the portal: null.
        /// Found the portal: the portal.
        /// </returns>
        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal.destination != destination || portal == this)
                {
                    continue;
                }
                return portal;
            }
            return null;
        }

        /// <summary>
        /// Reset the player's position near the portal.
        /// </summary>
        /// <param name="otherPortal">Destination portal</param>
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            var transform1 = otherPortal.spawnPoint.transform;
            player.transform.position = transform1.position;
            player.transform.rotation = transform1.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
