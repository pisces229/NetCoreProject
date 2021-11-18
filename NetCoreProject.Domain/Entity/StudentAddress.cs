
namespace NetCoreProject.Domain.Entity
{
    public partial class StudentAddress
    {
        public string StudentId { get; set; }
        public string Address { get; set; }
        public Student Student { get; set; }
    }
}
