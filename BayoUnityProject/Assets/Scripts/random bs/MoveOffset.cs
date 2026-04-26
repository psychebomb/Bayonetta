using UnityEngine;
public class MoveOffset : MonoBehaviour
{
    public float startOffset = 0f;
    //public float startTiling = 1f;
    public float idealOffset = -.95f;
    //public float idealTiling = 1f;
    public float slideDur = 0.1f;
    //private string texName = "ef_bayonetta_line01_red";
    public float atSpeedMult = 1f;
    public float duration = 0f;
    public bool smooth = true;
    public bool atStart = true;
    public bool atEnd = false;

    private Material mat;
    private ParticleSystem ps;
    private bool tiling = false;
    //private int id = 0;

    void Start()
    {
        mat = GetComponent<ParticleSystemRenderer>().material;
        ps = GetComponent<ParticleSystem>();
        if (startOffset < idealOffset) tiling = true;
        //id = mat.GetTexturePropertyNameIDs()[0];
    }

    // Update is called once per frame
    void Update()
    {
        float offsetVal = 0f;
        float durStart = duration - slideDur;
        if (atStart)
        {
            if (smooth) offsetVal = Mathf.SmoothStep(startOffset, idealOffset, ps.time / (slideDur / atSpeedMult));
            else offsetVal = Mathf.Lerp(startOffset, idealOffset, ps.time / (slideDur / atSpeedMult));
        }
        if (atEnd && ps.time >= durStart)
        {
            float endTime = ps.time - durStart;
            if (smooth) offsetVal = Mathf.SmoothStep(idealOffset, startOffset, endTime / (slideDur / atSpeedMult));
            else offsetVal = Mathf.Lerp(idealOffset, startOffset, endTime / (slideDur / atSpeedMult));
        }
        mat.mainTextureOffset = new Vector2(0, offsetVal);
        if (tiling)
        {
            float start = (startOffset * -1) + 1.5f;
            float tilingVal = 0f;
            if (atStart)
            {
                if (smooth) tilingVal = Mathf.SmoothStep(start, 1, ps.time / (slideDur / atSpeedMult));
                else tilingVal = Mathf.Lerp(start, 1, ps.time / (slideDur / atSpeedMult));
                mat.mainTextureScale = new Vector2(1, tilingVal);
            }
            if (atEnd && ps.time >= durStart)
            {
                float endTime = ps.time - durStart;
                if (smooth) tilingVal = Mathf.SmoothStep(1, start, endTime / (slideDur / atSpeedMult));
                else tilingVal = Mathf.Lerp(1, start, endTime / (slideDur / atSpeedMult));
                mat.mainTextureScale = new Vector2(1, tilingVal);
            }
        }
    }
}