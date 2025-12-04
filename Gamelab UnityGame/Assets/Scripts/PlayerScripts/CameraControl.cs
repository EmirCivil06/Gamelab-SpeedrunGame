using UnityEngine;
using Unity.Cinemachine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera camera;
    [SerializeField] private Transform defaultFollow;
    [SerializeField] private Transform defaultLookAt;
    [SerializeField] private bool alsoSetFollow = true;

    [SerializeField] private Transform boss;

    private void Awake()
    {
        if (camera == null)
        {
            camera = GetComponent<CinemachineCamera>();
        }
        FindBoss();

    }


    private void FindBoss()
    {
        var bossObject = GameObject.FindGameObjectWithTag("Boss");
        boss = bossObject != null ? bossObject.transform : null; // boss değişkenine boss'un pozisyonu atanıyor
    }
    
    public void FocusBoss()
    {
        if (boss == null)
        {
            FindBoss();
            if (boss == null)
            {
                Debug.Log("Boss tag'li obje bulunamadi.");
                return;
            }
        }
        if (alsoSetFollow)
        {
            camera.Follow = boss;
        }
        camera.LookAt = boss;
    }

    public void FocusDefault()
    {
        if (defaultLookAt == null)
        {
            return;
        }
        if (alsoSetFollow && defaultFollow != null)
        {
            camera.Follow = defaultFollow;
        }
        camera.LookAt = defaultLookAt;
    }
}
