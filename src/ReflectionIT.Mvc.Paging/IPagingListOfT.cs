namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// Represents a strongly typed paged list that combines paging metadata with enumeration
/// of the items contained in the current page.
/// </summary>
/// <typeparam name="T">The type of the items contained in the paged list.</typeparam>
/// <remarks>
/// This interface extends <see cref="IPagingList"/> to provide paging information and
/// <see cref="System.Collections.Generic.IEnumerable{T}"/> to enumerate the items on the current page.
/// </remarks>
/// <seealso cref="IPagingList"/>
public interface IPagingList<T> : IPagingList, IEnumerable<T> {

}
