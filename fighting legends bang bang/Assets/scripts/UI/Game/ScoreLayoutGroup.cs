using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLayoutGroup : MonoBehaviour
{

    public GameObject scrollView;
    public GameObject playerListingGroup;
    public GameObject playerListing;
    public List<ScoreListing> listings = new List<ScoreListing>();
    private bool ready;

    // Use this for initialization
    void Start()
    {
        scrollView.SetActive(false);
        ScoreManager.Instance.scoreLayout = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !ready)
        {
            ready = true;
            ScoreManager.Instance.view.RPC("RPC_SetPlayerReady", PhotonTargets.All, PhotonNetwork.player);
        }
    }

    public void CreateScoreScreen(List<ScoreManager.Score> scores)
    {
        scrollView.SetActive(true);

        foreach (ScoreManager.Score score in scores)
        {
            var obj = Instantiate(playerListing, playerListingGroup.transform, false);
            ScoreListing scorel = obj.GetComponent<ScoreListing>();
            scorel.SetText(score.Name, score.CharName, score.place, score.color);
            scorel.AddData(String.Format("Total kills: {0}", score.kills));
            scorel.AddData(String.Format("Total deaths: {0}", score.deaths));
            scorel.AddData(String.Format("Total damage done: {0}", score.damageDone));
            scorel.AddData(String.Format("Total ults used: {0}", score.ultsUsed));
            scorel.AddData(String.Format("Total ult damage done: {0}", score.damageDoneWithUlt));
            scorel.AddData(String.Format("Total damage blocked: {0}", score.damageBlocked));
            scorel.AddData(String.Format("Total damage blocked: {0}", score.damageHealed));
            scorel.AddData(String.Format("Total damage healed: {0}", score.damageHealed));
            scorel.AddData(String.Format("Total damage taken: {0}", score.damageTaken));
            scorel.id = score.id;
            listings.Add(scorel);

        }
    }
}
