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
            var inputFolder = @"D:\UnityProjects\zeus-framework\Project\Workspace\zeus-framework-demo\Packages\com.pwrd.zeus-framework.lua\Runtime\Lua\Framework\UI";
            Console.WriteLine(inputFolder);

            Console.WriteLine("请输入生成代码的存储文件夹");
            //var outputFolder = Console.ReadLine();
            var outputFolder = @"D:\UnityProjects\zeus-framework\Project\Workspace\zeus-framework-demo\Packages\com.pwrd.zeus-framework.lua\Runtime\Lua\Framework\EmmyLua";
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
        static Regex functionDeclarationRx = new Regex(@"function (.*):(.*)\n");
        static Regex nameRex = new Regex(@"\w+\b");
        public static string GenerateComments(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = File.ReadAllText(filePath);
            var matches = functionDeclarationRx.Matches(content);

            var strBuid = new StringBuilder();
            strBuid.AppendLine(string.Format("---@class {0}", fileName));

            foreach (Match item in matches)
            {
                var icotent = "---@field";
                var names = nameRex.Matches(item.Value);

                //函数名index = 3,之后是参数
                int index = 0;
                foreach (Match na in names)
                {
                    index++;
                    if(index == 3)
                    {
                        icotent = string.Format("{0} {1} fun(", icotent, na.Value);  
                    }
                    if(index > 3 && index < names.Count)
                    {
                        icotent =icotent + string.Format("{0}:some,",na.Value);
                    }
                    if(index > 3 && index == names.Count)
                    {
                        icotent =icotent + string.Format("{0}:some",na.Value);
                    }
                }

                strBuid.AppendLine(icotent + ")");
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
