using BayoMod.Survivors.Bayo;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Characters.Survivors.Bayo.Components;

namespace BayoMod.Modules.Components
{
    internal class SetFreezeOnBodyRequest : INetMessage
    {
        NetworkInstanceId netID;
        float duration;

        public SetFreezeOnBodyRequest()
        {

        }

        public SetFreezeOnBodyRequest(NetworkInstanceId netID, float duration)
        {
            this.netID = netID;
            this.duration = duration;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            duration = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(duration);
        }

        public void OnReceived()
        {
            ForceFreezeState();
        }

        //Lots of checks in here.
        public void ForceFreezeState()
        {
            GameObject masterobject = Util.FindNetworkObject(netID);

            if (!masterobject)
            {
                Debug.Log("Specified GameObject not found!");
                return;
            }
            CharacterMaster charMaster = masterobject.GetComponent<CharacterMaster>();
            if (!charMaster)
            {
                Debug.Log("charMaster failed to locate");
                return;
            }

            if (!charMaster.hasEffectiveAuthority)
            {
                return;
            }

            GameObject bodyObject = charMaster.GetBodyObject();

            HealthComponent healthComponent = bodyObject.GetComponent<HealthComponent>();
            EntityStateMachine[] stateMachines = bodyObject.GetComponents<EntityStateMachine>();
            //"No statemachines?"
            if (!stateMachines[0])
            {
                Debug.LogWarning("StateMachine search failed! Wrong object?");
                return;
            }

            if (!healthComponent)
            {
                return;
            }

            foreach (EntityStateMachine stateMachine in stateMachines)
            {
                if (stateMachine.customName == "Body")
                {
                    foreach (Type blacklistType in BayoStaticValues.BLACKLIST_STATES)
                    {
                        if (stateMachine.state.GetType() == blacklistType)
                        {
                            return;
                        }

                    }

                    if (healthComponent.health > 0) //Fucking idiot.
                    {
                        masterobject.AddComponent<KissCountdown>();
                        stateMachine.SetNextState(new BayoFreeze { duration = this.duration });
                    }
                    return;
                }
            }
        }
    }
}