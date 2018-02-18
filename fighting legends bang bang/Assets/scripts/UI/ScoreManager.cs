using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public class Score
    {
        public PhotonPlayer phoPlayer;
        public PlayerBase pb;
        public string Name;
        public string CharName;
        public int id;
        public Color color = new Color(0, 0, 0);
        public int kills;
        public int deaths;
        public int place;
        public int damageTaken;
        public int damageDone;
        public int damageHealed;
        public int damageBlocked;
        public int damageDoneWithUlt;
        public int ultsUsed;
        public bool ready;

        public override string ToString()
        {
            string s = "";

            s = String.Format("n:{0},cn:{1},r:{2},g:{3},b:{4},k:{5},d:{6},p:{7},dt:{8},dd:{9},dh:{10},db:{11},du:{12},uu:{13},i:{14}",
                Name, CharName, color.r, color.g, color.b, kills, deaths, place, damageTaken, damageDone, damageHealed, damageBlocked, damageDoneWithUlt, ultsUsed, id);

            return s;
        }
    }

    public static ScoreManager Instance;
    public List<Score> players = new List<Score>();
    public PhotonView view;
    private int placesLeft;
    public ScoreLayoutGroup scoreLayout;
    private int totalReady;

    private void Awake()
    {
        Instance = this;
        view = GetComponent<PhotonView>();

    }

    public List<Score> stringToList(string s)
    {
        char[] delimiters = new char[] { '/' };
        List<Score> scs = new List<Score>();
        var scores = s.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        foreach (string score in scores)
        {
            Score sc = new Score();
            var data = score.Split(',');
            foreach (string dat in data)
            {
                var idata = dat.Split(':');
                switch (idata[0])
                {
                    case "n":
                        sc.Name = idata[1];
                        break;
                    case "cn":
                        sc.CharName = idata[1];
                        break;
                    case "r":
                        sc.color.r = float.Parse(idata[1]);
                        break;
                    case "g":
                        sc.color.g = float.Parse(idata[1]);
                        break;
                    case "b":
                        sc.color.b = float.Parse(idata[1]);
                        break;
                    case "k":
                        sc.kills = int.Parse(idata[1]);
                        break;
                    case "d":
                        sc.deaths = int.Parse(idata[1]);
                        break;
                    case "p":
                        sc.place = int.Parse(idata[1]);
                        break;
                    case "dt":
                        sc.damageTaken = int.Parse(idata[1]);
                        break;
                    case "dd":
                        sc.damageDone = int.Parse(idata[1]);
                        break;
                    case "dh":
                        sc.damageHealed = int.Parse(idata[1]);
                        break;
                    case "db":
                        sc.damageBlocked = int.Parse(idata[1]);
                        break;
                    case "du":
                        sc.damageDoneWithUlt = int.Parse(idata[1]);
                        break;
                    case "uu":
                        sc.ultsUsed = int.Parse(idata[1]);
                        break;
                    case "i":
                        sc.id = int.Parse(idata[1]);
                        break;
                }
            }
            scs.Add(sc);
        }

        return scs;
    }

    public void AddPlayer(PhotonPlayer p, PlayerBase pb)
    {
        print(p);
        totalReady = 0;

        Score s = new Score();
        s.phoPlayer = p;
        s.pb = pb;
        s.Name = p.NickName;
        s.CharName = GameManager.Instance.charNames[(int)p.CustomProperties["charId"]].ToUpper();
        s.color = new Color((float)p.CustomProperties["pColorR"], (float)p.CustomProperties["pColorG"], (float)p.CustomProperties["pColorB"]);
        s.kills = 0;
        s.deaths = 0;
        s.place = 0;
        s.damageTaken = 0;
        s.damageDone = 0;
        s.damageHealed = 0;
        s.damageBlocked = 0;
        s.damageDoneWithUlt = 0;
        s.ultsUsed = 0;
        s.id = p.ID;
        placesLeft++;

        players.Add(s);
    }

    public void PlayerLost(PhotonPlayer pp)
    {

        var p = players.FindIndex(x => x.phoPlayer == pp);
        players[p].place = placesLeft;

        placesLeft--;

        if (placesLeft == 1)
        {
            foreach (Score player in players)
            {
                if (PhotonNetwork.playerList.Contains(player.phoPlayer))
                {
                    string s = "";

                    foreach (var score in players)
                    {
                        s += score + "/";
                    }

                    view.RPC("RPC_EndingGame", player.phoPlayer, s);
                }
            }
        }
    }

    #region RPC region

    [PunRPC]
    public void RPC_AddKills(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).kills += i;
    }

    [PunRPC]
    public void RPC_AddDeath(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).deaths += i;
    }

    [PunRPC]
    public void RPC_AddDamageTaken(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).damageTaken += i;
    }

    [PunRPC]
    public void RPC_AddDamaDone(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).damageDone += i;
    }

    [PunRPC]
    public void RPC_AddDamageHealed(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).damageHealed += i;
    }

    [PunRPC]
    public void RPC_AddDamageBlocked(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).damageBlocked += i;
    }

    [PunRPC]
    public void RPC_AddDamageDoneWithUlt(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).damageDoneWithUlt += i;
    }

    [PunRPC]
    public void RPC_AddUltsUsed(PhotonPlayer pp, int i)
    {
        players.Find(x => x.phoPlayer == pp).ultsUsed += i;
    }

    [PunRPC]
    public void RPC_EndingGame(string s)
    {
        print("ending game");

        FindObjectOfType<ScoreLayoutGroup>().CreateScoreScreen(stringToList(s));

    }

    [PunRPC]
    public void RPC_SetPlayerReady(PhotonPlayer p)
    {
        scoreLayout.listings.Find(x => x.id == p.ID).SetReady();

        if (PhotonNetwork.isMasterClient)
        {
            totalReady++;

            if (totalReady == PhotonNetwork.playerList.Length)
            {
                CurrentGameManager.Instance.phoView.RPC("RPC_ReturnToRoom", PhotonTargets.All);
            }
        }
    }
    #endregion


}
