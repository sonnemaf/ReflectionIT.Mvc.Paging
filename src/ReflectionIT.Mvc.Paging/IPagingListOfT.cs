using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace ReflectionIT.Mvc.Paging {

    public interface IPagingList<T> : IPagingList, IEnumerable<T> {

    }
}
