
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SamenoLab.TeaTime
{
    public enum Leaf
    {
        NULL, Tea, ButterflyPea, Rose
    }

    public enum Drip
    {
        NULL, Tea, ButterflyPea, Rose
    }

    public enum Option
    {
        NULL, Lemon, Milk, Honey
    }

    public enum Event
    {
        NULL, Reset, Trigger, ParticleWater, ParticleLeaf, ParticleDrip, ParticleOption
    }

    public enum TeaPressState
    {
        Empty_Close, Empty_Open, Heating, Empty_Heated, LeafReady, LeafReady_Heated, Extracting_Open, Extracting_Close,  Extracted_2, Extracted_1, Extracted_0, Extracted_Open
    }

    public enum SpoonState
    {
        Empty, HoldTea, HoldButterflyPea, HoldRose, ReleaseTea, ReleaseButterflyPea, ReleaseRose, HoldMilk, ReleaseMilk, HoldSugar
    }

    public enum CupState
    {
        Empty, Heating, Empty_Heated, Extracted_2, Extracted_1, Extracted_0
    }

    public class Main : UdonSharpBehaviour
    {

    }
}