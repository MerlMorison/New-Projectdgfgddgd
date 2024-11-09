
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class MilkPot : UdonSharpBehaviour
    {
        private Animator m_animator;

        [UdonSynced]
        bool m_isMilkPotDripping = false;

        void Start()
        {
            m_animator = this.gameObject.GetComponent<Animator>();
        }

        public override void OnPickupUseDown()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isMilkPotDripping = true;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnPickupUseUp()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isMilkPotDripping = false;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetBool("IsMilkPotDripping", m_isMilkPotDripping);
        }
    }
}