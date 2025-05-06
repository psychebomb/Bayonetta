using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveOffset : MonoBehaviour
{
    public float startOffset = -1f;
    //public float startTiling = 1f;
    public float idealOffset = -1f;
    //public float idealTiling = 1f;
    public float slideDur = 0f;
    //private string texName = "ef_bayonetta_line01_red";

    private Material mat;
    private ParticleSystem ps;
    //private int id = 0;

    void Start()
    {
        mat = GetComponent<ParticleSystemRenderer>().material;
        ps = GetComponent<ParticleSystem>();
        //id = mat.GetTexturePropertyNameIDs()[0];
    }

    // Update is called once per frame
    void Update()
    {
        float offsetVal = Mathf.Lerp(startOffset, idealOffset, ps.time / slideDur);
        //float tilingVal = Mathf.Lerp(startTiling, idealTiling, Time.time / slideDur);
        //mat.SetTextureOffset(id, new Vector2(0, offsetVal));
        mat.mainTextureOffset = new Vector2(0, offsetVal);
        //mat.Til(texName, new Vector2(0, offsetVal));
    }
}