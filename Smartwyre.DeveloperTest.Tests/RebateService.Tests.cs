using FluentAssertions;
using Moq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private readonly RebateService _rebateService;

    private readonly Mock<IProductDataStore> _productDataStoreMock;
    private readonly Mock<IRebateDataStore> _rebateDataStoreMock;
    private readonly List<IIncentiveCalculator> _calculators;

    private CalculateRebateRequest _request;

    public RebateServiceTests()
    {
        _productDataStoreMock = new Mock<IProductDataStore>();
        _rebateDataStoreMock = new Mock<IRebateDataStore>();
        _calculators = new List<IIncentiveCalculator>();

        _rebateService = new RebateService(_productDataStoreMock.Object, _rebateDataStoreMock.Object, _calculators);
    }

    [Fact]
    public void Calculate_WhenRebateIsNull_ShouldReturnFalse()
    {
        // Arrange
        _request = new CalculateRebateRequest();

        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((Rebate)null);

        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(product);

        //Act
        var result = _rebateService.Calculate(_request);

        //Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Calculate_WhenProductIsNull_ShouldReturnFalse()
    {
        // Arrange
        _request = new CalculateRebateRequest();

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);

        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((Product)null);

        //Act
        var result = _rebateService.Calculate(_request);

        //Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Calculate_WhenCalculatorIsNull_ShouldReturnFalse()
    {
        // Arrange
        _request = new CalculateRebateRequest();

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);

        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(product);

        _calculators.Clear();

        //Act
        var result = _rebateService.Calculate(_request);

        //Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Calculate_WhenCalculatorIsNotValid_ShouldReturnFalse()
    {
        // Arrange
        _request = new CalculateRebateRequest();

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);

        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(product);

        var calculator = new Mock<IIncentiveCalculator>();
        calculator.Setup(x => x.IncentiveType).Returns(IncentiveType.FixedCashAmount);
        calculator.Setup(x => x.IsValid(rebate, product, _request)).Returns(false);
        _calculators.Add(calculator.Object);

        //Act
        var result = _rebateService.Calculate(_request);

        //Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Calculate_WhenCalculatorIsValid_ShouldStoreCalculationResult()
    {
        // Arrange
        _request = new CalculateRebateRequest();

        var rebate = new Rebate { Incentive = IncentiveType.FixedCashAmount };
        _rebateDataStoreMock.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(rebate);

        var product = new Product { SupportedIncentives = SupportedIncentiveType.FixedCashAmount };
        _productDataStoreMock.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(product);

        var calculator = new Mock<IIncentiveCalculator>();
        calculator.Setup(x => x.IncentiveType).Returns(IncentiveType.FixedCashAmount);
        calculator.Setup(x => x.IsValid(rebate, product, _request)).Returns(true);
        calculator.Setup(x => x.Calculate(rebate, product, _request)).Returns(100);
        _calculators.Add(calculator.Object);

        //Act
        var result = _rebateService.Calculate(_request);

        //Assert
        _rebateDataStoreMock.Verify(x => x.StoreCalculationResult(rebate, 100), Times.Once);
        result.Success.Should().BeTrue();
    }
}
