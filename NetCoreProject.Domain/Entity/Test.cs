using System;

namespace NetCoreProject.Domain.Entity
{
    public partial class Test
    {
        public int ROW { get; set; }
        public string NAME { get; set; }
        public DateTime MAKE_DATE { get; set; }
        public int? SALE_AMT { get; set; }
        public DateTime? SALE_DATE { get; set; }
        public decimal? TAX { get; set; }
        public string REMARK { get; set; }
        public string UPDATE_USER_ID { get; set; }
        public string UPDATE_PROG_CD { get; set; }
        public DateTime? UPDATE_DATE_TIME { get; set; }
    }
}
