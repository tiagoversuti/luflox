using FluentAssertions;
using Smartwyre.DeveloperTest.Services.Calculators;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests
{
    public class AmountPerUomCalculatorTests
    {
        private readonly AmountPerUomCalculator _calculator;

        public AmountPerUomCalculatorTests()
        {
            _calculator = new AmountPerUomCalculator();
        }

        [Fact]
        public void IsValid_WhenProductDoesntSupportAmountPerUom_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = 10 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedCashAmount };
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
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.AmountPerUom };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WhenRequestVolumeIsZero_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = 10 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.AmountPerUom };
            var request = new Types.CalculateRebateRequest { Volume = 0 };

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
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.AmountPerUom };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(10, 5, 50)]
        [InlineData(7.1, 4.2, 29.82)]
        public void Calculate_ShouldReturnCorrectAmount(decimal amount, decimal volume, decimal calculatedRebate)
        {
            // Arrange
            var rebate = new Types.Rebate { Amount = amount };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.AmountPerUom };
            var request = new Types.CalculateRebateRequest { Volume = volume };

            // Act
            var result = _calculator.Calculate(rebate, product, request);

            // Assert
            result.Should().Be(calculatedRebate);
        }
    }
}
