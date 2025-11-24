using UnityEngine;

public class ps5Dondurme : MonoBehaviour
{
    public float donmeHizi = -60f;  //saniyede 360 derece

    void Update()
    {
        transform.Rotate(0, 0, donmeHizi * Time.deltaTime);
    }

}
