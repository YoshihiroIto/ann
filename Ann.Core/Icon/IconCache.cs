using System.Data.Linq.Mapping;

namespace Ann.Core.Icon
{
    [Table(Name = "IconCache")]
    public class IconCache
    {
        [Column(Name = "Path", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public string Path { get; set; }

        [Column(Name = "Image", DbType = "BLOB", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public byte[] Image { get; set; }

        [Column(Name = "TimeStamp", DbType = "NVARCHAR", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string TimeStamp { get; set; }
    }
}