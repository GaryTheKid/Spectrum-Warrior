using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLookController : MonoBehaviour
{
    // references
    [SerializeField] private GameObject playerCam;
    public GameObject crossHair;
    private PhotonView PV;

    // values
    private float CamVRotation;
    private bool isLocked;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && !isLocked && !GetComponent<PlayerController>().isDead)
        {
            // mouse input
            float lookV = Input.GetAxis("Mouse Y") * Time.deltaTime * 200f;
            float lookH = Input.GetAxis("Mouse X") * Time.deltaTime * 200f;

            // rotate player
            transform.Rotate(new Vector3(0f, lookH, 0f));

            // rotate camera
            CamVRotation -= lookV;
            CamVRotation = Mathf.Clamp(CamVRotation, -90f, 90f);
            playerCam.transform.localRotation = Quaternion.Euler(CamVRotation, 0f, 0f);
        }
    }

    // lock movement
    public void Lock()
    {
        isLocked = true;
    }

    // unlock movement
    public void Unlock()
    {
        isLocked = false;
    }
}
