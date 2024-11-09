
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class PizzaPickup : ManualSyncUB
    {
        [SerializeField] private PizzaController _main;
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        private int _layer = 13;

        private VRCPickup _pickup;
        private VRCObjectSync _objSync;

        private void Start()
        {
            _objSync = this.gameObject.GetComponent<VRCObjectSync>();
            _pickup = this.gameObject.GetComponent<VRCPickup>();
        }

        /// <summary>
        /// Pickup時にmain部もオーナ権限取得
        /// </summary>
        public override void OnPickup()
        {
            if (!Networking.IsOwner(_main.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, _main.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject target = other.gameObject;
            if (target.layer == _layer)
            {
                _main.TriggerEnter(target);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == _layer)
            {
                _main.TriggerStay(other.gameObject);
            }
        }

        /// <summary>
        /// 位置を移動する
        /// </summary>
        /// <param name="fixPoint">異動先</param>
        public void MoveTo(Transform fixPoint)
        {
            GetOwner(this.gameObject);
            _pickup.Drop();
            if (_objSync)
            {
                _objSync.FlagDiscontinuity();
                _objSync.TeleportTo(fixPoint);
            }
            else
            {
                this.gameObject.transform.SetPositionAndRotation(
                    fixPoint.position, fixPoint.rotation);
            }
            Log("位置を移動されました。 " + fixPoint.position.ToString());
        }

        //無効化の際にリセットさせる
        private void OnDisable()
        {
            ResetAll();
        }

        public void ResetAll()
        {
            _main.ResetAll();
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]PizzaPickup : " + msg);
        }
    }
}