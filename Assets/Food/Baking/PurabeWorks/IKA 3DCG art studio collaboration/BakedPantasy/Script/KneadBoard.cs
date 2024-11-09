
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class KneadBoard : UdonSharpBehaviour
    {
        [SerializeField, Header("固定点")]
        private Transform _fixPoint;
        [SerializeField, Header("食パン塊の固定点")]
        private Transform _loafFixPoint;

        /// <summary>
        /// パン生地とピザの固定点
        /// </summary>
        public Transform FixPoint { get { return _fixPoint; } }

        /// <summary>
        /// 食パン塊の固定点
        /// </summary>
        public Transform LoafFixPoint { get { return _loafFixPoint; } }
    }
}