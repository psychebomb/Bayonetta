using UnityEngine;

namespace BayoMod.Characters.Survivors.Bayo.Components.Demon
{
    public class SummonHair : MonoBehaviour
    {
        // Start is called before the first frame update
        public float startOffset = -0.13f;
        public float startCutoff = 1f;
        public float endOffset = 0f;
        public float endCutoff = 0.25f;
        public float offDelay = 1.68f;
        public float cutDelay = 1.68f;
        public float slideDur = 0.32f;

        private Material mat;
        private float stopwatch = 0f;
        private bool madeVisisble = false;
        void Start()
        {
            mat = GetComponent<SkinnedMeshRenderer>().material;
            mat.SetFloat("_Cutoff", startCutoff);
            mat.mainTextureOffset = new Vector2(0, startOffset);

            Color noAlpha = mat.color;
            noAlpha.a = 0f;
            mat.SetColor("_Color", noAlpha);
        }

        // Update is called once per frame
        void Update()
        {
            stopwatch += Time.deltaTime;

            if (stopwatch >= cutDelay)
            {
                if (!madeVisisble)
                {
                    madeVisisble = true;
                    Color alphaYes = mat.color;
                    alphaYes.a = 1f;
                    mat.SetColor("_Color", alphaYes);
                }
                float curCut = Mathf.Lerp(startCutoff, endCutoff, (stopwatch - cutDelay) / slideDur);
                mat.SetFloat("_Cutoff", curCut);
            }
            if (stopwatch >= offDelay)
            {
                if (!madeVisisble)
                {
                    madeVisisble = true;
                    Color alphaYes = mat.color;
                    alphaYes.a = 1f;
                    mat.SetColor("_Color", alphaYes);
                }
                float curOff = Mathf.Lerp(startOffset, endOffset, (stopwatch - offDelay) / slideDur);
                mat.mainTextureOffset = new Vector2(0, curOff);
            }
        }
    }
}