using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CashRegister1.MainClass;
using System.Net.NetworkInformation;
using System.Drawing.Printing;

namespace CashRegister1
{
    public partial class Form1 : Form
    {
        MainClass mainClass = new MainClass();
        public Form1()
        {
            InitializeComponent();

            // Set up the DataGridView columns
            dgvShoppingList.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn itemNumberColumn = new DataGridViewTextBoxColumn();
            itemNumberColumn.DataPropertyName = "ItemNumber";
            itemNumberColumn.HeaderText = "Item Number";
            dgvShoppingList.Columns.Add(itemNumberColumn);

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "Name";
            nameColumn.HeaderText = "Name";
            dgvShoppingList.Columns.Add(nameColumn);

            DataGridViewTextBoxColumn priceColumn = new DataGridViewTextBoxColumn();
            priceColumn.DataPropertyName = "Price";
            priceColumn.HeaderText = "Price";
            dgvShoppingList.Columns.Add(priceColumn);

            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.DataPropertyName = "Quantity";
            quantityColumn.HeaderText = "Quantity";
            dgvShoppingList.Columns.Add(quantityColumn);

            DataGridViewTextBoxColumn discountColumn = new DataGridViewTextBoxColumn();
            discountColumn.DataPropertyName = "Discount";
            discountColumn.HeaderText = "Discount";
            dgvShoppingList.Columns.Add(discountColumn);

            DataGridViewTextBoxColumn totalPriceColumn = new DataGridViewTextBoxColumn();
            totalPriceColumn.DataPropertyName = "TotalPrice";
            totalPriceColumn.HeaderText = "Total Price";
            dgvShoppingList.Columns.Add(totalPriceColumn);

        }


        List<ShoppingListItem> shoppingList = new List<ShoppingListItem>();

        // Load Items To ListBox methode
        private void LoadItemsToListBox(ListBox listBox)
        {
            // Clear the ListBox
            listBox.Items.Clear();

            // Check if the file exists
            string fileName = "items.json";
            if (File.Exists(fileName))
            {
                // Load data from the JSON file
                string json = File.ReadAllText(fileName);

                // Deserialize the JSON data into a list of items
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);

                // Add each item to the ListBox
                foreach (Item item in items)
                {
                    listBox.Items.Add($"Item Number: {item.ItemNumber}   |   Name: {item.Name}  |   Price: {item.Price}$");
                }
            }
            else
            {
                // File does not exist, ListBox will be empty
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(itemNumberTextBox.Text) ||
                string.IsNullOrEmpty(itemNameTextBox.Text) ||
                string.IsNullOrEmpty(itemDescriptionTextBox.Text) ||
                string.IsNullOrEmpty(itemPriceTextBox.Text))
            {
                MessageBox.Show("Please fill in all the fields.");
                return;
            }

            // Check if the item number and price fields contain valid numbers
            if (!int.TryParse(itemNumberTextBox.Text, out int itemNumber) || !double.TryParse(itemPriceTextBox.Text, out double price))
            {
                MessageBox.Show("Please enter valid numbers in the Item Number and Price fields.");
                return; 
            }


            int itemNumb = Convert.ToInt32(itemNumberTextBox.Text);
            string name = itemNameTextBox.Text;
            string description = itemDescriptionTextBox.Text;
            double pri = Convert.ToDouble(itemPriceTextBox.Text);

            ItemService itemService = new ItemService("items.json");
            bool success = itemService.CreateItem(itemNumb, name, description, pri);
            

            if (success)
            {
                MessageBox.Show("Item created successfully!");
                // Reload the items into the ListBox
                LoadItemsToListBox(itemListBox);
            }
            else
            {
                MessageBox.Show("Item with the same number already exists!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadItemsToListBox(itemListBox);
        }




        //-----------------------------------------------------------------------
        //add items to shopping list 
        

        private void addToList_Click(object sender, EventArgs e)
        {
            int itemNumber;
            if (int.TryParse(NumberTextBox.Text, out itemNumber))
            {

                ShoppingListItem existingItem = shoppingList.FirstOrDefault(item => item.ItemNumber == itemNumber);

                if (existingItem != null)
                {
                    // Item already exists, increase quantity
                    existingItem.Quantity++;
                    existingItem.Discount = string.IsNullOrEmpty(discountTextBox.Text) ? 0 : double.Parse(discountTextBox.Text);
                }
                else
                {
                    // Item does not exist, add new item
                    Item item = mainClass.GetItemByNumber(itemNumber);

                    if (item != null)
                    {
                        ShoppingListItem newItem = new ShoppingListItem();
                        newItem.ItemNumber = itemNumber;
                        newItem.Name = item.Name;
                        newItem.Price = item.Price;
                        newItem.Quantity = 1;
                        newItem.Discount = string.IsNullOrEmpty(discountTextBox.Text) ? 0 : double.Parse(discountTextBox.Text);

                        shoppingList.Add(newItem);
                        //MessageBox.Show("Success! ");

                    }
                    else
                    {
                        MessageBox.Show("Item not found. Please enter a valid item number.");
                    }
                }

                // Refresh the shopping list display or perform any additional tasks
                dgvShoppingList.DataSource = null;
                dgvShoppingList.DataSource = shoppingList;
                double totalPrice = shoppingList.Sum(item => item.TotalPrice);
                TotalPriceTxt.Text = totalPrice.ToString();

            }
            else
            {
                MessageBox.Show("Please enter a valid item number.");
            }

        }
       
        private void clearbtn_Click(object sender, EventArgs e)
        {

            NumberTextBox.Text = "";
            discountTextBox.Text = "";
            TotalPriceTxt.Text = "";
            // Clear the shopping list
            shoppingList.Clear();

            // Clear the DataGridView
            dgvShoppingList.DataSource = null;
            dgvShoppingList.Rows.Clear();
            //dgvShoppingList.Columns.Clear();
        }

        private void savePurchase_Click(object sender, EventArgs e)
        {
            // Create an object to store the purchase details
            
            if (TotalPriceTxt.Text=="")
            {
                MessageBox.Show("you should add items to shopping list");
            }
            else
            {
                Purchase purchase = new Purchase();
                purchase.Items = shoppingList;
                purchase.FinalPrice = double.Parse(TotalPriceTxt.Text);
                // Convert the purchase object to JSON
                string json = JsonConvert.SerializeObject(purchase);

                // Write the JSON to a file
                File.WriteAllText("purchase.json", json);

                MessageBox.Show("Purchase saved successfully!");

            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            // Read the JSON file
            string json = File.ReadAllText("purchase.json");

            // Convert the JSON to a Purchase object
            Purchase purchase = JsonConvert.DeserializeObject<Purchase>(json);

            // Create a StringBuilder to build the factura
            StringBuilder factura = new StringBuilder();

            // Add the factura header
            factura.AppendLine("Purchase: \n");

            // Add the shopping list items
            factura.AppendLine("Shopping List Items:");
            foreach (var item in purchase.Items)
            {
                factura.AppendLine($"- {item.Name} - Quantity: {item.Quantity}, Price: {item.Price}, Discount: {item.Discount}");
            }

            // Add the final price
            factura.AppendLine($"Final Price: {purchase.FinalPrice}");

            // Print the factura
            PrintDocument document = new PrintDocument();
            document.PrintPage += (s, ev) =>
            {
                Font font = new Font("Arial", 12);
                ev.Graphics.DrawString(factura.ToString(), font, Brushes.Black, ev.MarginBounds.Left, ev.MarginBounds.Top);
            };

            PrintDialog dialog = new PrintDialog();
            dialog.Document = document;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                document.Print();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user confirms the logout, close all windows of the application
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

    }
}
