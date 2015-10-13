using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using System.Collections.Generic;
using UnityOSC;

[RequireComponent(typeof(CarController))]
public class OSCCarControl : MonoBehaviour
{
	private CarController m_Car; // the car controller we want to use
	//create queue
	Queue packetQ;
	public bool clientConnect = false;
	private Dictionary<string,OSCClient> clients = new Dictionary<string, OSCClient> ();
	public int queueSize = 25;
	float turn = 0;
	float move = 0;
	public void CreateServer (string serverId, int port)
	{
		OSCServer server = new OSCServer (port);
		server.PacketReceivedEvent += HandlePacketReceivedEvent;
		print ("server created");
	}
		
	void HandlePacketReceivedEvent (OSCServer sender, OSCPacket packet)
	{
		//print ("got packet");
		packetQ.Enqueue (packet);
	}
		
	void Awake ()
	{
		print ("awake");
		// get the car controller
		m_Car = GetComponent<CarController> ();
		clientConnect = true;
		Queue q = new Queue (queueSize);
		//create a thread safe queue;
		packetQ = Queue.Synchronized (q);
		CreateServer ("mrmr-accel", 6666);
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}

	 void Update ()
	{
		bool gotData = false;
		while (packetQ.Count > 0) {
			gotData=true;
			OSCPacket packet = (OSCPacket)packetQ.Dequeue ();
			if (packet.Address.Contains ("/angle/3")) {
				int data = (int)packet.Data[0];
				//value will be 90-270
				turn =  map ((float) data, 90f,270f,1f,-1f);
				m_Car.Move(move,0.5f,0.5f,0);
				//print ("got value "+turn+ " for "+ packet.Address);
			} else if (packet.Address.Contains ("/accelerometerX")) {
				int data = (int)packet.Data[0];
				//value will be 500-1000, 750 = neutral
				move = Mathf.Clamp (map ((float) data, 250,1000,1f,-1f),-1,1);
				//print ("got value "+move+ " for "+ packet.Address);
			}
			//Debug.Log("got packet from "+packet.Address+" at time "+Time.frameCount);
		}
		if (gotData){
			//print ("turn="+turn+ " and move="+move);
			m_Car.Move(turn,move,move,0);
		}
	}
}
