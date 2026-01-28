using System;
using System.Collections.Generic;
using static ZkFingerprintBridge.ZKTecoConnection;

namespace ZkFingerprintBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            ZKTecoConnection zk = new ZKTecoConnection();

            // Replace with your device's IP address
            string deviceIP = "192.168.1.21";
            int port = 4370;

            Console.WriteLine("Attempting to connect to ZKTeco device...\n");

            if (zk.ConnectToDevice(deviceIP, port))
            {
                // Connection successful - perform operations
                zk.GetDeviceInfo();

                // Make device beep to confirm connection
                Console.WriteLine("\nTesting device beep...");
                

                // Get device status
                zk.GetDeviceStatus();

                // Get all users
                zk.GetAllUsers();

                // Get attendance logs
                List<BasmaEntry> basmaEntries = zk.GetAttendanceLogs(); // now u have the json list
                // zk.InsertBasmaRecord();
                
                // Disconnect
                zk.DisconnectDevice();
            }
            else
            {
                Console.WriteLine("Failed to connect to device. Please check:");
                Console.WriteLine("1. Device IP address is correct");
                Console.WriteLine("2. Device is powered on and connected to network");
                Console.WriteLine("3. Firewall is not blocking the connection");
                Console.WriteLine("4. Platform target is set to x86");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}