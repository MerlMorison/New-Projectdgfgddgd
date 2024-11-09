
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class CookieController : DoughController
    {
        public override void UseDown()
        {
            if (_state < 2)
            {
                // 手で丸めるor広げる
                Process("手で丸めるor広げました。");
            }
        }

        public override void UseCookie()
        {
            // メロンパン生地に貼り付け
            _state = 2; //丸めてなくても貼り付ける
            Process("クッキー生地を貼り付けました。");
            // リターン
            ReturnThis();
        }
    }
}
