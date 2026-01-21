//using ZkFingerprintBridge;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace HRsystem.Services
//{
//    public class FingerprintService
//    {
//        private ZKTecoConnection _zkConnection;
//        private string _deviceIP = "192.168.1.21"; // Change to your device IP
//        private int _devicePort = 4370;

//        public FingerprintService()
//        {
//            _zkConnection = new ZKTecoConnection();
//        }

//        public FingerprintService(string deviceIP, int devicePort = 4370)
//        {
//            _zkConnection = new ZKTecoConnection();
//            _deviceIP = deviceIP;
//            _devicePort = devicePort;
//        }

//        /// <summary>
//        /// Connect to the fingerprint device
//        /// </summary>
//        public bool Connect()
//        {
//            return _zkConnection.ConnectToDevice(_deviceIP, _devicePort);
//        }

//        /// <summary>
//        /// Disconnect from the device
//        /// </summary>
//        public void Disconnect()
//        {
//            _zkConnection.DisconnectDevice();
//        }

//        /// <summary>
//        /// Check if connected
//        /// </summary>
//        public bool IsConnected()
//        {
//            return _zkConnection.IsConnected();
//        }

//        /// <summary>
//        /// Get device information
//        /// </summary>
//        public void GetDeviceInfo()
//        {
//            _zkConnection.GetDeviceInfo();
//        }

//        /// <summary>
//        /// Sync attendance logs from device
//        /// </summary>
//        public List<AttendanceLog> SyncAttendanceLogs()
//        {
//            var logs = new List<AttendanceLog>();

//            if (!_zkConnection.IsConnected())
//            {
//                Console.WriteLine("Device not connected!");
//                return logs;
//            }

//            // Here you would implement the logic to read logs
//            // and convert them to your AttendanceLog model
//            _zkConnection.GetAttendanceLogs();

//            return logs;
//        }

//        /// <summary>
//        /// Get all users from device
//        /// </summary>
//        public void GetAllUsers()
//        {
//            _zkConnection.GetAllUsers();
//        }

//        /// <summary>
//        /// Test device connection with beep
//        /// </summary>
//        public void TestConnection()
//        {
//            if (_zkConnection.IsConnected())
//            {
//                Console.WriteLine("Device test successful - beep sent!");
//            }
//            else
//            {
//                Console.WriteLine("Device not connected!");
//            }
//        }

//        /// <summary>
//        /// Clear all attendance logs from device
//        /// </summary>
        

//        /// <summary>
//        /// Get device status
//        /// </summary>
//        public void GetStatus()
//        {
//            _zkConnection.GetDeviceStatus();
//        }
//    }

//    // Model for attendance logs
//    public class AttendanceLog
//    {
//        public string EmployeeId { get; set; }
//        public DateTime LogTime { get; set; }
//        public string VerifyMode { get; set; }
//        public string InOutMode { get; set; }
//    }
//}