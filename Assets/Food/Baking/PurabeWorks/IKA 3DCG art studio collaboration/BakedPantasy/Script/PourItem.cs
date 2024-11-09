
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRC_Pickup))]
    public class PourItem : UdonSharpBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _particle;

        private void Start()
        {
            _particle.Stop(true);
        }

        public override void OnPickupUseDown()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PourOn));
        }

        public override void OnPickupUseUp()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PourOff));
        }

        public void PourOn()
        {
            if (_animator)
            {
                _animator.SetBool("Opened", true);
            }
            _particle.Play(true);
        }

        public void PourOff()
        {
            if (_animator)
            {
                _animator.SetBool("Opened", false);
            }
            _particle.Stop(true);
        }

        public void ResetAll()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(ResetAllSub));
        }

        public void ResetAllSub()
        {
            if (_animator)
            {
                _animator.SetBool("Opened", false);
                _animator.SetTrigger("Reset");
            }
        }
    }
}