using UnityEngine;
public class VFXrm : MonoBehaviour
{
    public Vector3 vfxPos = Vector3.zero;
    public Quaternion vfxRot = Quaternion.identity;


    void Start()
    {
        vfxPos = transform.position;
        vfxRot = transform.rotation;    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float vfxx = vfxPos.x;
        float vfxy = this.gameObject.transform.position.y;
        float vfxz = vfxPos.z;

        this.gameObject.transform.position = new Vector3(vfxx, vfxy, vfxz);
        this.gameObject.transform.rotation = vfxRot;
    }
}