using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;

namespace DashBoard
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------
        //
        //      41953 CPL GIHAN RL
        //      Github Rep-987456
        //      V0.1
        //
        //------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }
        //Initialize lists
        List<string> ipAddresses = new List<string>();
        List<string> locations = new List<string>();
        List<string> subLocations = new List<string>();

        int _countLocation=0;
       // int _countDevices;
        int _countDown=0;

        private void ReadXml() {
            try
            {
                string xmlFilePath = "List.xml";


                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFilePath);

                XmlNodeList ipAddressNodes = xmlDoc.SelectNodes("//IPAddress");

                foreach (XmlNode node in ipAddressNodes)
                {
                    string location = node.SelectSingleNode("Location").InnerText;
                    string sublocations = node.SelectSingleNode("SubLocation").InnerText;
                    string ipAddress = node.SelectSingleNode("Address").InnerText;


                    locations.Add(location);
                    subLocations.Add(sublocations);
                    ipAddresses.Add(ipAddress);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Xml Reading Error : " + ex.ToString());
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ReadXml();
             _countLocation = locations.Count;
            lblcountLocation.Text = _countLocation.ToString();
            PingIPs();
        }

        
        private Task<PingReply> PingIPAddress(string ipAddress)
        {
            var ping = new Ping();
            return ping.SendPingAsync(ipAddress);
        }

        private async void PingIPs() {
           
            Color customColor = Color.FromArgb(234, 49, 46);

            dataGridView1.Rows.Clear();
            
            _countDown =0;       

            for (int i = 0; i < ipAddresses.Count; i++)
            {
                string ipAddress = ipAddresses[i];
                string location = locations[i];
                string sublocation = subLocations[i];

                _countLocation++;

                PingReply reply = await PingIPAddress(ipAddress);
                string status = reply.Status.ToString();
                string roundTripTime = reply.RoundtripTime.ToString();

                dataGridView1.Rows.Add(location, sublocation, ipAddress, status, roundTripTime);

                if (i < dataGridView1.Rows.Count && reply.Status != IPStatus.Success)
                {
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = customColor;
                    _countDown++;
                }
            }           
            lblcountDown.Text = _countDown.ToString();

            if (dataGridView1.Rows.Count == int.Parse(lblcountLocation.Text))
            {
                ProgressBar1.Maximum = _countLocation;
                ProgressBar1.Value = _countDown;

                Thread.Sleep(5000);
                PingIPs();                
            }
        }       

        private void btnMinimized_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

       
    }
}
