﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPReSP.Classes
{
    class WeeksRevenue
    {

        public List<double> revenues = new List<double>();
        public List<string> weeks = new List<string>();

        public WeeksRevenue()
        {

            PopulateLists();

        }

        public void PopulateLists()
        {
            revenues.Clear();
            weeks.Clear();

            MySqlConnection connection = new MySqlConnection(
           "server=localhost;uid=root;pwd=password;database=phpsreps_db");

            connection.Open();

            MySqlCommand cmd = new MySqlCommand("SELECT week(sales.SaleDate) AS Week, SUM(sales.NumberSold * products.SellPrice) AS Revenue FROM sales INNER JOIN Products ON Products.productID = sales.ProductID GROUP BY Week ORDER BY sales.SaleDate; ", connection);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                { 
                    this.weeks.Add(reader[0].ToString());
                    this.revenues.Add(Convert.ToDouble(reader[1]));
                }
            }


            reader.Close();

            connection.Close();

        }

    }
}
