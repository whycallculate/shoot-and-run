using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;


public class AimState : MonoBehaviour 
{
    private static AimState instance;
    public static AimState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AimState>();
            }
            return instance;
        }
    }

    [Header("Cam Movement")]
    public Cinemachine.AxisState xAxis, yAxis;
    [HideInInspector] Transform camFollowPos;
    [SerializeField] public Animator anim;
    [HideInInspector] public GameObject playerCamera;
    CinemachineVirtualCamera vCam;

    [Header("ZoomInOutAim")]
    public float adsFov = 30;
    [SerializeField] public float hipFov;
    [SerializeField] public float currentFov;
    public float fovSmoothSpeed = 10;

    [Header("Aim")]
    public Transform aimPos;  // Artık private olacak
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;
    PhotonView pw;

    [Header("Rigging")]
    public MultiAimConstraint bodyAim;
    public MultiAimConstraint headAim;
    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;
    public RigBuilder rig;


    private void Awake()
    {
        pw = this.GetComponent<PhotonView>();
        rig = this.GetComponent<RigBuilder>();
    }

    void Start()
    {
        if (pw.IsMine)
        {
            SetCameraStartMethod();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if(!pw.IsMine)
        {
            // Diğer oyuncuların kameralarını devre dışı bırak
            playerCamera = transform.GetChild(2).gameObject;
            playerCamera.SetActive(false);


        }
    }

    void Update()
    {
        if (pw.IsMine)
        {
            PosUpdate();
        }
    }

    private void LateUpdate()
    {
        if (pw.IsMine)
        {

            CamFollowPos();
            GetAim();
            ScreenCentre();
        }

    }
    public void SetValue(Transform aimPos)
    {
        UpdateAimConstraint(bodyAim, aimPos);
        UpdateAimConstraint(headAim, aimPos);
        UpdateAimConstraint(rHandAim, aimPos);
    }
    private void UpdateAimConstraint(MultiAimConstraint aim, Transform target)
    {
        var data = aim.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(target, 1));
        aim.data.sourceObjects = data;
        rig.Build(); // Rebuild the rig to apply changes
    }
    public void SetCameraStartMethod()
    {
        // Bu oyuncu local oyuncuysa, kamerayı etkinleştir
        playerCamera = transform.GetChild(2).gameObject;
        playerCamera.SetActive(true);

        vCam = transform.GetChild(2).GetComponent<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView;
        currentFov = hipFov;

        camFollowPos = transform.GetChild(1);

        vCam.Follow = camFollowPos;
        vCam.LookAt = camFollowPos;
    }
    public void ScreenCentre()
    {
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);
    
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            if (pw.IsMine)
            {
                aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);

            }

        }


    }

    public void PosUpdate()
    {
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    public void CamFollowPos()
    {
        camFollowPos.localEulerAngles = new Vector3(yAxis.Value, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.Value, camFollowPos.localEulerAngles.z);
    }

    public void GetAim()
    {
        if (pw.IsMine)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                anim.SetBool("Aiming", true);
                currentFov = adsFov;
                vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                anim.SetBool("Aiming", false);
                currentFov = hipFov;
                vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
            }
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, hipFov, fovSmoothSpeed * Time.deltaTime);
        }
    }
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // Local player, data gönder
    //        Debug.Log("Sending data..");
    //        stream.SendNext(aimPos.position);

    //    }
    //    else
    //    {

    //        Debug.Log("Recive data");
    //        // Remote player, data al
    //        aimPos.position = (Vector3)stream.ReceiveNext();


    //        // Aim Constraint'i güncelle
    //        bodyAim.data.sourceObjects.SetTransform(0, aimPos);
    //        headAim.data.sourceObjects.SetTransform(0, aimPos);
    //        rHandAim.data.sourceObjects.SetTransform(0, aimPos);


    //    }
    //}
}

