using UnityEngine;
using RoR2;
using BayoMod.Characters.Survivors.Bayo.SkillStates;
using BayoMod.Survivors.Bayo;
using UnityEngine.TextCore.Text;
using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using RoR2.ConVar;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class KissCountdown : MonoBehaviour
    {
        public float waitTime = 2.5f;
        private float stopwatch;
        private CharacterMaster cm;
        private bool done = false;
        private string oldMusic;
        private BaseConVar convar;
        void Start()
        {
            stopwatch = 0f;
            cm = GetComponent<CharacterMaster>();

            if (cm.hasAuthority)
            {
                convar = RoR2.Console.instance.FindConVar("volume_music");
                if (convar != null)
                {
                    oldMusic = convar.GetString();
                    if (oldMusic != "0") convar.SetString("0");
                }
            }
        }

        private void OnDestroy()
        {
            if (convar != null)
            {
                if (oldMusic != "0") convar.SetString(oldMusic);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(cm != null)
            {
                if(cm.money <= 0)
                {
                    stopwatch += Time.deltaTime;
                    if(stopwatch >= waitTime){

                        GameObject bodyObject = cm.GetBodyObject();
                        if(bodyObject != null)
                        {
                            HealthComponent healthComponent = bodyObject.GetComponent<HealthComponent>();
                            EntityStateMachine[] stateMachines = bodyObject.GetComponents<EntityStateMachine>();
                            //"No statemachines?"
                            if (stateMachines[0])
                            {
                                foreach (EntityStateMachine stateMachine in stateMachines)
                                {

                                    if (healthComponent && stateMachine.customName == "Body")
                                    {
                                        if (healthComponent.health > 0) //yayyy
                                        {
                                            stateMachine.SetNextState(new Kiss());
                                        }

                                    }
                                }
                            }
                        }
                        Destroy(this);
                    }
                }
            }
        }
    }
}
