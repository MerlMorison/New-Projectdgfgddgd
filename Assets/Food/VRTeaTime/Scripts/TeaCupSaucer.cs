
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class TeaCupSaucer : UdonSharpBehaviour
    {
        [SerializeField]
        TeaCup m_teaCup;

        private bool m_isFixMode = true;

        public void Initialize(TeaCup teaCup)
        {
            this.m_teaCup = teaCup;
            teaCup.SetFixMode(m_isFixMode);
        }

        public override void OnPickupUseDown()
        {
            if (!Networking.IsOwner(m_teaCup.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, m_teaCup.gameObject);
            }

            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

            m_isFixMode = !m_isFixMode;
            m_teaCup.SetFixMode(m_isFixMode);
        }
    }
}