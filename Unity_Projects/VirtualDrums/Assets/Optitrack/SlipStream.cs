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
	
	void Start()
	{
		mReceiveBuffer = new byte[kMaxSubPacketSize];
		mPacket        = System.String.Empty;
		mRemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
		mListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		mListener.Bind(mRemoteIpEndPoint);
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

    void OnApplicationQuit() {
        mListener.Close();
    }
}
