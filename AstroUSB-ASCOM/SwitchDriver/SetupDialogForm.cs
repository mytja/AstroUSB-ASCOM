using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ASCOM.Utilities;

namespace ASCOM.mytjaAstroUSB.Switch
{
    [ComVisible(false)] // Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        const string NO_PORTS_MESSAGE = "No COM ports found";
        TraceLogger tl; // Holder for a reference to the driver's trace logger

        public SetupDialogForm(TraceLogger tlDriver)
        {
            InitializeComponent();

            // Save the provided trace logger for use within the setup dialogue
            tl = tlDriver;

            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void CmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here and update the state variables with results from the dialogue

            tl.Enabled = chkTrace.Checked;

            // Update the COM port variable if one has been selected
            if (comboBoxComPort.SelectedItem is null) // No COM port selected
            {
                tl.LogMessage("Setup OK", $"New configuration values - COM Port: Not selected");
            }
            else if (comboBoxComPort.SelectedItem.ToString() == NO_PORTS_MESSAGE)
            {
                tl.LogMessage("Setup OK", $"New configuration values - NO COM ports detected on this PC.");
            }
            else // A valid COM port has been selected
            {
                SwitchHardware.comPort = (string)comboBoxComPort.SelectedItem;
                tl.LogMessage("Setup OK", $"New configuration values - COM Port: {comboBoxComPort.SelectedItem}");
            }

            for (int i = 0; i < SwitchHardware.switches.Count; i++)
            {
                String id = SwitchHardware.switches[i].InternalID;
                if (id == "0") SwitchHardware.switches[i].Name = usb1Name.Text;
                else if (id == "1") SwitchHardware.switches[i].Name = usb2Name.Text;
                else if (id == "2") SwitchHardware.switches[i].Name = usb3Name.Text;
                else if (id == "3") SwitchHardware.switches[i].Name = usb4Name.Text;
                else if (id == "4") SwitchHardware.switches[i].Name = usb5Name.Text;
                else if (id == "5") SwitchHardware.switches[i].Name = usb6Name.Text;
                else if (id == "6") SwitchHardware.switches[i].Name = usb7Name.Text;
            }
        }

        private void CmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("https://ascom-standards.org/");
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {

            // Set the trace checkbox
            chkTrace.Checked = tl.Enabled;

            // set the list of COM ports to those that are currently available
            comboBoxComPort.Items.Clear(); // Clear any existing entries
            using (Serial serial = new Serial()) // User the Se5rial component to get an extended list of COM ports
            {
                comboBoxComPort.Items.AddRange(serial.AvailableCOMPorts);
            }

            // If no ports are found include a message to this effect
            if (comboBoxComPort.Items.Count == 0)
            {
                comboBoxComPort.Items.Add(NO_PORTS_MESSAGE);
                comboBoxComPort.SelectedItem = NO_PORTS_MESSAGE;
            }

            // select the current port if possible
            if (comboBoxComPort.Items.Contains(SwitchHardware.comPort))
            {
                comboBoxComPort.SelectedItem = SwitchHardware.comPort;
            }

            for (int i = 0; i < SwitchHardware.switches.Count; i++)
            {
                String id = SwitchHardware.switches[i].InternalID;
                String value = SwitchHardware.switches[i].Name;
                if (id == "0") usb1Name.Text = value;
                else if (id == "1") usb2Name.Text = value;
                else if (id == "2") usb3Name.Text = value;
                else if (id == "3") usb4Name.Text = value;
                else if (id == "4") usb5Name.Text = value;
                else if (id == "5") usb6Name.Text = value;
                else if (id == "6") usb7Name.Text = value;
            }

            tl.LogMessage("InitUI", $"Set UI controls to Trace: {chkTrace.Checked}, COM Port: {comboBoxComPort.SelectedItem}");
        }

        private void SetupDialogForm_Load(object sender, EventArgs e)
        {
            // Bring the setup dialogue to the front of the screen
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            else
            {
                TopMost = true;
                Focus();
                BringToFront();
                TopMost = false;
            }
        }
    }
}