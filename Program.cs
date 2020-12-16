using System;
using Gtk;

namespace TCPTunnel2_GUI
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("info.privatedns.godarklight.tcptunnel2", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new Tunnel();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
