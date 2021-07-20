using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerWeaponController : MonoBehaviour
{
    private PhotonView PV;

    public int AttackDmg;
    public float attackSpeed;

    public GameObject target;
    [SerializeField] private Image targetConfirmImg;

    [SerializeField] private Image CDfill;
    private bool attackCD;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.gameObject.CompareTag("player"))
        {
            if (target == null)
            {
                target = other.gameObject.transform.parent.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent.gameObject.CompareTag("player"))
        {
            target = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlayerController>().isDead)
        {
            if (Input.GetMouseButtonDown(0) && PV.IsMine)
            {
                GetComponent<Animator>().SetTrigger("isAttacking");

                if (target != null)
                {
                    targetConfirmImg.gameObject.SetActive(true);
                    if (!attackCD)
                    {
                        // TODO: Add attack animation

                        PV.RPC("attack", RpcTarget.All, AttackDmg);
                    }
                }
            }

            if (target != null)
            {
                targetConfirmImg.gameObject.SetActive(false);
            }
        }   
    }


    [PunRPC]
    void attack(int dmg)
    {
        attackCD = true;
        target.GetComponent<PlayerController>().takeDmg(dmg);
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        // count the cool down
        var counter = 0f;
        while (counter < attackSpeed)
        {
            counter += Time.deltaTime;
            CDfill.fillAmount = counter / attackSpeed;
            yield return new WaitForEndOfFrame();
        }

        // reset
        counter = 0;
        CDfill.fillAmount = 0;
        attackCD = false;
    }
}
