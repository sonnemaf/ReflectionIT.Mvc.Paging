using Microsoft.EntityFrameworkCore;

namespace ReflectionIT.Mvc.Paging.Test;
public class PagingListTests {

    public class Item {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options) {
        public DbSet<Item> Items => Set<Item>();
    }

    private static List<Item> MakeItems(int count) =>
        Enumerable.Range(1, count).Select(i => new Item { Id = i, Name = $"Name{i:00}" }).ToList();

    private static TestDbContext CreateContext(int count) {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new TestDbContext(options);
        ctx.Items.AddRange(MakeItems(count));
        ctx.SaveChanges();
        return ctx;
    }

    private static string Ids(IEnumerable<Item> items) => string.Join(",", items.Select(i => i.Id));

    // ---- Create(IEnumerable<T>, pageSize, pageIndex) ----

    [Test]
    public async Task Create_ReturnsRequestedPageAndMetadata() {
        var result = PagingList.Create(MakeItems(10), 3, 2);

        await Assert.That(Ids(result)).IsEqualTo("4,5,6");
        await Assert.That(result.PageIndex).IsEqualTo(2);
        await Assert.That(result.PageCount).IsEqualTo(4);
        await Assert.That(result.TotalRecordCount).IsEqualTo(10);
        await Assert.That(result.Action).IsEqualTo("Index");
        await Assert.That(result.SortExpression).IsEqualTo(string.Empty);
        await Assert.That(result.DefaultSortExpression).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Create_LastPartialPageContainsRemainingItems() {
        var result = PagingList.Create(MakeItems(10), 3, 4);

        await Assert.That(Ids(result)).IsEqualTo("10");
        await Assert.That(result.PageIndex).IsEqualTo(4);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-5)]
    public async Task Create_PageIndexBelowOne_ClampsToFirstPage(int pageIndex) {
        var result = PagingList.Create(MakeItems(10), 3, pageIndex);

        await Assert.That(result.PageIndex).IsEqualTo(1);
        await Assert.That(Ids(result)).IsEqualTo("1,2,3");
    }

    [Test]
    public async Task Create_PageIndexAboveCount_ClampsToLastPage() {
        var result = PagingList.Create(MakeItems(10), 3, 99);

        await Assert.That(result.PageIndex).IsEqualTo(4);
        await Assert.That(Ids(result)).IsEqualTo("10");
    }

    [Test]
    public async Task Create_EmptySource_ReturnsEmptyListOnPageOne() {
        var result = PagingList.Create(new List<Item>(), 5, 3);

        await Assert.That(result.Count).IsEqualTo(0);
        await Assert.That(result.PageIndex).IsEqualTo(1);
        await Assert.That(result.PageCount).IsEqualTo(0);
        await Assert.That(result.TotalRecordCount).IsEqualTo(0);
    }

    [Test]
    public async Task Create_ExactMultipleOfPageSize_HasNoExtraPage() {
        var result = PagingList.Create(MakeItems(9), 3, 3);

        await Assert.That(result.PageCount).IsEqualTo(3);
        await Assert.That(Ids(result)).IsEqualTo("7,8,9");
    }

    [Test]
    public async Task Create_PageSizeLargerThanSource_ReturnsAllItemsOnSinglePage() {
        var result = PagingList.Create(MakeItems(4), 100, 1);

        await Assert.That(result.PageCount).IsEqualTo(1);
        await Assert.That(Ids(result)).IsEqualTo("1,2,3,4");
    }

    [Test]
    public async Task Create_NullSource_Throws() {
        await Assert.That(() => PagingList.Create<Item>(null!, 3, 1)).Throws<ArgumentNullException>();
    }

    // ---- Create(IEnumerable<T>, pageSize, pageIndex, sortExpression, defaultSortExpression) ----

    [Test]
    public async Task Create_WithSort_Ascending_OrdersBeforePaging() {
        var items = MakeItems(6);
        items.Reverse();

        var result = PagingList.Create(items, 2, 2, "Id", "Id");

        await Assert.That(Ids(result)).IsEqualTo("3,4");
        await Assert.That(result.SortExpression).IsEqualTo("Id");
        await Assert.That(result.DefaultSortExpression).IsEqualTo("Id");
        await Assert.That(result.TotalRecordCount).IsEqualTo(6);
        await Assert.That(result.PageCount).IsEqualTo(3);
    }

    [Test]
    public async Task Create_WithSort_Descending_OrdersBeforePaging() {
        var result = PagingList.Create(MakeItems(6), 2, 1, "-Id", "Id");

        await Assert.That(Ids(result)).IsEqualTo("6,5");
        await Assert.That(result.SortExpression).IsEqualTo("-Id");
        await Assert.That(result.DefaultSortExpression).IsEqualTo("Id");
    }

    [Test]
    public async Task Create_WithSort_PageIndexOutOfRange_Clamps() {
        var result = PagingList.Create(MakeItems(5), 2, 50, "Id", "Id");

        await Assert.That(result.PageIndex).IsEqualTo(3);
        await Assert.That(Ids(result)).IsEqualTo("5");
    }

    [Test]
    public async Task Create_WithSort_UnknownProperty_Throws() {
        await Assert.That(() => PagingList.Create(MakeItems(3), 2, 1, "DoesNotExist", "Id")).Throws<ArgumentException>();
    }

    // ---- Create(IEnumerable<T>, pageSize, pageIndex, totalRecordCount, sortExpression, defaultSortExpression) ----

    [Test]
    public async Task Create_WithTotalCount_UsesGivenTotalAndKeepsAllItems() {
        var pageItems = MakeItems(3);

        var result = PagingList.Create(pageItems, 3, 2, 10, "Name", "Id");

        await Assert.That(Ids(result)).IsEqualTo("1,2,3");
        await Assert.That(result.PageIndex).IsEqualTo(2);
        await Assert.That(result.PageCount).IsEqualTo(4);
        await Assert.That(result.TotalRecordCount).IsEqualTo(10);
        await Assert.That(result.SortExpression).IsEqualTo("Name");
        await Assert.That(result.DefaultSortExpression).IsEqualTo("Id");
    }

    [Test]
    public async Task Create_WithTotalCount_ClampsPageIndexToValidRange() {
        var low = PagingList.Create(MakeItems(3), 3, -1, 10, "Id", "Id");
        var high = PagingList.Create(MakeItems(3), 3, 100, 10, "Id", "Id");

        await Assert.That(low.PageIndex).IsEqualTo(1);
        await Assert.That(high.PageIndex).IsEqualTo(4);
    }

    [Test]
    public async Task Create_WithTotalCountZero_ReturnsPageOneWithZeroPages() {
        var result = PagingList.Create(new List<Item>(), 10, 5, 0, "Id", "Id");

        await Assert.That(result.PageIndex).IsEqualTo(1);
        await Assert.That(result.PageCount).IsEqualTo(0);
        await Assert.That(result.TotalRecordCount).IsEqualTo(0);
    }

    // ---- CreateAsync(IOrderedQueryable<T>, pageSize, pageIndex) ----

    [Test]
    public async Task CreateAsync_Ordered_ReturnsRequestedPageAndMetadata() {
        using var ctx = CreateContext(10);

        var result = await PagingList.CreateAsync(ctx.Items.OrderBy(i => i.Id), 4, 2);

        await Assert.That(Ids(result)).IsEqualTo("5,6,7,8");
        await Assert.That(result.PageIndex).IsEqualTo(2);
        await Assert.That(result.PageCount).IsEqualTo(3);
        await Assert.That(result.TotalRecordCount).IsEqualTo(10);
    }

    [Test]
    public async Task CreateAsync_Ordered_PageIndexOutOfRange_Clamps() {
        using var ctx = CreateContext(10);

        var low = await PagingList.CreateAsync(ctx.Items.OrderBy(i => i.Id), 4, 0);
        var high = await PagingList.CreateAsync(ctx.Items.OrderBy(i => i.Id), 4, 99);

        await Assert.That(low.PageIndex).IsEqualTo(1);
        await Assert.That(Ids(low)).IsEqualTo("1,2,3,4");
        await Assert.That(high.PageIndex).IsEqualTo(3);
        await Assert.That(Ids(high)).IsEqualTo("9,10");
    }

    [Test]
    public async Task CreateAsync_Ordered_EmptySource_ReturnsEmptyPageOne() {
        using var ctx = CreateContext(0);

        var result = await PagingList.CreateAsync(ctx.Items.OrderBy(i => i.Id), 4, 2);

        await Assert.That(result.Count).IsEqualTo(0);
        await Assert.That(result.PageIndex).IsEqualTo(1);
        await Assert.That(result.PageCount).IsEqualTo(0);
        await Assert.That(result.TotalRecordCount).IsEqualTo(0);
    }

    // ---- CreateAsync(IQueryable<T>, pageSize, pageIndex, sortExpression, defaultSortExpression) ----

    [Test]
    public async Task CreateAsync_WithSort_Ascending_OrdersBeforePaging() {
        using var ctx = CreateContext(7);

        var result = await PagingList.CreateAsync(ctx.Items, 3, 2, "Id", "Id");

        await Assert.That(Ids(result)).IsEqualTo("4,5,6");
        await Assert.That(result.PageIndex).IsEqualTo(2);
        await Assert.That(result.PageCount).IsEqualTo(3);
        await Assert.That(result.TotalRecordCount).IsEqualTo(7);
        await Assert.That(result.SortExpression).IsEqualTo("Id");
        await Assert.That(result.DefaultSortExpression).IsEqualTo("Id");
    }

    [Test]
    public async Task CreateAsync_WithSort_Descending_OrdersBeforePaging() {
        using var ctx = CreateContext(7);

        var result = await PagingList.CreateAsync(ctx.Items, 3, 1, "-Id", "Id");

        await Assert.That(Ids(result)).IsEqualTo("7,6,5");
        await Assert.That(result.SortExpression).IsEqualTo("-Id");
    }

    [Test]
    public async Task CreateAsync_WithSort_PageIndexOutOfRange_Clamps() {
        using var ctx = CreateContext(7);

        var result = await PagingList.CreateAsync(ctx.Items, 3, 42, "Id", "Id");

        await Assert.That(result.PageIndex).IsEqualTo(3);
        await Assert.That(Ids(result)).IsEqualTo("7");
    }

    [Test]
    public async Task CreateAsync_WithSort_UnknownProperty_Throws() {
        using var ctx = CreateContext(3);

        await Assert.That(async () => await PagingList.CreateAsync(ctx.Items, 2, 1, "DoesNotExist", "Id"))
            .Throws<ArgumentException>();
    }
}
