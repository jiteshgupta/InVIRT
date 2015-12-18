

// This script is intended to be attached to a Game Object.  It will receive
// XML data from the NatNet UnitySample application and notify any listening
// objects via the PacketNotification delegate.

using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public delegate void PacketReceivedHandler(object sender, string PacketData);

public class SlipStream : MonoBehaviour
{
	public string IP = "127.0.0.1";
	public int Port  = 16000;
	public event PacketReceivedHandler PacketNotification;

	private IPEndPoint mRemoteIpEndPoint;
	private Socket     mListener;
	private byte[]     mReceiveBuffer;
	private string     mPacket;
	private int        mPreviousSubPacketIndex = 0;
	private const int  kMaxSubPacketSize       = 1400;
	//private static SlipStream _instance;
	/*
	#region Singleton Creation
	public static SlipStream instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<SlipStream>();
				
				//Tell unity not to destroy this object when loading a new scene.
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	void Awake() 
	{	
		if(_instance == null)
		{
			//If I am the first instance, make me the Singleton.
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find another reference in scene, destroy it.
			if(this != _instance)
				Destroy(this.gameObject);
		}
	}
	#endregion
	*/
	void Start()
	{
		mReceiveBuffer = new byte[kMaxSubPacketSize];
		mPacket        = System.String.Empty;
		mRemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
		mListener = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		mListener.Bind (mRemoteIpEndPoint);
        mListener.Blocking          = false;
		mListener.ReceiveBufferSize = 128*1024;
	}
 
	public void UDPRead()
	{
		try
		{
			int bytesReceived = mListener.Receive(mReceiveBuffer);
			
			int maxSubPacketProcess = 200;
			
			while(bytesReceived>0 && maxSubPacketProcess>0)
			{
				//== ensure header is present ==--
				if(bytesReceived>=2)
				{
					int  subPacketIndex = mReceiveBuffer[0];
					bool lastPacket     = mReceiveBuffer[1]==1;
					
					if(subPacketIndex==0)
					{
						mPacket = System.String.Empty;
					}
					
					if(subPacketIndex==0 || subPacketIndex==mPreviousSubPacketIndex+1)
					{
						mPacket += Encoding.ASCII.GetString(mReceiveBuffer, 2, bytesReceived-2);
						
						mPreviousSubPacketIndex = subPacketIndex;
						
						if(lastPacket)
						{
							//== ok packet has been created from sub packets and is complete ==--
							//== notify listeners ==--
							if(PacketNotification!=null) {
								PacketNotification(this, mPacket);
							}
						}
					}			
				}
				
				bytesReceived = mListener.Receive(mReceiveBuffer);
				
				//== time this out of packets are coming in faster than we can process ==--
				maxSubPacketProcess--;
			}
		}
		catch(System.Exception ex)
		{}
	}
 
	void Update()
	{
		UDPRead();
	}

	void OnDestroy(){
		mListener.Close();
	}

    void OnApplicationQuit() {
        mListener.Close();
    }
}
