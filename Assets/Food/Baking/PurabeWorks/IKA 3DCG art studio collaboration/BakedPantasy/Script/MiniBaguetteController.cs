
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class MiniBaguetteController : CutDoughController
    {
        //二次発酵タイミング
        protected override int FermentState => 2;
        //焼成タイミング
        protected override int BakeState => 5;
        //終了状態
        protected override int EndState => 8;

        public override void UseDown()
        {
            Eat("プチフランスパン");

            if (!_baked && _state == 0)
            {
                // プチフランスパン生地の成形
                Process("プチフランスパン生地を丸めました。");
            }
        }

        public override void TriggerEnter(GameObject go)
        {
            if (go.layer != _layer)
            {
                return;
            }

            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null)
            {
                int itemType = kitchenware.ItemType;

                if (itemType == 7 && _state == 1)
                {
                    //二次発酵
                    SetOnFermentPlace(go, "プチフランスパン");
                }
                else if (itemType == 8 && _state == 3)
                {
                    // クーペナイフで切れ込みを入れる
                    Process("プチフランスパン生地に切れ込みを入れました。");
                }
                else if (itemType == 9 && _state == 4)
                {
                    // オーブン皿に固定する
                    SetOnOvenTray(go, "プチフランスパン");
                }
            }
        }
    }
}