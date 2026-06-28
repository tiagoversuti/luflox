using FluentAssertions;
using Smartwyre.DeveloperTest.Services.Calculators;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests
{
    public class FixedRateRebateCalculatorTests
    {
        private readonly FixedRateRebateCalculator _calculator;

        public FixedRateRebateCalculatorTests()
        {
            _calculator = new FixedRateRebateCalculator();
        }

        [Fact]
        public void IsValid_WhenProductDoesntSupportFixedRateRebate_ShouldReturnFalse()
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
        public void IsValid_WhenRebatePercentageIsZero_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Percentage = 0 };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedRateRebate };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WhenProductPriceIsZero_ShouldReturnFalse()
        {
            // Arrange
            var rebate = new Types.Rebate { Percentage = 0.1m };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedRateRebate, Price = 0 };
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
            var rebate = new Types.Rebate { Percentage = 0.1m };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedRateRebate, Price = 100 };
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
            var rebate = new Types.Rebate { Percentage = 0.1m };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedRateRebate, Price = 100 };
            var request = new Types.CalculateRebateRequest { Volume = 5 };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 100, 5, true)]
        [InlineData(0, 100, 5, false)]
        [InlineData(0.1, 0, 5, false)]
        [InlineData(0.1, 100, 0, false)]
        public void IsValid_WhenCalledWithDifferentParameters_ShouldReturnExpectedResult(decimal percentage, decimal price, decimal volume, bool calculatedRebate)
        {
            // Arrange
            var rebate = new Types.Rebate { Percentage = percentage };
            var product = new Types.Product { SupportedIncentives = Types.SupportedIncentiveType.FixedRateRebate, Price = price };
            var request = new Types.CalculateRebateRequest { Volume = volume };

            // Act
            var result = _calculator.IsValid(rebate, product, request);

            // Assert
            result.Should().Be(calculatedRebate);
        }
    }
}
