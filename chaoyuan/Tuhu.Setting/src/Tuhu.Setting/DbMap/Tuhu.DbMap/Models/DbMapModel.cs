using System.Collections.Generic;

namespace Tuhu.DbMap.Models
{

	public class TableMapModel
	{
		public string DbName { get; set; }
		public string TableName { get; set; }
		public int TableId { get; set; }
		public string FieldName { get; set; }
		public string FieldId { get; set; }
		public string FieldType { get; set; }
		public int ByteCount { get; set; }
		public int FieldSize { get; set; }
		public int DecimalSize { get; set; }
		public bool IsNullable { get; set; }
		public bool IsExist { get; set; }
		public string DefaultValue { get; set; }
		public string Description { get; set; }
		public bool IsPk { get; set; }
		public bool IsIdentity { get; set; }
		public bool IsIndex { get; set; }
	}

    public class TreeViewModel
    {
        public string text { get; set; }
        public string id { get; set; }
        public List<TreeViewModel> nodes { get; set; }
    }
    public class DbMapModel
    {
        public string DbName { get; set; }
        public string TableName { get; set; }
        public int TableId { get; set; }
    }

    public class SimpleDbMapModel
    {
        public string FieldName { get; set; }
        public string FieldType { get; set;}
        public int ByteCount { get; set; }
        public int FieldSize { get; set; }
        public bool IsNullable { get; set; }
        public string Description { get; set; }
    }
}