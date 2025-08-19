using BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates;
using BayoMod.Modules.Components;
using R2API.Networking.Interfaces;
using R2API.Networking;
using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo;
using UnityEngine.Networking;
using RoR2.ConVar;
using BayoMod.Survivors.Bayo.Components;
using BayoMod.Characters.Survivors.Bayo.Components.Demon;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates
{
    public class SummonGom : BaseEmote
    {
        private uint sound;
        private bool voiced = false;
        public CharacterBody enemyBody;

        private bool music;
        private BaseConVar convar;
        private string oldMusic;

        private CameraController cam;
        private GameObject hairPrefab;
        public override void OnEnter()
        {
            animString = "avocado";
            animDuration = 3.433f;
            canCancel = false;
            Util.PlaySound("avocado", this.gameObject);
            playBuffer = false;
            bodyName = "Body";
            enemyBody = base.GetComponent<ClimaxTracker>().GetTrackingTarget().healthComponent.body;

            music = Modules.Config.musicOn2.Value;
            bool client = Modules.Config.musicClient.Value;
            if (music)
            {
                if (client && isAuthority)
                {
                    Util.PlaySound("summusic", this.gameObject);
                }
                else if (!client)
                {
                    Util.PlaySound("summusic", this.gameObject);
                }
                if (isAuthority)
                {
                    convar = RoR2.Console.instance.FindConVar("volume_music");
                    if (convar != null)
                    {
                        oldMusic = convar.GetString();
                        if (oldMusic != "0") convar.SetString("0");
                    }
                }
            }

            zoom = false;
            cam = this.gameObject.GetComponent<CameraController>();
            cam.fov = 50f;
            cam.SetCam();

            if (NetworkServer.active && characterBody)
            {
                characterBody.AddTimedBuff(RoR2.RoR2Content.Buffs.HiddenInvincibility, animDuration + 0.3f);
            }

            base.OnEnter();

            if (enemyBody)
            {
                if (enemyBody.master) new SetFreezeOnBodyRequest(enemyBody.masterObjectId, 21.38f).Send(NetworkDestination.Clients);
                if (NetworkServer.active) enemyBody.AddBuff(BayoBuffs.climaxed);

                if (enemyBody.characterDirection)
                {
                    Vector3 targetDirection = this.transform.position - enemyBody.transform.position;
                    Vector3 lookDir = Vector3.RotateTowards(enemyBody.characterDirection.forward, targetDirection, 360f, 0f);
                    enemyBody.characterDirection.forward = lookDir;
                }

                if (characterDirection)
                {
                    Vector3 targetDirection = enemyBody.transform.position - this.transform.position;
                    Vector3 lookDir = Vector3.RotateTowards(characterDirection.forward, targetDirection, 360f, 0f);
                    characterDirection.forward = lookDir;

                }
                float dist = Vector3.Distance(this.characterBody.transform.position, enemyBody.transform.position);
                if (dist <= 20)
                {
                    float moveDistance = -1 * (20 - dist);
                    //Chat.AddMessage("distance = " + moveDistance.ToString());
                    this.characterMotor.rootMotion += characterDirection.forward * moveDistance;
                }
            }

            hairPrefab = BayoAssets.summonHair;
            if (characterDirection) Object.Instantiate(hairPrefab, this.characterBody.footPosition, Quaternion.LookRotation(characterDirection.forward, Vector3.up));
        }
        public override void OnExit()
        {
            if (enemyBody != null)
            {
                GameObject demon =Object.Instantiate(BayoAssets.gomorrah, enemyBody.footPosition, Quaternion.LookRotation(enemyBody.characterDirection.forward));
                demon.GetComponent<DemonController>().enemyBody = enemyBody;
                if (base.GetComponent<ClimaxTracker>()) base.GetComponent<ClimaxTracker>().ReleaseTarget();
                this.gameObject.GetComponent<BayoController>().oldMusic = oldMusic;
                ChildLocator component2 = demon.GetComponent<ChildLocator>();
                if ((bool)component2)
                {
                    int childIndex = component2.FindChildIndex("cambone");
                    Transform transformm = component2.FindChild(childIndex);
                    cam.demonCamTrans = transformm;
                    childIndex = component2.FindChildIndex("trackbone");
                    transformm = component2.FindChild(childIndex);
                    cam.demonTrackTrans = transformm;
                    cam.demonTime = 6f;
                    cam.DemonHandoff();
                }
            }
            else
            {
                cam.UnsetCam();
            }

            base.OnExit();
        }

    }
}
