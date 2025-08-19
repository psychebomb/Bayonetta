using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.Components.Demon
{
    public class DemonVisualController : MonoBehaviour
    {
        public bool reverse = false;
        private bool reversed = false;

        private Material mat0;
        private Material mat1;
        private Material mat2;
        private Material mat3;
        private Material mat4;
        private Material demonMat;
        private Color noAlpha;

        public float initialDelay = 0.24f;
        public float doneTime = 3f;
        public float meshDur = 1f;
        private float stopwatch = 0f;
        private float myTime = 0f;
        private float interval = 0f;
        //private int id = 0;

        void Start()
        {
            mat0 = transform.Find("hair0").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            mat1 = transform.Find("hair1").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            mat2 = transform.Find("hair2").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            mat3 = transform.Find("hair3").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            mat4 = transform.Find("hair4").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            demonMat = transform.Find("head").gameObject.GetComponent<SkinnedMeshRenderer>().material;

            noAlpha = mat0.color;
            noAlpha.a = 0f;
            mat0.SetColor("_Color", noAlpha);
            mat1.SetColor("_Color", noAlpha);
            mat2.SetColor("_Color", noAlpha);
            mat3.SetColor("_Color", noAlpha);
            mat4.SetColor("_Color", noAlpha);
            demonMat.SetColor("_Color", noAlpha);
            stopwatch = 0f;

            interval = (doneTime - meshDur - initialDelay) / 5f;
        }

        /*
        void OnEnable()
        {
            origSize = transform.localScale;
            startSize = origSize * 0.2f;
            //transform.localScale = startSize;
            mat = transform.Find("wingmesh").gameObject.GetComponent<SkinnedMeshRenderer>().material;
            origColor = mat.color;
            origColor.a = 1f;
            mat.SetColor("_Color", origColor);
            stopwatch = 0f;
        }
        */

        private void FadeInMesh(Material mat, float startTime, float finishTime)
        {
            Color newColor = mat.color;
            newColor.a = 1f;
            if (stopwatch >= startTime) mat.SetColor("_Color", newColor);

            float cutOffValue = Mathf.Lerp(1f, 0.5f, (stopwatch - startTime) / (finishTime - startTime));
            mat.SetFloat("_Cutoff", cutOffValue);
            float offsetValue = Mathf.Lerp(-0.5f, 0, (stopwatch - startTime) / (finishTime - startTime));
            mat.mainTextureOffset = new Vector2(0, offsetValue);
        }

        private void FadeInDemonMesh(Material mat, float startTime, float finishTime)
        {
            Color newColor = mat.color;
            if (stopwatch <= startTime)
            {
                newColor.a = 0f;
                mat.SetColor("_Color", newColor);
            }
            else
            {
                newColor.a = 1f;
                float val = Mathf.Lerp(0f, 1f, (stopwatch - startTime) / (finishTime - startTime));
                newColor.r = val;
                newColor.g = val;
                newColor.b = val;
                mat.SetColor("_Color", newColor);
            }
        }

        private void FadeOutMesh(Material mat, float startTime, float finishTime)
        {
            Color newColor = mat.color;
            newColor.a = 0f;
            if (stopwatch >= finishTime) mat.SetColor("_Color", newColor);

            float cutOffValue = Mathf.Lerp(0.5f, 1f, (stopwatch - startTime) / (finishTime - startTime));
            mat.SetFloat("_Cutoff", cutOffValue);
            float offsetValue = Mathf.Lerp(0f, -0.5f, (stopwatch - startTime) / (finishTime - startTime));
            mat.mainTextureOffset = new Vector2(0, offsetValue);
        }

        private void FadeOutDemonMesh(Material mat, float startTime, float finishTime)
        {
            Color newColor = mat.color;
            if (stopwatch >= finishTime)
            {
                newColor.a = 0f;
                mat.SetColor("_Color", newColor);
            }
            else
            {
                newColor.a = 1f;
                float val = Mathf.Lerp(1f, 0f, (stopwatch - startTime) / (finishTime - startTime));
                newColor.r = val;
                newColor.g = val;
                newColor.b = val;
                mat.SetColor("_Color", newColor);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (stopwatch >= initialDelay)
            {
                FadeInMesh(mat0, initialDelay, initialDelay + meshDur);
                FadeInMesh(mat1, initialDelay + interval * 1, initialDelay + interval * 1 + meshDur);
                FadeInMesh(mat2, initialDelay + interval * 2, initialDelay + interval * 2 + meshDur);
                FadeInMesh(mat3, initialDelay + interval * 3, initialDelay + interval * 3 + meshDur);
                FadeInMesh(mat4, initialDelay + interval * 4, initialDelay + interval * 4 + meshDur);
                FadeInDemonMesh(demonMat, initialDelay + interval * 5f, initialDelay + interval * 5f + meshDur);
            }

            if (reverse)
            {
                if (!reversed)
                {
                    reversed = true;
                    stopwatch = 0f;
                }

                if (stopwatch >= initialDelay)
                {
                    FadeOutDemonMesh(demonMat, initialDelay + interval * 0, initialDelay + interval * 0f + meshDur);
                    FadeOutMesh(mat4, initialDelay + interval * 1, initialDelay + interval * 1 + meshDur);
                    FadeOutMesh(mat3, initialDelay + interval * 2, initialDelay + interval * 2 + meshDur);
                    FadeOutMesh(mat2, initialDelay + interval * 3, initialDelay + interval * 3 + meshDur);
                    FadeOutMesh(mat1, initialDelay + interval * 4, initialDelay + interval * 4 + meshDur);
                    FadeOutMesh(mat0, initialDelay + interval * 5, initialDelay + interval * 5 + meshDur);
                }
            }

            stopwatch += Time.deltaTime;
        }
    }
}

