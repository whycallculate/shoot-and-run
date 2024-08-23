using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSync : MonoBehaviour, IPunObservable
{
    PhotonView pw;
    private void Start()
    {
        pw = GetComponent<PhotonView>();

    }
    private void Update()
    {
        if (pw.IsMine)
        {
            // Local player updates aimPos position
            transform.position = AimState.Instance.aimPos.position;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Local player sends the position to the network
            stream.SendNext(transform.position);
        }
        else
        {
            // Remote players receive the position from the network
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }


}
