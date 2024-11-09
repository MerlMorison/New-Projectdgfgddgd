
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class CroissantController : CutDoughController
    {
        //二次発酵なし
        protected override int FermentState => -1;
        //焼成タイミング
        protected override int BakeState => 2;
        //終了状態 
        protected override int EndState => 6;

        public override void UseDown()
        {
            Eat("クロワッサン");

            if (!_baked && _state == 0)
            {
                // クロワッサン生地の成形
                Process("クロワッサン生地を丸めました。");
            }
        }

        public override void TriggerEnter(GameObject go)
        {
            if (go.layer != _layer)
            {
                return;
            }

            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null && kitchenware.ItemType == 9
                && _state == 1)
            {
                // オーブン皿に固定する
                SetOnOvenTray(go, "クロワッサン");
            }
        }
    }
}