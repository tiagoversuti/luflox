using FluentAssertions;
using Smartwyre.DeveloperTest.Services.Calculators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests
{
    public class FixedCashAmountCalculatorTests
    {
        private readonly FixedCashAmountCalculator _calculator;

        public FixedCashAmountCalculatorTests()
        {
            _calculator = new FixedCashAmountCalculator();
        }

        [Fact]
        public void IsValid_WhenProductDoesntSupportFixedCashAmount_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = 10 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.AmountPerUom };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WhenRebateAmountIsZero_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = 0 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedCashAmount };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WhenAllConditionsAreMet_ShouldReturnTrue()
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = 10 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedCashAmount };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(10, 5, 10)]
        [InlineData(20, 10, 20)]
        public void Calculate_ShouldReturnRebateAmount(decimal amount, decimal volume, decimal calculatedRebate)
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = amount };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedCashAmount };
            var request = new Types.CalculateRebateRequest { Volume = volume };

            // Act
            var result = _calculator.Calculate(rebate, product, request);

            // Assert
            result.Should().Be(calculatedRebate);
        }
    }
}
