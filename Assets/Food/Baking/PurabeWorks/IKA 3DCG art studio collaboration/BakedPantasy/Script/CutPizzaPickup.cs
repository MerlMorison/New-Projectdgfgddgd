
using UnityEngine;
using VRC.SDK3.Components;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class CutPizzaPickup : ManualSyncUB
    {
        //一切れのピザ制御Pickup
        [SerializeField, Header("相対位置")] private Transform _offset;
        [SerializeField] private CutPizzaController _main;

        /// <summary>
        /// Spawn用の相対rot
        /// </summary>
        /// <returns>相対rotation</returns>
        public Quaternion RelativeRotation()
        {

            return Quaternion.Inverse(this.transform.rotation) * _offset.rotation;
        }

        public override void OnPickup()
        {
            GetOwner(_main.gameObject);
        }

        public override void OnPickupUseDown()
        {
            _main.Eat();
        }

        private void OnDisable()
        {
            _main.ResetAll();
        }
    }
}