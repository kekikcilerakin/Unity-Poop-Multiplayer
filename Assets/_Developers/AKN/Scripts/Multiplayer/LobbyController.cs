using UnityEngine;
using Mirror;
using Steamworks;
using System.Linq;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TMP_Text LobbyNameText;

    public GameObject PlayerInfoPanel;
    public GameObject PlayerInfoCardPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyId;
    public bool IsPlayerInfoCreated = false;
    private List<PlayerInfoCard> playerInfoCards = new List<PlayerInfoCard>();
    public PlayerObjectController localPlayerController;

    public Button StartGameButton;
    public TMP_Text ReadyButtonText;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    public void ReadyPlayer()
    {
        localPlayerController.ChangeReady();
    }

    public void UpdateButton()
    {
        if (localPlayerController.Ready)
        {
            ReadyButtonText.text = "Unready";
        }
        else
        {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady()
    {
        bool allReady = false;

        foreach (PlayerObjectController player in Manager.players)
        {
            if (player.Ready)
            {
                allReady = true;
            }
            else
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            if (localPlayerController.PlayerIdNumber == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyId = Manager.GetComponent<SteamLobby>().CurrentLobbyId;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyId), "name");
    }

    public void UpdatePlayerList()
    {
        if (!IsPlayerInfoCreated) { CreateHostPlayerItem(); } //host
        if (playerInfoCards.Count < Manager.players.Count) { CreateClientPlayerItem(); }
        if (playerInfoCards.Count > Manager.players.Count) { RemovePlayerItem(); }
        if (playerInfoCards.Count == Manager.players.Count) { UpdatePlayerItem(); }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        localPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.players)
        {
            GameObject newPlayerCard = Instantiate(PlayerInfoCardPrefab) as GameObject;
            PlayerInfoCard newPlayerInfoScript = newPlayerCard.GetComponent<PlayerInfoCard>();

            newPlayerInfoScript.PlayerName = player.PlayerName;
            newPlayerInfoScript.ConnectionId = player.ConnectionId;
            newPlayerInfoScript.PlayerSteamId = player.PlayerSteamId;
            newPlayerInfoScript.Ready = player.Ready;
            newPlayerInfoScript.SetPlayerValues();

            newPlayerCard.transform.SetParent(PlayerInfoPanel.transform);
            newPlayerCard.transform.localScale = Vector3.one;

            playerInfoCards.Add(newPlayerInfoScript);
        }
        IsPlayerInfoCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.players)
        {
            if (!playerInfoCards.Any(b => b.ConnectionId == player.ConnectionId))
            {
                GameObject newPlayerCard = Instantiate(PlayerInfoCardPrefab) as GameObject;
                PlayerInfoCard newPlayerInfoScript = newPlayerCard.GetComponent<PlayerInfoCard>();

                newPlayerInfoScript.PlayerName = player.PlayerName;
                newPlayerInfoScript.ConnectionId = player.ConnectionId;
                newPlayerInfoScript.PlayerSteamId = player.PlayerSteamId;
                newPlayerInfoScript.Ready = player.Ready;
                newPlayerInfoScript.SetPlayerValues();

                newPlayerCard.transform.SetParent(PlayerInfoPanel.transform);
                newPlayerCard.transform.localScale = Vector3.one;

                playerInfoCards.Add(newPlayerInfoScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.players)
        {
            foreach (PlayerInfoCard playerInfoScript in playerInfoCards)
            {
                if (playerInfoScript.ConnectionId == player.ConnectionId)
                {
                    playerInfoScript.PlayerName = player.PlayerName;
                    playerInfoScript.Ready = player.Ready;
                    playerInfoScript.SetPlayerValues();

                    if (player == localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }

        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        List<PlayerInfoCard> playerInfoCardsToRemove = new List<PlayerInfoCard>();

        foreach (PlayerInfoCard playerInfoCard in playerInfoCards)
        {
            if (!Manager.players.Any(b => b.ConnectionId == playerInfoCard.ConnectionId))
            {
                playerInfoCardsToRemove.Add(playerInfoCard);
            }
        }
        if (playerInfoCardsToRemove.Count > 0)
        {
            foreach (PlayerInfoCard playerInfoCardToRemove in playerInfoCardsToRemove)
            {
                GameObject objectToRemove = playerInfoCardToRemove.gameObject;
                playerInfoCards.Remove(playerInfoCardToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
}
