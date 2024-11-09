
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BowlBreadMakeTrigger : UdonSharpBehaviour
    {
        /* 材料をこねる判定 */

        [SerializeField] BowlBreadController bowlBreadController;

        public override void Interact()
        {
            bowlBreadController.Make();
        }
    }
}
