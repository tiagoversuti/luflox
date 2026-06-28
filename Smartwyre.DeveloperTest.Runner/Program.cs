using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Services.Calculators;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        var calculators = new List<IIncentiveCalculator>
        {
            new FixedCashAmountCalculator(),
            new FixedRateRebateCalculator(),
            new AmountPerUomCalculator()
        };

        var rebateStore = new InMemoryRebateDataStore();
        var productStore = new InMemoryProductDataStore();
        var service = new RebateService(productStore, rebateStore, calculators);

        if (args.Length != 3 || !decimal.TryParse(args[2], out decimal volume))
        {
            Console.WriteLine();
            Console.WriteLine("Usage: runner <rebateId> <productId> <volume>");
            Console.WriteLine();
            Console.WriteLine("Available rebates:  CASH  RATE  UOM");
            Console.WriteLine("Available products: PROD-A  PROD-B");
            Console.WriteLine();
            return;
        }

        var result = service.Calculate(new CalculateRebateRequest
        {
            RebateIdentifier = args[0],
            ProductIdentifier = args[1],
            Volume = volume
        });

        Console.WriteLine();
        Console.WriteLine(result.Success
            ? "Result: SUCCESS — rebate calculated and stored."
            : "Result: FAILED — invalid inputs or unsupported incentive type for this product.");
        Console.WriteLine();
    }
}

class InMemoryRebateDataStore : IRebateDataStore
{
    private static readonly Dictionary<string, Rebate> _rebates = new()
    {
        ["CASH"] = new Rebate { Identifier = "CASH", Incentive = IncentiveType.FixedCashAmount, Amount = 50m },
        ["RATE"] = new Rebate { Identifier = "RATE", Incentive = IncentiveType.FixedRateRebate, Percentage = 0.05m },
        ["UOM"]  = new Rebate { Identifier = "UOM",  Incentive = IncentiveType.AmountPerUom, Amount = 2m }
    };

    public Rebate GetRebate(string rebateIdentifier) =>
        _rebates.GetValueOrDefault(rebateIdentifier);

    public void StoreCalculationResult(Rebate account, decimal rebateAmount)
    {
        Console.WriteLine();
        Console.WriteLine($"  [Store] Saved rebate amount: {rebateAmount:C}");
        Console.WriteLine();
    }
}

class InMemoryProductDataStore : IProductDataStore
{
    private static readonly Dictionary<string, Product> _products = new()
    {
        ["PROD-A"] = new Product { Identifier = "PROD-A", Price = 100m, Uom = "each", SupportedIncentives = SupportedIncentiveType.FixedCashAmount | SupportedIncentiveType.FixedRateRebate },
        ["PROD-B"] = new Product { Identifier = "PROD-B", Price = 200m, Uom = "kg",   SupportedIncentives = SupportedIncentiveType.AmountPerUom }
    };

    public Product GetProduct(string productIdentifier) =>
        _products.GetValueOrDefault(productIdentifier);
}
