using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour
{

    [SerializeField] private GameObject roomListingGameObject;

    private List<RoomListing> roomListingButtons = new List<RoomListing>();

    private void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = roomListingButtons.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject roomListingObj = Instantiate(roomListingGameObject);
                roomListingObj.transform.SetParent(transform, false);

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                roomListingButtons.Add(roomListing);

                index = (roomListingButtons.Count - 1);
            }
        }

        if (index != -1)
        {
            RoomListing roomListing = roomListingButtons[index];
            roomListing.SetRoomNameText(room);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();

        foreach (RoomListing listing in roomListingButtons)
        {
            if (!listing.Updated)
                removeRooms.Add(listing);
            else
                listing.Updated = false;
        }

        foreach (RoomListing removeRoom in removeRooms)
        {
            GameObject roomListingObj = removeRoom.gameObject;
            roomListingButtons.Remove(removeRoom);
            Destroy(roomListingObj);
        }
    }

}
