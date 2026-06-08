using Newtonsoft.Json.Bson;
using RoR2;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class BayoSpinController : MonoBehaviour
    {
        private float stopwatch = 0f;
        public float spinSpeed = 0.6f;
        private Transform boneTrans;
        private Quaternion baseRotation;
        private float rotateAngle = 0f;

        public CharacterBody bayoBody;
        public bool spin2 = false;
        private bool changedSpin = false;
        private float startRot = 0f;
        private float endRot = 360f;
        private float spinSpeed2 = 0.5f;
        private float rotateAngle2 = 0f;
        private float stopwatch2 = 0f;
        private void Start()
        {
            ModelLocator component = this.gameObject.GetComponent<ModelLocator>();
            /*
            ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2)
            {
                int childIndex = component2.FindChildIndex("Base");
                boneTrans = component2.FindChild(childIndex);
            }
            */
            Transform component2 = component.modelTransform;
            foreach (Transform child in component2)
            {
                if (child.name.Contains("Arm"))
                {
                    boneTrans = child.Find("ROOT").Find("base");
                    break;
                }
            }
            if (boneTrans == null) boneTrans = component.modelTransform;
            baseRotation = boneTrans.rotation;
        }

        public void Update()
        {
            if (spin2 && !changedSpin)
            {
                changedSpin = true;
                UpdateSpin();
            }
        }

        private void UpdateSpin()
        {
            spinSpeed = 2f;
            startRot = 90f;
            endRot = 450f;
        }
        private void LateUpdate()
        {
            stopwatch += Time.deltaTime;
            if(stopwatch >= spinSpeed)
            {
                stopwatch = 0f;
            }

            rotateAngle = Mathf.Lerp(startRot, endRot, stopwatch / spinSpeed);
            boneTrans.localRotation = baseRotation * Quaternion.AngleAxis(rotateAngle, Vector3.right);
            if (spin2)
            {
                stopwatch2 += Time.deltaTime;
                if (stopwatch2 >= spinSpeed2)
                {
                    stopwatch2 = 0f;
                }
                rotateAngle2 = Mathf.Lerp(0, -360, stopwatch2 / spinSpeed2);
                boneTrans.localRotation *= Quaternion.AngleAxis(rotateAngle2, Vector3.up);
            }
            //boneTrans.Rotate(direction * rotateAngle);
        }

        public void OnDestroy()
        {
            if(boneTrans != null)
            {
                boneTrans.rotation = baseRotation;
            }
        }
    }
}
