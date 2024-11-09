
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.Utilities;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Tongs : UdonSharpBehaviour
    {
        private Animator m_animator;

        [UdonSynced]
        bool m_isTongsClosed = false;
        [UdonSynced]
        bool m_isTakeLemon = false;
        [UdonSynced]
        bool m_isTakeSuger = false;

        [SerializeField]
        TongsLemon m_tongsLemon;

        private bool m_onColliderLemon = false;
        private bool m_onColliderSugar = false;        

        void Start()
        {
            m_animator = this.gameObject.GetComponent<Animator>();

            if (IsValid(m_tongsLemon))
            {
                m_tongsLemon.Initialize(this);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                return;
            }

            string[] nameColliderSplited = other.name.Split(new char[] { '_' });

            if (nameColliderSplited[0] != "Option")
            {
                return;
            }

            switch (nameColliderSplited[1])
            {
                case "Lemon":
                    m_onColliderLemon = true;
                    break;
                case "Sugar":
                    m_onColliderSugar = true;
                    break;
                default:
                    break;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                return;
            }

            string[] nameColliderSplited = other.name.Split(new char[] { '_' });

            if (nameColliderSplited[0] != "Option")
            {
                return;
            }

            switch (nameColliderSplited[1])
            {
                case "Lemon":
                    m_onColliderLemon = false;
                    break;
                case "Sugar":
                    m_onColliderSugar = false;
                    break;
                default:
                    break;
            }
        }

        public override void OnPickupUseDown()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            if (m_onColliderLemon)
            {
                m_isTakeLemon = true;
            }
            else if (m_onColliderSugar)
            {
                m_isTakeSuger = true;
            }
            else
            {
                m_isTongsClosed = true;

            }

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnPickupUseUp()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isTakeLemon = false;
            m_isTakeSuger = false;
            m_isTongsClosed = false;

            RequestSerialization();
            LocalUpdate();
        }

        public void PutLemon()
        {
            m_isTakeLemon = false;
            m_isTongsClosed = true;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetBool("IsTongsClosed", m_isTongsClosed);
            m_animator.SetBool("IsTakeLemon", m_isTakeLemon);
            m_animator.SetBool("IsTakeSuger", m_isTakeSuger);
        }

        
    }
}