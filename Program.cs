using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace JsonToCsvConverter
{
    public class Transaction
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Income / Expense
        public decimal Amount { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string jsonFilePath = "transactions.json";
            string csvFilePath = "transactions_summary.csv";

            Console.WriteLine("==================================================");
            Console.WriteLine("   C# JSON to CSV Financial Data Converter");
            Console.WriteLine("==================================================\n");

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine($"[ERROR] File '{jsonFilePath}' not found.");
                return;
            }

            try
            {
                Console.WriteLine("[1/3] Reading JSON file...");
                string jsonString = File.ReadAllText(jsonFilePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<Transaction>? transactions = JsonSerializer.Deserialize<List<Transaction>>(jsonString, options);

                if (transactions == null || transactions.Count == 0)
                {
                    Console.WriteLine("[WARNING] No transactions found in JSON.");
                    return;
                }

                Console.WriteLine($"[2/3] Processing {transactions.Count} records...");

                decimal totalIncome = 0;
                decimal totalExpense = 0;

                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    // Write Header CSV
                    writer.WriteLine("TransactionId,Date,Category,Type,Amount");

                    foreach (var item in transactions)
                    {
                        writer.WriteLine($"{item.TransactionId},{item.Date},{item.Category},{item.Type},{item.Amount}");

                        if (item.Type.Equals("Income", StringComparison.OrdinalIgnoreCase))
                        {
                            totalIncome += item.Amount;
                        }
                        else if (item.Type.Equals("Expense", StringComparison.OrdinalIgnoreCase))
                        {
                            totalExpense += item.Amount;
                        }
                    }
                }

                Console.WriteLine("[3/3] CSV Generated Successfully!\n");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Total Income  : Rp {totalIncome:N0}");
                Console.WriteLine($"Total Expense : Rp {totalExpense:N0}");
                Console.WriteLine($"Net Profit    : Rp {(totalIncome - totalExpense):N0}");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"Output saved to: {csvFilePath}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRITICAL ERROR] An error occurred: {ex.Message}");
            }
        }
    }
}