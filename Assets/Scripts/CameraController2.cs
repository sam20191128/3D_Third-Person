using UnityEngine;

public class CameraController2 : MonoBehaviour
{
    public Transform hero;
    public float smoothing;
    private Vector3 offset;

    private void Start()
    {
        //offset = transform.position - hero.position;
        //offset = new Vector3(-0.2173055f, 3.476311f, -3.444581f);
        offset = new Vector3(-0f, 3f, -3f);//(左右，高度，前后(负的在角色背后))
    }

    private void Update()
    {
        //transform.position = offset + hero.position;

        if (Input.GetAxis("Mouse ScrollWheel") < 0)//拉远
        {
            if (Camera.main.fieldOfView <= 60)//透视
            {
                Camera.main.fieldOfView += 2;
            }
            if (Camera.main.orthographicSize <= 20)//正交
            {
                Camera.main.orthographicSize += 0.5F;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)//拉近
        {
            if (Camera.main.fieldOfView > 20)//透视
            {
                Camera.main.fieldOfView -= 2;
            }
            if (Camera.main.orthographicSize >= 1)//正交
            {
                Camera.main.orthographicSize -= 0.5F;
            }
        }
    }

    void FixedUpdate()
    {
        //Vector3 targetCamPos = hero.position + offset;

        ////相机平滑的移动到目标位置，插值
        //transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        //target.TransformDirection(Offset) 将偏移从局部坐标变为世界坐标,达到摄像机永远在角色背后的目的
        transform.position = Vector3.Lerp(transform.position, hero.position + hero.TransformDirection(offset), Time.deltaTime * smoothing);
        //摄像机朝向角色
        transform.LookAt(hero);
    }
}