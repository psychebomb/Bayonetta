using EntityStates;
using UnityEngine;
using RoR2;
using BayoMod.Characters.Survivors.Bayo.Components;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.SkillStates.ClimaxStates
{
    public class GrabStun : MonoBehaviour
    {
        private Transform modelTrans;
        private ModelLocator modelLocator;
        private CapsuleCollider capsuleCollider;
        private SphereCollider sphereCollider;
        private Quaternion originalRotation;
        private Vector3 originalPosition;
        private Vector3 moveDir;
        private bool debugFlag = false;

        private float middle;
        private Vector3 pos;
        private CharacterBody body;
        private CharacterDirection direction;
        private CharacterMotor motor;
        private Rigidbody rigidBody;
        private bool released = false;
        private float stopwatch = 0f;
        private Quaternion newRot;

        public Transform pivot;

        private void Awake()
        {
            this.modelLocator = this.GetComponent<ModelLocator>();
            this.body = this.GetComponent<CharacterBody>();
            this.rigidBody = this.GetComponent<Rigidbody>();
            this.motor = this.GetComponent<CharacterMotor>();
            this.direction = this.GetComponent<CharacterDirection>();
            this.capsuleCollider = this.GetComponent<CapsuleCollider>();
            this.sphereCollider = this.GetComponent<SphereCollider>();

            if (this.direction) this.direction.enabled = false;
            
            if (this.capsuleCollider)
            {
                if(body && body.name.Contains("BeetleQueen")){
                    middle = this.capsuleCollider.bounds.extents.x;
                }
                else
                {
                    middle = this.capsuleCollider.bounds.extents.y;
                }
                this.capsuleCollider.enabled = false;
            }

            if (this.sphereCollider) this.sphereCollider.enabled = false;

            if (modelLocator.modelTransform)
            {
                modelTrans = modelLocator.modelTransform;
                originalRotation = modelTrans.rotation;
            }

            if (this.body) 
            {
                originalPosition = this.body.footPosition;
            }

            if (modelLocator) this.modelLocator.enabled = false;

        }
        private void FixedUpdate()
        {
            if (!released)
            {
                if (this.motor)
                {
                    this.motor.disableAirControlUntilCollision = true;
                    this.motor.velocity = Vector3.zero;
                    this.motor.rootMotion = Vector3.zero;

                    this.motor.Motor.SetPosition(this.pivot.position, true);
                }

                if (this.pivot)
                {
                    this.transform.position = this.pivot.position;
                    moveDir = this.pivot.transform.TransformDirection(new Vector3(0, -1, 0));
                }
                else
                {
                    this.Release();
                }

                if (this.modelTrans)
                {
                    this.modelTrans.position = this.pivot.position + (moveDir * middle);
                    this.modelTrans.rotation = this.pivot.rotation;
                }
            }
            /*
            else
            {
                stopwatch += Time.deltaTime;
                if (this.modelTrans)
                {
                    this.modelTrans.rotation = originalRotation;
                    this.modelTrans.position = Vector3.Lerp(this.pivot.position, originalPosition, stopwatch / 0.5f);
                }
                if (this.motor)
                {
                    this.motor.Motor.SetPosition(Vector3.Lerp(this.pivot.position, originalPosition, stopwatch / 0.5f), true);
                }
            }
            */
        }
        public void Release()
        {
            released = true;
            if (this.modelLocator) this.modelLocator.enabled = true;
            if (this.modelTrans)
            {
                newRot = this.modelTrans.rotation;
                if (this.direction) this.direction.enabled = true;
                if (this.capsuleCollider) this.capsuleCollider.enabled = true;
                if (this.sphereCollider) this.sphereCollider.enabled = true;
                //Destroy(this);
            }
        }
    }
}
