
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.Utilities;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class TeaSpoon : UdonSharpBehaviour
    {
        private Animator m_animator;
        private bool m_isHoldLeaf = false;
        private bool m_isHoldMilk = false;
        private bool m_isHoldSugar = false;

        [SerializeField]
        TeaSpoonBowl m_teaSpoonBowl;
        [SerializeField]
        TeaSpoonCollider m_teaSpoonCollider;
        [SerializeField]
        TeaSpoonSync m_teaSpoonSync;

        private byte m_state = (byte)SpoonState.Empty;

        void Start()
        {
            m_animator = this.gameObject.GetComponent<Animator>();

            if (IsValid(m_teaSpoonBowl))
            {
                m_teaSpoonBowl.Initialize(this);
            }

            if (IsValid(m_teaSpoonCollider))
            {
                m_teaSpoonCollider.Initialize(this);
            }

            if (IsValid(m_teaSpoonSync))
            {
                m_teaSpoonSync.Initialize(this);
            }
        }

        public void ScoopUpLeaf(byte state)
        {
            m_state = state;
            m_isHoldLeaf = true;
            m_isHoldMilk = false;
            m_isHoldSugar = false;

            if (Networking.IsOwner(this.gameObject))
            {
                m_teaSpoonSync.GlovalSync(m_state);
            }            
            LocalUpdate();
        }

        public void ScoopUpMilk()
        {
            m_state = (byte)SpoonState.HoldMilk;
            m_isHoldMilk = true;
            m_isHoldLeaf = false;
            m_isHoldSugar = false;

            if (Networking.IsOwner(this.gameObject))
            {
                m_teaSpoonSync.GlovalSync(m_state);
            }
            LocalUpdate();
        }

        public void ScoopUpSugar()
        {
            m_state = (byte)SpoonState.HoldSugar;
            m_isHoldSugar = true;
            m_isHoldMilk = false;
            m_isHoldLeaf = false;

            if (Networking.IsOwner(this.gameObject))
            {
                m_teaSpoonSync.GlovalSync(m_state);
            }
            LocalUpdate();
        }

        public override void OnPickupUseDown()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            switch (m_state)
            {
                case (byte)SpoonState.HoldTea:
                    m_state = (byte)SpoonState.ReleaseTea;
                    break;
                case (byte)SpoonState.HoldButterflyPea:
                    m_state = (byte)SpoonState.ReleaseButterflyPea;
                    break;
                case (byte)SpoonState.HoldRose:
                    m_state = (byte)SpoonState.ReleaseRose;
                    break;
                case (byte)SpoonState.HoldMilk:
                    m_state = (byte)SpoonState.ReleaseMilk;
                    break;
                default:
                    m_state = (byte)SpoonState.Empty;
                    break;
            }

            m_isHoldLeaf = false;
            m_isHoldMilk = false;
            m_isHoldSugar = false;

            m_teaSpoonSync.GlovalSync(m_state);
            LocalUpdate();
        }

        public override void OnPickupUseUp()
        {
            if (m_state == (byte)SpoonState.Empty)
            {
                return;
            }

            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_state = (byte)SpoonState.Empty;

            m_teaSpoonSync.GlovalSync(m_state);
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetInteger("State", m_state);
        }

        public void SyncUpdate(byte state)
        {
            m_state = state;
            m_animator.SetInteger("State", m_state);
        }

    }
}
