﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Data;

using System.Windows.Navigation;
using PHPReSP.Classes;

namespace PHPReSP
{ 

    public partial class GenerateReport : Window
    {

        private DataManager manager = new DataManager();

        public GenerateReport()
        {
            InitializeComponent();
            Predict_Sales();
            BarChartData(Period.SelectedIndex);
            LineChartData(Period.SelectedIndex);
            RestockWarning();
        }

        private void RestockWarning()
        {
            string restockquery =
                "SELECT Products.productName AS Product, " +
                "CurrentInventory AS Stock_Level, " + "RestockLevel AS Restock_Level, " +
                "IF (SUM(Products.CurrentInventory) < SUM(Products.RestockLevel), \"RESTOCK REQUIRED\", \"\") AS Restock FROM Products " +
                "WHERE ((Products.CurrentInventory) < (Products.RestockLevel)) " + "GROUP BY ProductName; ";

            MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=password;database=phpsreps_db");
            MySqlCommand cmd = new MySqlCommand(restockquery, connection);

            DataTable dti = new DataTable();
            connection.Open();
            MySqlDataReader sdr = cmd.ExecuteReader();
            dti.Load(sdr);
            connection.Close();
            InvGrid.ItemsSource = dti.DefaultView;

        }

        private void RestockWarning(object sender, EventArgs e)
        {
            RestockWarning();
        }

        //View all from these viewer buttons
        private void ViewLineChart(object sender, RoutedEventArgs e)
        {
            LineChartViewer page = new LineChartViewer();
            page.ShowDialog();
        }

        private void ViewBarChart(object sender, RoutedEventArgs e)
        {
            BarChartViewer page = new BarChartViewer();
            page.ShowDialog();
        }

        private void ViewPredictionCharts(object sender, RoutedEventArgs e)
        {
            PredictionPage page = new PredictionPage();
            page.ShowDialog();
        }

        private void ViewProducts(object sender, RoutedEventArgs e)
        {
            ProductRecordPage page = new ProductRecordPage();
            page.ShowDialog();
        }

        public void LineChartData(int index)
        {
            manager.ConvertDBtoSalesObj();

            if(index == 0)
            {
                var revenues = new MonthsRevenue();
                var chartData = new KeyValuePair<string, double>[revenues.months.Count()];


                for (int i = 0; i <= revenues.months.Count() - 1; i++)
                {
                    chartData[i] = new KeyValuePair<string, double>(revenues.months[i], revenues.revenues[i]);
                }
               ((LineSeries)LChart.Series[0]).ItemsSource = chartData;
            }
            else if(index == 1)
            {
                var revenues = new WeeksRevenue();
                var chartData = new KeyValuePair<string, double>[revenues.weeks.Count()];


                for (int i = 0; i <= revenues.weeks.Count() - 1; i++)
                {
                    chartData[i] = new KeyValuePair<string, double>(revenues.weeks[i], revenues.revenues[i]);
                }
               ((LineSeries)LChart.Series[0]).ItemsSource = chartData;
            }

           

        }

        public void BarChartData(int index)
        {
            manager.ConvertDBtoSalesObj();

            if (index == 0)
            {
                var revenues = new ItemsRevenue();
                var chartData = new KeyValuePair<string, double>[revenues.items.Count()];


                for (int i = 0; i <= revenues.items.Count() - 1; i++)
                {
                    chartData[i] = new KeyValuePair<string, double>(revenues.items[i], revenues.revenues[i]);
                }

                ((ColumnSeries)BChart.Series[0]).ItemsSource = chartData;
            }
            else if(index == 1)
            {
                var revenues = new WeeksRevenue();
                var chartData = new KeyValuePair<string, double>[revenues.weeks.Count()];


                for (int i = 0; i <= revenues.weeks.Count() - 1; i++)
                {
                    chartData[i] = new KeyValuePair<string, double>(revenues.weeks[i], revenues.revenues[i]);
                }
                ((ColumnSeries)BChart.Series[0]).ItemsSource = chartData;

               
            }
            else
            {
                var revenues = new MonthsRevenue();
                var chartData = new KeyValuePair<string, double>[revenues.months.Count()];


                for (int i = 0; i <= revenues.months.Count() - 1; i++)
                {
                    chartData[i] = new KeyValuePair<string, double>(revenues.months[i], revenues.revenues[i]);
                }
                ((ColumnSeries)BChart.Series[0]).ItemsSource = chartData;
            }

        }

        //Code for Prediction on Sales
        private void Predict_Sales()
        {
            String queryString = "";

            String queryMonthCategory = "SELECT products.Category, SUM(sales.NumberSold) AS Predicted_Sales, " +
              "CurrentInventory - SUM(sales.NumberSold) AS Remaining, " +
              "IF(SUM(CurrentInventory) < SUM(sales.NumberSold), \"RESTOCK REQUIRED\", \"\") AS Restock  FROM sales " +
              "INNER JOIN Products ON Products.productID = sales.ProductID " +
              "WHERE MONTH(SaleDate) = MONTH(CURRENT_DATE() - INTERVAL 1 MONTH) " +
              "AND YEAR(SaleDate) = YEAR(CURRENT_DATE() - INTERVAL 1 MONTH) " +
              "GROUP BY Category; ";

            String queryWeekCategory = "SELECT products.Category, SUM(sales.NumberSold) AS Predicted_Sales, " +
                "CurrentInventory - SUM(sales.NumberSold) AS Remaining, " +
                "IF(SUM(CurrentInventory) < SUM(sales.NumberSold), \"RESTOCK REQUIRED\", \"\") AS Restock  FROM sales " +
                "INNER JOIN Products ON Products.productID = sales.ProductID " +
                "WHERE YEARWEEK(SaleDate) = YEARWEEK(NOW() - INTERVAL 1 WEEK) " +
                "GROUP BY Category;";

            String queryMonthProduct = "SELECT products.productName, SUM(sales.NumberSold) AS Predicted_Sales, " +
                "CurrentInventory - SUM(sales.NumberSold) AS Remaining, " +
                "IF(SUM(CurrentInventory) < SUM(sales.NumberSold), \"RESTOCK REQUIRED\", \"\") AS Restock  FROM sales " +
                "INNER JOIN Products ON Products.productID = sales.ProductID " +
                "WHERE MONTH(SaleDate) = MONTH(CURRENT_DATE() - INTERVAL 1 MONTH) " +
                "AND YEAR(SaleDate) = YEAR(CURRENT_DATE() - INTERVAL 1 MONTH) " +
                "GROUP BY ProductName; ";

            String queryWeekProduct = "SELECT products.productName, SUM(sales.NumberSold) AS Predicted_Sales, " +
                "CurrentInventory - SUM(sales.NumberSold) AS Remaining, " +
                "IF(SUM(CurrentInventory) < SUM(sales.NumberSold), \"RESTOCK REQUIRED\", \"\") AS Restock  FROM sales " +
                "INNER JOIN Products ON Products.productID = sales.ProductID " +
                "WHERE YEARWEEK(SaleDate) = YEARWEEK(NOW() - INTERVAL 1 WEEK) " +
                "GROUP BY ProductName;";

            String type = Type.SelectionBoxItem.ToString();
            String period = Period.SelectionBoxItem.ToString();

            if (period == "Month" && type == "Category")
            {
                queryString = queryMonthCategory;
            }
            if (period == "Week" && type == "Category")
            {
                queryString = queryWeekCategory;
            }
            if (period == "Month" && type == "Product")
            {
                queryString = queryMonthProduct;
            }
            if (period == "Week" && type == "Product")
            {
                queryString = queryWeekProduct;
            }

            MySqlConnection connection = new MySqlConnection("server=localhost;uid=root;pwd=password;database=phpsreps_db");
            MySqlCommand cmd = new MySqlCommand(queryString, connection);

            DataTable dt = new DataTable();
            connection.Open();
            MySqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            connection.Close();
            dtg.ItemsSource = dt.DefaultView;

        }

        private void Predict_Sales(object sender, EventArgs e)
        {
            Predict_Sales();
        }



        //Code for Charts



        private void Close(object sender, RoutedEventArgs e)
        {
            HomeInterface page = new HomeInterface();
            page.Show();
            this.Close();
        }

        private void Period_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BarChartData(Period.SelectedIndex);
            LineChartData(Period.SelectedIndex);
        }
    }
}
