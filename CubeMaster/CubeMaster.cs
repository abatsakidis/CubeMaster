using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

//for the shake to save JPG

namespace CubeMaster
{
    public partial class CubeMaster : Form
    {
        public CubeMaster()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            status.Text = "";

            //Create caculation
            WebService.CalculationClient client = new WebService.CalculationClient();

            //Create account
            WebService.Account Account = new WebService.Account();
            Account.UserID = textBox1.Text;
            Account.Password = textBox2.Text;
            Account.Company = textBox3.Text;

            //Create options
            WebService.Options Options = new WebService.Options();
            Options.CalculationSaved = true;
            Options.GraphicsCreated = true;
            Options.GraphicsImageWidth = 300;
            Options.GraphicsImageDepth = 300;

            Options.UOM = WebService.Options.UOMEnum.UnitMetric;//0=UnitEnglish, 1=UnitMetric, 2=UnitHighMetric

            //Create shipment
            WebService.Shipment Shipment = client.GetShipment();

            Shipment.Title = textBox5.Text;
            Shipment.Description = textBox6.Text;

            //Create cargoes
            WebService.Cargo NewCargo1 = new WebService.Cargo();
            NewCargo1.Name = textBox7.Text;
            NewCargo1.Length = double.Parse(textBox8.Text);
            NewCargo1.Width = double.Parse(textBox9.Text);
            NewCargo1.Height = double.Parse(textBox10.Text);
            NewCargo1.Qty = int.Parse(textBox11.Text);

            Shipment.Cargoes.Add(NewCargo1);

            WebService.Cargo NewCargo2 = new WebService.Cargo();
            NewCargo2.Name = textBox16.Text;
            NewCargo2.Length = double.Parse(textBox15.Text); 
            NewCargo2.Width = double.Parse(textBox14.Text);
            NewCargo2.Height = double.Parse(textBox13.Text);
            NewCargo2.Qty = int.Parse(textBox12.Text);

            Shipment.Cargoes.Add(NewCargo2);

            //Define containers
            WebService.Container NewContainer = new WebService.Container();
            NewContainer.ContainerType = WebService.Container.ContainerTypeEnum.SeaVan;
            NewContainer.Name = textBox19.Text;
            NewContainer.Length = double.Parse(textBox18.Text);
            NewContainer.Width = double.Parse(textBox17.Text);
            NewContainer.Height = double.Parse(textBox20.Text);

            Shipment.Containers.Add(NewContainer);

            //Define Rules
            Shipment.Rules.IsWeightLimited = false;

            //Run
            WebService.LoadPlan LoadPlan = client.Run(Shipment, Account, Options);

            //Show the return
            status.Text += LoadPlan.Status;
            status.Text += "\r\n";


            if (LoadPlan.Status == "OK")
            {
                //Access the restuls
                status.Text += "# of Containers =" + LoadPlan.FilledContainers.Count;
                status.Text += "\r\n";
                try
                {
                    foreach (WebService.FilledContainer Container in LoadPlan.FilledContainers)
                    {
                        status.Text += Container.Name;
                        status.Text += "\r\n";

                         //Manifest (Load List)
                        foreach (WebService.ManifestLine ManifestLine in Container.Manifest)
                        {
                            status.Text += ManifestLine.Sequence + ":" + ManifestLine.Cargo.Name + " x " + ManifestLine.CargoQty;
                            status.Text += "\r\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    status.Text += ex.Message;
                    status.Text += "\r\n";
                }
            }//LoadPlan.Status == "OK"

            status.Text += "Calculation saved at www.cubemaster.net = " + (Options.CalculationSaved == true ? "Yes" : "No");
            status.Text += "\r\n";
            status.Text += "3D graphics created = " + (Options.GraphicsCreated == true ? "Yes" : "No" + "\r\n");
            status.Text += "\r\n";

            if (LoadPlan.Status == "OK")
            {
                if (MessageBox.Show("Press OK to see a report in your default web browser", "Complete!", MessageBoxButtons.OK) == DialogResult.OK)
                {

                    //See a report
                    //A new member ReportType for which report to be shown
                    Options.ReportType = WebService.Options.ReportEnum.LoadingDiagram;

                    //Create the report link object and call the function
                    //Pass the account, option and a title of the the shipment to be open
                    WebService.HyperLink ReportLink = client.GetReportLink(Account, Options, Shipment.Title);

                    //Open the web browser of your PC to navigate to the URL returned by the abive function
                    System.Diagnostics.Process.Start(ReportLink.URL);

                }

            }

            client.Close();

          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

