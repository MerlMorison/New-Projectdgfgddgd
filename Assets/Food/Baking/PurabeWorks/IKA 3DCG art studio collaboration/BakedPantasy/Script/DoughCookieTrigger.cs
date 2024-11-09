
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    public class DoughCookieTrigger : ManualSyncUB
    {
        [SerializeField] private DoughCookieController _main;

        private void OnDisable()
        {
            ResetAll();
        }

        private void OnTriggerEnter(Collider other)
        {
            _main.TriggerEnter(other.gameObject);
        }

        public void ResetAll()
        {
            _main.ResetAll();
        }
    }
}