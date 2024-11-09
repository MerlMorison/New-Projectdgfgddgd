
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class OvenTrayPickup : UdonSharpBehaviour
    {
        [SerializeField] private OvenTray _main;

        public override void OnPickup()
        {
            //メイン部のオーナ権限を取得
            if (!Networking.IsOwner(_main.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, _main.gameObject);
            }
        }
    }
}