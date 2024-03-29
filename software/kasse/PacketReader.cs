﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationAPI.Radio;

namespace Kasse
{
    // Parse packet: <cmd|{args|}csum>
    public class PacketReader
    {
        public const string PACKET_START = "<";
        public const string PACKET_END = ">";
        public const string PACKET_DELIMITER = "|";

        private string packet;
        private string content;
        private string[] args;

        public string PacketString
        {
            get
            {
                return packet;
            }
        }

        public string ChkSum
        {
            get
            {
                return args[args.Length - 1];
            }
        }

        public List<string> Args
        {
            get
            {
                List<string> list = new List<string>();
                for (int i = 1; i < args.Length - 1; i++ )
                {
                    list.Add(args[i]);
                }

                return list;
            }
        }

        public LampCommand Command
        {
            get
            {
                try
                {
                    return (LampCommand)int.Parse(args[0]);
                }
                catch
                {
                    return LampCommand.Invalid;
                }
            }
        }

        public bool IsValidChkSum
        {
            get
            {
                //<a|b|c|d|e> CHK: a|b|c|d|
                string toCheck = String.Empty;
                for (int i = 0; i < args.Length - 1; i++)
                {
                    toCheck += args[i] + "|";
                }
                Console.WriteLine("Compare csums: {0} ?= {1} from packet: {2}", GetChecksum(toCheck), ChkSum, toCheck);
                return GetChecksum(toCheck) == ChkSum;
            }
        }

        public PacketReader(string packet)
        {
            this.packet = packet;
            
            int startPos = packet.IndexOf(PACKET_START) + 1;
            int endPos = packet.Length;
            content = packet.Substring(startPos, endPos - startPos);
            Console.WriteLine("Packet content: " + content);
            args = content.Split(new string[] { PACKET_DELIMITER }, StringSplitOptions.None);            
        }

        private string GetChecksum(string str)
        {
            byte chkSum = 0;
            foreach (char c in str)
            {
                chkSum += (byte)c;
            }

            return chkSum.ToString();
        }


    }
}
