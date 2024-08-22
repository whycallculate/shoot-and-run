using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class AimState : MonoBehaviour
{
    [Header("Cam Movement")]
    public Cinemachine.AxisState xAxis, yAxis;
    [SerializeField] Transform camFollowPos;
    [SerializeField] public Animator anim;
    [SerializeField] public CinemachineVirtualCamera vCam;

    [Header("ZoomInOutAim")]
    public float adsFov = 30;
    [SerializeField] public float hipFov;
    [SerializeField] public float currentFov;
    public float fovSmoothSpeed = 10;

    [Header("Aim")]
    [SerializeField] Transform aimPos;
    [SerializeField] float aimSmoothSpeed=  20;
    [SerializeField] LayerMask aimMask;

    // Start is called before the first frame update
    void Start()
    {
        hipFov = vCam.m_Lens.FieldOfView;
        currentFov = hipFov;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        PosUpdate();
        
    }
    private void LateUpdate()
    {
        CamFollowPos();
        GetAim();
        ScreenCentre();
    }

    public void ScreenCentre()
    {
        Vector2 screenCentre = new Vector2(Screen.width/2,Screen.height/2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if(Physics.Raycast(ray ,out RaycastHit hit, Mathf.Infinity,aimMask))
        {
            aimPos.position = Vector3.Lerp(aimPos.position,hit.point,aimSmoothSpeed*Time.deltaTime);
        }
    }

    public void PosUpdate()
    {
        //Cinemachineden aldigimiz Mouse degerleri.
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    public void CamFollowPos()
    {
        //Kamera X Y koordinatlarini aldigimiz inputlara gore set etme islemi
        camFollowPos.localEulerAngles = new Vector3(yAxis.Value,camFollowPos.localEulerAngles.y,camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,xAxis.Value,camFollowPos.localEulerAngles.z);
    }

    public void GetAim()
    {
        // Aim alip almadigimiz ve burada zoom in out ayni zaman da Animasyon islemleri
        if (Input.GetKey(KeyCode.Mouse1))
        {
            
            anim.SetBool("Aiming", true);
            currentFov = adsFov;
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        }
        else if(Input.GetKeyUp(KeyCode.Mouse1)) 
        {
            

            anim.SetBool("Aiming", false);
            currentFov = hipFov;
            vCam.m_Lens.FieldOfView = vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        }
        //Oyun basinda currentfovu vererek ZoomInOut buglamamasi icin currentFovu Default haline cekiyoruz.
        vCam.m_Lens.FieldOfView = vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
    }
}
