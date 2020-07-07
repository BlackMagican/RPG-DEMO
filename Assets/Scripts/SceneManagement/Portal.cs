using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        /*
         * This enum is used to determine a couple of portals.
         * Player cross portal A, then it will be teleported to Portal B
         * which have same identifier. 
         */
        enum DestinationIdentifier
        {
            A, B, C, D
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination;

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
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
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
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.transform.position);
            player.transform.rotation = otherPortal.spawnPoint.transform.rotation;
        }
    }
}
