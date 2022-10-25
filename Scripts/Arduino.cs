using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Text;
using System;

public class Arduino : MonoBehaviour {

    private SerialPort port;
    private Thread t;

    bool TheadEnd = false;
    bool KeepReceived = true;
    public int sensorX = 0;
    public int sensorY = 0;
    public float steerInput = 0;
    public float forwardInput = 0;
    public bool isopen = false;
    public int isjump = 0;
    public string portstring = "COM3";
    public char CharStart = '*';
    public char CharEnd = '#';

    void Start () {
        port = new SerialPort(portstring, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        port.ReadTimeout = 100000;
        port.Open();

        t = new Thread(new ThreadStart(Cal));
        t.Start();
               
        }
       
        // Update is called once per frame
        void Update () {
               
        }

    void FixedUpdate() {
        steerInput = sensorX / 512.0f - 1.0f;
        forwardInput = 1.0f-sensorY / 512.0f;
        isopen = port.IsOpen;
    }
   
    public void DataResolve(string Data) {
        string[] sArray = Data.Split(',');
        sensorX = int.Parse(sArray[0]);
        sensorY = int.Parse(sArray[1]);
        isjump = int.Parse(sArray[2]);
   
   
    }

    public int CharToASCII(char a) {
        int b = 0;
        b = Convert.ToInt32(a);
        return b;
   
    }
    public string ASCIIToChar(int a) {
        string b = "";
        b = Convert.ToChar(a).ToString();
        return b;
    }
    void OnDestroy() { port.Close(); }
    void Cal()
    {
        TheadEnd = false;
        if (port.IsOpen)
        {
            List<byte> _byteData = new List<byte>();
            bool start = false;
            while (KeepReceived) {
                try {
                    byte[] readBuffer = new byte[port.ReadBufferSize + 1];
                    int count = port.Read(readBuffer, 0, port.ReadBufferSize);
                    for (int i = 0; i < count; i++)
                    {
                        if (start) 
						{
							_byteData.Add(readBuffer[i]);
							Debug.Log(readBuffer[i]);
						}
                        if (readBuffer[i] == CharToASCII(CharStart)) { start = true; }
                        else if (readBuffer[i] == CharToASCII(CharEnd)) {
                            if (start)
                            {
                                string temp = Encoding.ASCII.GetString(_byteData.ToArray(), 0, _byteData.Count - 1);
                                _byteData.Clear();
                                DataResolve(temp);
                                temp = "";
                                start = false;
                            }
							else{
								OnDestroy();
							}
                        }  
                    }
               
                }
                catch (TimeoutException) { }
            }
        
        }
        TheadEnd = true;

    }

}
