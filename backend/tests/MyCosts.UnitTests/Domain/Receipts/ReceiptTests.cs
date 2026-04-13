using MyCosts.Domain.Receipts;
using MyCosts.UnitTests.Builders;
using Xunit;

namespace MyCosts.UnitTests.Domain.Receipts;

public sealed class ReceiptTests
{
    [Fact]
    public void AddLine_SetsReceiptId()
    {
        var receipt = new ReceiptBuilder().Build();

        var line = receipt.AddLine(Guid.CreateVersion7(), LinePricing.ByQuantity(1, 100));

        Assert.Equal(receipt.Id, line.ReceiptId);
    }

    [Fact]
    public void AddLine_LineAppearsInLines()
    {
        var receipt = new ReceiptBuilder().Build();

        var line = receipt.AddLine(Guid.CreateVersion7(), LinePricing.ByQuantity(1, 100));

        Assert.Contains(line, receipt.Lines);
    }

    [Fact]
    public void AddLine_MultipleTimes_AllLinesAppearInLines()
    {
        var receipt = new ReceiptBuilder().Build();

        var line1 = receipt.AddLine(Guid.CreateVersion7(), LinePricing.ByQuantity(1, 50));
        var line2 = receipt.AddLine(Guid.CreateVersion7(), LinePricing.ByQuantity(2, 30));

        Assert.Equal(2, receipt.Lines.Count);
        Assert.Contains(line1, receipt.Lines);
        Assert.Contains(line2, receipt.Lines);
    }
}
