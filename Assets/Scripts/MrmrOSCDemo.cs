using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

public class MrmrOSCDemo : MonoBehaviour {

	//create queue
	Queue packetQ;
	public bool clientConnect = false;
	private Dictionary<string,OSCClient> clients = new Dictionary<string, OSCClient>();
	public int queueSize = 25;
	public GameObject cubeGrid;
	private List<Transform> cubes = new List<Transform>();

	public void CreateServer(string serverId, int port) {
		OSCServer server = new OSCServer(port);
		server.PacketReceivedEvent += HandlePacketReceivedEvent;
	}

	void HandlePacketReceivedEvent (OSCServer sender, OSCPacket packet) {
		packetQ.Enqueue(packet);
	}

	void Awake(){
		//add all cubes (including parent, unfortunately) to cubes list.
		cubeGrid.GetComponentsInChildren<Transform>(cubes);
		//get rid of parent
		cubes.Remove(cubeGrid.transform);
		clientConnect = true;
		Queue q = new Queue(queueSize);
		//create a thread safe queue;
		packetQ = Queue.Synchronized(q);
		CreateServer("mrmr-QCP",6666);
	}

	// Update is called once per frame
	void Update () {
		while (packetQ.Count > 0){
			OSCPacket packet = (OSCPacket) packetQ.Dequeue();
			if (packet.Address.StartsWith("/mrmr")){
				string[] path = packet.Address.Split('/');
				int oscInput = 0;
				//somebody hit a button
				if (path.Length >= 4 && path[2].Equals("pushbutton")){
					oscInput = int.Parse(path[3]);
					int data = (int)packet.Data[0];
					MeshRenderer mr = cubes[oscInput].gameObject.GetComponent<MeshRenderer>();
					if (data == 0){
						mr.material = Resources.Load<Material>("Materials/Off");
					} else {
						mr.material = Resources.Load<Material>("Materials/On");
					}
				}
				//somebody used a slider
				//somebody hit a button
				if (path.Length >= 5 && path[2].Equals("slider")){
					oscInput = int.Parse(path[4]);
					//slider data is from 0-1000
					int data = (int)packet.Data[0];
					foreach (Transform cube in cubes){
						if (oscInput == 8){ //rotate cubes
							float yRot = ((float)data/1000.0f) * 360.0f;
							Vector3 angles = cube.localEulerAngles;
							angles.y = yRot;
							cube.localEulerAngles = angles;
						}
						if (oscInput == 9){ //resize cubes
							float scale = ((float)data/1000.0f) + 0.5f;
							cube.localScale = new Vector3(scale, scale, scale);
						}
					}
				}
			} else {
				Debug.Log("unrecognized OSC packet from "+packet.Address);
			}
			//Debug.Log("got packet from "+packet.Address+" at time "+Time.frameCount);
		}
	}
}
