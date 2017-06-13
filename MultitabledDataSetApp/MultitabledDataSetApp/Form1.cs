using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;

namespace MultitabledDataSetApp
{
    public partial class Form1 : Form
    {
        //form wide dataset
        private DataSet _autoLotDs = new DataSet("AutoLot");

        //make use of command builders to simplify data adapter configuration
        private SqlCommandBuilder _sqlCbInventory;
        private SqlCommandBuilder _sqlCbCustomers;
        private SqlCommandBuilder _sqlCbOrders;

        private SqlDataAdapter _invTableAdapter;
        private SqlDataAdapter _custTableAdapter;
        private SqlDataAdapter _ordersTableAdapter;

        private string _connectionString;

        public Form1()
        {
            InitializeComponent();

            _connectionString = ConfigurationManager.ConnectionStrings["AutoLotSqlProvider"].ConnectionString;

            _invTableAdapter = new SqlDataAdapter("select * from inventory", _connectionString);
            _custTableAdapter = new SqlDataAdapter("select * from customers", _connectionString);
            _ordersTableAdapter = new SqlDataAdapter("select * from orders", _connectionString);

            _sqlCbCustomers = new SqlCommandBuilder(_invTableAdapter);
            _sqlCbInventory = new SqlCommandBuilder(_custTableAdapter);
            _sqlCbOrders = new SqlCommandBuilder(_ordersTableAdapter);

            _invTableAdapter.Fill(_autoLotDs, "Inventory");
            _custTableAdapter.Fill(_autoLotDs, "Customers");
            _ordersTableAdapter.Fill(_autoLotDs, "Orders");

            BuildTableRelationship();

            InventoryDataGridView.DataSource = _autoLotDs.Tables["Inventory"];
            CustomersDataGridView.DataSource = _autoLotDs.Tables["Customers"];
            OrdersDataGridView.DataSource = _autoLotDs.Tables["Orders"];
        }

        private void BuildTableRelationship()
        {
            //Create CustomerOrder data relation object
            DataRelation dr = new DataRelation("CustomerOrder", _autoLotDs.Tables["Customers"].Columns["CustID"], _autoLotDs.Tables["Orders"].Columns["CustID"]);
            _autoLotDs.Relations.Add(dr);
            //Create InventoryOrder data relation object
            dr = new DataRelation("InventoryOrder", _autoLotDs.Tables["Inventory"].Columns["CarID"], _autoLotDs.Tables["Orders"].Columns["CarID"]);
            _autoLotDs.Relations.Add(dr);
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                _invTableAdapter.Update(_autoLotDs, "Inventory");
                _custTableAdapter.Update(_autoLotDs, "Customers");
                _ordersTableAdapter.Update(_autoLotDs, "Orders");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetOrdersButton_Click(object sender, EventArgs e)
        {
            string strOrderInfo = string.Empty;

            int custID = int.Parse(GetOrderTextBox.Text);

            //based on id, get the correct row in customers table
            var drsCust = _autoLotDs.Tables["Customers"].Select($"CustID = {custID}");
            strOrderInfo += $"Customer {drsCust[0]["CustID"]}: {drsCust[0]["FirstName"].ToString().Trim()} {drsCust[0]["LastName"].ToString().Trim()}\n";

            //Navigate from customer table to order table
            var drsOrder = drsCust[0].GetChildRows(_autoLotDs.Relations["CustomerOrder"]);

            //loop through all orders for this customer
            foreach(DataRow order in drsOrder)
            {
                strOrderInfo += $"----\nOrder Number: {order["OrderID"]}\n";

                //Get the car referenced by this order
                DataRow[] drsInv = order.GetParentRows(_autoLotDs.Relations["InventoryOrder"]);

                //Get infor for (SINGLE) car info for this order
                DataRow car = drsInv[0];
                strOrderInfo += $"Make: {car["Make"]}\n";
                strOrderInfo += $"Color: {car["Color"]}\n";
                strOrderInfo += $"PetName: {car["PetName"]}\n";
            }

            MessageBox.Show(strOrderInfo, "Order Details");
        }
    }
}
