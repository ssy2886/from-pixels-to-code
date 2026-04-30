using SumStruct;
using System;
// CodeGeneration/Models/CodeGenerationModels.cs
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMAToXML.CodeGeneration
{
    /// <summary>
    /// 代码生成配置
    /// </summary>
    public class CodeGenConfig
    {
        public string TargetLanguage { get; set; } = "ST"; // ST, IL, FBD
        public string NamingConvention { get; set; } = "PascalCase"; // PascalCase, CamelCase, UpperSnake
        public bool GenerateComments { get; set; } = true;
        public bool IncludeVariableDeclaration { get; set; } = true;
        public bool OptimizeCode { get; set; } = true;
        public int IndentSize { get; set; } = 4;

        // 从Form2获取配置
        public static CodeGenConfig FromForm2()
        {
            // 直接返回默认配置
            return new CodeGenConfig();
        }
        // 添加FromApplicationSettings方法
        public static CodeGenConfig FromApplicationSettings()
        {
            return new CodeGenConfig(); // 简化版本
        }
    }

    /// <summary>
    /// 整合后的系统模型
    /// </summary>
    public class UnifiedSystemModel
    {
        public List<IntegratedBlock> Blocks { get; set; } = new List<IntegratedBlock>();
        public List<Connection> Connections { get; set; } = new List<Connection>();
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

        public bool Validate()
        {
            Errors.Clear();

            // 1. 检查未定义的功能块
            var undefinedBlocks = Blocks.Where(b => b.StDefinition == null).ToList();
            foreach (var block in undefinedBlocks)
            {
                Errors.Add(new ValidationError
                {
                    Type = ValidationErrorType.UndefinedBlock,
                    Message = $"未找到功能块 '{block.DwgInfo.Name}' 的ST定义",
                    BlockId = block.Id
                });
            }

            // 2. 检查孤立块
            var isolatedBlocks = Blocks.Where(b =>
                !Connections.Any(c => c.SourceBlockId == b.Id || c.TargetBlockId == b.Id)
            ).ToList();

            // 3. 检查循环依赖（简化版，实际需要图算法）
            // ... 这里需要实现环检测算法

            return Errors.Count == 0;
        }

        public List<IntegratedBlock> GetTopologicalOrder()
        {
            // 直接返回块列表
            return Blocks.ToList();
        }

        private bool TopologicalSortDFS(IntegratedBlock block, HashSet<string> visited,
            HashSet<string> visiting, List<IntegratedBlock> result)
        {
            if (visiting.Contains(block.Id))
                return false; // 检测到环

            if (visited.Contains(block.Id))
                return true;

            visiting.Add(block.Id);

            // 获取所有依赖的块
            var dependencies = Connections
                .Where(c => c.TargetBlockId == block.Id)
                .Select(c => Blocks.FirstOrDefault(b => b.Id == c.SourceBlockId))
                .Where(b => b != null)
                .ToList();

            foreach (var dep in dependencies)
            {
                if (!TopologicalSortDFS(dep, visited, visiting, result))
                    return false;
            }

            visiting.Remove(block.Id);
            visited.Add(block.Id);
            result.Add(block);

            return true;
        }
    }

    /// <summary>
    /// 整合后的功能块
    /// </summary>
    public class IntegratedBlock
    {
        public string Id { get; set; }
        public BlockReferenceInfo DwgInfo { get; set; }
        public STFunBlockInfo StDefinition { get; set; }
        public string InstanceName { get; set; }
        public List<PortInfo> InputPorts { get; set; } = new List<PortInfo>();
        public List<PortInfo> OutputPorts { get; set; } = new List<PortInfo>();

        // 自动生成实例名
        public void GenerateInstanceName(int counter)
        {
            var blockType = StDefinition?.FunctionName ?? DwgInfo.Name;
            InstanceName = $"{blockType}_{counter:D3}";
        }
    }

    /// <summary>
    /// 连接关系
    /// </summary>
    public class Connection
    {
        public string SourceBlockId { get; set; }
        public string TargetBlockId { get; set; }
        public string SourcePort { get; set; } = "OUT";
        public string TargetPort { get; set; } = "IN";
        public string DataType { get; set; } = "REAL";
    }

    /// <summary>
    /// 端口信息
    /// </summary>
    public class PortInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public PortDirection Direction { get; set; }
    }

    public enum PortDirection { Input, Output }

    /// <summary>
    /// 验证错误
    /// </summary>
    public class ValidationError
    {
        public ValidationErrorType Type { get; set; }
        public string Message { get; set; }
        public string BlockId { get; set; }
        public Severity Severity { get; set; } = Severity.Error;
    }

    public enum ValidationErrorType
    {
        UndefinedBlock,
        CyclicDependency,
        PortMismatch,
        IsolatedBlock
    }

    public enum Severity { Info, Warning, Error }

    /// <summary>
    /// 代码生成结果
    /// </summary>
    public class GenerationResult
    {
        public string GeneratedCode { get; set; }
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();
        public List<string> Warnings { get; set; } = new List<string>();
        public Dictionary<string, string> Statistics { get; set; } = new Dictionary<string, string>();
        public TimeSpan GenerationTime { get; set; }
        public bool Success => Errors.Count == 0;
    }

    /// <summary>
    /// 模板定义
    /// </summary>
    public class CodeTemplate
    {
        public string TemplateName { get; set; }
        public string TargetLanguage { get; set; }
        public string BlockType { get; set; }
        public string TemplateText { get; set; }
        public List<string> RequiredParameters { get; set; } = new List<string>();

        public string Instantiate(Dictionary<string, string> parameters)
        {
            var result = TemplateText;

            // 检查必需参数
            foreach (var param in RequiredParameters)
            {
                if (!parameters.ContainsKey(param))
                {
                    throw new ArgumentException($"缺少必需参数: {param}");
                }
            }

            // 替换占位符
            foreach (var kvp in parameters)
            {
                result = result.Replace($"{{{kvp.Key}}}", kvp.Value);
            }

            // 处理可选参数（用空字符串替换未提供的参数）
            var pattern = @"\{(\w+)\}";
            var matches = System.Text.RegularExpressions.Regex.Matches(result, pattern);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var paramName = match.Groups[1].Value;
                if (!parameters.ContainsKey(paramName) && !RequiredParameters.Contains(paramName))
                {
                    result = result.Replace($"{{{paramName}}}", "");
                }
            }

            return result;
        }
    }



}