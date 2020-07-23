using System;
using System.Collections;
using Saving;
using UnityEngine;

namespace SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string DefaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadTheLastScene());
        }

        private IEnumerator LoadTheLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
            
        }
        public void Save()
        {
            GetComponent<SavingSystem>().Save(DefaultSaveFile);
        }
        public void Load()
        {
            GetComponent<SavingSystem>().Load(DefaultSaveFile);
        }

        private void Delete()
        {
            GetComponent<SavingSystem>().Delete(DefaultSaveFile);
        }
    }
}
