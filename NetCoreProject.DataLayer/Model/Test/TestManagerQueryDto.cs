using System;

namespace NetCoreProject.DataLayer.Model.Test
{
    public class TestManagerQueryDto
    {
        public int ROW { get; set; }
        public string NAME { get; set; }
        public DateTime MAKE_DATE { get; set; }
        public int? SALE_AMT { get; set; }
        public DateTime? SALE_DATE { get; set; }
        public decimal? TAX { get; set; }
        public string REMARK { get; set; }
    }
}
