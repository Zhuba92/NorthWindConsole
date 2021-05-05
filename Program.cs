using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Mode;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Add a product");
                    Console.WriteLine("6) Edit a product");
                    Console.WriteLine("7) View all products");
                    Console.WriteLine("8) View a specific product");
                    Console.WriteLine("9) Edit a category");
                    Console.WriteLine("10) Display all categories");
                    Console.WriteLine("11) Display all categories and their related active products");
                    Console.WriteLine("12) Display a specific category and its active products");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (choice == "2") // add a category
                    {
                        Categories category = new Categories();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NWConsole_96_ZPHContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.addCategory(category);
                            }
                            logger.Info("Category added");
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    else if(choice == "9") // edit category (refine)
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.OrderBy(c => c.CategoryId);
                        Categories category = new Categories();
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}. {item.CategoryName} - {item.Description}");
                        }
                        Console.Write("Which number category do you wish to edit: ");
                        int selectedCat = Int32.Parse(Console.ReadLine());
                        category = db.Categories.FirstOrDefault(c => c.CategoryId == selectedCat);
                        Console.WriteLine("What would you like to edit: \n1. Name\n2. Description");
                        int selection = Int32.Parse(Console.ReadLine());
                        while(selection != 1 || selection != 2)
                        {
                            if(selection == 1)
                            {
                                Console.Write("What is the new name: ");
                                category.CategoryName = Console.ReadLine();
                            }
                            else
                            {
                                Console.Write("WHat is the new description: ");
                                category.CategoryName = Console.ReadLine();
                            }
                        }
                        db.EditCategory(category);
                        logger.Info("Category edited");
                    }
                    else if(choice == "10") //display all categories names and desc.
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.OrderBy(c => c.CategoryId);
                        Categories category = new Categories();
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    }
                    else if(choice == "11") // display all categories and their related active products
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.Include("Products").OrderBy(c => c.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                            foreach(Products p in item.Products)
                            {
                                if(p.Discontinued == false)
                                {
                                    Console.WriteLine($"{p.ProductName}");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    else if(choice == "12") // display a specific category and its active products
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}. {item.CategoryName}");
                        }
                        Console.Write("Which category number do you want to view: ");
                        int selection = Int32.Parse(Console.ReadLine());
                        Categories selectedCategory = query.FirstOrDefault(c => c.CategoryId == selection);
                        Console.WriteLine(selectedCategory.CategoryName);
                        foreach(Products p in selectedCategory.Products)
                        {
                            if(p.Discontinued == false)
                            {
                                Console.WriteLine($"{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "3")
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Categories category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Products p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    else if (choice == "4")
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Products p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    else if (choice == "5") // add a product
                    {
                        var db = new NWConsole_96_ZPHContext();
                        Products product = new Products();
                        Console.WriteLine("What is the product name: ");
                        product.ProductName = Console.ReadLine();
                        Console.WriteLine("What is the supplier ID (1 - 29): ");
                        int id = Int32.Parse(Console.ReadLine());
                        if(id < 1 || id > 29)
                        {
                            Console.WriteLine("Not a valid Supplier... Please enter a valid ID");
                            Console.WriteLine("What is the supplier ID (1 - 29): ");
                            id = Int32.Parse(Console.ReadLine());
                        }
                        else product.SupplierId = id;
                        var query = db.Categories.OrderBy(p => p.CategoryName);
                        int count = 1;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{count}. {item.CategoryName}");
                            count++;
                        }
                        Console.WriteLine("Enter the number of the category: ");
                        try
                        {
                            int cId = Int32.Parse(Console.ReadLine());
                            product.CategoryId = cId;
                        }
                        catch (Exception zz){logger.Error(zz.Message);}
                        Console.WriteLine("What is the quantity per unit? eg: 12 - 8oz jars");
                        try
                        {
                            product.QuantityPerUnit = Console.ReadLine();
                            Console.WriteLine("What is the unit price: ");
                            Double price = Double.Parse(Console.ReadLine());
                            string stringPrice = price.ToString("0.00");
                            Decimal doublePrice = Decimal.Parse(stringPrice);
                            product.UnitPrice = doublePrice;
                        }
                        catch(Exception x){logger.Error(x.Message);}
                        Console.WriteLine("How many units are in stock: ");
                        try
                        {
                            short units = short.Parse(Console.ReadLine());
                            product.UnitsInStock = units;
                        }
                        catch (Exception xx){logger.Error(xx.Message);}
                        Console.WriteLine("How many units are on order: ");
                        try
                        {
                            short unitsOnOrder = short.Parse(Console.ReadLine());
                            product.UnitsOnOrder = unitsOnOrder;
                        }
                        catch (Exception xx){logger.Error(xx.Message);}
                        Console.WriteLine("What is the reorder level: ");
                        try
                        {
                            short reOrder = short.Parse(Console.ReadLine());
                            product.UnitsOnOrder = reOrder;
                        }
                        catch (Exception xx){logger.Error(xx.Message);}
                        Boolean discontinued = false;
                        product.Discontinued = discontinued;
                        db.addProduct(product);
                        logger.Info("Product added");
                    }
                    else if(choice == "6") // Edit a product
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query2 = db.Categories.OrderBy(c => c.CategoryId);
                        Categories category = new Categories();
                        var query = db.Products.OrderBy(p => p.ProductId);
                        foreach (var p in query)
                        {
                            Console.WriteLine($"\t{p.ProductId}. {p.ProductName}, {p.SupplierId}, {p.CategoryId}, {p.QuantityPerUnit}, {p.UnitPrice}, {p.UnitsInStock}, {p.UnitsOnOrder}, {p.ReorderLevel}, {p.Discontinued}");
                        }
                        Console.WriteLine("Enter the number of the product you would like to edit: ");
                        int id = Int32.Parse(Console.ReadLine());
                        Products selectedProduct = db.Products.FirstOrDefault(p => p.ProductId == id);
                        Console.WriteLine("What would you like to change: \n1. Name\n2. Supplier ID\n3. Category ID\n4. Quantity per unit\n5. Unit Price\n6. Units in stock\n7. Units on order\n8. Reorder level\n9. active status");
                        int selection = Int32.Parse(Console.ReadLine());
                        switch (selection)
                        {
                            case 1:
                                Console.Write("What is the new name: ");
                                selectedProduct.ProductName = Console.ReadLine();
                                break;
                            case 2:
                                Console.WriteLine("What is the supplier ID (1 - 29): ");
                                int sId = Int32.Parse(Console.ReadLine());
                                if(sId < 1 || sId > 29)
                                {
                                    Console.WriteLine("Not a valid Supplier... Please enter a valid ID");
                                    Console.WriteLine("What is the supplier ID (1 - 29): ");
                                    sId = Int32.Parse(Console.ReadLine());
                                }
                                else selectedProduct.SupplierId = sId;
                                break;
                            case 3:
                                foreach (var item in query2)
                                {
                                    Console.WriteLine($"{item.CategoryId}. {item.CategoryName}");
                                }
                                Console.Write("What is the new Category ID: ");
                                selectedProduct.CategoryId = Int32.Parse(Console.ReadLine());
                                break;
                            case 4:
                                Console.Write("What is the new Quantity per unit: ");
                                selectedProduct.QuantityPerUnit = Console.ReadLine();
                                break;
                            case 5:
                                Console.Write("What is the new Unit price: ");
                                try
                                {
                                    selectedProduct.QuantityPerUnit = Console.ReadLine();
                                    Console.WriteLine("What is the unit price: ");
                                    Double price = Double.Parse(Console.ReadLine());
                                    string stringPrice = price.ToString("0.00");
                                    Decimal doublePrice = Decimal.Parse(stringPrice);
                                    selectedProduct.UnitPrice = doublePrice;
                                }
                                catch(Exception x){logger.Error(x.Message);}
                                break;
                            case 6:
                                Console.Write("How many units are in stock: ");
                                selectedProduct.UnitsInStock = short.Parse(Console.ReadLine());
                                break;
                            case 7:
                                Console.Write("How many units are on order: ");
                                selectedProduct.UnitsOnOrder = short.Parse(Console.ReadLine());
                                break;
                            case 8:
                                Console.Write("What is the reorder level: ");
                                selectedProduct.ReorderLevel = short.Parse(Console.ReadLine());
                                break;
                            case 9:
                                Console.Write("Is the product discontinued (Y/N): ");
                                string disc = Console.ReadLine();
                                if(disc.Equals("y"))
                                {
                                    selectedProduct.Discontinued = true;
                                }
                                else selectedProduct.Discontinued = false;
                                break;
                            default:
                                break;

                        }
                        db.EditProduct(selectedProduct);
                        logger.Info($"{selectedProduct.ProductName} edited");
                    }
                    else if(choice == "7") //Display all records in products table(name only) distringuish discontinued products
                    {
                        Console.WriteLine("1. Display all the products");
                        Console.WriteLine("2. Display all the current products");
                        Console.WriteLine("3. Display all the discontinued products");
                        int selection = Int32.Parse(Console.ReadLine());
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Products.OrderBy(p => p.ProductId);
                        if(selection == 1)
                        {
                            query = db.Products.OrderBy(p => p.ProductId);
                        }
                        else if(selection == 2)
                        {
                            query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductId);
                        }
                        else
                        {
                            query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductId);
                        }
                        foreach(var item in query)
                        {
                            Console.WriteLine(item.ProductName);
                        }
                    }
                    else if(choice == "8") // View specific product and all its fields (make nicer)
                    {
                        var db = new NWConsole_96_ZPHContext();
                        var query = db.Products.OrderBy(p => p.ProductId);
                        Console.Write("Which product would you like to see: ");
                        string productViewed = Console.ReadLine();
                        Products product = db.Products.FirstOrDefault(p => p.ProductName == productViewed);
                        Console.WriteLine($"{product.ProductId}, {product.ProductName}");
                    }

                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
