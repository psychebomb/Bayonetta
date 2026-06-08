
using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates;
using BayoMod.Survivors.Bayo;
using EntityStates.BeetleGuardMonster;
using EntityStates.ClayBoss;
using RoR2;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class EnemySpin : EnemyFreeze
    {
        private BayoSpinController spinner;
        private CharacterBody bayoBody;
        private bool detonateNextFrame = false;
        private Quaternion effectRotation;

        protected float blastDamage = 6f;
        protected float blastRadius = 3f;
        private float minSpinDur = 0.2f;
        private GameObject impactVFX = GroundSlam.slamEffectPrefab;
        public override void OnEnter()
        {
            freeze = false;
            spinner = this.gameObject.GetComponent<BayoSpinController>();

            if (base.isAuthority)
            {
                base.characterMotor.onMovementHit += OnMovementHit;
            }

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!spinner)
            {
                spinner = this.gameObject.GetComponent<BayoSpinController>();
            }

            if (!bayoBody)
            {
                this.bayoBody = spinner.bayoBody;
            }

            if(detonateNextFrame || (base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround && stopwatch >= minSpinDur))
            {
                DetonateAuthority();
                EffectManager.SimpleEffect(impactVFX, base.transform.position, effectRotation, transmit: true);
                if (spinner.spin2)
                {
                    Util.PlaySound("Play_lemurianBruiser_m1_explode", base.gameObject);
                }
                outer.SetNextStateToMain();
                return;
            }
        }
        private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            detonateNextFrame = true;
            effectRotation = Quaternion.LookRotation(movementHitInfo.hitNormal);
            if (spinner.spin2)
            {
                impactVFX = BayoAssets.bigExplode;
                blastDamage = 10f;
                blastRadius = 5f;
            }
        }
        public override void OnExit()
        {
            if(spinner) GameObject.Destroy(spinner);
            base.OnExit();
        }

        protected BlastAttack.Result DetonateAuthority()
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.attacker = bayoBody.gameObject;
            blastAttack.baseDamage = bayoBody.damage * blastDamage;
            blastAttack.baseForce = 500f;
            blastAttack.bonusForce = Vector3.zero;
            blastAttack.crit = RollCrit();
            blastAttack.damageType = DamageType.Stun1s;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.procCoefficient = 1f;
            blastAttack.radius = blastRadius;
            blastAttack.position = base.characterBody.corePosition;
            blastAttack.attackerFiltering = AttackerFiltering.AlwaysHitSelf;
            //blastAttack.impactEffect = EffectCatalog.FindEffectIndexFromPrefab(gunEffectPrefab);
            blastAttack.teamIndex = bayoBody.teamComponent.teamIndex;
            blastAttack.damageType = DamageTypeCombo.GenericPrimary;
            return blastAttack.Fire();
        }
    }
}
