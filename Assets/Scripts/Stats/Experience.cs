using Saving;
using UnityEngine;

namespace Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoint;

        public void GainExperience(float experience)
        {
            experiencePoint += experience;
        }

        public float ExperiencePoint => experiencePoint;

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            if (state is float experience)
            {
                experiencePoint = experience;
            }
        }
    }
}
