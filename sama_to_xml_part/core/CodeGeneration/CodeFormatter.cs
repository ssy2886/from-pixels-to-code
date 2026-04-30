using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CodeGeneration/CodeFormatter.cs
using System.Text.RegularExpressions;

namespace SAMAToXML.CodeGeneration
{
    /// <summary>
    /// 代码格式化器
    /// </summary>
    public class CodeFormatter
    {
        private readonly CodeGenConfig _config;

        public CodeFormatter(CodeGenConfig config)
        {
            _config = config;
        }

        public string Format(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
                return rawCode;

            var lines = rawCode.Split('\n');
            var formattedLines = new StringBuilder();
            int indentLevel = 0;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    formattedLines.AppendLine();
                    continue;
                }

                // 处理结束标记（减少缩进）
                if (trimmed.StartsWith("END_") || trimmed == "END_PROGRAM")
                {
                    indentLevel = Math.Max(0, indentLevel - 1);
                }

                // 添加当前行的缩进
                var indent = new string(' ', indentLevel * _config.IndentSize);
                formattedLines.Append(indent);
                formattedLines.AppendLine(trimmed);

                // 处理开始标记（增加缩进）
                if (trimmed.StartsWith("PROGRAM ") ||
                    trimmed.StartsWith("FUNCTION ") ||
                    trimmed.StartsWith("FUNCTION_BLOCK ") ||
                    trimmed == "VAR" ||
                    trimmed == "IF" ||
                    trimmed == "THEN" ||
                    trimmed == "ELSE" ||
                    trimmed == "CASE")
                {
                    indentLevel++;
                }
            }

            var result = formattedLines.ToString();

            // 应用其他格式化规则
            if (_config.OptimizeCode)
            {
                result = OptimizeCode(result);
            }

            return result;
        }

        private string OptimizeCode(string code)
        {
            // 1. 移除多余的空行
            code = Regex.Replace(code, @"(\r?\n){3,}", "\n\n");

            // 2. 对齐赋值操作符（如果有）
            if (_config.IndentSize > 0)
            {
                code = AlignAssignmentOperators(code);
            }

            // 3. 标准化布尔值
            code = code.Replace("True", "TRUE")
                      .Replace("False", "FALSE")
                      .Replace("true", "TRUE")
                      .Replace("false", "FALSE");

            // 4. 标准化注释
            code = StandardizeComments(code);

            return code;
        }

        private string AlignAssignmentOperators(string code)
        {
            var lines = code.Split('\n');
            var maxAssignmentPos = 0;

            // 第一遍：找出赋值操作符的最大位置
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"([A-Za-z_][A-Za-z0-9_]*)\s*:=");
                if (match.Success)
                {
                    var pos = match.Index + match.Length;
                    if (pos > maxAssignmentPos)
                        maxAssignmentPos = pos;
                }
            }

            // 第二遍：对齐赋值操作符
            if (maxAssignmentPos > 0)
            {
                var result = new StringBuilder();
                foreach (var line in lines)
                {
                    var match = Regex.Match(line, @"([A-Za-z_][A-Za-z0-9_]*)\s*:=");
                    if (match.Success)
                    {
                        var currentPos = match.Index + match.Length;
                        var spacesNeeded = maxAssignmentPos - currentPos;

                        if (spacesNeeded > 0)
                        {
                            var alignedLine = line.Replace(":=", new string(' ', spacesNeeded) + ":=");
                            result.AppendLine(alignedLine);
                        }
                        else
                        {
                            result.AppendLine(line);
                        }
                    }
                    else
                    {
                        result.AppendLine(line);
                    }
                }

                return result.ToString();
            }

            return code;
        }

        private string StandardizeComments(string code)
        {
            // 标准化单行注释
            code = Regex.Replace(code, @"//\s*(.*)", "(* $1 *)");

            // 确保多行注释有正确的缩进
            var lines = code.Split('\n');
            var inMultiLineComment = false;
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("(*") && trimmed.EndsWith("*)"))
                {
                    // 单行注释，保持原样
                    result.AppendLine(line);
                }
                else if (trimmed.StartsWith("(*"))
                {
                    inMultiLineComment = true;
                    result.AppendLine(line);
                }
                else if (inMultiLineComment && trimmed.EndsWith("*)"))
                {
                    inMultiLineComment = false;
                    result.AppendLine(line);
                }
                else
                {
                    result.AppendLine(line);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 添加执行层注释
        /// </summary>
        public string AddExecutionLayerComments(string code, List<string> layerComments)
        {
            if (layerComments == null || layerComments.Count == 0)
                return code;

            var result = new StringBuilder();
            var lines = code.Split('\n');
            var layerIndex = 0;

            foreach (var line in lines)
            {
                result.AppendLine(line);

                // 在适当位置插入层注释
                if (line.Contains("BEGIN") && layerIndex < layerComments.Count)
                {
                    result.AppendLine();
                    result.AppendLine($"(* ===== {layerComments[layerIndex]} ===== *)");
                    layerIndex++;
                }
            }

            return result.ToString();
        }
    }
}
