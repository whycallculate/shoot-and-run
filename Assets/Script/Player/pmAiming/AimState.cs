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
using System;


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
    [HideInInspector] public CinemachineVirtualCamera virtualCamera;

    CinemachineVirtualCamera vCam;

    [Header("ZoomInOutAim")]
    public float adsFov = 30;
    [SerializeField] public float hipFov;
    [SerializeField] public float currentFov;
    public float fovSmoothSpeed = 10;

    [Header("Aim")]
    public PhotonView pw;
    public bool isAiming;

    [Header("Rigging")]
    [SerializeField] public GameObject aimPos;
    public float aimDuration = 0.3f;
    [SerializeField] MultiAimConstraint bodyAim;
    [SerializeField] MultiAimConstraint bodyAim2;
    [SerializeField] MultiAimConstraint headAim;
    [SerializeField] MultiAimConstraint weaponAim;
    [SerializeField] RigBuilder rig;




    private void Awake()
    {
        pw = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();

    }
    void Start()
    {


        if (pw.IsMine)
        {

            virtualCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            SetValueRigging(aimPos);
            SetCameraStartMethod();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            SetValueRigging(aimPos);
            playerCamera = transform.GetChild(0).gameObject;
            playerCamera.SetActive(false);
        }

    }



    private void Update()
    {
        if (pw.IsMine)
        {
            CamFollowPos();
            GetAim();
            PosUpdate();
            GetWeaponOnHand();
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
    private void OnAnimatorMove()
    {
        
    }
    public void SetValueRigging(GameObject aimPos)
    {
        //Target objesini burada initilate ediyoruz.
        UpdateAimConstraint(bodyAim, aimPos.transform);
        UpdateAimConstraint(headAim, aimPos.transform);
        UpdateAimConstraint(bodyAim2, aimPos.transform);
        UpdateAimConstraint(weaponAim, aimPos.transform);
    }

    private void UpdateAimConstraint(MultiAimConstraint aim, Transform target)
    {
        var data = aim.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(target, 1));
        aim.data.sourceObjects = data;
        //rig.Build(); // Rebuild the rig to apply changes
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
                
                currentFov = adsFov;
                vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
            }
            else
            {
                //pw.RPC("WeaponPoseMethod", RpcTarget.Others, false);
                currentFov = hipFov;
                vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
            }
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, hipFov, fovSmoothSpeed * Time.deltaTime);
        }
    }
    
    public void GetWeaponOnHand()
    {
        if (Input.GetKeyDown(InputManager.getWeaponOnHand) && !isAiming)
        {
            isAiming = true;

        }
        else if(Input.GetKeyDown(InputManager.getWeaponOnHand) && isAiming)
        {
            isAiming = false;
        }

    }
    
}

