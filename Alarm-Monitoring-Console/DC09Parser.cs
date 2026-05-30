using Nito.HashAlgorithms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Numerics;
using System.Text;

namespace Alarm_Monitoring_Console
{
    internal class DC09Parser
    {
        private string _rawMessage = string.Empty;
        private string _messageCRC = string.Empty;
        private string _messageLength = string.Empty;
        private string _protocolID = string.Empty;
        private string _messageSeq = string.Empty;
        private string _receiverNum = string.Empty;
        private string _linePrefix = string.Empty;
        private string _accountNum = string.Empty;
        private string _messageData = string.Empty;
        private string _messageXData = string.Empty;
        private string _messageTimestamp = string.Empty;

        public string RawMessage { get { return _rawMessage; } }
        public string MessageCRC { get {  return _messageCRC; }  }
        public string MessageLength { get { return _messageLength; } }
        public string ProtocolID { get { return _protocolID.Replace("\"",""); } }
        public string MessageSeq { get { return _messageSeq; }  }
        public string ReceiverNum { get { return _receiverNum.Replace("R", ""); ; }  }
        public string LinePrefix { get { return _linePrefix.Replace("L", "") ; }  }
        public string AccountNumber {  get {  return _accountNum.Replace("#",""); }  }
        public string MessageData {  get {  return _messageData; } } 
        public string MessageXData {  get { return _messageXData; }  }
        public string MessageTimestamp {  get { return _messageTimestamp.Replace("_",""); } }

        public DC09Parser(string rawMessage)
        {
            ParseMessage(rawMessage);
        }

        public DC09Parser()
        {
        }

        public bool ParseMessage(string rawMessage)
        {
            _rawMessage = rawMessage;

            //Confirm message starts with LF and ends with CR if not we know right away that it's not a valid Ademco CID message
            if ((rawMessage[0] != '\n') || (rawMessage[^1] != '\r'))
            {
                return false;
            }

            int curPos = 1;
            int rLen = 0;

            _messageCRC = rawMessage.Substring(curPos, 4); //CRC comes right after the LF character and is 4 characters long
            curPos += 4;

            //TODO Add CRC Check Here

            _messageLength = rawMessage.Substring(curPos, 4); //Message length starts with 0 and is 3 hex digits right after the CRC
            curPos += 4;

            //TODO Add Message Length Check Here

            rLen = rawMessage.IndexOf('"', curPos + 1);
            rLen = rLen - curPos+1;
            _protocolID = rawMessage.Substring(curPos, rLen); //Protocol ID is 8 characters including the "" characters surrounding it
            curPos += rLen;

            _messageSeq = rawMessage.Substring(curPos, 4);
            curPos += 4;

            if (rawMessage[curPos] == 'R') {

                //Can be 1-6 digits after R so we need to find the next field which starts with L
                rLen = rawMessage.IndexOf('L', curPos);
                rLen = rLen - curPos;
                _receiverNum = rawMessage.Substring(curPos, rLen);
                curPos += rLen;
            }

            rLen = rawMessage.IndexOf('#');
            if ((rLen <= 0) || (rLen <= curPos))
                return false; // no # present message is invalid

            rLen = rLen - curPos;

            _linePrefix = rawMessage.Substring(curPos, rLen);
            curPos += rLen;

            //After Line Prefix comes Account number and is prefixed with # and can be 3-16 characters expects to see [ as the next character
            rLen = rawMessage.IndexOf('[');
            if ((rLen <= 0) || (rLen <= curPos))
                return false; // no [ present message is invalid
            rLen = rLen - curPos;
            _accountNum = rawMessage.Substring(curPos, rLen);
            curPos += rLen;

            //Next we pull the main message body, surrounded by []
            rLen = rawMessage.IndexOf(']');
            if ((rLen <= 0) || (rLen <= curPos))
                return false; // no ] present message is invalid
            rLen = rLen - curPos + 1;
            _messageData = rawMessage.Substring(curPos, rLen);
            curPos += rLen;

            //Next we have to check to see if we have any Extended Data
            if (rawMessage[curPos] == '[')
            {
                rLen = rawMessage.IndexOf(']',curPos);
                if ((rLen <= 0) || (rLen <= curPos))
                    return false; // no ] present message is invalid
                rLen = (rLen - curPos) + 1;
                _messageXData = rawMessage.Substring(curPos, rLen);
                curPos += rLen;
            }

            //Next we check to see if we have a timestamp, if so it's 20 characters and we pull it otherwise we're done
            if (rawMessage[curPos] == '_') {
                _messageTimestamp = rawMessage.Substring(curPos, 20);//Timestamp is 20 characters
            }

            return true;
        }

        public Dictionary<string, string> GetDictionary()
        {

            Dictionary<string, string> kvPairs = new Dictionary<string, string>();

            kvPairs.Add("RawMessage", _rawMessage.Replace("\n", "\\n").Replace("\r", "\\r"));
            kvPairs.Add("MessageCRC", MessageCRC);
            kvPairs.Add("MessageLength", MessageLength);
            kvPairs.Add("ProtocolID", ProtocolID);
            kvPairs.Add("MessageSeq", MessageSeq);
            kvPairs.Add("RecieverNum", ReceiverNum);
            kvPairs.Add("LinePrefix", LinePrefix);       
            kvPairs.Add("AccountNumber", AccountNumber);
            kvPairs.Add("MessageData", MessageData);
            kvPairs.Add("MessageXData", MessageXData);
            kvPairs.Add("TimeStamp", MessageTimestamp);

            return kvPairs;
           
        }

        public bool CheckCRCMatch()
        {
            string currentCRC = CreateInternalCRC();
            //Console.WriteLine("Current CRC: " + currentCRC);
            //Console.WriteLine("Stored  CRC: " +  _messageCRC);
            if (currentCRC == _messageCRC)
            {
                return true;
            }
            return false;
        }

        private string CreateInternalCRC()
        {

            string crcString = (_protocolID + _messageSeq + _receiverNum + _linePrefix + _accountNum + _messageData + _messageXData + _messageTimestamp);
            return DC09Parser.CreateCRC(crcString);

        }

        public static string CreateCRC(string rawData)
        {
            int startPos = rawData.IndexOf('"');
            int endPos = rawData.Length;
            if (rawData[endPos - 1] == '\r')
                endPos--;

            string crcString = rawData.Substring(startPos,endPos-startPos);
            byte[] crcBytes = Encoding.ASCII.GetBytes(crcString);
            byte[] crcCalc = new byte[2];
            byte tmp;

            var calc = new CRC16();
            crcCalc = calc.ComputeHash(crcBytes);

            //Flip the bits around to get the correct CRC
            tmp = crcCalc[0];
            crcCalc[0] = crcCalc[1];
            crcCalc[1] = tmp;

            string? crc = Convert.ToHexString(crcCalc);
            if (crc != null)
            {
                return crc;
            }
            return string.Empty;
        }

        //Creates an ACK String
        //ACK Format: <LF><CRC><0LLL><"ACK"><seq><Rrcvr><Lpref><#acct>[]<CR>

        /// <summary>
        /// Creates an ACK String message in response to a NULL message for Monitoring
        /// </summary>
        /// <param name="seq">MUST BE 4 DIGITS</param>
        /// <param name="rcvr">1-6 Digits in length if present</param>
        /// <param name="linePrefix">1-6 Digits in length</param>
        /// <param name="acctNum">3-16 Digits in length</param>
        /// <returns>string ready to be sent over the network</returns>
        public static string CreateACK(string seq, string rcvr, string linePrefix, string acctNum)
        {
            string rawData = string.Empty;
            if (rcvr != string.Empty)
                rawData = $"\"ACK\"{seq}R{rcvr}L{linePrefix}#{acctNum}[]\r";
            else
                rawData = $"\"ACK\"{seq}L{linePrefix}#{acctNum}[]\r";
            rawData = "\n" + DC09Parser.CreateCRC(rawData) + DC09Parser.CalculateLength(rawData) + rawData;

            return rawData;
        }

        public static string CalculateLength(string rawData)
        {
            int startPos = rawData.IndexOf('"');
            int endPos = rawData.Length - 1;

            int messageLen = endPos - startPos;
            return "0" + messageLen.ToString("X3");
        }

        public static string GenerateTimeStamp()
        {
            //Get GMT Time and then create it in a specific format, then add _ to the beginning of it.
            //<_HH:MM:SS,MM-DD-YYYY> <-Date Time Format that needs to be returned
            DateTime gmtNow = DateTime.UtcNow;

            string formattedDate = gmtNow.ToString("HH:mm:ss,MM-dd-yyyy", CultureInfo.InvariantCulture);
            formattedDate = "_" + formattedDate;
            return formattedDate;
        }


    }
}
 