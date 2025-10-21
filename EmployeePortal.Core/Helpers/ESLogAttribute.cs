using System;

namespace EmployeePortal.Core.Helpers
{
    public class ESLogAttribute: Attribute
    {
        public string DisplayKey { get; set; }
        public bool IsIncluded { get; set; }
        public bool ShowValues { get; set; }

        public ESLogAttribute(string displayKey, bool isIncluded = true, bool showValues = true)
        {
            DisplayKey = displayKey;
            IsIncluded = isIncluded;
            ShowValues = showValues;
        }
    }
}
