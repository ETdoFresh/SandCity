using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLatency : NetworkBehaviour
{
    NetworkClient _networkClient;
    Text _latencyText;
    int _latency;

    public override void OnStartLocalPlayer()
    {
        _networkClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
        _latencyText = GameObject.Find("LatencyText").GetComponent<Text>();
    }

    void Update()
    {
        ShowLatency();
    }

    void ShowLatency()
    {
        if (isLocalPlayer)
        {
            _latency = _networkClient.GetRTT();
            _latencyText.text = _latency.ToString();
        }
    }
}
