using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

public class SimpleOSC : MonoBehaviour {

	//create queue
	Queue packetQ;
	public bool clientConnect = false;
	private Dictionary<string,OSCClient> clients = new Dictionary<string, OSCClient>();
	public int queueSize = 25;
	public void CreateServer(string serverId, int port)
	{
		OSCServer server = new OSCServer(port);
		server.PacketReceivedEvent += HandlePacketReceivedEvent;
	}

	void HandlePacketReceivedEvent (OSCServer sender, OSCPacket packet) {
		packetQ.Enqueue(packet);
	}

	void Awake(){
		clientConnect = true;
		Queue q = new Queue(queueSize);
		//create a thread safe queue;
		packetQ = Queue.Synchronized(q);
		CreateServer("SimpleOSC",6666);
	}

	// Update is called once per frame
	void Update () {
		while (packetQ.Count > 0){
			OSCPacket packet = (OSCPacket) packetQ.Dequeue();
			Debug.Log("got packet from "+packet.Address+" at time "+Time.frameCount);
			foreach(var data in packet.Data){
				Debug.Log("Data: "+data.ToString()+ " type: "+data.GetType().Name);
			}
		}
	}
}
