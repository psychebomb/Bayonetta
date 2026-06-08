using UnityEngine;

public class RotateSkulls : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private GameObject skullObj;

    public float xMult = 0.0222222f;
    public float yMult = 0.0222222f;
    public float negXMult = 0.0111111f;
    public float negYMult = 0.0111111f;
    private float origX;
    private float origY;
    private float origZ;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        skullObj = gameObject.transform.Find("skull").gameObject;

        origX = skullObj.transform.localPosition.x;
        origY = skullObj.transform.localPosition.y;
        origZ = skullObj.transform.localPosition.z;
    }

    void LateUpdate()
    {
        ps.GetParticles(particles);
        if (particles.Length > 0)
        {
            float zRot = particles[0].rotation3D.z;

            if (zRot > 0)
            {
                float xpos = origX * (1 + (zRot * xMult));
                float ypos = origY * (1 + (zRot * yMult));
                float zpos = origZ;

                if (skullObj) skullObj.transform.localPosition = new Vector3(xpos, ypos, zpos);

            }
            else
            {
                float xpos = origX * (1 + (zRot * negXMult));
                float ypos = origY * (1 + (zRot * negYMult));
                float zpos = origZ;

                if (skullObj) skullObj.transform.localPosition = new Vector3(xpos, ypos, zpos);
            }

            if (skullObj)
            {
                float x = skullObj.transform.rotation.eulerAngles.x;
                float y = skullObj.transform.rotation.eulerAngles.y;
                skullObj.transform.eulerAngles = new Vector3(x, y, zRot);
            }
        }
    }
}