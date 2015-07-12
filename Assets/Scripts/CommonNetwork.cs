using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommonNetwork : NetworkManager
{
    public void HostGame()
    {
        SetNetworkPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetNetworkAddress();
        SetNetworkPort();
        NetworkManager.singleton.StartClient();
    }

    void SetNetworkAddress()
    {
        string networkAddress = GameObject.Find("JoinInput").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = networkAddress;
    }

    void SetNetworkPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
            SetupMenuSceneButtons();
        else
            SetupOtherSceneButtons();
    }

    void SetupMenuSceneButtons()
    {
        GameObject.Find("HostButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("HostButton").GetComponent<Button>().onClick.AddListener(HostGame);

        GameObject.Find("JoinButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("JoinButton").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("DisconnectButton").GetComponent<Button>().onClick.AddListener(DisconnectGame);
    }

    void DisconnectGame()
    {
        Debug.Log("Disconnecting Host");
        NetworkManager.singleton.StopHost();
        NetworkServer.Reset();
    }

}
