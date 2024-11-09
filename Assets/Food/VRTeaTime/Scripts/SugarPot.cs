
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.Utilities;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class SugarPot : UdonSharpBehaviour
    {
        private Animator m_animator;
        private bool m_isSugarPotOpened = false;

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

            m_isSugarPotOpened = !m_isSugarPotOpened;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetBool("IsSugarPotOpened", m_isSugarPotOpened);
        }
    }
}