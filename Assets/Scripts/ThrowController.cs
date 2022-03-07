using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ThrowController : MonoBehaviour
{

    private Animator anim;
    private Hero hero;
    private Rigidbody weaponRb;
    private WeaponScript weaponScript;
    private float returnTime;

    private Vector3 origLocPos;//初始位置
    private Vector3 origLocRot;//初始旋转
    private Vector3 pullPosition;//到达的位置，召回的位置

    [Header("Public References")]
    public Transform weapon;
    public Collider weaponcollider;
    public Transform hand;
    public Transform spine;
    public Transform curvePoint;
    [Space]
    [Header("Parameters")]
    public float throwPower = 30;
    //public float cameraZoomOffset = .3f;
    [Space]
    [Header("Bools")]
    public bool walking = true;
    public bool aiming = false;
    public bool hasWeapon = true;
    public bool pulling = false;
    [Space]
    [Header("Particles and Trails")]
    public ParticleSystem glowParticle;
    public ParticleSystem catchParticle;
    public ParticleSystem trailParticle;
    public TrailRenderer trailRenderer;
    [Space]
    [Header("UI")]
    public Image reticle;//UI准星

    [Space]
    //Cinemachine Shake
    public CinemachineFreeLook virtualCamera;
    public CinemachineImpulseSource impulseSource;

    [SerializeField] private PostProcessVolume post;
    public PostProcessProfile postasset;

    void Start()
    {
        Cursor.visible = false;
        anim = GetComponent<Animator>();
        hero = GetComponent<Hero>();
        weaponRb = weapon.GetComponent<Rigidbody>();
        weaponcollider = weaponRb.GetComponent<BoxCollider>();
        weaponcollider.enabled = false;
        weaponScript = weapon.GetComponent<WeaponScript>();
        origLocPos = weapon.localPosition;//初始位置
        origLocRot = weapon.localEulerAngles;//初始旋转
        reticle.DOFade(0, 0);//UI准星
        trailRenderer.emitting = false;

        post = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PostProcessVolume>();
        post.profile.GetSetting<ChromaticAberration>().active = false;
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            aiming = true;
            hero.RotateToCamera(transform);//人物随相机旋转
            reticle.DOFade(1, .2f);
            post.profile.GetSetting<ChromaticAberration>().active = true;
        }
        else
        {
            reticle.DOFade(0, .2f);
            aiming = false;
            post.profile.GetSetting<ChromaticAberration>().active = false;
        }

        if (hasWeapon)//持有武器
        {
            if (aiming && Input.GetMouseButtonDown(0))//瞄准中并按下左键
            {
                anim.SetTrigger("throw");//播放抛出动画
                weaponcollider.enabled = true;
            }
        }
        else//未持有武器
        {
            if (Input.GetMouseButtonDown(0)) //未持有武器时并按下左键
            {
                WeaponStartPull();//武器召回
                weaponcollider.enabled = true;
            }
        }

        if (pulling)//召回状态
        {
            if (returnTime < 1)
            {
                weapon.position = GetQuadraticCurvePoint(returnTime, pullPosition, curvePoint.position, hand.position);
                returnTime += Time.deltaTime * 1.5f;
            }
            else
            {
                WeaponCatch();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }

    public void WeaponThrow()//抛出武器
    {
        hasWeapon = false;//持有武器状态关闭
        weaponScript.activated = true;//武器脚本激活
        weaponRb.isKinematic = false;//解冻，控制物理是否影响rigidbody
        weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;//碰撞检测方式改为连续检测
        weapon.parent = null;//武器脱离父级
        weapon.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);//武器欧拉角
        weapon.transform.position += transform.right / 5;
        weaponRb.AddForce(Camera.main.transform.forward * throwPower + transform.up * 2, ForceMode.Impulse);//给个屏幕相机方向的脉冲力

        //Trail
        trailRenderer.emitting = true;
        //trailParticle.Play();
    }

    public void WeaponStartPull()//武器召回
    {
        anim.SetBool("WeaponCatch", true);
        pullPosition = weapon.position;//召回的位置
        weaponRb.Sleep();//刚体休息
        weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;//碰撞检测方式改为连续检测
        weaponRb.isKinematic = true;//冻结，控制物理是否影响rigidbody
        weapon.DORotate(new Vector3(-90, -90, 0), .2f).SetEase(Ease.InOutSine);
        weapon.DOBlendableLocalRotateBy(Vector3.right * 90, .5f);
        weaponScript.activated = true;//武器脚本激活
        pulling = true;//召回状态开启
    }

    public void WeaponCatch()
    {
        anim.SetBool("WeaponCatch", false);
        returnTime = 0;
        pulling = false;
        weapon.parent = hand;
        weaponScript.activated = false;
        weapon.localEulerAngles = origLocRot;
        weapon.localPosition = origLocPos;
        hasWeapon = true;
        trailRenderer.emitting = false;
        catchParticle.Play();
        impulseSource.GenerateImpulse(Vector3.right);//屏幕震动
        weaponcollider.enabled = false;
    }
    public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        return (uu * p0) + (2 * u * t * p1) + (tt * p2);
    }
}