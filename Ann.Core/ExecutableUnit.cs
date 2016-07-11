using System.Data.Linq.Mapping;

namespace Ann.Core
{
    [Table(Name = "ExecutableUnit")]
    public class ExecutableUnit
    {
        [Column(Name = "Id", DbType = "INT", CanBeNull = false, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(Name = "Name", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Name { get; set; }

        [Column(Name = "Path", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Path { get; set; }
    }
}