using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using zkemkeeper;

namespace ZkFingerprintBridge
{
    public class ZKTecoConnection
    {
        private CZKEM axCZKEM1 = new CZKEM();
        private bool bIsConnected = false;
        private int iMachineNumber = 1;
        private readonly string _connectionString =
            "Server=localhost;Database=HR_DB;Trusted_Connection=True;";

        public void InsertBasmaRecord(string name, int age)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql =
                    "INSERT INTO BasmaRecords (Name, Age) VALUES (@Name, @Age)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Age", age);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Connect to ZKTeco device via network
        /// </summary>
        /// <param name="ipAddress">Device IP address (e.g., "192.168.1.201")</param>
        /// <param name="port">Device port (default: 4370)</param>
        /// <returns>True if connected successfully</returns>
        public bool ConnectToDevice(string ipAddress, int port = 4370)
        {
            try
            {
                bIsConnected = axCZKEM1.Connect_Net(ipAddress, port);

                if (bIsConnected)
                {
                    Console.WriteLine("Connected successfully!");
                    axCZKEM1.RegEvent(iMachineNumber, 65535); // Register all events
                    return true;
                }
                else
                {
                    int errorCode = 0;
                    axCZKEM1.GetLastError(ref errorCode);
                    Console.WriteLine($"Connection failed. Error code: {errorCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during connection: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Connect to ZKTeco device via serial port (COM port)
        /// </summary>
        /// <param name="comPort">COM port number (e.g., 1 for COM1)</param>
        /// <param name="machineNumber">Machine number (default: 1)</param>
        /// <param name="baudRate">Baud rate (e.g., 115200)</param>
        /// <returns>True if connected successfully</returns>
        public bool ConnectToDeviceSerial(int comPort, int machineNumber = 1, int baudRate = 115200)
        {
            try
            {
                iMachineNumber = machineNumber;
                bIsConnected = axCZKEM1.Connect_Com(comPort, machineNumber, baudRate);

                if (bIsConnected)
                {
                    Console.WriteLine("Connected via Serial successfully!");
                    return true;
                }
                else
                {
                    int errorCode = 0;
                    axCZKEM1.GetLastError(ref errorCode);
                    Console.WriteLine($"Serial connection failed. Error code: {errorCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during serial connection: {ex.Message}");
                return false;
            }
        }
        private string ConvertToArabic(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            try
            {

                byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
                return Encoding.GetEncoding(1256).GetString(bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encoding error: {ex.Message}");
                return text;
            }
        }

        /// <summary>
        /// Disconnect from the device
        /// </summary>
        public void DisconnectDevice()
        {
            if (bIsConnected)
            {
                axCZKEM1.Disconnect();
                bIsConnected = false;
                Console.WriteLine("Disconnected successfully!");
            }
        }

        /// <summary>
        /// Get device information (name, serial number, firmware version)
        /// </summary>
        public void GetDeviceInfo()
        {
            if (!bIsConnected)
            {
                Console.WriteLine("Device not connected!");
                return;
            }

            string serialNumber = "";
            string firmwareVersion = "";
            int deviceName = 0;

            axCZKEM1.GetSerialNumber(iMachineNumber, out serialNumber);
            axCZKEM1.GetFirmwareVersion(iMachineNumber, ref firmwareVersion);
            axCZKEM1.GetDeviceInfo(iMachineNumber, 1, ref deviceName);

            Console.WriteLine("=== Device Information ===");
            Console.WriteLine($"Device Name: {deviceName}");
            Console.WriteLine($"Serial Number: {serialNumber}");
            Console.WriteLine($"Firmware Version: {firmwareVersion}");
            Console.WriteLine("==========================");
        }


        public class BasmaEntry
        {
            public int Id { get; set; }
            public DateTime Time { get; set; }
            public bool InOutMode { get; set; }

        }
        
        /// <summary>
        /// Get all attendance logs from the device
        /// </summary>
        public List<BasmaEntry> GetAttendanceLogs()
        {
            if (!bIsConnected)
            {
                Console.WriteLine("Device not connected!");
                return null;
            }

            Console.WriteLine("Reading attendance logs...");
            

            List<BasmaEntry> basmaEntries = new List<BasmaEntry>();
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))
            {
                string enrollNumber = "";
                int verifyMode = 0;
                int inOutMode = 0;
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
                int workCode = 0;
                int logCount = 0;
                Console.WriteLine("\n=== Attendance Logs ===");
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out enrollNumber,
                       out verifyMode, out inOutMode, out year, out month, out day,
                       out hour, out minute, out second, ref workCode))
                {
                    DateTime logTime = new DateTime(year, month, day, hour, minute, second);
                    string verifyModeStr = GetVerifyModeString(verifyMode);
                    string inOutModeStr = GetInOutModeString(inOutMode);
                    var basmaEntry = new BasmaEntry
                    {
                        Id = int.Parse(enrollNumber),
                        Time = logTime,
                        InOutMode = inOutMode % 2 == 0 // Even codes for Check In, odd for Check Out
                    };
                    basmaEntries.Add(basmaEntry);
                    Console.WriteLine($"User ID: {enrollNumber} | Time: {logTime:yyyy-MM-dd HH:mm:ss} | Verify: {verifyModeStr} | InOut: {inOutModeStr}");
                    logCount++;
                }

                Console.WriteLine($"Total logs retrieved: {logCount}");
                Console.WriteLine("=======================\n");
                return basmaEntries;
            }
            else
            {
                Console.WriteLine("Failed to read attendance logs.");
                return null;
            }

        }

        /// <summary>
        /// Get all registered users from the device
        /// </summary>
        public class User
        {
            public string EnrollNumber { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
            public string PrivilegeStr { get; set; }
            public string Status {  get; set; }
        }
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            if (!bIsConnected)
            {
                Console.WriteLine("Device not connected!");
                return null;
            }

            Console.WriteLine("Reading all users...");
            

            if (axCZKEM1.ReadAllUserID(iMachineNumber))
            {
                string enrollNumber = "";
                string name = "";
                string password = "";
                int privilege = 0;
                bool enabled = false;
                int userCount = 0;

                

                Console.WriteLine("\n=== Registered Users ===");
                while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out enrollNumber,
                       out name, out password, out privilege, out enabled))
                {
                    string privilegeStr = GetPrivilegeString(privilege);
                    string status = enabled ? "Enabled" : "Disabled";
                    Encoding win1256 = Encoding.GetEncoding(1256);
                    string fixedName = Encoding.UTF8.GetString(win1256.GetBytes(name));


                    var user = new User
                    {
                        EnrollNumber = enrollNumber,
                        Name = fixedName,
                        Password = password,
                        PrivilegeStr = privilegeStr,
                        Status = status,
                    };
                    users.Add(user);
                    Console.WriteLine($"ID: {enrollNumber} | Name: {name} | Privilege: {privilegeStr} | Status: {status}");
                    userCount++;
                }

                Console.WriteLine($"Total users: {userCount}");
                Console.WriteLine("========================\n");
            }
            else
            {
                Console.WriteLine("Failed to read users.");
            }
            return users;
        }

        

        /// <summary>
        /// Get device status and capacity information
        /// </summary>
        public void GetDeviceStatus()
        {
            if (!bIsConnected)
            {
                Console.WriteLine("Device not connected!");
                return;
            }

            int userCount = 0;
            int fpCount = 0;
            int logCount = 0;
            int adminCount = 0;

            axCZKEM1.GetDeviceStatus(iMachineNumber, 2, ref userCount); // User count
            axCZKEM1.GetDeviceStatus(iMachineNumber, 1, ref fpCount);   // Fingerprint count
            axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref logCount);  // Log count
            axCZKEM1.GetDeviceStatus(iMachineNumber, 21, ref adminCount); // Admin count

            Console.WriteLine("\n=== Device Status ===");
            Console.WriteLine($"Total Users: {userCount}");
            Console.WriteLine($"Total Fingerprints: {fpCount}");
            Console.WriteLine($"Total Logs: {logCount}");
            Console.WriteLine($"Total Admins: {adminCount}");
            Console.WriteLine("=====================\n");
        }

        // Helper methods to convert codes to readable strings
        private string GetVerifyModeString(int mode)
        {
            switch (mode)
            {
                case 0: return "Password";
                case 1: return "Fingerprint";
                case 2: return "Card";
                case 3: return "Fingerprint or Password";
                case 4: return "Fingerprint or Card";
                case 5: return "Fingerprint and Password";
                case 6: return "Fingerprint and Card";
                case 15: return "Face";
                default: return $"Unknown ({mode})";
            }
        }

        private string GetInOutModeString(int mode)
        {
            switch (mode)
            {
                case 0: return "Check In";
                case 1: return "Check Out";
                case 2: return "Break Out";
                case 3: return "Break In";
                case 4: return "Overtime In";
                case 5: return "Overtime Out";
                default: return $"Unknown ({mode})";
            }
        }

        private string GetPrivilegeString(int privilege)
        {
            switch (privilege)
            {
                case 0: return "User";
                case 1: return "Enroller";
                case 2: return "Administrator";
                case 3: return "Super Administrator";
                default: return $"Unknown ({privilege})";
            }
        }

        /// <summary>
        /// Check if device is currently connected
        /// </summary>
        public bool IsConnected()
        {
            return bIsConnected;
        }
    }
}