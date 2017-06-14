using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridViewDataDesigner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'autoLotDataSet.Inventory' table. You can move, or remove it, as needed.
            this.inventoryTableAdapter.Fill(this.autoLotDataSet.Inventory);

        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.inventoryTableAdapter.Update(this.autoLotDataSet.Inventory);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.inventoryTableAdapter.Fill(this.autoLotDataSet.Inventory);
        }
    }
}
