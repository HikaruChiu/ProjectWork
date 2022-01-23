using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class JsHelper
    {
        private enum URISetType
        {
            None,
            Reserved,
            Unescaped
        }

        /// <summary>
        /// 同 Javascript 的 escape() 編碼
        /// </summary>
        /// <param name="string">原始字串</param>
        /// <returns></returns>
        public static string escape(object @string)
        {
            string str = Convert.ToString(@string);
            int length = str.Length;
            var sb = new StringBuilder(length * 2);
            string str2 = "0123456789ABCDEF";
            int num3 = -1;
            while (++num3 < length)
            {
                char ch = str[num3];
                int num2 = ch;
                if ((((0x41 > num2) || (num2 > 90)) && ((0x61 > num2) || (num2 > 0x7a))) && ((0x30 > num2) || (num2 > 0x39)))
                {
                    switch (ch)
                    {
                        case '@':
                        case '*':
                        case '_':
                        case '+':
                        case '-':
                        case '.':
                        case '/':
                            goto Label_0125;
                    }
                    sb.Append('%');
                    if (num2 < 0x100)
                    {
                        sb.Append(str2[num2 / 0x10]);
                        ch = str2[num2 % 0x10];
                    }
                    else
                    {
                        sb.Append('u');
                        sb.Append(str2[(num2 >> 12) % 0x10]);
                        sb.Append(str2[(num2 >> 8) % 0x10]);
                        sb.Append(str2[(num2 >> 4) % 0x10]);
                        ch = str2[num2 % 0x10];
                    }
                }
            Label_0125:
                sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 同 Javascript 的 unescape() 解碼
        /// </summary>
        /// <param name="string">原始字串</param>
        /// <returns></returns>
        public static string unescape(object @string)
        {
            string str = Convert.ToString(@string);
            int length = str.Length;
            StringBuilder builder = new StringBuilder(length);
            int num6 = -1;
            while (++num6 < length)
            {
                char ch = str[num6];
                if (ch == '%')
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    if (((((num6 + 5) < length) && (str[num6 + 1] == 'u')) && (((num2 = HexDigit(str[num6 + 2])) != -1) && ((num3 = HexDigit(str[num6 + 3])) != -1))) && (((num4 = HexDigit(str[num6 + 4])) != -1) && ((num5 = HexDigit(str[num6 + 5])) != -1)))
                    {
                        ch = (char)((((num2 << 12) + (num3 << 8)) + (num4 << 4)) + num5);
                        num6 += 5;
                    }
                    else if ((((num6 + 2) < length) && ((num2 = HexDigit(str[num6 + 1])) != -1)) && ((num3 = HexDigit(str[num6 + 2])) != -1))
                    {
                        ch = (char)((num2 << 4) + num3);
                        num6 += 2;
                    }
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private static bool InURISet(char ch, URISetType flags)
        {
            if ((flags & URISetType.Unescaped) == URISetType.None)
            {
                goto Label_0066;
            }
            if ((((ch >= '0') && (ch <= '9')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= 'a') && (ch <= 'z')))
            {
                return true;
            }
            if (ch <= '.')
            {
                switch (ch)
                {
                    case '\'':
                    case '(':
                    case ')':
                    case '*':
                    case '-':
                    case '.':
                    case '!':
                        return true;
                }
                goto Label_0066;
            }
            if ((ch != '_') && (ch != '~'))
            {
                goto Label_0066;
            }
            return true;

        Label_0066:
            if ((flags & URISetType.Reserved) != URISetType.None)
            {
                switch (ch)
                {
                    case '#':
                    case '$':
                    case '&':
                    case '+':
                    case ',':
                    case '/':
                    case ':':
                    case ';':
                    case '=':
                    case '?':
                    case '@':
                        return true;
                }
            }
            return false;
        }

        private static void AppendInHex(StringBuilder bs, int value)
        {
            bs.Append('%');
            int num = (value >> 4) & 15;
            bs.Append((num >= 10) ? ((char)((num - 10) + 0x41)) : ((char)(num + 0x30)));
            num = value & 15;
            bs.Append((num >= 10) ? ((char)((num - 10) + 0x41)) : ((char)(num + 0x30)));
        }


        private static byte HexValue(char ch1, char ch2)
        {
            int num = 0;
            int num1 = HexDigit(ch1);
            if ((num1 < 0) || ((num = HexDigit(ch2)) < 0))
            {
                throw new Exception();
            }
            return (byte)((num1 << 4) | num);
        }

        internal static int HexDigit(char c)
        {
            if ((c >= '0') && (c <= '9'))
            {
                return (c - '0');
            }
            if ((c >= 'A') && (c <= 'F'))
            {
                return (('\n' + c) - 0x41);
            }
            if ((c >= 'a') && (c <= 'f'))
            {
                return (('\n' + c) - 0x61);
            }
            return -1;
        }

        private static string Decode(object encodedURI, URISetType flags)
        {
            string str = Convert.ToString(encodedURI);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch != '%')
                {
                    builder.Append(ch);
                }
                else
                {
                    char ch2;
                    int startIndex = i;
                    if ((i + 2) >= str.Length)
                    {
                        throw new Exception();
                    }
                    byte num3 = HexValue(str[i + 1], str[i + 2]);
                    i += 2;
                    if ((num3 & 0x80) == 0)
                    {
                        ch2 = (char)num3;
                    }
                    else
                    {
                        int num4 = 1;
                        while (((num3 << num4) & 0x80) != 0)
                        {
                            num4++;
                        }
                        if (((num4 == 1) || (num4 > 4)) || ((i + ((num4 - 1) * 3)) >= str.Length))
                        {
                            throw new Exception();
                        }
                        int num5 = num3 & (((int)0xff) >> (num4 + 1));
                        while (num4 > 1)
                        {
                            if (str[i + 1] != '%')
                            {
                                throw new Exception();
                            }
                            num3 = HexValue(str[i + 2], str[i + 3]);
                            i += 3;
                            if ((num3 & 0xc0) != 0x80)
                            {
                                throw new Exception();
                            }
                            num5 = (num5 << 6) | (num3 & 0x3f);
                            num4--;
                        }
                        if ((num5 >= 0xd800) && (num5 < 0xe000))
                        {
                            throw new Exception();
                        }
                        if (num5 < 0x10000)
                        {
                            ch2 = (char)num5;
                        }
                        else
                        {
                            if (num5 > 0x10ffff)
                            {
                                throw new Exception();
                            }
                            builder.Append((char)((((num5 - 0x10000) >> 10) & 0x3ff) + 0xd800));
                            builder.Append((char)(((num5 - 0x10000) & 0x3ff) + 0xdc00));
                            goto Label_01D4;
                        }
                    }
                    if (InURISet(ch2, flags))
                    {
                        builder.Append(str, startIndex, (i - startIndex) + 1);
                    }
                    else
                    {
                        builder.Append(ch2);
                    }
                Label_01D4:;
                }
            }
            return builder.ToString();
        }

        private static string Encode(object uri, URISetType flags)
        {
            string str = Convert.ToString(uri);
            StringBuilder bs = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (InURISet(ch, flags))
                {
                    bs.Append(ch);
                }
                else
                {
                    int num2 = ch;
                    if ((num2 >= 0) && (num2 <= 0x7f))
                    {
                        AppendInHex(bs, num2);
                    }
                    else if ((num2 >= 0x80) && (num2 <= 0x7ff))
                    {
                        AppendInHex(bs, (num2 >> 6) | 0xc0);
                        AppendInHex(bs, (num2 & 0x3f) | 0x80);
                    }
                    else if ((num2 < 0xd800) || (num2 > 0xdfff))
                    {
                        AppendInHex(bs, (num2 >> 12) | 0xe0);
                        AppendInHex(bs, ((num2 >> 6) & 0x3f) | 0x80);
                        AppendInHex(bs, (num2 & 0x3f) | 0x80);
                    }
                    else
                    {
                        if ((num2 >= 0xdc00) && (num2 <= 0xdfff))
                        {
                            throw new Exception();
                        }
                        if (++i >= str.Length)
                        {
                            throw new Exception();
                        }
                        int num3 = str[i];
                        if ((num3 < 0xdc00) || (num3 > 0xdfff))
                        {
                            throw new Exception();
                        }
                        num2 = (((num2 - 0xd800) << 10) + num3) + 0x2400;
                        AppendInHex(bs, (num2 >> 0x12) | 240);
                        AppendInHex(bs, ((num2 >> 12) & 0x3f) | 0x80);
                        AppendInHex(bs, ((num2 >> 6) & 0x3f) | 0x80);
                        AppendInHex(bs, (num2 & 0x3f) | 0x80);
                    }
                }
            }
            return bs.ToString();
        }

        /// <summary>
        /// 轉換為 Key / Value 資料
        /// </summary>
        /// <param name="json">json 資料</param>
        /// <returns></returns>
        public static Dictionary<string, string> JsonToDict(string json)
        {
            var rtn = new Dictionary<string, string>();
            if (json == null || json == "")
            {
                return rtn;
            }
            try
            {
                rtn = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
            }
            return rtn;
        }
    }
}
