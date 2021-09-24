/// <summary>
/// GridColumn
/// </summary>
public class GridColumn
{
    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    /// <value>
    /// The data.
    /// </value>
    public string Data { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="GridColumn"/> is orderable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if orderable; otherwise, <c>false</c>.
    /// </value>
    public bool Orderable { get; set; }

    /// <summary>
    /// Gets or sets the search.
    /// </summary>
    /// <value>
    /// The search.
    /// </value>
    public GridSearch Search { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="GridColumn"/> is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if visible; otherwise, <c>false</c>.
    /// </value>
    public bool Visible { get; set; }
}