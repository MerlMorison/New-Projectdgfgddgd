
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CutPizzaController : ManualSyncUB
    {
        //一切れのピザ制御
        [SerializeField] private Animator _animator;
        [SerializeField, Header("SE音源")] private AudioSource _audio;
        [SerializeField, Header("食べるSE")] private AudioClip _seEat;
        [SerializeField, Header("制御pickup")] private VRCPickup _pickup;

        [UdonSynced(UdonSyncMode.None)]
        private bool _eat = false;

        /// <summary>
        /// 食べる
        /// </summary>
        public void Eat()
        {
            _pickup.Drop();
            _eat = true;
            Synchronize();
            PlaySE();
        }

        /* 同期制御 */
        protected override void AfterSynchronize()
        {
            _animator.SetBool("Eat", _eat);
        }


        /* SE再生 */
        private void PlaySE()
        {
            ExecuteOnAll(nameof(PlaySESub));
        }

        public void PlaySESub()
        {
            _audio.PlayOneShot(_seEat);
        }

        /* リセット */
        public void ResetAll()
        {
            _eat = false;
            Synchronize();
        }
    }
}