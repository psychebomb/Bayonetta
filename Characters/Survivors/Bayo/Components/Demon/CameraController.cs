using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using static RoR2.CameraTargetParams;
using System;
using UnityEngine.UIElements;
using BayoMod.Survivors.Bayo;

namespace BayoMod.Characters.Survivors.Bayo.Components.Demon
{
    public class CameraController : MonoBehaviour
    {
        public CharacterBody body;
        public CharacterDirection direction;
        public Transform camTrans;
        public Transform trackTrans;
        public Transform demonCamTrans;
        public Transform demonTrackTrans;
        private CameraTargetParams camParams;
        private CameraRigController cameraRig;
        private UIController ui;
        private bool baseAIPresent = false;

        public GameObject camObject;
        public Transform previousParent;
        public CameraParamsOverrideHandle handle;

        public float smoothDampTime = 0.2f;
        public float maxSmoothDampSpeed = 50f;
        public float fov = 50f;
        public Vector3 smoothDampVelocity;

        public bool shouldRotateCamera;
        public float stopwatch;
        public Quaternion startRotation;
        public static float lerpTime = 0f;
        public float demonStopwatch;
        public float demonTime = 0f;
        private bool inDemonCam = false;
        private bool inCam = false;

        public bool useCamObj = false;
        public bool useFeetAnim = false;
        public GameObject camParentObj;

        //debug stuff
        public bool activatePan = false;
        private bool inDebug = false;
        public Quaternion baseRotation;
        public Vector3 debugPosition = Vector3.zero;
        public float panAmount = 60f;
        public float panDuration = 1f;
        public bool up = false;
        public bool down = false;
        public bool left = false;
        public bool right = false;
        private float debugStopwatch = 0f;
        public void Awake()
        {

        }
        public void Start()
        {
            body = gameObject.GetComponent<CharacterBody>();
            direction = gameObject.GetComponent<CharacterDirection>();
            camParams = GetComponent<CameraTargetParams>();
            ui = GetComponent<UIController>();
            cameraRig = body.master.playerCharacterMasterController.networkUser.cameraRigController;

            ModelLocator component = gameObject.GetComponent<ModelLocator>();
            ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
            if ((bool)component2 && !useCamObj)
            {
                int childIndex = component2.FindChildIndex("cambone");
                Transform transformm = component2.FindChild(childIndex);
                camTrans = transformm;
                childIndex = component2.FindChildIndex("trackbone");
                transformm = component2.FindChild(childIndex);
                trackTrans = transformm;
            }

            CharacterMaster master = body.master;
            BaseAI baseAI = master.GetComponent<BaseAI>();
            baseAIPresent = baseAI;

            if (!master) baseAIPresent = true; // Disable UI Just in case.

            try
            {
                camObject = Camera.main.gameObject;
                previousParent = camObject.transform.parent;
            }
            catch (NullReferenceException e)
            {
                Debug.Log($"Should be alright: {e}");
            }

            baseRotation = camObject.transform.rotation;
        }

        public void SetCam()
        {
            if (baseAIPresent)
            {
                return;
            }

            smoothDampTime = 0.001f;
            maxSmoothDampSpeed = 9999999f;

            if (useCamObj)
            {
                ModelLocator component = gameObject.GetComponent<ModelLocator>();
                ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                if (component2)
                {
                    Transform transform = component2.FindChild("rootTransform") ?? body.coreTransform;
                    if (transform)
                    {
                        if (useFeetAnim)
                        {
                            /*
                            Vector3 lowerPos = transform.position;
                            lowerPos.y -= 2;
                            Vector3 lookDir = direction.forward;
                            lookDir.y = 0f;
                            */
                            Transform lowerPos = transform;
                            Vector3 lowerVec = transform.position;
                            lowerVec.y -= 3;
                            lowerPos.set_position_Injected(ref lowerVec);
                            camParentObj = UnityEngine.Object.Instantiate(BayoAssets.camObj, lowerPos.position, lowerPos.rotation);
                            camTrans = camParentObj.transform.Find("camParent").gameObject.transform;

                            PlayAnim(camParentObj.transform.Find("camParent").gameObject.GetComponent<Animator>(), "feetpan");
                        }
                        else
                        {
                            Vector3 newPos = transform.position;
                            newPos.y += 8;
                            camParentObj = UnityEngine.Object.Instantiate(BayoAssets.camObj, newPos, transform.rotation);
                            camTrans = camParentObj.transform.Find("camParent").gameObject.transform;
                        }
                    }
                }
            }
            else
            {
                ModelLocator component = gameObject.GetComponent<ModelLocator>();
                ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
                if ((bool)component2)
                {
                    int childIndex = component2.FindChildIndex("cambone");
                    Transform transformm = component2.FindChild(childIndex);
                    camTrans = transformm;
                }
            }

            if (camTrans != null) camObject.transform.SetParent(camTrans, true);
            if(useCamObj) camObject.transform.localRotation = Quaternion.identity;
            shouldRotateCamera = false;
            inCam = true;

            CharacterCameraParamsData cameraParamsData = camParams.currentCameraParamsData;
            cameraParamsData.fov = fov;
            //cameraParamsData.wallCushion = 0f;

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = cameraParamsData,
                priority = 0,
            };

            ui.SetRORUIActiveState(false);

            
            handle = camParams.AddParamsOverride(request, 0.05f);

        }

        public void UnsetCam()
        {
            if (baseAIPresent)
            {
                return;
            }

            smoothDampTime = 0.5f;
            maxSmoothDampSpeed = 80f;
            shouldRotateCamera = true;
            inCam = false;
            startRotation = camObject.transform.localRotation;
            //Camera.main.nearClipPlane = oldClip;
            //cameraRig.fadeEndDistance = oldClip;

            camObject.transform.SetParent(previousParent, true);
            camParams.RemoveParamsOverride(handle, 1f);

            foreach (CharacterModel instances in InstanceTracker.GetInstancesList<CharacterModel>())
            {

                if (instances.visibility != VisibilityLevel.Invisible)
                {
                    for (int i = 0; i < instances.baseRendererInfos.Length; i++)
                    {
                        instances.baseRendererInfos[i].renderer.enabled = true;
                    }
                }
            }

            if(camParentObj)UnityEngine.Object.Destroy(camParentObj);

            ui.SetRORUIActiveState(true);
        }

        public void DemonHandoff()
        {
            if (baseAIPresent)
            {
                return;
            }

            camObject.transform.SetParent(demonCamTrans, true);
            inDemonCam = true;
            //oldClip = Camera.main.nearClipPlane;
            //Camera.main.nearClipPlane = 4f;
            //oldClip = cameraRig.fadeEndDistance;
            //cameraRig.fadeEndDistance = 20f;
        }

        public void Update()
        {
            if (!camObject)
            {
                camObject = Camera.main.gameObject;
                previousParent = camObject.transform.parent;
                cameraRig = camObject.GetComponent<CameraRigController>();
            }

            if (camObject && !inDebug)
            {
                camObject.transform.localPosition = Vector3.SmoothDamp(camObject.transform.localPosition, Vector3.zero, ref smoothDampVelocity, smoothDampTime, maxSmoothDampSpeed, Time.deltaTime);
            }

            if (inDemonCam)
            {
                demonStopwatch += Time.deltaTime;
                camObject.transform.LookAt(demonTrackTrans.position);

                foreach (CharacterModel instances in InstanceTracker.GetInstancesList<CharacterModel>())
                {
                    float nearestHurtBoxDistance = instances.GetNearestHurtBoxDistance(camObject.transform.position);
                    bool proximityCheck = (nearestHurtBoxDistance < 5f);

                    if(instances.visibility != VisibilityLevel.Invisible)
                    {
                        if (proximityCheck)
                        {
                            for (int i = 0; i < instances.baseRendererInfos.Length; i++)
                            {
                                instances.baseRendererInfos[i].renderer.enabled = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < instances.baseRendererInfos.Length; i++)
                            {
                                instances.baseRendererInfos[i].renderer.enabled = true;
                            }
                        }
                    }
                }

                if (demonStopwatch >= demonTime)
                {
                    demonStopwatch = 0;
                    demonTime = 0;
                    inDemonCam = false;
                    inCam = false;
                    UnsetCam();
                }
            }
            else if (inCam && !useCamObj && !inDebug && trackTrans)
            {
                camObject.transform.LookAt(trackTrans.position);
            }
            if (shouldRotateCamera)
            {
                stopwatch += Time.deltaTime;
                camObject.transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.identity, stopwatch / lerpTime);
                //Chat.AddMessage($"{stopwatch / lerpTime} {cameraObject.transform.localRotation}");
                if (stopwatch >= lerpTime)
                {
                    stopwatch = 0f;
                    shouldRotateCamera = false;
                }
            }


            if(activatePan && !inDebug)
            {
                inDebug = true;
                SetCam();
            }
            if (inDebug)
            {
                debugStopwatch += Time.deltaTime;
                if(debugStopwatch >= panDuration)
                {
                    debugStopwatch = 0f;
                }

                camObject.transform.localPosition = debugPosition;
                Quaternion panResult = baseRotation;

                if (up)
                {
                    float halfRot = panAmount / 2f;
                    Quaternion lowerQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.right);
                    Quaternion upperQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.left);
                    panResult = Quaternion.Slerp(lowerQuat, upperQuat, debugStopwatch / panDuration);
                }
                if (down)
                {
                    float halfRot = panAmount / 2f;
                    Quaternion lowerQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.left);
                    Quaternion upperQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.right);
                    panResult = Quaternion.Slerp(lowerQuat, upperQuat, debugStopwatch / panDuration);
                }
                if (left)
                {
                    float halfRot = panAmount / 2f;
                    Quaternion lowerQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.up);
                    Quaternion upperQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.down);
                    panResult = Quaternion.Slerp(lowerQuat, upperQuat, debugStopwatch / panDuration);
                }
                if (right)
                {
                    float halfRot = panAmount / 2f;
                    Quaternion lowerQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.down);
                    Quaternion upperQuat = baseRotation * Quaternion.AngleAxis(halfRot, Vector3.up);
                    panResult = Quaternion.Slerp(lowerQuat, upperQuat, debugStopwatch / panDuration);
                }

                camObject.transform.localRotation = panResult;
            }
            if(!activatePan && inDebug)
            {
                inDebug = false;
                UnsetCam();
            }
        }

        public void ZoomIn()
        {
            //Debug.Log("ZOOMIN HERE PUH LEASEEEE");
            camParams.RemoveParamsOverride(handle, 0.001f);
            CharacterCameraParamsData cameraParamsData = camParams.currentCameraParamsData;
            cameraParamsData.fov = 5f;

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = cameraParamsData,
                priority = 0,
            };
            handle = camParams.AddParamsOverride(request, 0.001f);
        }
        public void ZoomOut()
        {
            //Debug.Log("ZOOMOUT HERE PLEASEEEEEEE");
            camParams.RemoveParamsOverride(handle, 0.001f);
            CharacterCameraParamsData cameraParamsData = camParams.currentCameraParamsData;
            cameraParamsData.fov = fov;

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = cameraParamsData,
                priority = 0,
            };
            handle = camParams.AddParamsOverride(request, 0.001f);
        }

        private void LateUpdate()
        {
            if (!camObject)
            {
                camObject = Camera.main.gameObject;
                previousParent = camObject.transform.parent;
                cameraRig = camObject.GetComponent<CameraRigController>();
            }

            if (camObject && inCam && cameraRig)
            {
                float num = ((cameraRig.localUserViewer == null) ? 1f : cameraRig.localUserViewer.userProfile.screenShakeScale);
                //float num = 1f;
                Vector3 position = camObject.transform.position;
                Vector3 rawScreenShakeDisplacement = ShakeEmitter.ComputeTotalShakeAtPoint(camObject.transform.position);
                Vector3 vector = rawScreenShakeDisplacement * num;
                Vector3 position2 = position + vector;
                if (vector != Vector3.zero && Physics.SphereCast(position, direction: vector, radius: Camera.main.nearClipPlane, hitInfo: out var hitInfo, maxDistance: vector.magnitude, layerMask: LayerIndex.world.mask, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                {
                    position2 = position + vector.normalized * hitInfo.distance;
                    position2 *= 0.5f;
                }
                camObject.transform.SetPositionAndRotation(position2, camObject.transform.rotation);
            }

            if (inCam && !useCamObj && !inDebug && trackTrans && camObject)
            {
                camObject.transform.LookAt(trackTrans.position);
            }
        }

        private void PlayAnim(Animator animator, string animName)
        {
            animator.speed = 1f;
            animator.Update(0f);
            int layerIndex = animator.GetLayerIndex("Base Layer");

            if (layerIndex >= 0)
            {
                animator.PlayInFixedTime(animName, layerIndex, 0f);
            }

        }
    }
}
