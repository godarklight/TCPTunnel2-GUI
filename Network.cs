using System;
using System.Threading;
using TCPTunnel2;

namespace TCPTunnel2_GUI
{
    public class Network
    {
        private bool currentState;
        private bool running = true;
        private int clientConnected;
        private TunnelSettings tunnelSettings;
        private UDPServer udpServer;
        private TunnelClient tunnelClient;
        private TunnelServer tunnelServer;
        private Thread watchdogThread;
        private Action<GUIData> guiCallback;
        private GUIData guiData = new GUIData();

        public Network(Action<GUIData> guiCallback)
        {
            this.guiCallback = guiCallback;
            watchdogThread = new Thread(new ThreadStart(WatchdogLoop));
            watchdogThread.Start();
        }

        private void WatchdogLoop()
        {
            long nextUpdate = 0;
            while (running)
            {
                long currentTime = DateTime.UtcNow.Ticks;
                if (currentState)
                {
                    int newCount = udpServer.GetConnections().Count;
                    if (newCount != clientConnected)
                    {
                        clientConnected = newCount;
                        if (clientConnected == 0)
                        {
                            guiData.labelText = $"Waiting for connections";
                            guiData.iconName = "gtk-yes";
                        }
                        else
                        {
                            guiData.labelText = $"{clientConnected} connected, speed: {guiData.GetSpeedString()}";
                            guiData.iconName = "gtk-connect";
                        }
                        UpdateGUI();
                    }
                }
                if (currentTime > nextUpdate)
                {
                    nextUpdate = currentTime + TimeSpan.TicksPerSecond;
                    if (clientConnected > 0)
                    {
                        guiData.labelText = $"{clientConnected} connected, {guiData.GetSpeedString()}";
                        guiData.iconName = "gtk-connect";
                    }
                    UpdateGUI();
                }
                Thread.Sleep(50);
            }
        }

        public void SetState(bool state)
        {
            if (state == currentState)
            {
                return;
            }
            if (state)
            {
                clientConnected = -1;
                tunnelSettings = new TunnelSettings();
                tunnelSettings.Load("TunnelSettings.txt");
                guiData.statistics = new Statistics();
                udpServer = new UDPServer(tunnelSettings, guiData.statistics);
                if (tunnelSettings.tunnelServer == "")
                {
                    tunnelServer = new TunnelServer(tunnelSettings, udpServer);
                }
                else
                {
                    tunnelClient = new TunnelClient(tunnelSettings, udpServer);
                }
                currentState = state;
            }
            else
            {
                currentState = state;
                if (tunnelClient != null)
                {
                    tunnelClient.Stop();
                    tunnelClient = null;
                }
                tunnelServer = null;
                udpServer.Stop();
                udpServer = null;
                tunnelSettings = null;
                guiData.labelText = "Disconnected";
                guiData.iconName = "gtk-no";
                UpdateGUI();
            }
        }

        private void UpdateGUI()
        {
            guiCallback(guiData);
        }
    }
}