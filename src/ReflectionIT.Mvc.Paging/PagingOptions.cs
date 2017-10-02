using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionIT.Mvc.Paging {
    public class PagingOptions {

        private static PagingOptions _current = new PagingOptions();

        public static PagingOptions Current {
            get {
                return _current;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(Current), "PagingOptions must be set");
                }
                _current = value;
            }
        }

        public string ViewName { get; set; } = "Bootstrap3";
        public string HtmlIndicatorUp { get; set; } = " <span class=\"glyphicon glyphicon glyphicon-chevron-up\" aria-hidden=\"true\"></span>";
        public string HtmlIndicatorDown { get; set; } = " <span class=\"glyphicon glyphicon glyphicon-chevron-down\" aria-hidden=\"true\"></span>";
        public int DefaultNumberOfPagesToShow { get; set; } = 5;

    }
}
