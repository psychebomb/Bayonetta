using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RoR2.EntityLogic;

namespace BayoMod.Modules.Components
{
    public class WickedWeave : MonoBehaviour
    {
        public float startTime = 0.32f;
        //public float startTiling = 1f;
        public float hitboxEnd = 1.12f;
        private Timer timer;
        private ProjectileOverlapAttack poa;
        private ShakeEmitter shakeEmitter;
        //private int id = 0;

        void Start()
        {
            poa = GetComponent<ProjectileOverlapAttack>();
            poa.enabled = false;
            timer = GetComponent<Timer>();
            shakeEmitter = GetComponent<ShakeEmitter>();
            shakeEmitter.enabled = false;
            //id = mat.GetTexturePropertyNameIDs()[0];
        }

        // Update is called once per frame
        void Update()
        {
            if (timer.stopwatch >= startTime && timer.stopwatch <= hitboxEnd)
            {
                poa.enabled = true;
                shakeEmitter.enabled = true;
            }

            if (timer.stopwatch > hitboxEnd)
            {
                poa.enabled = false;
            }
        }
    }
}
