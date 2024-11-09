
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class BowlCookiePickup : UdonSharpBehaviour
    {
        [SerializeField] BowlCookieController _main;

        private void OnTriggerEnter(Collider other)
        {
            _main.TriggerEnter(other.gameObject);
        }
    }
}