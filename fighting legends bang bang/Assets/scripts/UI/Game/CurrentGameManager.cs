using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurrentGameManager : MonoBehaviour
{

    public static CurrentGameManager Instance;
    public PhotonView phoView;
    public Vector3[] SpawnPoints;
    private List<Vector3> SpawnPointsLeft;



    private void Awake()
    {
        Instance = this;
        phoView = GetComponent<PhotonView>();
        SpawnPointsLeft = SpawnPoints.ToList();
    }

    public Vector3 GetSpawnPoint()
    {
        Vector3 spawn = SpawnPointsLeft[Random.Range(0, SpawnPointsLeft.Count)];
        SpawnPointsLeft.Remove(spawn);

        return spawn;
    }

    public void CreatePlayer(PhotonPlayer p, Vector3 spawn)
    {

        string pre = GameManager.Instance.charPrefabs[(int)p.CustomProperties["charId"]];
        Debug.Log(pre);

        GameObject obj = PhotonNetwork.Instantiate(pre, spawn, Quaternion.identity, 0);
        PlayerBase pBase = obj.GetComponent<PlayerBase>();
        pBase.SpawnPoint = spawn;
    }

    public IEnumerator RespawnPlayer(GameObject player, Vector3 spawn)
    {
        player.transform.position = spawn;

        yield break;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LoadLevel(2);
            }
            else
            {
                phoView.RPC("RPC_ReturnToRoom", PhotonTargets.All);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 point in SpawnPoints)
        {
            Gizmos.DrawSphere(point, 1);
        }
    }


    [PunRPC]
    public void RPC_ReturnToRoom()
    {
        PhotonNetwork.LoadLevel(2);
    }
}
