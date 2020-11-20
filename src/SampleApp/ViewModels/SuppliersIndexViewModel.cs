using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.ViewModels {

    public class SuppliersIndexViewModel {

        public string Title { get; set; }

        public PagingList<Suppliers> Suppliers { get; set; }
    }
}
