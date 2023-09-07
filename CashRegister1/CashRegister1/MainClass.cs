using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CashRegister1
{
    internal class MainClass
    {
        // Cashier model class
        public class Cashier
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // Item  class
        public class Item
        {
            public int ItemNumber { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
        }


        public class ShoppingListItem
        {
            public int ItemNumber { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public double Discount  { get; set; }
            public double TotalPrice => (Price - (Price*Discount/100)) * Quantity;
        }
        
        
        //purchase
        public class Purchase
        {
            public List<ShoppingListItem> Items { get; set; }
            public double FinalPrice { get; set; }
        }


        //get items number methodes : 
        public List<Item> LoadItemsFromFile()
        {
            string json = File.ReadAllText("items.json");
            return JsonConvert.DeserializeObject<List<Item>>(json);
        }

        public Item GetItemByNumber(int itemNumber)
        {
            List<Item> items = LoadItemsFromFile();
            return items.FirstOrDefault(item => item.ItemNumber == itemNumber);
        }



        public class LoginService
        {
            private List<Cashier> cashiers;
            private string cashiersFilePath;

            public LoginService(string cashiersFilePath)
            {
                this.cashiersFilePath = cashiersFilePath;
                LoadCashiers();
            }

            private void LoadCashiers()
            {
                if (File.Exists(cashiersFilePath))
                {
                    string json = File.ReadAllText(cashiersFilePath);
                    cashiers = JsonConvert.DeserializeObject<List<Cashier>>(json);
                }
                else
                {
                    cashiers = new List<Cashier>();
                }
            }

            private void SaveCashiers()
            {
                string json = JsonConvert.SerializeObject(cashiers);
                File.WriteAllText(cashiersFilePath, json);
            }

            public bool Login(string username, string password)
            {
                Cashier cashier = cashiers.FirstOrDefault(c => c.Username == username && c.Password == password);
                return cashier != null;
            }

            public bool Register(string username, string password)
            {
                if (cashiers.Any(c => c.Username == username))
                {

                    return false;
                }
                else
                {
                    Cashier newCashier = new Cashier { Username = username, Password = password };
                    cashiers.Add(newCashier);
                    SaveCashiers();
                    return true;
                }

            }
        }


        

        // items / create item / save to json file
        public class ItemService
        {
            private List<Item> items;
            private string itemsFilePath;

            public ItemService(string itemsFilePath)
            {
                this.itemsFilePath = itemsFilePath;
                LoadItems();
            }

            private void LoadItems()
            {
                if (File.Exists(itemsFilePath))
                {
                    string json = File.ReadAllText(itemsFilePath);
                    items = JsonConvert.DeserializeObject<List<Item>>(json);
                }
                else
                {
                    items = new List<Item>();
                }
            }

            private void SaveItems()
            {
                string json = JsonConvert.SerializeObject(items);
                File.WriteAllText(itemsFilePath, json);
            }

            public bool CreateItem(int itemNumber, string name, string description, double price)
            {
                if (items.Exists(i => i.ItemNumber == itemNumber))
                {
                    return false;
                }
                else
                {
                    Item newItem = new Item
                    {
                        ItemNumber = itemNumber,
                        Name = name,
                        Description = description,
                        Price = price
                    };
                    items.Add(newItem);
                    SaveItems();
                    return true;
                }
            }
        }


    }
}
