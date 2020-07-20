using System.Collections;
using UnityEngine;

namespace SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        /// <summary>
        /// Make canvas cover the screen.
        /// </summary>
        /// <param name="time">
        /// How long does it take to cover the screen.
        /// </param>
        public IEnumerator FadeOut(float time)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        /// <summary>
        /// Make canvas disappear.
        /// </summary>
        /// <param name="time">
        /// How long does it take to show the screen.
        /// </param>
        public IEnumerator FadeIn(float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
