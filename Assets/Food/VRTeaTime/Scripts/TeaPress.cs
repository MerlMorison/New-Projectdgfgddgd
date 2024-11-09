
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class TeaPress : UdonSharpBehaviour
    {
        private Animator m_animator;
        private int m_eventStatus = (byte)Event.NULL;

        [UdonSynced]
        byte m_state = (byte)TeaPressState.Empty_Close;
        [UdonSynced]
        byte m_typeLeaf = (byte)Leaf.NULL;

        void Start()
        {
            m_animator = this.gameObject.GetComponent<Animator>();

        }

        public override void OnPickupUseDown()
        {
            switch (m_state)
            {
                case (byte)TeaPressState.Empty_Close:
                    break;
                case (byte)TeaPressState.Empty_Open:
                    break;
                case (byte)TeaPressState.Heating:
                    break;
                case (byte)TeaPressState.Extracting_Open:
                    break;
                case (byte)TeaPressState.Extracting_Close:
                    break;
                case (byte)TeaPressState.Extracted_2:
                    break;
                case (byte)TeaPressState.Extracted_1:
                    break;
                case (byte)TeaPressState.Extracted_0:
                    break;
                case (byte)TeaPressState.Extracted_Open:
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
                case (byte)TeaPressState.Empty_Open:
                    break;
                case (byte)TeaPressState.Empty_Heated:
                    break;
                case (byte)TeaPressState.LeafReady:
                    break;
                case (byte)TeaPressState.LeafReady_Heated:
                    break;
                default:
                    return;
            }

            string[] nameColliderSplited = otherObject.name.Split(new char[] { '_' });

            if (nameColliderSplited[0] != "Particle") return;

            switch (nameColliderSplited[1])
            {
                case "Water":
                    switch (m_state)
                    {
                        case (byte)TeaPressState.Empty_Open:
                            break;
                        case (byte)TeaPressState.LeafReady:
                            break;
                        case (byte)TeaPressState.LeafReady_Heated:
                            break;
                        default:
                            return;
                    }

                    m_eventStatus = (byte)Event.ParticleWater;
                    m_state = TransitionState();

                    break;
                case "Leaf":
                    switch (m_state)
                    {
                        case (byte)TeaPressState.Empty_Open:
                            break;
                        case (byte)TeaPressState.Empty_Heated:
                            break;
                        default:
                            return;
                    }

                    m_eventStatus = (byte)Event.ParticleLeaf;

                    switch (nameColliderSplited[2])
                    {
                         case "Tea":
                            m_typeLeaf = (byte)Leaf.Tea;
                            break;
                        case "ButterflyPea":
                            m_typeLeaf = (byte)Leaf.ButterflyPea;
                            break;
                        case "Rose":
                            m_typeLeaf = (byte)Leaf.Rose;
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

        public override void OnDeserialization()
        {
            LocalUpdate();
        }

        public void LocalUpdate()
        {
            m_animator.SetInteger("State", m_state);
            m_animator.SetInteger("TypeLeaf", m_typeLeaf);
        }

        public byte TransitionState()
        {
            byte nextState = m_state;

            switch (m_state)
            {
                case (byte)TeaPressState.Empty_Close:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Empty_Open;
                        break;
                    }
                    break;
                case (byte)TeaPressState.Empty_Open:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Empty_Close;
                        break;
                    }

                    if (m_eventStatus == (byte)Event.ParticleWater) 
                    {
                        nextState = (byte)TeaPressState.Heating;
                        break;
                    }

                    if (m_eventStatus == (byte)Event.ParticleLeaf)
                    {
                        nextState = (byte)TeaPressState.LeafReady;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Heating:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Empty_Heated;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Empty_Heated:
                    if (m_eventStatus == (byte)Event.ParticleLeaf)
                    {
                        nextState = (byte)TeaPressState.LeafReady_Heated;
                        break;
                    }

                    break;
                case (byte)TeaPressState.LeafReady:
                    if (m_eventStatus == (byte)Event.ParticleWater)
                    {
                        nextState = (byte)TeaPressState.Extracting_Open;
                        break;
                    }

                    break;
                case (byte)TeaPressState.LeafReady_Heated:
                    if (m_eventStatus == (byte)Event.ParticleWater)
                    {
                        nextState = (byte)TeaPressState.Extracting_Open;
                        break;
                    }
                    break;
                case (byte)TeaPressState.Extracting_Open:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Extracting_Close;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Extracting_Close:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Extracted_2;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Extracted_2:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Extracted_1;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Extracted_1:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Extracted_0;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Extracted_0:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Extracted_Open;
                        break;
                    }

                    break;
                case (byte)TeaPressState.Extracted_Open:
                    if (m_eventStatus == (byte)Event.Trigger)
                    {
                        nextState = (byte)TeaPressState.Empty_Open;
                        break;
                    }

                    break;

                default:
                    break;
            }

            return nextState;
        }
    }
}