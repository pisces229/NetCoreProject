using System.Collections.Generic;

namespace NetCoreProject.Domain.Entity
{
    public partial class Grade
    {
        public string GradeId { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
