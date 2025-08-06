using RacingCalendar.ViewModels;
using System.Collections.Generic;
using Xunit;

public class PaginatedListTests
{
    [Fact]
    public void Constructor_CalculatesTotalPages_Correctly()
    {
        var items = new List<int> { 1, 2, 3 };
        var paginated = new PaginatedList<int>(items, count: 10, pageIndex: 1, pageSize: 3);

        Assert.Equal(4, paginated.TotalPages);
        Assert.Equal(1, paginated.PageIndex);
        Assert.Equal(items, paginated.Items);
    }

    [Theory]
    [InlineData(1, 1, false, false)]
    [InlineData(2, 4, true, true)]
    [InlineData(4, 4, true, false)]
    public void HasPreviousAndNextPage_WorkAsExpected(int pageIndex, int totalPages, bool hasPrev, bool hasNext)
    {
        var paginated = new PaginatedList<int>(new List<int>(), count: totalPages, pageIndex: pageIndex, pageSize: 1)
        {
        };

        Assert.Equal(hasPrev, paginated.HasPreviousPage);
        Assert.Equal(hasNext, paginated.HasNextPage);
    }

    [Fact]
    public void Items_ReturnsExpectedCollection()
    {
        var items = new List<string> { "a", "b" };
        var paginated = new PaginatedList<string>(items, count: 2, pageIndex: 1, pageSize: 2);

        Assert.Equal(items, paginated.Items);
    }
}