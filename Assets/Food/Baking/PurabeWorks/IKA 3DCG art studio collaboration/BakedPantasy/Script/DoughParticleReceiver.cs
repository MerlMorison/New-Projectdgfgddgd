
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DoughParticleReceiver : UdonSharpBehaviour
    {
        [SerializeField] private DoughController _main;
        [SerializeField,Header("対象レイヤー"),Tooltip("16: StereoRight")]
        private int _layer = 16;

        private void OnParticleCollision(GameObject other)
        {
            if (other.layer == _layer)
            {
                _main.ParticleCollision(other);
                Debug.Log("[pura]DoughParticleReceiver : 材料が入りました。");
            }
        }
    }
}