using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanelContainer : MonoBehaviour
{
    [SerializeField] private GameObject playerListingGameObject;

    public List<PlayerPanel> playerPanels = new List<PlayerPanel>();


    public void PlayerJoinedInterface(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)
            return;
        PlayerLeftInterface(photonPlayer);

        GameObject playerPanelsObj = Instantiate(playerListingGameObject);
        playerPanelsObj.transform.SetParent(transform, false);

        PlayerPanel playerpanels = playerPanelsObj.GetComponent<PlayerPanel>();
        playerpanels.ApplyPhotonPlayer(photonPlayer);

        playerPanels.Add(playerpanels);
    }

    public void PlayerLeftInterface(PhotonPlayer p)
    {
            int index = playerPanels.FindIndex(x => x.photonPlayer == p);
            if (index != -1)
            {
                Destroy(playerPanels[index].gameObject);
                playerPanels.RemoveAt(index);
            }
    }
}
