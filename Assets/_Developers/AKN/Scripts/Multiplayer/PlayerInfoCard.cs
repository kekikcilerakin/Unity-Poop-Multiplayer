using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Steamworks;

public class PlayerInfoCard : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionId;
    public ulong PlayerSteamId;
    private bool isAvatarReceived;

    public TMP_Text PlayerNameText;
    public RawImage PlayerAvatar;
    public TMP_Text PlayerReadyText;
    public bool Ready;

    protected Callback<AvatarImageLoaded_t> AvatarLoaded;

    private void Start()
    {
        AvatarLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarLoaded);
    }

    public void ChangeReadyStatus()
    {
        if (Ready)
        {
            PlayerReadyText.text = "Ready";
        }
        else
        {
            PlayerReadyText.text = "";
        }
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus();

        if (!isAvatarReceived)
            GetPlayerAvatar();
    }

    private void GetPlayerAvatar()
    {
        int AvatarId = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamId);

        if (AvatarId == -1) { return; }

        PlayerAvatar.texture = GetSteamAvatarAsTexture(AvatarId);
    }

    private void OnAvatarLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamId)
        {
            PlayerAvatar.texture = GetSteamAvatarAsTexture(callback.m_iImage);
        }
        else //another player
        {
            return;
        }
    }

    private Texture2D GetSteamAvatarAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        isAvatarReceived = true;
        return texture;
    }
}
