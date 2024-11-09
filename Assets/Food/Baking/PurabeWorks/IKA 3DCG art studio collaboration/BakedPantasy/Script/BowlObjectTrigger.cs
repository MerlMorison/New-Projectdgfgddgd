
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BowlObjectTrigger : UdonSharpBehaviour
    {
        /* 材料をボウルに入れる,ツールの動作を受ける */

        [SerializeField] BowlController _main;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != _main.Layer)
            {
                return;
            }

            /* 材料追加 */
            Ingredient ing = other.gameObject.GetComponent<Ingredient>();
            if (ing != null)
            {
                bool res = _main.AddIngredient(ing.ItemType);
                Log("材料が入りました。itemType= " + ing.ItemType);

                if (res)
                {
                    //消費アニメーション再生
                    ing.SendCustomEvent(nameof(ConsumableIngredient.Consume));
                }
            }

            /* 調理器具での操作 */
            int bowlType = _main.Type;
            Kitchenware kit = other.gameObject.GetComponent<Kitchenware>();
            if (bowlType == 2 && kit && kit.ItemType == 1)
            {
                //木べらでクッキー生地をこねる
                _main.Make();
                Log("木べらでクッキー生地をこねました。");
            }
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]BowlObjectTrigger : " + msg);
        }
    }
}
