using System.Data.Linq.Mapping;

namespace Ann.Core
{
    [Table(Name = "ExecutableUnit")]
    public class ExecutableUnit
    {
        [Column(Name = "Path", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public string Path { get; set; }

        [Column(Name = "Name", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Name { get; set; }
    }
}