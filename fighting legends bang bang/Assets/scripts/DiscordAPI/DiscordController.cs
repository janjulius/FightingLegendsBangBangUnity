using System;
using DiscordPresence;
using UnityEngine;



public class DiscordController : MonoBehaviour
{
    public static DiscordController discord;

    public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
    public string applicationId;
    public string optionalSteamId;
    public int callbackCalls;
    public int clickCounter;
    public DiscordRpc.JoinRequest joinRequest;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    public UnityEngine.Events.UnityEvent hasResponded;


    DiscordRpc.EventHandlers handlers;

    public void InMenus()
    {
        presence.state = "In Menus";
        presence.details = "";
        presence.largeImageKey = "icon_large";
        presence.startTimestamp = 0;
        DiscordRpc.UpdatePresence(presence);
    }

    public void InRoom()
    {
        presence.state = "In Room";
        presence.details = "";
        presence.largeImageKey = "icon_large";
        presence.startTimestamp = 0;
        DiscordRpc.UpdatePresence(presence);
    }

    public void InGame(int charid)
    {
        presence.state = "Playing " + GameManager.Instance.CharacterData[charid].CharacterName;
        presence.details = "In Game";
        presence.largeImageKey = "char_"+ GameManager.Instance.CharacterData[charid].CharacterName.ToLower().Replace(" ",string.Empty);
        presence.startTimestamp = DiscordTime.TimeNow();
        DiscordRpc.UpdatePresence(presence);
    }

    public void RequestRespondYes()
    {
        Debug.Log("Discord: responding yes to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
        hasResponded.Invoke();
    }

    public void RequestRespondNo()
    {
        Debug.Log("Discord: responding no to Ask to Join request");
        DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
        hasResponded.Invoke();
    }

    public void ReadyCallback()
    {
        ++callbackCalls;
        Debug.Log("Discord: ready");
        onConnect.Invoke();
        presence.largeImageKey = "icon_large";
        DiscordRpc.UpdatePresence(presence);
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        ++callbackCalls;
        Debug.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
    }


    void Start()
    {
    }

    void Update()
    {
        DiscordRpc.RunCallbacks();
    }

    void Awake()
    {
        discord = this;
    }

    void OnEnable()
    {
        Debug.Log("Discord: init");
        callbackCalls = 0;

        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback = ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
    }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        DiscordRpc.ClearPresence();
        DiscordRpc.Shutdown();
    }

    void OnDestroy()
    {

    }
}
