
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks
{
    [RequireComponent(typeof(VRCPickup))]
    public class ToggleOrientation : UdonSharpBehaviour
    {
        /* VRかDesktopかでPickupの挙動を変更 */
        void Start()
        {
            VRCPlayerApi localPlayer = Networking.LocalPlayer;
            if(localPlayer == null)
            {
                return;
            }
            
            VRCPickup pickup = (VRCPickup)GetComponent(typeof(VRCPickup));
            pickup.ExactGrip = null;
            pickup.ExactGun = null;

            if(localPlayer.IsUserInVR())
            {
                // VRモードなら移動なし
                pickup.orientation = VRC_Pickup.PickupOrientation.Grip;
            } else
            {
                // Desktopなら移動あり
                pickup.orientation = VRC_Pickup.PickupOrientation.Any;
            }
        }
    }
}