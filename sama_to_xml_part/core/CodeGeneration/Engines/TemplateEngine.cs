using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CodeGeneration/Engines/TemplateEngine.cs

using System.IO;
using System.Reflection;

namespace SAMAToXML.CodeGeneration.Engines
{
    /// <summary>
    /// 模板引擎
    /// </summary>
    public class TemplateEngine
    {
        private Dictionary<string, CodeTemplate> _templates = new Dictionary<string, CodeTemplate>();
        private readonly CodeGenConfig _config;

        public TemplateEngine(CodeGenConfig config)
        {
            _config = config;
            LoadTemplates();
        }

        // 添加LoadTemplateFromFile方法
        public void LoadTemplateFromFile(string filePath)
        {
            try
            {
                var content = File.ReadAllText(filePath);
                var template = new CodeTemplate
                {
                    TemplateName = Path.GetFileNameWithoutExtension(filePath),
                    TargetLanguage = "ST",
                    TemplateText = content
                };

                _templates[template.TemplateName] = template;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载模板文件失败 {filePath}: {ex.Message}");
            }
        }

        private void LoadTemplates()
        {
            // 从嵌入式资源或文件加载模板
            LoadDefaultTemplates();

            // 也可以从用户自定义目录加载
            if (Directory.Exists("Templates"))
            {
                LoadTemplatesFromDirectory("Templates");
            }
        }

        private void LoadDefaultTemplates()
        {
            // 基础变量声明模板
            _templates["VAR_DECLARATION"] = new CodeTemplate
            {
                TemplateName = "VAR_DECLARATION",
                TargetLanguage = "ST",
                TemplateText = @"VAR
{declarations}
END_VAR",
                RequiredParameters = new List<string> { "declarations" }
            };

            // 程序结构模板
            _templates["PROGRAM_STRUCTURE"] = new CodeTemplate
            {
                TemplateName = "PROGRAM_STRUCTURE",
                TargetLanguage = "ST",
                TemplateText = @"PROGRAM {program_name}
{var_declaration}

{main_logic}
END_PROGRAM",
                RequiredParameters = new List<string> { "program_name", "var_declaration", "main_logic" }
            };

            // 函数块调用模板
            _templates["FUNCTION_BLOCK_CALL"] = new CodeTemplate
            {
                TemplateName = "FUNCTION_BLOCK_CALL",
                TargetLanguage = "ST",
                TemplateText = @"(* {comment} *)
{instance_name}(
{parameters}
);",
                RequiredParameters = new List<string> { "instance_name", "parameters" }
            };

            // IF语句模板
            _templates["IF_STATEMENT"] = new CodeTemplate
            {
                TemplateName = "IF_STATEMENT",
                TargetLanguage = "ST",
                TemplateText = @"IF {condition} THEN
{then_block}
{else_block}END_IF;",
                RequiredParameters = new List<string> { "condition", "then_block" }
            };

            // 赋值模板
            _templates["ASSIGNMENT"] = new CodeTemplate
            {
                TemplateName = "ASSIGNMENT",
                TargetLanguage = "ST",
                TemplateText = "{variable} := {expression};"
            };

            // 定时器模板
            _templates["TON"] = new CodeTemplate
            {
                TemplateName = "TON",
                TargetLanguage = "ST",
                TemplateText = @"TON_{instance_name}(
    IN := {input_condition},
    PT := {preset_time},
    Q => {output_variable}
);",
                RequiredParameters = new List<string> { "instance_name", "input_condition", "preset_time", "output_variable" }
            };
        }

        private void LoadTemplatesFromDirectory(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath, "*.tmpl"))
            {
                try
                {
                    var content = File.ReadAllText(file);
                    var lines = content.Split('\n');

                    if (lines.Length > 0)
                    {
                        var template = new CodeTemplate
                        {
                            TemplateName = Path.GetFileNameWithoutExtension(file),
                            TargetLanguage = "ST", // 从文件元数据读取
                            TemplateText = content
                        };

                        _templates[template.TemplateName] = template;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载模板失败 {file}: {ex.Message}");
                }
            }
        }

        public CodeTemplate GetTemplate(string templateName)
        {
            if (_templates.ContainsKey(templateName))
                return _templates[templateName];

            throw new KeyNotFoundException($"未找到模板: {templateName}");
        }

        public bool TemplateExists(string templateName) => _templates.ContainsKey(templateName);

        public List<string> GetAvailableTemplates() => _templates.Keys.ToList();
    }
}