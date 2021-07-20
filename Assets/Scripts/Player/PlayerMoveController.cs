using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerMoveController : MonoBehaviour {

    // references
    [SerializeField] private ParticleSystem BoosterFire1;
    [SerializeField] private ParticleSystem BoosterFire2;
    [SerializeField] private GameObject BoosterLight;
    [SerializeField] private AudioSource BoosterAudio;
    private Animator characterAnimator;
    private PhotonView PV;

    // values
    public float speed;
    public bool isMoving;
    public bool isSpriting;
    public float accJump;
    public float gravity;
    private bool isLocked;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        speed = 3f;
        BoosterFire1.Stop();
        BoosterFire2.Stop();
        characterAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if not locked, process moving
        if (PV.IsMine && !isLocked && !GetComponent<PlayerController>().isDead)
        {
            // keyboard input
            float moveV;
            float moveUpDown;

            // set move V
            if (Input.GetKey(KeyCode.W))
            {
                moveV = 1f * Time.deltaTime * speed;
                // sprite cam
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    isSpriting = true;
                    moveV *= 2;
                }
                else
                {
                    isSpriting = false;
                }
                isSpriting = false;
                characterAnimator.SetInteger("MoveForward", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveV = -1f * Time.deltaTime * speed;
                characterAnimator.SetInteger("MoveBackward", 1);
            }
            else
            {
                characterAnimator.SetInteger("MoveForward", 0);
                characterAnimator.SetInteger("MoveBackward", 0);
                moveV = 0f;
            }

            // set move Up and Down
            if (Input.GetKey(KeyCode.LeftControl))
            {
                characterAnimator.SetInteger("MoveDown", 1);
                moveUpDown = -1f * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                characterAnimator.SetInteger("MoveUp", 1);
                moveUpDown = 1f * Time.deltaTime * speed;
            }
            else
            {
                characterAnimator.SetInteger("MoveDown", 0);
                characterAnimator.SetInteger("MoveUp", 0);
                moveUpDown = 0f;
            }

            // move player
            Vector3 movement = new Vector3(0, moveUpDown, moveV);
            transform.Translate(movement);

            // set animator and isMovving flag
            if (movement.magnitude > 0 && movement.y >= 0)
            {
                if (PV.IsMine)
                {
                    PV.RPC("MoveEffect", RpcTarget.All);
                }
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("StaticEffect", RpcTarget.All);
                }
            }
        }
    }

    // calculate jump ccceleration
    private void AccelerateJump()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0f, accJump, 0f), ForceMode.Impulse);
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

    [PunRPC]
    void MoveEffect()
    {
        isMoving = true;

        // light
        if (!BoosterFire1.isPlaying)
        {
            BoosterFire1.Play();
        }
        if (!BoosterFire2.isPlaying)
        {
            BoosterFire2.Play();
        }
        if (!BoosterLight.activeSelf)
        {
            BoosterLight.SetActive(true);
        }

        // sound
        if (!BoosterAudio.isPlaying)
        {
            BoosterAudio.Play();
        }
    }

    [PunRPC]
    void StaticEffect()
    {
        isMoving = false;

        // light
        if (BoosterFire1.isPlaying)
        {
            BoosterFire1.Stop();
        }
        if (BoosterFire2.isPlaying)
        {
            BoosterFire2.Stop();
        }
        if (BoosterLight.activeSelf)
        {
            BoosterLight.SetActive(false);
        }

        // sound
        if (BoosterAudio.isPlaying)
        {
            BoosterAudio.Stop();
        }
    }
}
