
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BowlParticleReceiver : UdonSharpBehaviour
    {
        /* パーティクル材料をボウルに追加する */

        [SerializeField] BowlController bowlController;
        [SerializeField, Header("対象レイヤー"), Tooltip("16: StereoRight")]
        private int layer = 16;

        private void OnParticleCollision(GameObject other)
        {
            if (other.layer != layer)
            {
                return;
            }

            Ingredient ing = other.GetComponent<Ingredient>();
            if (ing != null)
            {
                bowlController.AddIngredient(ing.ItemType);
                Debug.Log("[pura]BowlParticleReceiver : 材料が入りました。itemType= " + ing.ItemType);
            }
        }
    }
}