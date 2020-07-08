using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.SceneManagement
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] Slider slider = null;
        [SerializeField] Text progressText = null;

        /// <summary>
        /// Loading Bar && Async load new level.
        /// </summary>
        /// <param name="sceneIndex">which scene to load</param>
        /// <returns></returns>
        public IEnumerator LoadLevel(int sceneIndex = 0)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            float progress = 0;
            while (progress < 1f)
            {
                progress = operation.progress / 0.9f;
                progressText.text = progress * 100f + "%";
                slider.value = progress;
                yield return null;
            }
            operation.allowSceneActivation = true;
        }
    }
}
