using NetCoreProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreProject.BusinessLayer.Model.Test
{
    public class TestLogicQueryGridInputModel
    {
        public string NAME { get; set; }
        public DateTime MAKE_DATE { get; set; }
        public int? SALE_AMT { get; set; }
        public DateTime? SALE_DATE { get; set; }
    }
}
