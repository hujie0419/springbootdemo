using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI;
using System.IO;
using Tuhu.CarArchiveService.Models;
using NPOI.XSSF.UserModel;

namespace Tuhu.CarArchiveService.Utils
{
    public class ExcelHelper
    {
        private string _path;

        public ExcelHelper(string path)
        {
            _path = path;
        }
        public IEnumerable<ShopIdentityModel> GetExcelData()
        {
            if (!File.Exists(_path))
            {
                throw new DirectoryNotFoundException($"{_path}目录不存在！");
            }
            List<ShopIdentityModel> result = new List<ShopIdentityModel>();
            var fileByte = File.ReadAllBytes(_path);
            XSSFWorkbook wk = new XSSFWorkbook(new MemoryStream(fileByte));
            var sheet = wk.GetSheetAt(0);
            var rows = sheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                XSSFRow temp = (XSSFRow)rows.Current;
                result.Add(new ShopIdentityModel
                {
                    ShopAccount = temp.GetCell(2).StringCellValue.Trim(),
                    PassWord = temp.GetCell(3).StringCellValue.Trim(),
                });
            }
            return result;
        }


    }
}
