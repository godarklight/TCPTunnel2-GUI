using System;
using TCPTunnel2;
namespace TCPTunnel2_GUI
{
    public class GUIData
    {
        public string iconName = "gtk-no";
        public string labelText = "0 connected";
        public Statistics statistics = new Statistics();
        public long lastTime;
        public long lastSendBytes;
        public long lastReceiveBytes;

        public string GetSpeedString()
        {
            long currentTime = DateTime.UtcNow.Ticks;
            double timeDiff = (currentTime - lastTime) / (double)TimeSpan.TicksPerSecond;
            double upSpeed = (statistics.sentBytes - lastSendBytes) / timeDiff;
            double downSpeed = (statistics.receivedBytes - lastReceiveBytes) / timeDiff;
            lastTime = currentTime;
            lastSendBytes = statistics.sentBytes;
            lastReceiveBytes = statistics.receivedBytes;
            return $"U:{Math.Round(upSpeed / 1024d, 2)}kB/s D:{Math.Round(downSpeed / 1024d, 2)}kB/s";
        }
    }
}