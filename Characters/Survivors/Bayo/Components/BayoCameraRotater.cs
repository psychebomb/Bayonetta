

using BayoMod.Characters.Survivors.Bayo.Components.Demon;
using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.Components
{
    public class BayoCameraRotater : MonoBehaviour
    {
        private BayoCameraController camController;
        public float rotateAmount = -20f;
        public float lerpTime = 0.75f;
        public Vector3 direction = Vector3.up;
        public bool rotateCam = true;

        private float stopwatch = 0f;
        private Quaternion startRotation;
        private Quaternion endRotation;
        private GameObject camObj;
        private bool deleteAfterRotation = false;
        public void Start()
        {
            camController = GetComponent<BayoCameraController>();
            camObj = camController.camObject;
            startRotation = camObj.transform.localRotation;
            endRotation = startRotation * Quaternion.AngleAxis(rotateAmount, direction);
        }

        public void FlipRotations(float newLerp)
        {
            Quaternion temp = startRotation;
            startRotation = endRotation;
            endRotation = temp;
            lerpTime = newLerp;
            deleteAfterRotation = true;
            rotateCam = true;
        }
        public void Update()
        {
            if (rotateCam)
            {
                stopwatch += Time.deltaTime;
                camObj.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, stopwatch / lerpTime);
                if (stopwatch >= lerpTime)
                {
                    stopwatch = 0f;
                    rotateCam = false;

                    if (deleteAfterRotation)
                    {
                        GameObject.Destroy(this);
                    }
                }
            }
        }
    }
}
