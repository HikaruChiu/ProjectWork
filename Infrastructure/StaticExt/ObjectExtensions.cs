using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{
    /// <summary>
    /// 對像擴充套件方法
    /// </summary>
    public static class ObjectExtensions
    {
        public static string DecodeBase64(this string str)
        {
            try
            {
                byte[] data = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 把Dictionary 加在string 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addition"></param>
        /// <returns></returns>
        public static string AppendDic(this string value, Dictionary<string, string> addition)
        {
            if (addition == null) return value;
            value += Environment.NewLine + "            [Tags]" + Environment.NewLine;
            foreach (KeyValuePair<string, string> item in addition)
            {
                value += "                 {Key:" + item.Key + ",Value:" + item.Value + "}" + Environment.NewLine;
            }
            return value;
        }



        /// <summary>
        /// 將流讀取成位元組組。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>位元組組。</returns>
        public static byte[] ReadBytes(this Stream stream)
        {
            if (!stream.CanRead)
                throw new NotSupportedException(stream + "不支援讀取。");

            Action trySeekBegin = () =>
            {
                if (!stream.CanSeek)
                    return;

                stream.Seek(0, SeekOrigin.Begin);
            };

            trySeekBegin();

            var list = new List<byte>(stream.CanSeek ? (stream.Length > int.MaxValue ? int.MaxValue : (int)stream.Length) : 300);

            int b;

            while ((b = stream.ReadByte()) != -1)
                list.Add((byte)b);

            trySeekBegin();

            return list.ToArray();
        }




    }
}
