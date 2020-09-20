using System;
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
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine("请输入要生成注释的代码文件夹");
            //var inputFolder = Console.ReadLine();
            var inputFolder = @"D:\Projects\EmmyLuaCode\LuaProject\Leo";
            Console.WriteLine(inputFolder);

            Console.WriteLine("请输入生成代码的存储文件夹");
            //var outputFolder = Console.ReadLine();
            var outputFolder = @"D:\Projects\EmmyLuaCode\EmmyLua";
            Console.WriteLine(outputFolder);

            var luaCodes = Directory.GetFiles(inputFolder, "*.lua",SearchOption.AllDirectories);
            foreach (var item in luaCodes)
            {
                EmmyLuaComment.MakeComments(item, outputFolder);
                //Console.WriteLine(string.Format("comments on file {0}",item));
            }
        }
    }

    class EmmyLuaComment
    {
        static Regex rx = new Regex(@"function (.*):(.*)\n");
        public static string GenerateComments(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = File.ReadAllText(filePath);
            var matches = rx.Matches(content);

            var strBuid = new StringBuilder();
            strBuid.AppendLine(string.Format("---@class {0}", fileName));

            foreach (Match item in matches)
            {
                foreach ( Capture ca in item.Groups)
                {
                    strBuid.AppendLine(ca.Value);
                }
            }

            return strBuid.ToString();
        }

        public static void MakeComments(string filePath, string storeFolder)
        {
            //get comment content
            var content = GenerateComments(filePath);

            //get store file path
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            fileName = fileName + "_emmyLua.lua";
            var storeFile = Path.Combine(storeFolder, fileName);


            //store file
            File.WriteAllText(storeFile,content);
        }
    }
}
