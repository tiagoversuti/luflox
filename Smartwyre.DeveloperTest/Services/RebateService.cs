using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IEnumerable<IIncentiveCalculator> _calculators;

    public RebateService(
        IProductDataStore productDataStore,
        IRebateDataStore rebateDataStore,
        IEnumerable<IIncentiveCalculator> calculators)
    {
        _productDataStore = productDataStore;
        _rebateDataStore = rebateDataStore;
        _calculators = calculators;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        var product = _productDataStore.GetProduct(request.ProductIdentifier);

        if (rebate == null || product == null)
            return new CalculateRebateResult { Success = false };

        var calculator = _calculators.SingleOrDefault(c => c.IncentiveType == rebate.Incentive);

        if (calculator == null || !calculator.IsValid(rebate, product, request))
            return new CalculateRebateResult { Success = false };

        var rebateAmount = calculator.Calculate(rebate, product, request);
        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

        return new CalculateRebateResult { Success = true };
    }
}
