
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class Basil : ManualSyncUB
    {
        [SerializeField] private Animator _animator;

        [UdonSynced(UdonSyncMode.None)]
        private bool _isOpened = false;

        public override void OnPickupUseDown()
        {
            _isOpened = !_isOpened;
            Synchronize();
        }

        protected override void AfterSynchronize()
        {
            _animator.SetBool("Opened", _isOpened);
        }

        public void ResetAll()
        {
            _isOpened = false;
            Synchronize();
        }
    }
}