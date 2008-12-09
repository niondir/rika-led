﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CommunicationAPI.DataTypes;

namespace StoreServer
{
    public class ConsoleHandler
    {
        private Queue<string> queue;
        private Thread thread;

        public ConsoleHandler()
        {
            queue = new Queue<string>();

            thread = new Thread(new ThreadStart(ConsoleListen));
            thread.Name = "Console Listener Thread";
            thread.Start();
        }

        public void ConsoleListen()
        {
            while (!Program.Closing)
            {
                string input = Console.ReadLine();

                lock (queue)
                {
                    queue.Enqueue(input);
                    Program.Set();
                }
            }

            Debug.WriteLine("Console Listener Thread: Closing");
        }

        public void Slice() {
            if (queue.Count > 0)
            {
                string msg = String.Empty;
                lock (queue)
                {
                    msg = queue.Dequeue();
                }

                string[] tokens = msg.ToLower().Split(new char[] {' '});
                HandleCommand(tokens);
            }
        }


        private SessionData session;
        private void HandleCommand(string[] tokens) {
            string command = tokens[0];

            try
            {
                switch (command)
                {
                    case "login":
                        try
                        {
                            if (tokens.Length > 1)
                                session = Program.ClientHandler.Login(new UserData(tokens[1], tokens[2]));
                            else
                                session = Program.ClientHandler.Login(new UserData("gast", "gast"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case "adduser":
                        UserData user = new UserData("holger", "hogerson");
                        user.Role = new RoleData("foo");

                        Program.ClientHandler.AddUser(session, user);
                        break;
                    case "addregion":
                        Program.ClientHandler.AddRegion(session, new RegionData(1, "foobar"));
                        break;
                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
