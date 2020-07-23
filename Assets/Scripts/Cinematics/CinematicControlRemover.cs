using System;
using Control;
using Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player = null;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pd"></param>
        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}

