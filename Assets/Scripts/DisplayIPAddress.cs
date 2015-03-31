using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.UI;
public class DisplayIPAddress : MonoBehaviour {

	public int port = 6666;
	// Use this for initialization
	void Start () {
		Text text = GetComponent<Text>();
		string ip = Network.player.ipAddress;
		text.text = "Connect to "+ip+":"+port;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
