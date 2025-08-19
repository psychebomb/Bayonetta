using RoR2;
using UnityEngine;
using BayoMod.Survivors.Bayo;
using RoR2.Projectile;
using RoR2.CameraModes;


namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class SmackEnd : StepEnd
    {

        public override void OnEnter()
        {
            duration = 1.48f;
            fireTime = 0.24f;
            animName = "SpankEnd";
            projectilePrefab = BayoAssets.fistDown;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Camera)
            {
                Vector3 targetAngles = cameraDir;
                targetAngles.y *= 15f;
                Vector3 newCamera = Vector3.Lerp(cameraDir, targetAngles, stopwatch/(fireTime + 0.24f));
                ((CameraModePlayerBasic.InstanceData)Camera.cameraMode.camToRawInstanceData[Camera]).SetPitchYawFromLookVector(newCamera);
            }
        }
        public override void FireProjectile()
        {
            /*
            //Vector3 dir = forwardDir;
            //dir.y = 0;
            //dir = Quaternion.AngleAxis(90f, Vector3.right) * dir;
            Vector3 dir = forwardDir;
            dir.y = 0f;
            Vector3 pos = characterBody.transform.position + (dir.normalized * 3f) + (cameraDir * 1.5f);
            //pos = pos + (.normalized * 2.5f);
            dir.y = -2f;
            pos.y = pos.y + 0.5f;
            */
            Vector3 dir = forwardDir;
            dir.y = 0;
            Vector3 pos = characterBody.transform.position + (dir.normalized * 2f); //+ (cameraDir * f);
            pos.y = pos.y - 1.5f;


            ProjectileManager.instance.FireProjectile(projectilePrefab, pos, Util.QuaternionSafeLookRotation(dir), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
        }
    }
}
