using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour


{
    private Vector3 targetpos;
    public AudioClip audio;
    public GameObject explosion;
    private List<PlayerBase> possibleTargets;
    private PhotonView phoview;

    private float fallSpeed = 10;
    private float speedIncr = 0.1f;

    private int dmg = 50;
    private int impactDmg = 100;
    private float impactDistance = 10;

    private int bombFallingAudio = 10;
    private int bombExplodingAudio = 11;

    void Awake()
    {
        phoview = GetComponent<PhotonView>();
        AudioManager.Instance.soundSource.clip = AudioManager.Instance.sounds[bombFallingAudio];
        AudioManager.Instance.soundSource.Play();
    }

    void Update()
    {
        if (!phoview.isMine)
            return;


        if (gameObject.transform.position.y > targetpos.y)
        {
            gameObject.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            fallSpeed += speedIncr * Time.deltaTime;
        }

        if (gameObject.transform.position.y <= targetpos.y)
        {
            PhotonNetwork.Destroy(gameObject);
            List<PlayerBase> targets = possibleTargets.FindAll(x =>
                x.gameObject != null &&
                Vector3.Distance(x.transform.position, transform.position) < impactDistance &&
                !x.healthController.death);

            foreach (var t in targets)
            {
                t.photonViewer.RPC("RPC_GotAttacked", t.netPlayer, impactDmg, FindBlastDirection(t.gameObject.transform.position), 1, PhotonNetwork.player);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerBase>().photonViewer.RPC("RPC_GotAttacked", other.GetComponent<PlayerBase>().netPlayer,
            dmg, FindBlastDirection(other.gameObject.transform.position), 1, PhotonNetwork.player);
    }

    public Vector2 FindBlastDirection(Vector3 otherpos)
    {
        return new Vector2(otherpos.z < gameObject.transform.position.z ? -1 : 1, 1);
    }

    public void Setup(Vector3 targetPos, List<PlayerBase> targets, int damage, int impactdamage, float distance, float fallspeed, float speedincr)
    {
        this.targetpos = targetPos;
        this.dmg = damage;
        this.impactDmg = impactdamage;
        this.impactDistance = distance;
        this.fallSpeed = fallspeed;
        this.speedIncr = speedincr;
        this.possibleTargets = targets;
    }

    void OnDestroy()
    {
        AudioManager.Instance.soundSource.Stop();
        AudioManager.Instance.soundSource.clip = null;
        GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(expl, 1.5f);
        PlayerNetwork.Instance.photonView.RPC("PlaySound", PhotonTargets.All, bombExplodingAudio);
    }
}
