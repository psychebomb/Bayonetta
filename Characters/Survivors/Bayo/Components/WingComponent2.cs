using UnityEngine;
public class WingComponent2 : MonoBehaviour
{


    private Vector3 origSize;
    private float growDur = 0.3f;
    private Vector3 startSize;

    private Material mat;
    private Color origColor;
    public float fadeStart = 0.55f;
    public float fadeEnd = 0.75f;

    private float stopwatch = 0f;
    private float myTime = 0f;
    //private int id = 0;

    void Start()
    {
        origSize = transform.localScale;
        startSize = origSize * 0.2f;
        transform.localScale = startSize;
        mat = transform.Find("wingmesh").gameObject.GetComponent<SkinnedMeshRenderer>().material;
        origColor = mat.color;
        origColor.a = 1f;
        mat.SetColor("_Color", origColor);
        stopwatch = 0f;
    }

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

    // Update is called once per frame
    void Update()
    {

        if (stopwatch <= growDur)
        {
            transform.localScale = Vector3.Lerp(startSize, origSize, stopwatch / growDur);
        }

        if (stopwatch >= fadeStart && stopwatch <= fadeEnd)
        {
            float emAll = Mathf.Lerp(0f, 0.75f, (stopwatch - fadeStart) / (fadeEnd - fadeStart));
            Color newColor = Color.black;
            newColor.b = emAll;
            newColor.r = emAll;
            newColor.g = emAll;
            mat.SetColor("_EmissionColor", newColor);

            /*
            float darker = Mathf.Lerp(origColor.r, 0.5f, (stopwatch - fadeStart) / (fadeEnd - fadeStart));
            newColor = origColor;
            newColor.r = darker;
            newColor.b = darker;
            newColor.g = darker;
            mat.SetColor("_Color", newColor);
            */
        }
        if (stopwatch >= fadeEnd)
        {
            Color newColor = origColor;
            newColor.a = 0f;
            mat.SetColor("_Color", newColor);
        }

        stopwatch += Time.deltaTime;
    }
}