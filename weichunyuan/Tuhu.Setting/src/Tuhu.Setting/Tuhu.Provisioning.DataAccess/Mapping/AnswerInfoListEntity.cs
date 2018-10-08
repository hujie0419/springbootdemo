using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tuhu.Provisioning.DataAccess.Mapping
{
    public class AnswerInfoListEntity:EntityTypeConfiguration<AnswerInfoListEntity>
    {
        public AnswerInfoListEntity()
        {
            this.ToTable("dbo.AnswerInfoList");
            this.HasKey(_ => _.PKID);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime LastUpdateDateTime { get; set; }

        public string Tip { get; set; }

        public string Answer { get; set; }

        public string OptionsA { get; set; }

        public string OptionsB { get; set; }

        public string OptionsC { get; set; }

        public string OptionsD { get; set; }

        public string OptionsReal { get; set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get {
                return _isEnabled;
            }
            set {
                _isEnabled = value;
            }
        }
    }
}
