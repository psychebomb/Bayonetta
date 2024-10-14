using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using BayoMod.Modules.Components;
using EntityStates.Seeker;
using System.Diagnostics;
using BayoMod.Survivors.Bayo.SkillStates;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.Weave
{
    public class WeaveEntry : BaseSkillState
    {
        public static SkillDef tetsuRealDef = BayoMod.Survivors.Bayo.BayoSurvivor.tetsuSkillDef;
        public static SkillDef stompRealDef = BayoMod.Survivors.Bayo.BayoSurvivor.stompSkillDef;
        public static SkillDef cancelDef = BayoMod.Survivors.Bayo.BayoSurvivor.weaveCancelSkillDef;

        public CameraTargetParams.AimRequest aimRequest;

        private BayoTracker tracker;

        private float stopwatch;

        private int secondStockMax;
        private int specialStockMax;
        private int secondStocks;
        private int specialStocks;
        private bool fired = false;
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.gameObject.AddComponent<BayoTracker>();
            this.tracker = base.GetComponent<BayoTracker>();

            secondStockMax = base.skillLocator.secondary.maxStock;
            specialStockMax = base.skillLocator.special.maxStock;
            secondStocks = base.skillLocator.secondary.stock;
            specialStocks = base.skillLocator.special.stock + 1;

            base.skillLocator.primary.SetSkillOverride(base.skillLocator.primary, WeaveEntry.tetsuRealDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.secondary.SetSkillOverride(base.skillLocator.secondary, WeaveEntry.stompRealDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.special.SetSkillOverride(base.skillLocator.special, WeaveEntry.cancelDef, GenericSkill.SkillOverridePriority.Contextual);

            if (base.cameraTargetParams) aimRequest = base.cameraTargetParams.RequestAimType((CameraTargetParams.AimType)UnseenHand.abilityAimType);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.SetAimTimer(3f);
            stopwatch += GetDeltaTime();
            if (base.isAuthority)
            {
                AuthorityFixedUpdate();
            }
        }

        private void AuthorityFixedUpdate()
        {
            if (base.inputBank.skill1.justReleased && this.tracker.GetTrackingTarget())
            {
                outer.SetNextStateToMain();
                fired = true;
                return;
            }
            if (base.inputBank.skill2.justReleased && this.tracker.GetTrackingTarget())
            {
                outer.SetNextStateToMain();
                fired = true;
                return;
            }
            if (base.inputBank.skill3.justReleased)
            {
                outer.SetNextStateToMain();
                if (this.tracker) Destroy(this.tracker);
                return;
            }
            if (base.inputBank.skill4.justPressed && !base.inputBank.skill4.wasDown &&stopwatch >= 0.1f)
            {
                outer.SetNextStateToMain();
                if (this.tracker) Destroy(this.tracker);
                return;
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            DoEnd();

        }

        private void DoEnd()
        {
            aimRequest?.Dispose();
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, WeaveEntry.tetsuRealDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.secondary.UnsetSkillOverride(base.skillLocator.secondary, WeaveEntry.stompRealDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.special.UnsetSkillOverride(base.skillLocator.special, WeaveEntry.cancelDef, GenericSkill.SkillOverridePriority.Contextual);

            base.skillLocator.secondary.DeductStock(secondStockMax - secondStocks);
            base.skillLocator.special.DeductStock(specialStockMax - specialStocks);
            if(fired) base.skillLocator.special.DeductStock(1);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
