
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TongsLemon : UdonSharpBehaviour
    {

        private Tongs m_tongs;

        public void Initialize(Tongs tongs)
        {
            this.m_tongs = tongs;
        }


        public void OnTriggerEnter(Collider other)
        {
            if (!Networking.IsOwner(m_tongs.gameObject))
            {
                return;
            }

            if (other.name == "Cup")
            {
                m_tongs.PutLemon();
            }
        }
    }
}