using UnityEngine;

namespace Resource
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] private float experiencePoint = 0;

        public void GainExperience(float experience)
        {
            experiencePoint += experience;
        }
    }
}
