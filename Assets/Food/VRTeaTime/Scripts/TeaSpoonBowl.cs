
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TeaSpoonBowl : UdonSharpBehaviour
    {
        private TeaSpoon m_teaSpoon;

        public void Initialize(TeaSpoon teaSpoon)
        {
            this.m_teaSpoon = teaSpoon;
        }

        public void OnTriggerEnter(Collider other)
        {
            string[] nameColliderSplited = other.name.Split(new char[] { '_' });

            if (nameColliderSplited[0] != "Canister" 
                && nameColliderSplited[0] != "Option" 
                && nameColliderSplited[0] != "Cup")
            {
                return;
            }
            switch (nameColliderSplited[0])
            {
                case "Canister":
                    switch (nameColliderSplited[1])
                    {
                        case "Tea":
                            m_teaSpoon.ScoopUpLeaf((byte)SpoonState.HoldTea);
                            break;
                        case "ButterflyPea":
                            m_teaSpoon.ScoopUpLeaf((byte)SpoonState.HoldButterflyPea);
                            break;
                        case "Rose":
                            m_teaSpoon.ScoopUpLeaf((byte)SpoonState.HoldRose);
                            break;
                        default:
                            break;
                    }
                    break;
                case "Option":
                    if (nameColliderSplited[1] != "Sugar")
                    {
                        break;
                    }
                    m_teaSpoon.ScoopUpSugar();
                    break;
                default:
                    break;
            }

        }
    }
}