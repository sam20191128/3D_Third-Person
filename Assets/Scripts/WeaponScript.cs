using UnityEngine;

public class WeaponScript : MonoBehaviour
{

    public bool activated;

    public float rotationSpeed;

    void Update()
    {
        if (activated)//如果激活
        {
            transform.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;//斧子自身旋转
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.layer == 11)
        //{
        print(collision.gameObject.name);//打印碰到的物体名字
        GetComponent<Rigidbody>().Sleep();
        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        GetComponent<Rigidbody>().isKinematic = true;//冻结，控制物理是否影响rigidbody
        activated = false;//激活关闭，旋转关闭
        //}

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Breakable"))
    //    {
    //        if(other.GetComponent<BreakBoxScript>() != null)
    //        {
    //            other.GetComponent<BreakBoxScript>().Break();
    //        }
    //    }
    //}
}
