using System.Data.Linq.Mapping;

namespace Ann.Core
{
    [Table(Name = "ExecutableUnit")]
    public class ExecutableUnit
    {
        [Column(Name = "Path", DbType = "TEXT", CanBeNull = false, UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public string Path { get; set; }

        [Column(Name = "Name", DbType = "TEXT", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Name { get; set; }

        [Column(Name = "Directory", DbType = "TEXT", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Directory { get; set; }

        [Column(Name = "FileName", DbType = "TEXT", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string FileName { get; set; }
    }
}