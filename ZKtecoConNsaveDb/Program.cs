using System;
using System.Collections.Generic;
using System.IO;
using ZkFingerprintBridge;
using static ZkFingerprintBridge.ZKTecoConnection;

namespace ZkFingerprintRunner
{
    class Program
    {
        static void Main()
        {

            ZKTecoConnection zk = new ZKTecoConnection();

            try
            {
                zk.Log("Job started");

                if (zk.ConnectToDevice("192.168.1.21", 4370))
                {
                    zk.Log("Connected to device");

                    var logs = zk.GetAttendanceLogs();

                    if (logs != null && logs.Count > 0)
                    {
                        zk.InsertBasmaRecords(logs);
                        zk.Log($"Pulled {logs.Count} attendance records");
                    }
                    else
                    {
                        zk.Log("No attendance records found");
                    }
                }
                else
                {
                    zk.Log("Connection FAILED");
                }
            }
            catch (Exception ex)
            {
                zk.Log(ex.ToString());
            }
            finally
            {
                zk.DisconnectDevice();
                zk.Log("Disconnected");
            }

        }



    }
}
