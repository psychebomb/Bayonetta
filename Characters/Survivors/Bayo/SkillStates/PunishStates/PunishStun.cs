using EntityStates;
using UnityEngine;
using RoR2;
using BayoMod.Modules.Components;
using UnityEngine.AI;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.PunishStates
{
    public class PunishStun : StunState
    {
        private Transform modelTrans;
        private Quaternion originalRotation;
        private bool debugFlag = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (modelLocator.modelTransform)
            {
                modelTrans = modelLocator.modelTransform;
                originalRotation = modelTrans.rotation;

                if (modelLocator.gameObject.name == "GreaterWispBody(Clone)")
                {
                    modelLocator.dontReleaseModelOnDeath = true;
                    modelLocator.dontDetatchFromParent = true;
                }
                modelLocator.enabled = false;
            }
            if (modelTrans)
            {
                modelTrans.rotation *= Quaternion.AngleAxis(90f, Vector3.right);
            }

        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelTrans && this.GetComponent<CapsuleCollider>() != null)
            {
                Vector3 pos2 = this.GetComponent<CapsuleCollider>().bounds.center;
                float y = this.GetComponent<CapsuleCollider>().bounds.extents.y;
                if (y <= 1)
                {
                    pos2.y -= (y * 0.7f);
                }
                else
                {
                    pos2.y -= y;
                }
                //pos2.z -= y;
                
                this.modelTrans.position = pos2;

                if (debugFlag)
                {
                    debugFlag = false;
                    Chat.AddMessage("x (cur): " + this.GetComponent<CapsuleCollider>().bounds.size.x);
                    Chat.AddMessage("y: " + this.GetComponent<CapsuleCollider>().bounds.size.y);
                    Chat.AddMessage("z: " + this.GetComponent<CapsuleCollider>().bounds.size.z);
                }

            }
        }

        public override void OnExit()
        {
            if (modelLocator) modelLocator.enabled = true;
            if (modelTrans) modelTrans.rotation = originalRotation;
            base.OnExit();
        }
    }
}
