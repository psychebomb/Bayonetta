
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class DemonController : MonoBehaviour
    {
        private float stopwatch;
        public float loopStart = 6.56f;
        public float loopDur = 3f;
        public float endDur = 7.92f;
        public float initalD = 3.5f;

        //final chomp at 12.28f
        //0.54f of extra transition time?

        private bool looping;
        private bool finishing;

        private Animator animator;
        private DemonVisualController dvc;

        void Start()
        {
            animator = GetComponent<Animator>();
            dvc = GetComponent<DemonVisualController>();
            PlayAnim("Spawn", 0f);
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
        }

        public void OnBite()
        {
            Debug.Log("CHOMP!!!!");
        }

        public void FinishingBlow()
        {
            Debug.Log("DIE!!!!");
        }

        public void ReleasePrey()
        {
            Debug.Log("ptooie");
            //if (enemyBody) enemyBody.GetComponent<GrabStun>().Release();
        }

        void Update()
        {
            stopwatch += Time.deltaTime;

            if (stopwatch >= loopStart && stopwatch <= loopStart + loopDur)
            {
                float loopSpeed = Mathf.SmoothStep(1f, 2f, (stopwatch - loopStart) / loopDur);

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
    }
}