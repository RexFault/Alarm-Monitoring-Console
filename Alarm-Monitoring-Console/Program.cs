using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using MySql.Data.MySqlClient;

namespace Alarm_Monitoring_Console
{
    internal class Program
    {

        private const int _listenPort = 3060;
        private static Socket _listenSocket;
        private static List<Socket> _clientSockets = new List<Socket>();
        
        static void Main(string[] args)
        {

            string connString = $"Server={DBSecrets.databaseServer};Database={DBSecrets.databaseName};User ID={DBSecrets.databaseUsername};Password={DBSecrets.databasePassword};";
            MySqlConnection conn = null ;


            bool isRunning = true;
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(new IPEndPoint(IPAddress.Any, _listenPort));
            _listenSocket.Listen(10);

            ShowBanner();

            //Connect to DB
            try
            {
                conn = new MySqlConnection(connString);
                LogMessage("Trying to connect to Database...");
                conn.Open();
                LogMessage($"Connected to Database: {DBSecrets.databaseServer}/{DBSecrets.databaseName}");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
                return; //Failed to connect, quit program
            }


            Console.WriteLine($"Server started on port {_listenPort}. Polling for activity...");
            //DC09ParserTest();

            do
            {

                //Repopulate list
                List<Socket> readList = new List<Socket> { _listenSocket };
                readList.AddRange(_clientSockets);

                //Poll the sockets for data
                Socket.Select(readList, null, null, 100000);

                //Loop through the ready sockes and handle them accordingly
                foreach (Socket sock in readList)
                {
                    if (sock == _listenSocket) //There's a new Connection!
                        HandleNewConnection();
                    HandleClientData(sock, conn);
                }

                //Remove closed/invalid sockets from our tracking
                _clientSockets.RemoveAll(x => !x.Connected);

                //Main loop for the console application
                if (CheckForQuitKey())
                {
                    isRunning = false;
                }

            } while (isRunning);

            conn.Close();
        }

        private static void HandleNewConnection() {
        
            try
            {
                Socket newClient = _listenSocket.Accept();
                _clientSockets.Add(newClient);
                LogMessage($"[CONNECTED] - Client Connected from {newClient.RemoteEndPoint}");
            } catch(SocketException ex)
            {
                LogMessage($"Accept Error: {ex.Message}");
            }

        }

        private static void HandleClientData(Socket client, MySqlConnection dbConnection) {

            //We want to read up to the first \r since our messages are wrapped in 
            //\n{data}/r
            //Once we have it we process it
            MemoryStream memStream = new MemoryStream();
            string rawMessage = string.Empty;
            byte[] singleBuff = new byte[1];
            try
            {
                while (true)
                {
                    int bytesRead = client.Receive(singleBuff);
                    if (bytesRead == 0)
                    {
                        //throw new SocketException((int)SocketError.ConnectionReset);
                        LogMessage("Disconnect");
                    }

                    memStream.WriteByte(singleBuff[0]);

                    if (singleBuff[0] == '\r')//Have we reached our terminating charactter
                        break;
                }

                rawMessage = Encoding.ASCII.GetString(memStream.ToArray());

                //This is where we handle our actual recieved message:
                LogMessage($"[RECIEVED] - FROM {client.RemoteEndPoint} : {rawMessage}");
                HandleProtocolData(rawMessage, client, dbConnection);

            }
            catch (SocketException ex)
            {
                //LogMessage($"[DISCONNECTED] - Client forceful Disconnect");
                //CloseClient(client);
            }


        }

        private static void HandleProtocolData(string rawData, Socket client, MySqlConnection dbConnection)
        {
            DC09Parser parser = new DC09Parser(rawData);
            //Confirm data good
            if (parser.CheckCRCMatch())
            {
                LogMessage("CRC Good!");
                //if (parser.ProtocolID == "NULL")
                if (true) //We should always send a positive acknowlegement to messages sent without errors
                {
                    string ackMessage = DC09Parser.CreateACK(parser.MessageSeq, parser.ReceiverNum, parser.LinePrefix, parser.AccountNumber);
                    byte[] asciiBytes = Encoding.ASCII.GetBytes(ackMessage);
                    int bytesSent = 0;
                    int bytesLeft = asciiBytes.Length;
                    do
                    {
                        int bytesSentNow = client.Send(asciiBytes, bytesSent, bytesLeft, SocketFlags.None);
                        bytesSent += bytesSentNow;
                        bytesLeft -= bytesSentNow;

                    } while (bytesLeft > 0);
                    LogMessage($"ACK Sent to {client.RemoteEndPoint} for Account: {parser.AccountNumber}");
                }
            }


            Dictionary<string, string> parsedData = parser.GetDictionary();

            //Write the data to the database
            string sqlCommand = "INSERT INTO events (account_number, raw_message, protocol, receiver_number, line_prefix, message_data, message_xdata, message_timestamp) " +
                "VALUES (@account_number, @raw_message, @protocol, @receiver_number, @line_prefix, @message_data, @message_xdata, @message_timestamp);";

            try
            {
                MySqlCommand sqlCmd = new MySqlCommand(sqlCommand, dbConnection);
                sqlCmd.Parameters.AddWithValue("@account_number", parser.AccountNumber);
                sqlCmd.Parameters.AddWithValue("@raw_message", parser.RawMessage);
                sqlCmd.Parameters.AddWithValue("@protocol", parser.ProtocolID);
                sqlCmd.Parameters.AddWithValue("@receiver_number", (parser.ReceiverNum.Length > 0) ? parser.ReceiverNum : "0");
                sqlCmd.Parameters.AddWithValue("@line_prefix", (parser.LinePrefix.Length > 0) ? parser.LinePrefix : "0");
                sqlCmd.Parameters.AddWithValue("@message_data", parser.MessageData);
                sqlCmd.Parameters.AddWithValue("@message_xdata", (parser.MessageXData.Length > 0) ? parser.MessageXData : "");
                sqlCmd.Parameters.AddWithValue("@message_timestamp", (parser.MessageTimestamp.Length > 0) ? parser.MessageTimestamp : "_00:00:00,00-00-0000");

                sqlCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }

            foreach (KeyValuePair<string, string> pair in parsedData)
            {

                LogMessage($"{pair.Key} : {pair.Value}");
            
            }
            LogMessage($"Checksum Validation: {parser.CheckCRCMatch()}");
        }

        private static void CloseClient(Socket client)
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
            }
            catch { /* Ignore if already down */ }
            finally
            {
                client.Close();
            }
        }


        private static void ShowBanner()
        {
            Console.WriteLine("#####################################################");
            Console.WriteLine("#                                                   #");
            Console.WriteLine("#               Alarm Monitoring Console            #");
            Console.WriteLine("#              Created by Shane McIntosh            #");
            Console.WriteLine("#           Copyright (c) 2026 RexFault-Labs        #");
            Console.WriteLine("#                                                   #");
            Console.WriteLine("#####################################################");
            Console.WriteLine();
            Console.WriteLine("Press the Q key at any time to quit the application.");
        }

        /// <summary>
        /// Checks to see if the Q key has been pressed to quit the application. 
        /// This allows the user to exit the console gracefully without having to force close it.
        /// </summary>
        /// <returns>
        /// Returns true if the Q key has been pressed, otherwise returns false.
        /// </returns>
        private static bool CheckForQuitKey()
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void DC09ParserTest()
        {

            //Test Cases
            string rawTestMessage1 = "\n9EC40027\"ADM-CID\"0001L0#1002[#1002|1602 00 001]\r";
            string rawTestMessage2 = "\n75140027\"ADM-CID\"0001L0#1002[#1002|3354 00 004]\r";
            //Old Tests
            //string rawTestMessage3 = "\nBE780027\"ADM-CID\"0041L0#1001[#1001|1602 00 030]\r";
            //string rawTestMessage4 = "\n4B540027\"ADM-CID\"0037L0#1001[#1001|3354 00 004]\r";
            //New Tests
            //Common Values in the messsage below are:
            //SEQ: 9876   RCVR: 579BDF pref: 789ABC acct: 12345A
            //Fire Alarm, Zone 129, SIA DC-04 Format, Non-Encrypted, No Timestamp, xData: MAC Address x1234567890AB
            //string rawTestMessage3 = "\n8C860043\"SIA-DCS\"9876R579BDFL789ABC#123456A[#123456A|NFA129][M1234567890AB]\r";
            //Fire Alarm, Zone 129, SIA DC-04 Format, Non-Encrypted, With TimeStamp, xData: MAC Address x1234567890AB
            //CRC & LENGTH will not CHECK OUT
            string rawTestMessage3 = "\n8C860043\"SIA-DCS\"9876R579BDFL789ABC#123456A[#123456A|NFA129][M1234567890AB]_13:14:15,02-15-2006\r";
            //Fire Alarm, Zone 129, SIA DC-04 Format, Non-Encrypted, With Timestamp
            string rawTestMessage4 = "\nDC530046\"SIA-DCS\"9876R579BDFL789ABC#12345A[#12345A|NFA129]_13:14:15,02-15-2006\r";
            //Unencrypted NULL Message for HeartBeat
            //<LF><CRC><0LLL><"NULL"><0000><Rrcvr><Lpref><#acct>[]<timestamp><CR>
            string rawTestMessage5 = "\"NULL\"0001R01L01#3501[]_13:14:15,02-15-2006\r";
            Console.WriteLine("CRC1: " + DC09Parser.CreateCRC(rawTestMessage5));
            rawTestMessage5 = "\n" + DC09Parser.CreateCRC(rawTestMessage5) + DC09Parser.CalculateLength(rawTestMessage5) + rawTestMessage5;
            Console.WriteLine("CRC2: " + DC09Parser.CreateCRC(rawTestMessage5));
            //rawTestMessage5 = "\n" + rawTestMessage5;

            //SIA DC-09 Parser Testing
            DC09Parser p1 = new DC09Parser(rawTestMessage1);
            DC09Parser p2 = new DC09Parser(rawTestMessage2);
            DC09Parser p3 = new DC09Parser(rawTestMessage3);
            DC09Parser p4 = new DC09Parser(rawTestMessage4);
            DC09Parser p5 = new DC09Parser(rawTestMessage5);
            
            Console.WriteLine("TestMessage1: ");
            foreach (KeyValuePair<string, string> pair in p1.GetDictionary())
            {
                Console.WriteLine($"\t{pair.Key} : {pair.Value}");
            }
            //Console.WriteLine("CRC Check: " + p1.CreateInternalCRC());
            Console.WriteLine("\tCRC Test: " + p1.CheckCRCMatch());
            Console.WriteLine("\tMSG Length: " + DC09Parser.CalculateLength(rawTestMessage1));

            Console.WriteLine("TestMessage2: ");
            foreach (KeyValuePair<string, string> pair in p2.GetDictionary())
            {
                Console.WriteLine($"\t{pair.Key} : {pair.Value}");
            }
            //Console.WriteLine("CRC Check: " + p2.CreateInternalCRC());
            Console.WriteLine("\tCRC Test: " + p2.CheckCRCMatch());
            Console.WriteLine("\tMSG Length: " + DC09Parser.CalculateLength(rawTestMessage2));

            Console.WriteLine("TestMessage3: ");
            foreach (KeyValuePair<string, string> pair in p3.GetDictionary())
            {
                Console.WriteLine($"\t{pair.Key} : {pair.Value}");
            }
            //Console.WriteLine("CRC Check: " + p3.CreateInternalCRC());
            Console.WriteLine("\tCRC Test: " + p3.CheckCRCMatch());
            Console.WriteLine("\tMSG Length: " + DC09Parser.CalculateLength(rawTestMessage3));

            Console.WriteLine("TestMessage4: ");
            foreach (KeyValuePair<string, string> pair in p4.GetDictionary())
            {
                Console.WriteLine($"\t{pair.Key} : {pair.Value}");
            }
            //Console.WriteLine("CRC Check: " + p4.CreateInternalCRC());
            Console.WriteLine("\tCRC Test: " + p4.CheckCRCMatch());
            Console.WriteLine("\tMSG Length: " + DC09Parser.CalculateLength(rawTestMessage4));

            Console.WriteLine("TestMessage5: ");
            foreach (KeyValuePair<string, string> pair in p5.GetDictionary())
            {
                Console.WriteLine($"\t{pair.Key} : {pair.Value}");
            }
            //Console.WriteLine("CRC Check: " + p4.CreateInternalCRC());
            Console.WriteLine("\tCRC Test: " + p5.CheckCRCMatch());
            Console.WriteLine("\tMSG Length: " + DC09Parser.CalculateLength(rawTestMessage5));
        }

        private static void LogMessage(string message)
        {

            Console.WriteLine(message); 
        }

    }
}
