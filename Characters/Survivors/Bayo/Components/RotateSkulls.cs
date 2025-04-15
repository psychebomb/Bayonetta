using UnityEngine;

public class RotateSkulls : MonoBehaviour
{
    private ParticleSystem ps;
    private GameObject skullObj;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        skullObj = gameObject.transform.Find("skull").gameObject;
        //id = mat.GetTexturePropertyNameIDs()[0];
    }

    // Update is called once per frame
    void Update()
    {
        float x = skullObj.transform.rotation.eulerAngles.x;
        float y = skullObj.transform.rotation.eulerAngles.y;
        skullObj.transform.eulerAngles = new Vector3(x, y, ps.main.startRotationZ.Evaluate(1));
    }
}