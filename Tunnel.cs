using System;
using System.Threading;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace TCPTunnel2_GUI
{
    class Tunnel : Window
    {
        [UI] private Image gtkImage = null;
        [UI] private Button btnConnect = null;
        [UI] private Label lblStatus = null;
        //State
        private bool connected = true;
        private Network networkThread;

        public Tunnel() : this(new Builder("Tunnel.glade")) { }

        private Tunnel(Builder builder) : base(builder.GetObject("Tunnel").Handle)
        {
            builder.Autoconnect(this);
            networkThread = new Network(GUICallback);
            UpdateGUI();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Connect(object sender, EventArgs a)
        {
            connected = !connected;
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            if (connected)
            {
                btnConnect.Label = "Disconnect";
                networkThread.SetState(connected);
            }
            else
            {
                btnConnect.Label = "Connect";
                networkThread.SetState(connected);
            }
        }

        private void GUICallback(GUIData guiData)
        {
            Gtk.Application.Invoke(delegate { SetGUIIconReal(guiData); });
        }

        private void SetGUIIconReal(GUIData guiData)
        {
            gtkImage.SetFromIconName(guiData.iconName, IconSize.Dialog);
            lblStatus.Text = guiData.labelText;
        }
    }
}
