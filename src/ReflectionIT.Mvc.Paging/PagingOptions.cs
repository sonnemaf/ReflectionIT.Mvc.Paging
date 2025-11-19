namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// Provides global/default configuration for the ReflectionIT Razor Pages paging helpers,
/// including view selection and query string parameter names.
/// </summary>
/// <remarks>
/// Assign <see cref="Current"/> at application startup to change the defaults globally,
/// or instantiate a new <see cref="PagingOptions"/> and pass it where supported.
/// </remarks>
/// <example>
/// Example: set global defaults at startup:
/// <code>
/// PagingOptions.Current = new PagingOptions {
///     ViewName = "Bootstrap5",
///     PageParameterName = "pageindex",
///     SortExpressionParameterName = "sort",
///     DefaultNumberOfPagesToShow = 7
/// };
/// </code>
/// or use
/// 
/// <code>
/// Builder.Services.AddPaging(options => {
///     options.ViewName = "Bootstrap5";
///     options.PageParameterName = "pageindex";
///     options.SortExpressionParameterName = "sort";
///     options.DefaultNumberOfPagesToShow = 7;
///     options.HtmlIndicatorDown = " <span>&darr;</span>";
///     options.HtmlIndicatorUp = " <span>&uarr;</span>";
///});
/// </code>
/// </example>
public class PagingOptions {

    private static PagingOptions _current = new PagingOptions();

    /// <summary>
    /// Gets or sets the global <see cref="PagingOptions"/> instance used by the paging helpers.
    /// </summary>
    /// <remarks>
    /// Setting this property to <see langword="null"/> will throw an <see cref="ArgumentNullException"/>.
    /// Configure this once during application startup to apply consistent behavior across the app.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when attempting to set <see cref="Current"/> to <see langword="null"/>.</exception>
    public static PagingOptions Current {
        get => _current;
        set => _current = value ?? throw new ArgumentNullException(nameof(Current), "PagingOptions must be set");
    }

    /// <summary>
    /// Gets or sets the name of the pager view/partial to render.
    /// </summary>
    /// <value>Defaults to <c>"Bootstrap5"</c>.</value>
    /// <remarks>
    /// Use this to select a specific rendering template (e.g., Bootstrap variants).
    /// </remarks>
    public string ViewName { get; set; } = "Bootstrap5";

    /// <summary>
    /// Gets or sets the HTML fragment used to indicate an ascending sort in a column header.
    /// </summary>
    /// <value>
    /// Defaults to a Bootstrap glyphicon-based span for an up chevron.
    /// </value>
    /// <remarks>
    /// This value is rendered as HTML. Ensure it is trusted content.
    /// </remarks>
    public string HtmlIndicatorUp { get; set; } = " <span class=\"glyphicon glyphicon glyphicon-chevron-up\" aria-hidden=\"true\"></span>";

    /// <summary>
    /// Gets or sets the HTML fragment used to indicate a descending sort in a column header.
    /// </summary>
    /// <value>
    /// Defaults to a Bootstrap glyphicon-based span for a down chevron.
    /// </value>
    /// <remarks>
    /// This value is rendered as HTML. Ensure it is trusted content.
    /// </remarks>
    public string HtmlIndicatorDown { get; set; } = " <span class=\"glyphicon glyphicon glyphicon-chevron-down\" aria-hidden=\"true\"></span>";

    /// <summary>
    /// Gets or sets the default number of page number links to display in the pager.
    /// </summary>
    /// <value>Defaults to <c>5</c>.</value>
    /// <remarks>
    /// Typical values are between 3 and 10. Must be a positive integer.
    /// </remarks>
    public int DefaultNumberOfPagesToShow { get; set; } = 5;

    /// <summary>
    /// Gets or sets the query string parameter name that carries the current page index.
    /// </summary>
    /// <value>Defaults to <c>"pageIndex"</c>.</value>
    /// <remarks>
    /// Use this to align with your route/query conventions (e.g., <c>"page"</c>).
    /// </remarks>
    public string PageParameterName { get; set; } = "pageIndex";

    /// <summary>
    /// Gets or sets the query string parameter name that carries the current sort expression.
    /// </summary>
    /// <value>Defaults to <c>"sortExpression"</c>.</value>
    /// <remarks>
    /// The sort expression is typically of the form <c>"Field"</c> or <c>"Field desc"</c>.
    /// </remarks>
    public string SortExpressionParameterName { get; set; } = "sortExpression";

}
