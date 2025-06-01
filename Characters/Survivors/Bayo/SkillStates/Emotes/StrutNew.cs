using EntityStates;
using RoR2;
using RoR2.ConVar;
using UnityEngine;
using static RoR2.BodyAnimatorSmoothingParameters;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.BaseStates
{
    public class StrutNew : BaseSkillState
    {
        public float animDuration = 1.92f;
        public float cancelDuration;
        private RootMotionAccumulator rootmotion;
        protected bool cancel;
        protected float stopwatch;

        protected bool jumped;
        protected bool flag1;
        private uint sound = 0;

        protected Animator animator;
        private CharacterAnimatorWalkParamCalculator animatorWalkParamCalculator;

        private CharacterCameraParams cameraParams;
        private CameraTargetParams.CameraParamsOverrideHandle cameraParamsOverrideHandle;
        private bool music;
        private bool zoom;
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
            bool client = Modules.Config.musicClient.Value;
            if (music)
            {
                if (client && isAuthority)
                {
                    sound = AkSoundEngine.PostEvent(2930662992, this.gameObject);
                }
                else if (!client)
                {
                    sound = AkSoundEngine.PostEvent(2930662992, this.gameObject);
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

            zoom = Modules.Config.eZoom.Value;
            if (base.cameraTargetParams && zoom)
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
            cancel = false;
            jumped = false;
            DetermineCancel();
            base.FixedUpdate();

            /*
            if (music && Util.HasEffectiveAuthority(this.gameObject))
            {
                float decr = Mathf.Lerp(float.Parse(oldMusic, CultureInfo.InvariantCulture), 0f, stopwatch / animDuration);
                convar.SetString(decr.ToString());
            }
            */

            if (jumped)
            {
                inputBank.jump.PushState(false);
            }

            if (cancel)
            {
                outer.SetNextStateToMain();
                return;
            }

            this.characterBody.moveSpeed = 6.5f * 0.635f;

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
            else if (flag1 && isAuthority)
            {

                if(inputBank.moveVector == Vector3.zero)
                {
                    inputBank.moveVector = characterDirection.forward;
                }

                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.moveDirection;

            }

            if (stopwatch >= animDuration && !flag1)
            {
                flag1 = true;
                animator.SetFloat(AnimationParameters.walkSpeed, base.characterBody.moveSpeed);
                PlayCrossfade("Body", "Walk", 0.5f);
            }

        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnExit()
        {
            base.OnExit();
            characterBody.hideCrosshair = false;

            if (music)
            {
                if (convar != null)
                {
                    if (oldMusic != "0") convar.SetString(oldMusic);
                }
                if (sound != 0)
                {
                    AkSoundEngine.StopPlayingID(sound);
                }
            }

            if (base.cameraTargetParams && cameraParamsOverrideHandle.isValid && zoom)
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
