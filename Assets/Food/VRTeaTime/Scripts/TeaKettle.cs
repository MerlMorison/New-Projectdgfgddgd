
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class TeaKettle : UdonSharpBehaviour
    {
        private Animator m_animator;

        [UdonSynced]
        bool m_isTeaKettleDripping = false;

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

            m_isTeaKettleDripping = true;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnPickupUseUp()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isTeaKettleDripping = false;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetBool("IsTeaKettleDripping", m_isTeaKettleDripping);
        }
    }
}