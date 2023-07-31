using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    public ulong CurrentLobbyId;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager customNetworkManager;

    public GameObject HostButton;
    public TMP_Text LobbyNameText;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        customNetworkManager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, customNetworkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created succesfully!");

        customNetworkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //everyone
        HostButton.SetActive(false);
        CurrentLobbyId = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        //client
        if (NetworkServer.active) { return; }

        customNetworkManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        customNetworkManager.StartClient();
    }
}
