using UnityEngine;
public class WickedWeave : MonoBehaviour
{


    private Vector3 origSize;
    private float growDur = 0.2f;
    private Vector3 startSize;
    private float waitDur = 0.24f;
    private float stopwatch = 0f;
    //private int id = 0;

    void Start()
    {
        origSize = transform.localScale;
        startSize = origSize * 0f;
        transform.localScale = startSize;
        stopwatch = 0f;
    }

    void OnEnable()
    {
        origSize = transform.localScale;
        startSize = origSize * 0.2f;
        //transform.localScale = startSize;
        stopwatch = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (stopwatch >= waitDur)
        {
            transform.localScale = Vector3.Lerp(startSize, origSize, (stopwatch - waitDur) / growDur);
        }

        stopwatch += Time.deltaTime;
    }
}
