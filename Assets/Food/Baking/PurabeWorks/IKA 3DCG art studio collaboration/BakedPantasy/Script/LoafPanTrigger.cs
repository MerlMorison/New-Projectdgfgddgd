
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LoafPanTrigger : Kitchenware
    {
        [SerializeField] private LoafPanController _main;
        public bool AddDough()
        {
            return _main.AddDough();
        }
        private void OnTriggerEnter(Collider other)
        {
            _main.TriggerEnter(other.gameObject);
        }
        private void OnTriggerStay(Collider other)
        {
            _main.TriggerStay(other.gameObject);
        }
    }
}