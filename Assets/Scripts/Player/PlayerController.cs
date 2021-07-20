using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    // linked objs
    public string playerName;
    public GameObject myCamera;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject HPCanvas;
    [SerializeField] private GameObject PlayerColor;
    private AudioListener listener;

    [SerializeField] private Text HPText;
    [SerializeField] private Image HPBarFill;
    [SerializeField] private Image HPImage;
    private int HP;

    [SerializeField] private GameObject deathMenu;
    public bool isDead;

    // PV
    private PhotonView PV;

    private void Awake()
    {
        // set PV
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        // link data
        listener = GetComponent<AudioListener>();

        // set player info
        HP = 100;
        playerName = "GaryTheKid";

        if (!PV.IsMine)
        {
            Destroy(listener);
            Destroy(myCamera.gameObject);
            PlayerColor.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red);
            UICanvas.gameObject.SetActive(false);
        }
        else
        {
            GameController.GC.me = this.gameObject;
            gameObject.tag = "Untagged";
            HPCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            HPCanvas.transform.LookAt(GameController.GC.me.GetComponent<PlayerController>().myCamera.transform);
        }
    }

    public void takeDmg(int dmg)
    {
        if (!isDead)
        {
            HP -= dmg;

            // check if the player has died
            if (HP < 0)
            {
                HP = 0;
                isDead = true;
                GetComponent<Animator>().SetBool("isDead", true);
                deathMenu.SetActive(true);
            }

            HPText.text = "HP == " + HP + " ==";
            HPBarFill.fillAmount = HP / 100f;
            HPImage.fillAmount = HP / 100f;
        }
    }
}
