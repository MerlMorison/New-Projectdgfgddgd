
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class Ingredient : ManualSyncUB
    {
        [SerializeField, Header("材料種別")]
        [Tooltip("1:小麦粉 2:砂糖 3:塩 4:牛乳 5:水 6:バター 7:ドライイースト 8:卵 9:トマトソース 10:バジル 11:チーズ 12:オリーブオイル")]
        private int _itemType = 0;

        public int ItemType { get { return _itemType; } }
    }
}