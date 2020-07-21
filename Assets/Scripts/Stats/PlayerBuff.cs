using UnityEngine;

namespace Stats
{
    public class PlayerBuff : MonoBehaviour
    {
        [SerializeField] private float attackBuffPercentage;
        [SerializeField] private float healthBuffPercentage;
        [SerializeField] private float attackAdditiveBuff;
        [SerializeField] private float healthAdditiveBuff;

        public float AttackBuffPercentage
        {
            get => attackBuffPercentage;
            set => attackBuffPercentage = value;
        }

        public float AttackAdditiveBuff
        {
            get => attackAdditiveBuff;
            set => attackAdditiveBuff = value;
        }

        public float HealthBuffPercentage
        {
            get => healthBuffPercentage;
            set => healthBuffPercentage = value;
        }

        public float HealthAdditiveBuff
        {
            get => healthAdditiveBuff;
            set => healthAdditiveBuff = value;
        }
    }
}
