using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[System.Serializable]
public struct PlayerSound
{
    public AudioClip[] fire;    // 인스펙터에서 할당됨. index 0은 라이플, index 1은 샷건
    public AudioClip[] reload;  // 인스펙터에서 할당됨
}

[System.Serializable] // public으로 선언된 클래스를 인스펙터 창에 노출
public class PlayerAnimation
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
    public AnimationClip sprint;
}

public partial class Player : MonoBehaviour
{
    public enum WeaponType { RIFLE = 0, SHOTGUN = 1 }
    public PlayerAnimation playerAnimation;
    public PlayerSound playerSound;
    public WeaponType weaponType = WeaponType.RIFLE;
    private Image magazineImage; // 탄창 이미지
    private Text magazineText;   // 탄창에 남은 총알 수
    //public float reloadTime = 2.0f; // 재장전 시간

    private float moveSpeed = 5f;
    private float finalMoveSpeed;
    private Vector3 moveDir;
    private float rotSpeed = 400f;
    private Rigidbody rb;
    private CapsuleCollider capcol;
    private Transform tr;
    private Animation ani;
    private float h, v, r;
    private bool runButton;

    private bool isFire = false;
    private bool isFireAuto = false;
    private float nextFire;         // 다음 발사 시간을 저장
    private float fireRate = 0.1f;  // 총알 발사 간격
    private bool isReload = false;  // 재장전 여부
    private int maxBullet = 10;
    private int currentBullet;
    private Sprite[] weaponIcons;   // 무기 변경 아이콘
    private Image weaponImage;      // 이미지 컴포넌트

    private int enemyLayer;         // 적 레이어 index
    private int barrelLayer;
    private int boxLayer;
    private int layerMask;

    private Transform firePos;
    public bool isRun;

    private ParticleSystem muzzFlash;
    private readonly string enemyTag = "Enemy";
    private readonly string enemyTag2 = "EnemySwat";
    private readonly string barrelTag = "Barrel";

    void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;    // 함수를 이벤트로 등록
    }

    void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;    // 게임매니저에서 스피드가 바뀌면 실시간으로 moveSpeed를 변경해줌
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capcol = GetComponent<CapsuleCollider>();
        tr = GetComponent<Transform>();
        ani = GetComponent<Animation>();
        ani.Play(playerAnimation.idle.name);

        moveSpeed = GameManager.instance.gameData.speed;
        finalMoveSpeed = moveSpeed;

        magazineImage = GameObject.Find("Canvas_UI").transform.GetChild(1).GetChild(2).GetComponent<Image>();
        magazineText = GameObject.Find("Canvas_UI").transform.GetChild(1).GetChild(0).GetComponent<Text>();

        weaponIcons = Resources.LoadAll<Sprite>("Icon");
        weaponImage = GameObject.Find("Canvas_UI").transform.GetChild(3).GetChild(0).GetComponent<Image>();
        weaponImage.sprite = weaponIcons[(int)weaponType];

        firePos = GameObject.FindWithTag("FirePos").transform;
        currentBullet = maxBullet;
        isRun = false;

        muzzFlash = firePos.GetChild(0).GetComponent<ParticleSystem>();
        muzzFlash.Stop();

        enemyLayer = LayerMask.NameToLayer("Enemy");    // 레이어 이름을 index 값으로 반환. 유니티에서 index 순서가 바뀌어도 상관무
        barrelLayer = LayerMask.NameToLayer("Barrel");
        boxLayer = LayerMask.NameToLayer("Boxes");
        layerMask = 1 << enemyLayer | 1 << barrelLayer | 1 << boxLayer;         // 레이어 마스크를 이용해 레이어를 묶어서 사용
    }

    void Update()
    {
        //isFireAuto = Check_isFireAuto();
        if (EventSystem.current.IsPointerOverGameObject()) return;  // UI 클릭 시 플레이어 움직임 X. 이벤트시스템.현재.마우스가옵젝위에존재시, 메서드종료.
        PlayerMove_All();

        PlayerGunFire();
        //PLayerGunFireAuto();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 Dir = context.ReadValue<Vector2>();
        moveDir = new Vector3(Dir.x, 0, Dir.y);
        h = Dir.x;
        v = Dir.y;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        r = context.ReadValue<Vector2>().x * 0.25f;
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        runButton = context.ReadValueAsButton();
    }
    
    private void PlayerMove_All()   // 플레이어의 움직이는 모든 것
    {
        MoveAni();
        MoveRun();
        tr.Translate(moveDir * finalMoveSpeed * Time.deltaTime, Space.Self);     // Space.Self 로컬좌표, Space.World 절대좌표
        tr.Rotate(Vector3.up * r * rotSpeed * Time.deltaTime, Space.Self);
        
    }
    private void MoveRun()
    {
        if (runButton && (Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1))
        {
            finalMoveSpeed = moveSpeed * 1.5f;
            isRun = true;
        }
        else
        {
            finalMoveSpeed = moveSpeed;
            isRun = false;
        }
    }
    private void MoveAni()  // 애니메이션만 재생
    {
        string animationName = playerAnimation.idle.name;
        float animationSpeed = 1.0f;

        if (h > 0.1f)
            animationName = playerAnimation.runRight.name;
        else if (h < -0.1f)
            animationName = playerAnimation.runLeft.name;
        else if (v > 0.1f)
            animationName = playerAnimation.runForward.name;
        else if (v < -0.1f)
            animationName = playerAnimation.runBackward.name;

        if (finalMoveSpeed == moveSpeed * 1.5f)
            animationSpeed = 1.5f;

        ani.CrossFade(animationName, 0.3f); // 0.3초 동안 천천히 전환
        ani[animationName].speed = animationSpeed;
    }
}