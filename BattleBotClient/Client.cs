using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BattleBotClient
{
    public class Client
    {
        public string MyPCName { get; private set; }

        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;


        public delegate void GameStartedHandler(object sender, EventArgs e);
        public event GameStartedHandler GameStarted;
        protected virtual void OnGameStarted(EventArgs e)
        {
            if (GameStarted != null)
            {
                GameStarted(this, e);
            }
        }

        public delegate void GameStoppedHandler(object sender, EventArgs e);
        public event GameStoppedHandler GameStopped;
        protected virtual void OnGameStopped(EventArgs e)
        {
            if (GameStopped != null)
            {
                GameStopped(this, e);
            }
        }

        public delegate void GamePausedHandler(object sender, EventArgs e);
        public event GamePausedHandler GamePaused;
        protected virtual void OnGamePaused(EventArgs e)
        {
            if (GamePaused != null)
            {
                GamePaused(this, e);
            }
        }

        public Client(int yourPCNr, string serverIP, int serverPort)
        {
            if (yourPCNr >= 0 && yourPCNr <= 4 && serverIP != "" && serverIP != null)
            {
                MyPCName = "pc" + yourPCNr;

                client = new TcpClient();
                client.Connect(serverIP, serverPort);
                writer = new StreamWriter(client.GetStream());
                reader = new StreamReader(client.GetStream());

                Thread receiveThread = new Thread(handleIncomingMessage);
                receiveThread.Start();

                sendMessage("connect");
            }
            else throw new Exception("vul de juiste waardes in!");
        }

        private void sendMessage(string message)
        {
            bool okMessage = true;

            foreach (char c in message)
            {
                if (c == '#' || c == '&' || c == '%') ;
                {
                    okMessage = false;
                }
            }

            if (message != null && message != "" && okMessage == true)
            {
                string toSend = ("#" + MyPCName + "&" + message + "%");
                writer.Write(toSend);
                writer.Flush();
            }
        }

        private void handleIncomingMessage()
        {
            while (true)
            {
                int incomingInt = reader.Read();
                char incomingChar = ' ';
                string targetPC = "";
                string incomingMessage = "";

                if (incomingInt != -1) // is er iets beschikbaar?
                {
                    incomingChar = Convert.ToChar(incomingInt);
                    if (incomingChar == '#') // als incoming # is begin
                    {
                        incomingInt = -1;
                        while (incomingInt == -1) // lees totdat er iets beschikbaar is
                        {
                            incomingInt = reader.Read();
                        }
                        incomingChar = Convert.ToChar(incomingInt);
                        while (incomingChar != '&') // ga door met whilen totdat incomingchar & is
                        {
                            targetPC += incomingChar; // stop alle binnenkomende tekens in een string
                            incomingInt = reader.Read();
                            incomingChar = Convert.ToChar(incomingInt);
                        }
                        if (targetPC == MyPCName) // ga door als het bericht voor deze pc bedoeld is
                        {
                            incomingInt = -1; // zet incoming op -1
                            while (incomingInt == -1) // ga door met receiven totdat er iets binnenkomt
                            {
                                incomingInt = reader.Read();
                            }
                            incomingChar = Convert.ToChar(incomingInt);
                            while (incomingChar != '%') // ga lezen totdat het binnenkomende teken % is
                            {
                                incomingMessage += incomingChar;
                                incomingInt = reader.Read();
                                incomingChar = Convert.ToChar(incomingInt);
                            }

                            if (incomingMessage == "start")
                            {
                                incomingMessage = "";
                                OnGameStarted(EventArgs.Empty);
                            }
                            else if (incomingMessage == "stop")
                            {
                                incomingMessage = "";
                                OnGameStopped(EventArgs.Empty);
                            }
                            else if (incomingMessage == "o.k. daddy")
                            {
                                incomingMessage = "";
                                System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=LB871SVYMhI");
                                /// leuk dat je daadwerkelijk naar mijn code kijkt btw
                            }
                            else if (incomingMessage == "pause")
                            {
                                incomingMessage = "";
                                OnGamePaused(EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }

        public void HitSomeone()
        {
            sendMessage("HIT"); ////___________WELKE MESSAGE PRECIES?__________-
            throw new NotImplementedException();
        }

        public void GotHit()
        {
            sendMessage("HIT???"); ////___________WELKE MESSAGE PRECIES?__________-
            throw new NotImplementedException();
        }
    }
}
