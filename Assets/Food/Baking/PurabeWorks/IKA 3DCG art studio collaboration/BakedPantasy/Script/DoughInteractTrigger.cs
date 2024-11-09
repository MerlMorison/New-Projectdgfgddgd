
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DoughInteractTrigger : UdonSharpBehaviour
    {
        [SerializeField] private DoughController _mainController;

        public override void Interact()
        {
            _mainController.Make();
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;
            if (go.layer == _mainController.Layer)
            {
                _mainController.TriggerEnter(go);
            }
        }
    }
}