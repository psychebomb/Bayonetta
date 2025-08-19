using BayoMod.Survivors.Bayo;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using RoR2.CameraModes;
using RoR2.ConVar;
using BayoMod.Characters.Survivors.Bayo.Components;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates
{
    public class EnemyFreeze : BaseSkillState
    {
        internal float duration;
        internal float previousAttackSpeedStat;
        private Animator modelAnimator;

        public bool gomorrah = true;
        private float stopwatch;
        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                //this.modelAnimator.enabled = false;
            }
            if (rigidbody && !rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                if (rigidbodyMotor)
                {
                    rigidbodyMotor.moveVector = Vector3.zero;
                }
            }

            if (characterBody.characterMotor) characterMotor.velocity = Vector3.zero;

            if (characterDirection)
            {
                characterDirection.moveVector = characterDirection.forward;
            }

            characterBody.isSprinting = false;

        }
        public override void OnExit()
        {
            if (modelAnimator)
            {
                //this.modelAnimator.enabled = true;
            }
            CharacterModel model = GetModelTransform().GetComponent<CharacterModel>();
            if (model)
            {
                model.forceUpdate = true;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.deltaTime;

            attackSpeedStat = 0f;

            if (gomorrah)
            {
                GomorrahThings();
            }

            if (characterDirection)
            {
                characterDirection.moveVector = characterDirection.forward;
            }

            if (characterBody.characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                if (!characterMotor.isGrounded) characterMotor.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y;
            }

            if (isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void GomorrahThings()
        {
            if (characterBody.inputBank && isAuthority)
            {
                if (stopwatch < 4.4f)
                {
                    Vector3 lookDir = characterDirection.forward;
                    lookDir.y = -0.5f;
                    characterBody.inputBank.aimDirection = lookDir;
                }
                else if (stopwatch >= 4.4f && stopwatch < 5.9f)
                {
                    Vector3 lookDir = characterDirection.forward;
                    lookDir.y = -0.5f;
                    Vector3 lookDir2 = characterDirection.forward;
                    Vector3 rotateAngle = Vector3.Lerp(lookDir, lookDir2, (stopwatch - 4.4f) / (4.9f - 4.4f));
                    characterBody.inputBank.aimDirection = rotateAngle;
                }
                else if (stopwatch >= 5.9f && stopwatch <= 8.9f)
                {
                    Vector3 lookDir = characterDirection.forward;
                    Vector3 lookDir2 = characterDirection.forward;
                    Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
                    lookDir2 = rotation * lookDir2;
                    Vector3 rotateAngle = Vector3.Lerp(lookDir, lookDir2, (stopwatch - 5.9f) / (8.4f - 5.9f));
                    characterBody.inputBank.aimDirection = rotateAngle;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            duration = reader.ReadSingle();
        }
    }
}