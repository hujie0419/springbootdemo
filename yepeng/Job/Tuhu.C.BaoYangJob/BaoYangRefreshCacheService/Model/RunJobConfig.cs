using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Models;

namespace BaoYangRefreshCacheService.Model
{
    public class RunJobConfig
    {
        [Column("PKID")]
        public int Id { get; set; }

        public string JobName { get; set; }
    }
}
