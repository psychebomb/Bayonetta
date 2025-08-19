using UnityEngine;
using RoR2;
using BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates;
using RoR2.ConVar;
using UnityEngine.Networking;
using EntityStates.ImpBossMonster;
using System.ComponentModel;

namespace BayoMod.Characters.Survivors.Bayo.Components.Demon
{
    public class DemonController : MonoBehaviour
    {
        private float stopwatch;
        public float loopStart = 6.56f;
        public float loopDur = 3.5f;
        public float endDur = 7.92f;
        public float initalD = 4f;
        public string oldMusic = "0";
        public CharacterBody enemyBody;

        private bool looping;
        private bool finishing;
        private DamageInfo damageInfo;
        private float biteDam;

        private Animator animator;
        private DemonVisualController dvc;
        private bool releaseLater = false;
        private uint sound;
        private GameObject hitEffect = GroundPound.hitEffectPrefab;
        private GameObject mouth1;
        private GameObject mouth2;
        private ShakeEmitter biteShake;

        void Start()
        {
            animator = GetComponent<Animator>();
            dvc = GetComponent<DemonVisualController>();
            PlayAnim("Spawn", 0f);
            ChildLocator component = gameObject.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                int childIndex = component.FindChildIndex("mouth");
                Transform transformm = component.FindChild(childIndex);
                mouth1 = transformm.gameObject;
                childIndex = component.FindChildIndex("mout2");
                transformm = component.FindChild(childIndex);
                mouth2 = transformm.gameObject;
                biteShake = mouth2.GetComponent<ShakeEmitter>();
            }

            Util.PlaySound("gomsum", mouth1);

            if (enemyBody)
            {
                if (enemyBody.healthComponent) biteDam = enemyBody.healthComponent.combinedHealth / 20f;
                if (enemyBody.name.Contains("Titan")) releaseLater = true;
            }
        }

        private void PlayAnim(string animName, float trans)
        {
            animator.speed = 1f;
            animator.Update(0f);
            int layerIndex = animator.GetLayerIndex("Base");

            if (layerIndex >= 0)
            {
                if (trans > 0f)
                {
                    animator.CrossFadeInFixedTime(animName, trans, layerIndex);
                }
                else
                {
                    animator.PlayInFixedTime(animName, layerIndex, 0f);
                }
            }
        }

        private void PlayAnim(string animName, string param, float trans)
        {
            animator.speed = 1f;
            animator.Update(0f);
            int layerIndex = animator.GetLayerIndex("Base");

            if (layerIndex >= 0)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    animator.SetFloat(param, 1f);
                }
                if (trans > 0f)
                {
                    animator.CrossFadeInFixedTime(animName, trans, layerIndex);
                }
                else
                {
                    animator.PlayInFixedTime(animName, layerIndex, 0f);
                }
            }

        }

        public void GrabEnemey()
        {
            Debug.Log("SNATCH!!!!");

            GrabStun gs = enemyBody.gameObject.AddComponent<GrabStun>();
            if (enemyBody && enemyBody.name.Contains("BeetleQueen"))
            {
                gs.pivot = mouth2.transform;
            }
            else
            {
                gs.pivot = mouth1.transform;
            }
            sound = AkSoundEngine.PostEvent(2187064797, mouth1);
            //biteShake.enabled = false;
            //biteShake.enabled = true;

            if (enemyBody)
            {
                TakeBite();
            }
        }

        public void OnBite()
        {
            Debug.Log("CHOMP!!!!");
            if (enemyBody) TakeBite();
        }

        public void FinishingBlow()
        {
            Debug.Log("DIE!!!!");
            if (enemyBody && !releaseLater) enemyBody.GetComponent<GrabStun>().Release();
            AkSoundEngine.StopPlayingID(sound);
            if (enemyBody) KillBoss();
        }

        public void ReleasePrey()
        {
            if (releaseLater && enemyBody) enemyBody.GetComponent<GrabStun>().Release();
        }

        void Update()
        {
            stopwatch += Time.deltaTime;

            if (stopwatch >= loopStart && stopwatch <= loopStart + loopDur)
            {
                float loopSpeed = Mathf.Lerp(1f, 2f, (stopwatch - loopStart) / loopDur);

                animator.SetFloat("BiteSpeed", loopSpeed);
            }

            if (stopwatch > loopStart + loopDur)
            {
                if (!finishing)
                {
                    finishing = true;
                    animator.SetBool("loopDone", true);
                    dvc.reverse = true;
                    dvc.initialDelay = initalD;
                }
            }
        }

        private void TakeBite()
        {
            Util.PlaySound("bite", mouth1);
            EffectManager.SimpleMuzzleFlash(hitEffect, gameObject, "mouth3", false);

            damageInfo = new DamageInfo();
            damageInfo.damageType = DamageType.BypassArmor;
            damageInfo.damage = biteDam;
            damageInfo.position = enemyBody.transform.position;

            enemyBody.healthComponent.TakeDamage(damageInfo);
        }

        private void KillBoss()
        {
            Util.PlaySound("gomend", mouth1);
            EffectManager.SimpleMuzzleFlash(hitEffect, gameObject, "mouth3", false);
            biteShake.enabled = false;
            biteShake.enabled = true;

            damageInfo = new DamageInfo();
            damageInfo.damageType = DamageType.BypassArmor;
            damageInfo.damage = enemyBody.healthComponent.fullCombinedHealth * 2f;
            damageInfo.position = enemyBody.transform.position;

            enemyBody.healthComponent.TakeDamage(damageInfo);
        }
    }
}