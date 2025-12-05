using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Vector2 parallaxSpeed;

    private Material mat;
    private Transform cam;
    private Vector3 lastPos;
    private Vector2 offset;

    void Start()
    {
        cam = Camera.main.transform;
        lastPos = cam.position;
        mat = GetComponent<SpriteRenderer>().material;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastPos;
        offset += new Vector2(delta.x * parallaxSpeed.x, delta.y * parallaxSpeed.y);
        mat.mainTextureOffset = offset;
        lastPos = cam.position;
    }
}
