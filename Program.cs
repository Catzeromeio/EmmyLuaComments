using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace EmmyLuaComments
{
    class Program
    {
        static void Main(string[] args)
        {
            EmmyLuaComment.GenerateComments(args[0]);
            Console.WriteLine(args[0]);
        }
        //static void Main(string[] args)
        //{
        //    Console.WriteLine(Directory.GetCurrentDirectory());
        //    Console.WriteLine("请输入要生成注释的代码文件夹");
        //    //var inputFolder = Console.ReadLine();
        //    var inputFolder = @"D:\UnityProjects\zeus-framework\Project\Workspace\zeus-framework-demo\Packages\com.pwrd.zeus-framework.lua\Runtime\Lua\Framework";
        //    Console.WriteLine(inputFolder);

        //    Console.WriteLine("请输入生成代码的存储文件夹");
        //    //var outputFolder = Console.ReadLine();
        //    var outputFolder = @"D:\UnityProjects\zeus-framework\Project\Workspace\zeus-framework-demo\Packages\com.pwrd.zeus-framework.lua\Runtime\Lua\Framework\EmmyLua";
        //    Console.WriteLine(outputFolder);

        //    var luaCodes = Directory.GetFiles(inputFolder, "*.lua", SearchOption.AllDirectories);
        //    foreach (var item in luaCodes)
        //    {
        //        EmmyLuaComment.GenerateComments(item);
        //        //Console.WriteLine(string.Format("comments on file {0}",item));
        //    }
        //}
    }

    class EmmyLuaComment
    {

        static Regex functionRx = new Regex(@"function ([\s\S]*?)\r\nend\r\n");
        static Regex functionDeclarationRx = new Regex(@"function (.*)[.:](.*)?\n");
        static Regex nameRex = new Regex(@"([\w]+\b)|(\.\.\.)");
        static Regex propertyRex = new Regex(@"self\.(.*)\b =");
        public static void GenerateComments(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = File.ReadAllText(filePath);
            var contentBuilder = new StringBuilder(content);

            //make header
            var hmes = propertyRex.Matches(content);
            var dicp = new Dictionary<string, string>();
            foreach (Match hm in hmes)
            {
                if(!dicp.ContainsKey(hm.Groups[1].Value))
                {
                    dicp.Add(hm.Groups[1].Value,hm.Groups[1].Value);
                }
            }

            var header = string.Format("---@class {0}\r\n", fileName);
            foreach (string value in dicp.Keys)
            {
                header += string.Format("---@field {0} \r\n", value);
            }

            int addedCharCount = 0;
            var hcharArray = header.ToCharArray();
            contentBuilder.Insert(0 + addedCharCount, hcharArray);
            addedCharCount += hcharArray.Length;

            var matches = functionRx.Matches(content);

            int matchCount = matches.Count;
            foreach (Match item in matches)
            {
                var fc = functionDeclarationRx.Matches(item.Value);
                if (fc.Count == 0)
                    continue;

                var names = nameRex.Matches(fc[0].Value);

                var icomments = string.Empty; 
                //函数名index = 3,之后是参数
                int index = 0;
                foreach (Match na in names)
                {
                    index++;
                    if(index > 3)
                    {
                        icomments =icomments + string.Format("---@param {0} \n",na.Value) ;
                    }
                }

                if (item.Value.Contains("return "))
                {
                    icomments =icomments + string.Format("---@return \n") ;
                }

                var charArray = icomments.ToCharArray();
                contentBuilder.Insert(item.Index + addedCharCount, charArray);
                addedCharCount += charArray.Length;
            }

            File.WriteAllText(filePath,contentBuilder.ToString());
        }
    }
}
