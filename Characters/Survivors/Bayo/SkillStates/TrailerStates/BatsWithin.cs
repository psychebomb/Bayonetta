using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using BayoMod.Survivors.Bayo;
using BayoMod.Survivors.Bayo.SkillStates;
using EntityStates;
using RoR2;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.TrailerStates
{
    public class BatsWithin : BaseSkillState
    {
        private CameraController cam;
        private bool startedAnim = false;
        public override void OnEnter()
        {
            base.OnEnter();
            characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (base.rigidbody && !base.rigidbody.isKinematic)
            {
                base.rigidbody.velocity = Vector3.zero;
                if (base.rigidbodyMotor)
                {
                    base.rigidbodyMotor.moveVector = Vector3.zero;
                }
            }

            if (characterBody.characterMotor) characterMotor.velocity = Vector3.zero;

            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            this.characterBody.isSprinting = false;

            cam = this.gameObject.GetComponent<CameraController>();
            PlayAnimation("Body", "DodgeBack");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.characterDirection.forward;
            }

            if (characterBody.characterMotor)
            {
                characterMotor.velocity.x = 0f;
                characterMotor.velocity.z = 0f;
            }

            if (characterBody && Physics.Raycast(characterBody.footPosition, Vector3.down, 7f, LayerIndex.world.mask) && !startedAnim && cam)
            {
                //start the next camera right before bayo touches the ground
                cam.useCamObj = false;
                cam.useFeetAnim = false;
                cam.UnsetCam();
            }

            if (isAuthority && characterMotor && characterMotor.Motor.GroundingStatus.IsStableOnGround)
            {
                outer.SetNextState(new Land());
                return;
            }
        }
    }
}
