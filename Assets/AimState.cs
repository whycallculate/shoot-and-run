using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using System.IO;
using Unity.VisualScripting;
using System.Data;
using UnityEngine.Animations;


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
    [HideInInspector] public Animator anim;
    [HideInInspector] public GameObject playerCamera;
    CinemachineVirtualCamera vCam;

    [Header("ZoomInOutAim")]
    public float adsFov = 30;
    [SerializeField] public float hipFov;
    [SerializeField] public float currentFov;
    public float fovSmoothSpeed = 10;

    [Header("Aim")]
    public GameObject aimPos;  
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;
    public PhotonView pw;

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
        anim = this.GetComponent<Animator>();

    }
    void Start()
    {


        if (pw.IsMine)
        {
            //Animation rigging target objesini burada uretiyoruz ve ayni zamanda photonview alarak Diger oyunculara bu objenin PhotomViewID gonderiyoruz ayni zaman da kendiminiki de yolluyoruz.
            Vector3 tempVector = new Vector3(1.0f, 1.0f, 1.0f);
            GameObject instantiatedAimPos = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "AimPos"), tempVector, Quaternion.identity);

            PhotonView aimPosPhotonView = instantiatedAimPos.GetComponent<PhotonView>();

            if (aimPosPhotonView.IsMine)
            {

                //Kendi targetimizi ayarladigimiz kisim.
                SetValueRigging(instantiatedAimPos);
                //Olusturdugumuz olan objenin photon view view id diger oyunculara gonderiyoruz.
                pw.RPC("SetOtherPlayerAimPos", RpcTarget.OthersBuffered, aimPosPhotonView.ViewID);
            }

            // Diğer kamera başlangıç ayarları
            SetCameraStartMethod();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            playerCamera = transform.GetChild(0).gameObject;
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

        //Animation rigging islemlerinin Diger oyunculara gonderme ve alma islemleri.
        if (pw.IsMine)
        {
            bodyAim.data.sourceObjects.SetTransform(0, aimPos.transform);
            headAim.data.sourceObjects.SetTransform(0, aimPos.transform);
            rHandAim.data.sourceObjects.SetTransform(0, aimPos.transform);
            CamFollowPos();
            GetAim();
            ScreenCentre();

        }
        else if(!pw.IsMine)
        {
            bodyAim.data.sourceObjects.SetTransform(0, aimPos.transform);
            headAim.data.sourceObjects.SetTransform(0, aimPos.transform);
            rHandAim.data.sourceObjects.SetTransform(0, aimPos.transform);
        }

    }
    
    public void UpdateMultiAimConstraint(Transform newTargetBone, MultiAimConstraint multiAimConstraint)
    {
        if (multiAimConstraint != null)
        {
            var data = multiAimConstraint.data.sourceObjects;
            data.Clear();  // Eski kaynakları temizliyoruz
            data.Add(new WeightedTransform(newTargetBone, 1f));  // Yeni hedef kemiği ekliyoruz
            multiAimConstraint.data.sourceObjects = data;
        }
    }
    public void UpdateTwoBoneIKConstraint(Transform newTargetBone, TwoBoneIKConstraint twoBoneIKConstraint)
    {
        if (twoBoneIKConstraint != null)
        {
            twoBoneIKConstraint.data.target = newTargetBone;
        }
    }

    public void SetValueRigging(GameObject aimPos)
    {
        //Target objesini burada initilate ediyoruz.
        this.aimPos = aimPos;
        UpdateAimConstraint(bodyAim, aimPos.transform);
        UpdateAimConstraint(headAim, aimPos.transform);
        UpdateAimConstraint(rHandAim, aimPos.transform);



    }
    [PunRPC]
    void SetOtherPlayerAimPos(int aimPosViewID)
    {
        //ufak bir ali cengiz oyunu ile Burada Photonviewleri karsilastiriyoruz. ve gonderdgimiz photonview objesinin gameobjectini diger oyuncular icin set ediyoruz.
        PhotonView aimPosPhotonView = PhotonView.Find(aimPosViewID);
        if (aimPosPhotonView != null && !aimPosPhotonView.IsMine)
        {
            SetValueRigging(aimPosPhotonView.gameObject);
        }
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
        playerCamera = transform.GetChild(0).gameObject;
        playerCamera.SetActive(true);

        vCam = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView;
        currentFov = hipFov;

        camFollowPos = transform.GetChild(1);

        vCam.Follow = camFollowPos;
        vCam.LookAt = camFollowPos;
    }
    public void ScreenCentre()
    {
        //Aim objesini ekranin ortasina koyuyoruz.
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);
    
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            if (pw.IsMine)
            {
                aimPos.transform.position = Vector3.Lerp(aimPos.transform.position, hit.point, aimSmoothSpeed * Time.deltaTime);

            }

        }


    }

    public void PosUpdate()
    {

        //Mouse hareket inputlari Cinemachine 
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    public void CamFollowPos()
    {
        //Cameranin playerobjectini takip eden method
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
    
}

