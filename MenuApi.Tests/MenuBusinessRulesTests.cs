using FluentAssertions;
using MenuApi.Services;

namespace MenuApi.Tests;

public class MenuBusinessRulesTests
{
    [Fact]
    public void EnsurePositivePrice_rejects_zero_and_negative()
    {
        var actZero = () => MenuBusinessRules.EnsurePositivePrice(0);
        var actNeg = () => MenuBusinessRules.EnsurePositivePrice(-1);
        actZero.Should().Throw<ArgumentOutOfRangeException>();
        actNeg.Should().Throw<ArgumentOutOfRangeException>();
        MenuBusinessRules.EnsurePositivePrice(0.01m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void EnsureDiscountPercent_rejects_out_of_range(int value)
    {
        var act = () => MenuBusinessRules.EnsureDiscountPercent(value);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(50)]
    public void EnsureDiscountPercent_accepts_valid_range(int value)
    {
        MenuBusinessRules.EnsureDiscountPercent(value).Should().Be(value);
    }

    [Theory]
    [InlineData(100, 10, 90)]
    [InlineData(50, 50, 25)]
    [InlineData(10, 1, 9.9)]
    public void ComputeSpecialPrice_matches_formula(decimal price, int discount, decimal expected)
    {
        var actual = MenuBusinessRules.ComputeSpecialPrice(price, discount);
        actual.Should().Be(expected);
    }
}
