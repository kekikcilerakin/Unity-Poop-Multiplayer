using Unity.Netcode;
using UnityEngine;

public class NewBehaviourScript : NetworkManager
{
    public void Host()
    {
        StartHost();
    }

    public void Client()
    {
        StartClient();
    }
}
