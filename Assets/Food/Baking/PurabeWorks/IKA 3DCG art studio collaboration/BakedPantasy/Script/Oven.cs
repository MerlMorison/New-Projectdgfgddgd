
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Oven : Kitchenware
    {
        //オーブン内部の制御
        [SerializeField, Header("オーブントレー固定点")]
        private Transform _trayFixPoint;
        [SerializeField, Header("ピザ固定点")]
        private Transform _pizzaFixPoint;
        [SerializeField, Header("食パン型固定点")]
        private Transform _loafPanFixPoint;
        [SerializeField, Header("オーブンSE音源")]
        private AudioSource _audio;
        [SerializeField, Header("焼成SE")]
        private AudioClip _seBake;
        [SerializeField, Header("焦げたSE")]
        private AudioClip _seBurn;

        /// <summary>
        /// オーブン皿の固定点
        /// </summary>
        public Transform TrayFixPoint
        {
            get { return _trayFixPoint; }
        }

        /// <summary>
        /// ピザの固定点
        /// </summary>
        public Transform PizzaFixpoint
        {
            get { return _pizzaFixPoint; }
        }

        /// <summary>
        /// 食パン型の固定点
        /// </summary>
        public Transform LoafPanFixPoint
        {
            get { return _loafPanFixPoint; }
        }

        /// <summary>
        /// 焼成時のSEを再生
        /// </summary>
        public void PlaySEBake()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(PlaySEBakeSub));
        }

        public void PlaySEBakeSub()
        {
            if (!_audio.isPlaying)
            {
                _audio.PlayOneShot(_seBake);
            }
        }

        /// <summary>
        /// 焦がした時のSEを再生
        /// </summary>
        public void PlaySEOops()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(PlaySEOopsSub));
        }

        public void PlaySEOopsSub()
        {
            if (!_audio.isPlaying)
            {
                _audio.PlayOneShot(_seBurn);
            }
        }
    }
}