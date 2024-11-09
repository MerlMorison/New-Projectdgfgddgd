
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.Utilities;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class TeaCup : UdonSharpBehaviour
    {
        private Animator m_animator;
        private byte m_eventStatus = (byte)Event.NULL;

        [SerializeField]
        TeaCupSaucer m_teaCupSaucer;

        [UdonSynced]
        bool m_isFixedSaucer = true;
        [UdonSynced]
        bool m_isFixMode = false;

        [UdonSynced]
        byte m_state = (byte)CupState.Empty;
        [UdonSynced]
        byte m_typeDrip = (byte)Drip.NULL;
        [UdonSynced]
        byte m_typeOption = (byte)Option.NULL;

        void Start()
        {
            m_animator = this.gameObject.GetComponent<Animator>();

            if (IsValid(m_teaCupSaucer))
            {
                m_teaCupSaucer.Initialize(this);
            }
        }

        public void SetFixMode(bool status)
        {
            m_isFixMode = status;
            m_isFixedSaucer = status;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnPickup()
        {
            if(!m_isFixMode)
            {
                return;
            }

            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isFixedSaucer = false;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDrop()
        {
            if (!m_isFixMode)
            {
                return;
            }

            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isFixedSaucer = true;

            RequestSerialization();
            LocalUpdate();
        }

        public override void OnPickupUseDown()
        {
            switch (m_state)
            {
                case (byte)CupState.Heating:
                    break;
                case (byte)CupState.Extracted_2:
                    break;
                case (byte)CupState.Extracted_1:
                    break;
                case (byte)CupState.Extracted_0:
                    break;
                default:
                    return;
            }

            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_eventStatus = (byte)Event.Trigger;
            m_state = TransitionState();
            m_eventStatus = (byte)Event.NULL;

            RequestSerialization();
            LocalUpdate();
        }

        void OnParticleCollision(GameObject otherObject)
        {
            switch (m_state)
            {
                case (byte)CupState.Empty:
                    break;
                case (byte)CupState.Empty_Heated:
                    break;
                case (byte)CupState.Extracted_2:
                    break;
                default:
                    return;
            }

            string[] nameColliderSplited = otherObject.name.Split(new char[] { '_' });

            if (nameColliderSplited[0] != "Particle")
            {
                return;
            }

            switch (nameColliderSplited[1])
            {
                case "Water":
                    if (m_state != (byte)CupState.Empty)
                    {
                        return;
                    }

                    m_eventStatus = (byte)Event.ParticleWater;
                    m_state = TransitionState();
                    m_typeOption = (byte)Option.NULL;
                    break;
                case "Drip":
                    switch (m_state)
                    {
                        case (byte)CupState.Empty:
                            break;
                        case (byte)CupState.Empty_Heated:
                            break;
                        default:
                            return;
                    }

                    m_eventStatus = (byte)Event.ParticleDrip;
                    m_typeOption = 0;
                    switch (nameColliderSplited[2])
                    {
                        case "Tea":
                            m_typeDrip = (byte)Drip.Tea;
                            break;
                        case "ButterflyPea":
                            m_typeDrip = (byte)Drip.ButterflyPea;
                            break;
                        case "Rose":
                            m_typeDrip = (byte)Drip.Rose;
                            break;
                        default:
                            break;
                    }
                    m_state = TransitionState();
                    break;
                case "Option":
                    if (m_typeOption != (byte)Option.NULL)
                    {
                        return;
                    }

                    if (m_state != (byte)CupState.Extracted_2)
                    {
                        return;
                    }

                    m_eventStatus = (byte)Event.ParticleOption;
                    switch (nameColliderSplited[2])
                    {
                        case "Lemon":
                            if (m_typeDrip == (byte)Drip.Tea || m_typeDrip == (byte)Drip.ButterflyPea)
                            {
                                m_typeOption = (byte)Option.Lemon;
                            }                            
                            break;
                        case "Milk":
                            if (m_typeDrip == (byte)Drip.Tea)
                            {
                                m_typeOption = (byte)Option.Milk;
                            }                            
                            break;
                        case "Huney":
                            if (m_typeDrip == (byte)Drip.Tea)
                            {
                                m_typeOption = (byte)Option.Honey;
                            }
                            break;
                        default:
                            break;
                    }
                    m_state = TransitionState();
                    break;
                default:
                    break;
            }

            m_eventStatus = (byte)Event.NULL;

            if (Networking.IsOwner(this.gameObject))
            {
                RequestSerialization();
            }

            LocalUpdate();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (m_state != (byte)CupState.Extracted_2)
            {
                return;
            }

            if (other.name != "Lemon")
            {
                return;
            }

            m_typeOption = (byte)Option.Lemon;

            if (Networking.IsOwner(this.gameObject))
            {
                RequestSerialization();
            }

            LocalUpdate();
        }

        public byte TransitionState()
        {
            byte nextState = m_state;

            switch (m_state)
            {
                case (byte)CupState.Empty:
                    if (m_eventStatus == (byte)Event.ParticleWater)
                    {
                        nextState = (byte)CupState.Heating;
                        break;
                    }
                    if (m_eventStatus == (byte)Event.ParticleDrip)
                    {
                        nextState = (byte)CupState.Extracted_2;
                        break;
                    }
                    break;
                case (byte)CupState.Heating:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)CupState.Empty_Heated;
                        break;
                    }
                    break;
                case (byte)CupState.Empty_Heated:
                    if (m_eventStatus == (byte)Event.ParticleDrip)
                    {
                        nextState = (byte)CupState.Extracted_2;
                        break;
                    }
                    break;
                case (byte)CupState.Extracted_2:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)CupState.Extracted_1;
                        break;
                    }
                    break;
                case (byte)CupState.Extracted_1:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)CupState.Extracted_0;
                        break;
                    }
                    break;
                case (byte)CupState.Extracted_0:
                    m_typeOption = (byte)Option.NULL;
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)CupState.Empty;
                        break;
                    }
                    break;
                default:
                    break;
            }

            return nextState;
        }

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetInteger("State", m_state);
            m_animator.SetInteger("TypeDrip", m_typeDrip);
            m_animator.SetInteger("TypeOption", m_typeOption);
            m_animator.SetBool("IsFixedSaucer", m_isFixedSaucer);
        }

    }
}
