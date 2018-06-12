using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.ThirdPart.Utils
{
    /// <summary>
    /// 调广发非托管dll解密接口，详见接口文件
    /// </summary>
    public class LibcryptAPIsmJni
    {
#if DEBUG
         private const string dllPath = @".\DLL\dev\libcryptAPIsm_64.dll";
#else
         private const string dllPath = @".\DLL\pro\libcryptAPIsm_64.dll";
#endif
        /// <summary>
        /// 密钥生成
        /// </summary>
        /// <param name="var1">0，密钥转换，密钥须为16的倍数；2，产生密钥密文，此时inkey、inlen无定义，outlen只能为16的倍数</param>
        /// <param name="var2">输入密钥，flag值为2时，该参数无定义</param>
        /// <param name="var3">密钥长度，只能为16的倍数，flag值为2时，该参数无定义</param>
        /// <param name="var4">返回转换后的密钥</param>
        /// <param name="var5">返回密钥长度</param>
        /// <returns>0表示成功，其它表示失败</returns>
        [DllImport(dllPath)]
        public static extern int CryptKey(int var1, byte[] var2, int var3, byte[] var4, int[] var5);
        /// <summary>
        /// 文件加解密接口
        /// </summary>
        /// <param name="var1">2为加密文件；3为解密文件</param>
        /// <param name="var2">加密密钥</param>
        /// <param name="var3">key长度</param>
        /// <param name="var4">源文件名</param>
        /// <param name="var6">目标文件名</param>
        /// <param name="var8">文件填充格式，1通用格式；其它值没定义</param>
        /// <returns>0表示成功，其它表示失败</returns>
        [DllImport(dllPath)]
        public static extern int CryptFile(int var1, byte[] var2, int var3, byte[] var4, byte[] var6, int var8);
    }
}
