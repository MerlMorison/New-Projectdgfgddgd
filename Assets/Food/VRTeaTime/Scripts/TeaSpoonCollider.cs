
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TeaSpoonCollider : UdonSharpBehaviour
    {
        private TeaSpoon m_teaSpoon;

        public void Initialize(TeaSpoon teaSpoon)
        {
            this.m_teaSpoon = teaSpoon;
        }

        void OnParticleCollision(GameObject otherObject)
        {
            if (!Networking.IsOwner(m_teaSpoon.gameObject))
            {
                return;
            }

            if (otherObject.name == "Particle_Option_Milk")
            {
                m_teaSpoon.ScoopUpMilk();
                return;
            }
        }
    }
}