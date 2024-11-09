
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TeaSpoonSync : UdonSharpBehaviour
    {
        [SerializeField]
        TeaSpoon m_teaSpoon;

        [UdonSynced]
        byte m_state = (byte)SpoonState.Empty;

        public void Initialize(TeaSpoon teaSpoon)
        {
            this.m_teaSpoon = teaSpoon;
        }

        public void GlovalSync(byte state)
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
            m_state = state;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            m_teaSpoon.SyncUpdate(m_state);
        }
    }
}