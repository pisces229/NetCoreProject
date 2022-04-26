
namespace NetCoreProject.Domain.Entity
{
    public partial class Student
    {
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        public string Name { get; set; }
        public Grade Grade { get; set; }
        public StudentAddress Address { get; set; }
    }
}
