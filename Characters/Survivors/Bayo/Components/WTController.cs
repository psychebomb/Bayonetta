using System.Collections.Generic;
using System.Runtime.InteropServices;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

public class WTController : NetworkBehaviour
{
    private struct OwnerInfo
    {
        public readonly GameObject gameObject;

        public readonly Transform transform;

        public readonly CharacterBody characterBody;

        public OwnerInfo(GameObject gameObject)
        {
            this.gameObject = gameObject;
            if ((bool)gameObject)
            {
                transform = gameObject.transform;
                characterBody = gameObject.GetComponent<CharacterBody>();
            }
            else
            {
                transform = null;
                characterBody = null;
            }
        }
    }

    public BuffWard buffWard;

    public GameObject owner;

    private OwnerInfo cachedOwnerInfo;

    public ParticleSystem[] auraParticles;

    private new Transform transform;

    private float actualRadius;

    private float scaleVelocity;

    private NetworkInstanceId ___ownerNetId;

    public GameObject Networkowner
    {
        get
        {
            return owner;
        }
        [param: In]
        set
        {
            SetSyncVarGameObject(value, ref owner, 2u, ref ___ownerNetId);
        }
    }

    private void Awake()
    {
        transform = base.transform;
        OnIciclesActivated();
    }

    private void FixedUpdate()
    {
        if (cachedOwnerInfo.gameObject != owner)
        {
            cachedOwnerInfo = new OwnerInfo(owner);
        }
        if (NetworkServer.active)
        {
            if (!owner)
            {
                Object.Destroy(base.gameObject);
                return;
            }
        }
        if ((bool)cachedOwnerInfo.characterBody)
        {
            
        }

        if ((bool)buffWard)
        {
            
        }
    }

    private void UpdateVisuals()
    {
        if ((bool)cachedOwnerInfo.gameObject)
        {
            transform.position = (cachedOwnerInfo.characterBody ? cachedOwnerInfo.characterBody.corePosition : cachedOwnerInfo.transform.position);
        }
        float num = Mathf.SmoothDamp(transform.localScale.x, actualRadius, ref scaleVelocity, 0.5f);
        transform.localScale = new Vector3(num, num, num);
    }

    private void OnIciclesDeactivated()
    {
        Util.PlaySound("Stop_item_proc_icicle", base.gameObject);
        ParticleSystem[] array = auraParticles;
        for (int i = 0; i < array.Length; i++)
        {
            ParticleSystem.MainModule main = array[i].main;
            main.loop = false;
        }
    }

    private void OnIciclesActivated()
    {
        Util.PlaySound("Play_item_proc_icicle", base.gameObject);
        ParticleSystem[] array = auraParticles;
        foreach (ParticleSystem obj in array)
        {
            ParticleSystem.MainModule main = obj.main;
            main.loop = true;
            obj.Play();
        }
    }

    private void LateUpdate()
    {
        UpdateVisuals();
    }
    public void OnDestroy()
    {
        OnIciclesDeactivated();
    }

    private void UNetVersion()
    {
    }

    public override bool OnSerialize(NetworkWriter writer, bool forceAll)
    {
        if (forceAll)
        {
            writer.Write(owner);
            return true;
        }
        bool flag = false;
        if ((base.syncVarDirtyBits & (true ? 1u : 0u)) != 0)
        {
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
                flag = true;
            }
        }
        if ((base.syncVarDirtyBits & 2u) != 0)
        {
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
                flag = true;
            }
            writer.Write(owner);
        }
        if (!flag)
        {
            writer.WritePackedUInt32(base.syncVarDirtyBits);
        }
        return flag;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        if (initialState)
        {
            ___ownerNetId = reader.ReadNetworkId();
            return;
        }
        int num = (int)reader.ReadPackedUInt32();
        if (((uint)num & (true ? 1u : 0u)) != 0)
        {
        }
        if (((uint)num & 2u) != 0)
        {
            owner = reader.ReadGameObject();
        }
    }

    public override void PreStartClient()
    {
        if (!___ownerNetId.IsEmpty())
        {
            Networkowner = ClientScene.FindLocalObject(___ownerNetId);
        }
    }
}
