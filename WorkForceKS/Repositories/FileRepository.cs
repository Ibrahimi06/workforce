using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

public class FileRepository
{
    private string filePath = "data/products.csv";

    public List<Product> GetAll()
    {
        return File.ReadAllLines(filePath)
            .Select(line =>
            {
                var parts = line.Split(',');
                return new Product
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Category = parts[2],
                    Price = double.Parse(parts[3], CultureInfo.InvariantCulture),
                    Quantity = int.Parse(parts[4])
                };
            }).ToList();
    }

    public Product GetById(int id)
    {
        return GetAll().FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        var products = GetAll();
        product.Id = products.Any() ? products.Max(p => p.Id) + 1 : 1;
        products.Add(product);
        Save(products);
    }

    public void Update(Product updated)
    {
        var products = GetAll();
        var index = products.FindIndex(p => p.Id == updated.Id);

        if (index != -1)
        {
            products[index] = updated;
            Save(products);
        }
    }

    public void Delete(int id)
    {
        var products = GetAll();
        products = products.Where(p => p.Id != id).ToList();
        Save(products);
    }

    public void Save(List<Product> products)
    {
        var lines = products.Select(p =>
            $"{p.Id},{p.Name},{p.Category},{p.Price},{p.Quantity}");

        File.WriteAllLines(filePath, lines);
    }
}
