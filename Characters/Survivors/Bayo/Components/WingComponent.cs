using UnityEngine;
public class WingComponent : MonoBehaviour
{


    private Vector3 origSize;
    private float growDur = 0.3f;
    private Vector3 startSize;

    private Material mat;
    private Color origColor;
    private float fadeStart = 0.5f;
    private float fadeEnd = 0.9f;

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
        origColor.a = 1;
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
        origColor.a = 1;
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
            float alpha = Mathf.Lerp(1f, 0f, (stopwatch - fadeStart) / (fadeEnd - fadeStart));
            origColor.a = alpha;
            mat.SetColor("_Color", origColor);
        }

        stopwatch += Time.deltaTime;
    }
}