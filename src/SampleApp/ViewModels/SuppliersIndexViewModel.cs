using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;

namespace SampleApp.ViewModels; 

public class SuppliersIndexViewModel {

    public string Title { get; set; }

    public PagingList<Suppliers> Suppliers { get; set; }
}
