//3.02及以下版本命名空间应将Teigha换为DWGdirect
#if Teigha302Dnow
using DWGdirect.DatabaseServices;
using DWGdirect.Geometry;
using DWGdirect.Colors;
using DWGdirect.Export_Import;
using DWGdirect.GraphicsInterface;
using DWGdirect.GraphicsSystem;
using DWGdirect.Runtime; 
#else
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Colors;
using Teigha.Export_Import;
using Teigha.GraphicsInterface;
using Teigha.GraphicsSystem;
using Teigha.Runtime;
#endif


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SumStruct
{
    // 自定义类，存储线段的信息
    public class LineInfo
    {
        public string EntityType { get; set; }
        public ObjectId EntityId { get; set; }
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }

        // 构造函数，用于初始化线段信息
        public LineInfo(string entityType, ObjectId entityId, Point3d startPoint, Point3d endPoint)
        {
            EntityType = entityType;
            EntityId = entityId;
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }

    // 自定义类，存储线段的信息
    public class PolyLineInfo
    {
        public string EntityType { get; set; }
        public ObjectId EntityId { get; set; }
        public List<Point3d> Points { get; set; }

        // 构造函数，用于初始化线段信息
        public PolyLineInfo(string entityType, ObjectId entityId, List<Point3d> points)
        {
            EntityType = entityType;
            EntityId = entityId;
            Points = points;
        }
    }

    // 自定义类，存储块的信息
    public class BlockReferenceInfo
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public ObjectId ObjectId { get; set; }
        public Point3d Position { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double MinX { get; set; }
        public double MinY { get; set; }

        public double Value { get; set; }

        // 构造函数
        public BlockReferenceInfo(string type, string name, ObjectId objectId, Point3d position, double width, double height,
             double maxX, double maxY, double minX, double minY)
        {
            Type = type;
            Name = name;
            ObjectId = objectId;
            Position = position;
            Width = width;
            Height = height;
            MaxX = maxX;
            MaxY = maxY;
            MinX = minX;
            MinY = minY;
        }
    }

    // 定义保存Hatch(填充的圆)属性的类
    public class HatchInfo
    {
        public string HatchType { get; set; }
        public ObjectId HatchId { get; set; }
        public Point3d HatchCenter { get; set; }
        public double HatchRadius { get; set; }

        // 构造函数
        public HatchInfo(string type, ObjectId id, Point3d center, double radius)
        {
            HatchType = type;
            HatchId = id;
            HatchCenter = center;
            HatchRadius = radius;
        }
        // 自定义类，存储文字的信息
    }
    public class TextInfo
    {
        public string EntityType { get; set; }
        public ObjectId EntityId { get; set; }
        public Point3d Position { get; set; }
        public string TextContent { get; set; }

        // 构造函数，用于初始化文字信息
        public TextInfo(string entityType, ObjectId entityId, Point3d position, string textContent)
        {
            EntityType = entityType;
            EntityId = entityId;
            Position = position;
            TextContent = textContent;
        }
    }

    //面向关系的多段线
    public class LineStruct
    {
        public ObjectId PolylineId { get; set; }
        public ObjectId StartBlockId { get; set; }
        public ObjectId EndBlockId { get; set; }

        public LineStruct(ObjectId polylineId, ObjectId startBlockId, ObjectId endBlockId)
        {
            PolylineId = polylineId;
            StartBlockId = startBlockId;
            EndBlockId = endBlockId;
        }
    }




    //面向关系的块
    public class BlockStruct
    {
        public ObjectId PolylineId { get; set; }
        public BlockReferenceInfo BlockInfo { get; set; }
        public String BlockName { get; set; }
        public List<ObjectId> InDegreePolyLineIds { get; set; } // 入度数组
        public List<ObjectId> OutDegreePolyLineIds { get; set; } // 出度数组
        // 【新增】直接存储 Handle 字符串，防止内存溢出错误
        public List<string> InDegreeHandles = new List<string>();
        public List<string> OutDegreeHandles = new List<string>();

        public BlockStruct(ObjectId polylineId, BlockReferenceInfo blockInfo, List<ObjectId> inDegreePolyLineIds, List<ObjectId> outDegreePolyLineIds)
        {
            PolylineId = polylineId;
            BlockInfo = blockInfo;
            InDegreePolyLineIds = inDegreePolyLineIds;
            OutDegreePolyLineIds = outDegreePolyLineIds;
        }
    }

    public class SourceBlock
    {


        public String BlockType { get; set; }
        public String BlockCode { get; set; }
        public List<string> InputVarList { get; set; }
        public List<string> OutputVarList { get; set; }

        //public SourceBlock()
        //{
        //    InputVarList = new List<string>();
        //    OutputVarList = new List<string>();
        //}
        public SourceBlock(String blocktype, String blockcode, List<string> inputvarlist, List<string> outputvarlist)
        {
            BlockType = blocktype;
            BlockCode = blockcode;
            InputVarList = inputvarlist;
            OutputVarList = outputvarlist;

        }

    }
    public class STBlock
    {


        public String BlockType { get; set; }
        public String BlockCodeName { get; set; }
        public int BlockVarNum { get; set; }
        public List<String> BlockInVar { get; set; }
        public List<String> BlockOutVar { get; set; }
        public List<String> BlockCode { get; set; }

        public STBlock(String blocktype, String blockcodename, int blockvarnum, List<String> blockinvar, List<String> blockoutvar, List<String> blockcode)
        {
            BlockType = blocktype;
            BlockCodeName = blockcodename;
            BlockVarNum = blockvarnum;
            BlockInVar = blockinvar;
            BlockOutVar = blockoutvar;
            BlockCode = blockcode;
        }

        public string FormatBlock()
        {
            var inVars = string.Join(", ", BlockInVar.Select((var, index) => $"IN{index + 1} := {var}"));
            var outVars = string.Join(", ", BlockOutVar.Select((var, index) => $"OUT{index + 1} => {var}"));
            return $"{BlockCodeName}({inVars}, {outVars} );";
        }

    }
    public class STFunBlockInfo
    {
        public string BlockName { get; set; }
        public string FunctionName { get; set; }
        public List<string> Inputs { get; set; }
        public List<string> Outputs { get; set; }
        public List<string> Temp { get; set; }
        public List<string> Code { get; set; }

        public STFunBlockInfo()
        {
            Inputs = new List<string>();
            Outputs = new List<string>();
            Temp = new List<string>();
            Code = new List<string>();
        }
        public STFunBlockInfo(string blockname, string functionname, List<string> inputs, List<string> outputs, List<string> temp, List<string> code)
        {
            BlockName = blockname;
            FunctionName = functionname;
            Inputs = inputs;
            Outputs = outputs;
            Temp = temp;
            Code = code;
        }
        public override string ToString()
        {
            return $"BlockName: {BlockName}\nFunctionName: {FunctionName}\nInputs: {string.Join("\n", Inputs)}\nOutput: {string.Join("\n", Outputs)}\nTemp: {string.Join("\n", Temp)}\nCode: {string.Join("\n", Code)}";
        }
    }
}
