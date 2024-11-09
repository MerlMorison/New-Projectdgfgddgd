
using UdonSharp;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class RespawnObjScript : UdonSharpBehaviour
{
    public VRCObjectSync[] ObjectSyncs;
    public override void Interact()
    {
        for (int i = 0; i < ObjectSyncs.Length; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, ObjectSyncs[i].gameObject);
            ObjectSyncs[i].Respawn();
        }
    }
}
