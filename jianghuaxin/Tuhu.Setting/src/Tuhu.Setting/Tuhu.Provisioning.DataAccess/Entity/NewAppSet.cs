namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class NewAppSet
    {
        public long Id { get; set; }
        public short? Apptype { get; set; }
        public int? Version { get; set; }
        public string Modelname { get; set; }
        public short? Modelfloor { get; set; }
        public int? Showorder { get; set; }
        public string Icoimgurl { get; set; }
        public string Jumph5url { get; set; }
        public short? Showstatic { get; set; }
        public System.DateTime? Starttime { get; set; }
        public System.DateTime? Overtime { get; set; }
        public short? Cpshowtype { get; set; }
        public string Cpshowbanner { get; set; }
        public string Appoperateval { get; set; }
        public string Operatetypeval { get; set; }
        public string Pronumberval { get; set; }
        public string Keyvaluelenth { get; set; }
        public string Umengtongji { get; set; }
        public System.DateTime? Createtime { get; set; }
        public System.DateTime? Updatetime { get; set; }
        public int? ModelType { get; set; }
        public System.Guid? ActivityID { get; set; }
    }
}
