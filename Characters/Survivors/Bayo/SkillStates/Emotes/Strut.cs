using BayoMod.Characters.Survivors.Bayo.SkillStates.Emotes;
using BayoMod.Survivors.Bayo;
using EntityStates;
using RoR2;
using RoR2.ConVar;
using System.Globalization;
using UnityEngine;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class Strut : BaseSkillState
    {
        public float animDuration = 1.92f;
        public float cancelDuration;
        private RootMotionAccumulator rootmotion;
        protected bool cancel;
        protected float stopwatch;

        protected bool jumped;
        protected bool flag1;
        private uint sound;

        protected Animator animator;

        private CharacterCameraParams cameraParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        private bool music;
        //private float currentMaster = float.Parse(AudioManager.cvVolumeMaster.GetString(), CultureInfo.InvariantCulture) / 100f;
        //private float actualMSX = float.Parse(AudioManager.cvVolumeParentMsx.GetString(), CultureInfo.InvariantCulture);
        private string oldMusic;
        BaseConVar convar;

        public override void OnEnter()
        {
            rootmotion = GetModelRootMotionAccumulator();
            flag1 = false;
            characterBody.hideCrosshair = true;
            animator = GetModelAnimator();
            PlayAnimation("FullBody, Override", "Dreadful", "Emote.playbackRate", animDuration);
            base.OnEnter();

            cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            cameraParams.name = "BreakFirst";
            cameraParams.data.wallCushion = 0.1f;
            cameraParams.data.idealLocalCameraPos = new Vector3(0f, -1.5f, -7f);

            music = Modules.Config.musicOn.Value;
            if (music)
            {
                sound = AkSoundEngine.PostEvent(2930662992, this.gameObject);
                convar = RoR2.Console.instance.FindConVar("volume_music");
                // set in game music volume to 0 so we hear the new music only.
                if (convar != null)
                {
                    oldMusic = convar.GetString();
                }
            }

            if (base.cameraTargetParams)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams.data,
                    priority = 1f
                }, 1.5f);
            }

        }

        private void DetermineCancel()
        {
            if (characterMotor)
            {
                if (!characterMotor.isGrounded) cancel = true;
            }
            if (inputBank)
            {
                if (inputBank.skill1.down) cancel = true;
                if (inputBank.skill2.down) cancel = true;
                if (inputBank.skill3.down) cancel = true;
                if (inputBank.skill4.down) cancel = true;
                if (inputBank.sprint.down) cancel = true;
                if (Input.GetKeyDown(Modules.Config.emote1Keybind.Value))
                {
                    cancel = true;
                }
                if (inputBank.jump.down)
                {
                    cancel = true;
                    jumped = true;
                }

            }
        }
        public override void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            this.characterBody.moveSpeed = 6.5f * 0.635f;
            cancel = false;
            jumped = false;
            DetermineCancel();
            base.FixedUpdate();

            if (music && Util.HasEffectiveAuthority(this.gameObject))
            {
                float decr = Mathf.Lerp(float.Parse(oldMusic, CultureInfo.InvariantCulture), 0f, stopwatch / animDuration);
                convar.SetString(decr.ToString());
            }
            else
            {
            }

            if (cancel)
            {
                outer.SetNextStateToMain();
                return;
            }

            if (rootmotion && !flag1)
            {
                Vector3 vector = rootmotion.ExtractRootMotion();
                if (isAuthority && characterMotor)
                {
                    characterMotor.rootMotion += vector;
                    characterMotor.moveDirection = Vector3.zero;
                    characterDirection.moveVector = Vector3.zero;
                }
            }
            else if (flag1)
            {
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;

            }

            if(isAuthority && stopwatch>= animDuration && !flag1)
            {
                flag1 = true;

            }

        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.hideCrosshair = false;

            if (music)
            {
                //AkSoundEngine.SetRTPCValue("Volume_MSX", currentMaster * actualMSX);
                convar.SetString(oldMusic);
                AkSoundEngine.StopPlayingID(sound);
            }

            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid)
            {
                cameraParamsOverrideHandle = base.cameraTargetParams.RemoveParamsOverride(cameraParamsOverrideHandle, 0.5f);
            }
            
            PlayAnimation("FullBody, Override", "BufferEmpty");

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
