// Excerpt generated from SAMAToXML/Form1.cs for paper companion repository.
// It preserves the key workflow sections used in the paper discussion.

// ===== Excerpt lines 1-120 =====
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using SumStruct;
using static SAMAToXML.Form1;
using System.Drawing.Imaging;
using System.Net.Http;
using Newtonsoft.Json;

using CadColor = Teigha.Colors.Color;
using DrawColor = System.Drawing.Color;
// 【添加这一行】明确告诉编译器，代码里的 Polyline 指的是 DatabaseServices 下的那个
using Polyline = Teigha.DatabaseServices.Polyline;



namespace SAMAToXML
{
    
    public partial class Form1 : Form
    {

        // 在Form1.cs顶部，添加数据管理器实例
        private DataManager _dataManager = new DataManager();

        // 修改现有的静态变量为实例变量
        private List<BlockReferenceInfo> _blockInfoList = new List<BlockReferenceInfo>();
        // 其他集合也改为实例变量...

        // 更新公共属性，返回数据管理器中的数据
        public List<SourceBlock> GetSourceBlocks() => _dataManager.SourceBlocks;
        public Dictionary<ObjectId, LineStruct> GetConnectionsP() => _dataManager.ConnectionsP;
        public Dictionary<ObjectId, BlockStruct> GetConnectionsB() => _dataManager.ConnectionsB;

        // AI模型服务配置
        private string _aiModelUrl = "http://localhost:5000/predict"; // 你的AI模型服务地址
        private HttpClient _httpClient = new HttpClient();
        private double _captureSize = 100.0; // 截图区域大小

        // 存储AI识别结果的类
        public class AiDetectionResult
        {
            public bool IsDetected { get; set; }
            public string Label { get; set; }
            public double Confidence { get; set; }
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        public void PassDataToForm5(Form5 form5)
        {
            if (form5 != null)
            {
                // 传递数据
                form5.SetSourceBlocks(sourceblockList);
                form5.SetConnections(mapForP, mapForB);
            }
        }

        // --- 在 Form1 类中添加此方法 ---
        // 这是一个“安全模式”的转换器，它不关心是否有连接，只要有块就转换
        private List<SourceBlock> ConvertAllBlocksToSourceBlocks()
        {
            List<SourceBlock> logicalBlocks = new List<SourceBlock>();

            Console.WriteLine($"准备转换 {blockInfoList.Count} 个原始块...");

            // 直接遍历 blockInfoList (这是最原始的识别结果，只要图里有，这里就有)
            foreach (var blockInfo in blockInfoList)
            {
                // 构造假的输入输出变量名，仅为了让代码生成器能跑通
                // 因为我们放弃了拓扑，所以不知道具体的连接线ID，就用占位符代替
                List<string> inputs = new List<string> { "IN1", "IN2" };
                List<string> outputs = new List<string> { "OUT1" };

                SourceBlock sb = new SourceBlock(
                    blockInfo.Name,       // 使用块名作为类型，例如 "LOGICAL_AND_16" 或 "DEMO01_1"
                    blockInfo.Name,       // 使用块名作为实例名
                    inputs,
                    outputs
                );

                logicalBlocks.Add(sb);
            }

            return logicalBlocks;
        }

        // 修改按钮点击事件
        private void button6_Click(object sender, EventArgs e)
        {

// ===== Excerpt lines 345-730 =====
        public Dictionary<ObjectId, LineStruct> mapForP = new Dictionary<ObjectId, LineStruct>();
        public Dictionary<ObjectId, BlockStruct> mapForB = new Dictionary<ObjectId, BlockStruct>();
        private List<STFunBlockInfo> stfunblocks = new List<STFunBlockInfo>();
        public List<SourceBlock> sourceblockList = new List<SourceBlock>();
        string file = "\\SamaSource.dwg";
        int blocknum = 1;
        public Form1()
        {
            InitializeComponent();
            InitializeBlockInfoPanel();
        }

       

       

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InitializeBlockInfoPanel()
        {
            listBox1.DisplayMember = nameof(BlockReferenceInfo.Name);

            textBoxName.ReadOnly = true;
            textBoxType.ReadOnly = true;
            textBoxX.ReadOnly = true;
            textBoxY.ReadOnly = true;
            textBoxWidth.ReadOnly = true;
            textBoxHeight.ReadOnly = true;

            progressBar1.Visible = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;

            ClearBlockDetails();
        }

        private void RefreshBlockList()
        {
            listBox1.BeginUpdate();
            try
            {
                listBox1.DataSource = null;
                listBox1.Items.Clear();
                listBox1.DataSource = blockInfoList.ToList();
                listBox1.DisplayMember = nameof(BlockReferenceInfo.Name);

                if (blockInfoList.Count > 0)
                {
                    listBox1.SelectedIndex = 0;
                    UpdateBlockDetails(blockInfoList[0]);
                }
                else
                {
                    ClearBlockDetails();
                }
            }
            finally
            {
                listBox1.EndUpdate();
            }
        }

        private void UpdateBlockDetails(BlockReferenceInfo selectedBlock)
        {
            if (selectedBlock == null)
            {
                ClearBlockDetails();
                return;
            }

            textBoxName.Text = selectedBlock.Name;
            textBoxType.Text = selectedBlock.Type;
            textBoxX.Text = selectedBlock.Position.X.ToString("F2");
            textBoxY.Text = selectedBlock.Position.Y.ToString("F2");
            textBoxWidth.Text = selectedBlock.Width.ToString("F2");
            textBoxHeight.Text = selectedBlock.Height.ToString("F2");
        }

        private void ClearBlockDetails()
        {
            textBoxName.Text = string.Empty;
            textBoxType.Text = string.Empty;
            textBoxX.Text = string.Empty;
            textBoxY.Text = string.Empty;
            textBoxWidth.Text = string.Empty;
            textBoxHeight.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*dwg*)|*.dwg*"; //设置要选择的文件的类型
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                 file = fileDialog.FileName;//返回文件的完整路径   
                axDwgViewX1.DrawingFile = file;
                filepath.Text = file;



            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // 初始化进度条
            progressBar1.Value = 0;

            try
            {
                Console.WriteLine($"\n=== 开始处理文件: {Path.GetFileName(file)} ===");

                // 2. 确保在这里彻底清空所有数据容器（防止上一张图的数据残留）
                blockInfoList.Clear();
                polylineInfoList.Clear();
                lineInfoList.Clear();
                mapForP.Clear();
                mapForB.Clear();
                sourceblockList.Clear();
                // 这一步很重要

                // 任务1: 获取数据
                GetData(file);
                RefreshBlockList();
                progressBar1.Value = 20;

                // 添加调试信息
                Console.WriteLine($"获取到的块数量: {blockInfoList.Count}");
                Console.WriteLine($"获取到的多段线数量: {polylineInfoList.Count}");
                Console.WriteLine($"获取到的直线数量: {lineInfoList.Count}");

                // 任务2: 分析数据
                AnalyzeData();
                progressBar1.Value = 40;

                // 任务3: 处理直线与多线段
                ProcessAllConnections();
                progressBar1.Value = 60;

                // 任务4: 寻找和处理悬空连接（调用AI识别）
                await ProcessDanglingConnections();
                RefreshBlockList();
                progressBar1.Value = 80;

                // 任务5: 重新处理连接关系（包含AI识别的虚拟块）
                ProcessAllConnections(); // 重新建立连接
                progressBar1.Value = 90;

                // 添加调试信息
                Console.WriteLine($"建立的连接关系数量 (mapForB): {mapForB.Count}");
                Console.WriteLine($"建立的连接关系数量 (mapForP): {mapForP.Count}");

                // 任务6: 广度优先搜索 - 添加条件判断
                if (mapForB.Count > 0)
                {
                    BreadthFirstSearch();
                }
                else
                {
                    MessageBox.Show("没有发现连接关系，无法进行广度优先搜索。\n请检查DWG文件中是否有块和多段线，并且它们是否相交。",
                        "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                
                progressBar1.Value = 100;
                MessageBox.Show("任务完成！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"处理过程中出现错误：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"错误：{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
            }
        }

        // 新增：统一处理所有连接的函数（同时处理Line和Polyline）
        private void ProcessAllConnections()
        {
            // 清空之前的连接关系
            mapForP.Clear();
            mapForB.Clear();

            // 合并所有线段（Line 和 Polyline）
            var allLines = new List<(ObjectId Id, Point3d Start, Point3d End, bool IsPolyline, List<Point3d> PolylinePoints)>();

            // 添加直线
            foreach (var line in lineInfoList)
            {
                allLines.Add((line.EntityId, line.StartPoint, line.EndPoint, false, null));
            }

            // 添加多段线
            foreach (var polyline in polylineInfoList)
            {
                if (polyline.Points.Count > 0)
                {
                    allLines.Add((polyline.EntityId, polyline.Points[0], polyline.Points[polyline.Points.Count - 1],
                                 true, polyline.Points));
                }
            }

            Console.WriteLine($"开始处理 {allLines.Count} 条线段（直线和多段线）的连接关系...");

            // 第一步：建立线段到块的连接关系 (mapForP)
            foreach (var line in allLines)
            {
                ObjectId startBlockId = ObjectId.Null;
                ObjectId endBlockId = ObjectId.Null;

                // 检查起点和终点是否连接到任何块
                foreach (var blockInfo in blockInfoList)
                {
                    if (IsPointOnBlock(line.Start, blockInfo))
                    {
                        startBlockId = blockInfo.ObjectId;
                    }
                    if (IsPointOnBlock(line.End, blockInfo))
                    {
                        endBlockId = blockInfo.ObjectId;
                    }
                }

                LineStruct lineStruct = new LineStruct(line.Id, startBlockId, endBlockId);
                mapForP[line.Id] = lineStruct;

                // 调试输出
                if (startBlockId == ObjectId.Null && endBlockId == ObjectId.Null)
                {
                    Console.WriteLine($"警告：线段 {line.Id} 两端都未连接");
                }
            }

            Console.WriteLine($"建立了 {mapForP.Count} 条线段连接关系");

            // 第二步：建立块到线段的连接关系 (mapForB)
            foreach (var blockInfo in blockInfoList)
            {
                ObjectId blockId = blockInfo.ObjectId;
                List<ObjectId> inDegreeIds = new List<ObjectId>();
                List<ObjectId> outDegreeIds = new List<ObjectId>();

                foreach (var line in allLines)
                {
                    // 检查起点和终点
                    if (IsPointOnBlock(line.Start, blockInfo))
                    {
                        outDegreeIds.Add(line.Id); // 出度：线从此块出发
                    }
                    if (IsPointOnBlock(line.End, blockInfo))
                    {
                        inDegreeIds.Add(line.Id); // 入度：线到达此块
                    }
                }

                if (inDegreeIds.Count > 0 || outDegreeIds.Count > 0)
                {
                    BlockStruct blockStruct = new BlockStruct(blockId, blockInfo, inDegreeIds, outDegreeIds);
                    mapForB[blockId] = blockStruct;
                }
            }

            Console.WriteLine($"建立了 {mapForB.Count} 个块连接关系");
        }

        // 新增：处理悬空连接并调用AI识别
        private async Task ProcessDanglingConnections()
        {
            Console.WriteLine("\n=== 开始寻找悬空连接 ===");

            // 合并所有线段
            var allCurves = new List<(ObjectId Id, Point3d Start, Point3d End)>();

            foreach (var line in lineInfoList)
            {
                allCurves.Add((line.EntityId, line.StartPoint, line.EndPoint));
            }

            foreach (var polyline in polylineInfoList)
            {
                if (polyline.Points.Count > 0)
                {
                    allCurves.Add((polyline.EntityId, polyline.Points[0],
                                 polyline.Points[polyline.Points.Count - 1]));
                }
            }

            int aiDetectionCount = 0;

            foreach (var curve in allCurves)
            {
                // 检查两端连接情况
                var startBlock = FindConnectedBlock(curve.Start);
                var endBlock = FindConnectedBlock(curve.End);

                Point3d? suspectPoint = null;
                ObjectId? connectedBlockId = null;

                // 情况：一头连着已知块，另一头悬空
                if (startBlock != null && endBlock == null)
                {
                    suspectPoint = curve.End;
                    connectedBlockId = startBlock.ObjectId;
                    Console.WriteLine($"发现悬空连接: 线段 {curve.Id} 从块 '{startBlock.Name}' 出发，终点悬空");
                }
                else if (startBlock == null && endBlock != null)
                {
                    suspectPoint = curve.Start;
                    connectedBlockId = endBlock.ObjectId;
                    Console.WriteLine($"发现悬空连接: 线段 {curve.Id} 到达块 '{endBlock.Name}'，起点悬空");
                }

                if (suspectPoint.HasValue)
                {
                    try
                    {
                        Console.WriteLine($"悬空点坐标: {suspectPoint.Value}");

                        // 截图并调用AI识别
                        var aiResult = await CaptureAndDetectBlock(suspectPoint.Value);

                        if (aiResult != null && aiResult.IsDetected)
                        {
                            aiDetectionCount++;

                            // 创建虚拟块
                            CreateVirtualBlockFromAiResult(aiResult, curve.Id, connectedBlockId);

                            Console.WriteLine($"AI识别成功: {aiResult.Label} (置信度: {aiResult.Confidence:P0})");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"AI处理失败: {ex.Message}");
                    }
                }
            }

            Console.WriteLine($"=== 完成悬空连接处理，AI识别了 {aiDetectionCount} 个新块 ===");
        }

        // 新增：查找与点连接的块
        private BlockReferenceInfo FindConnectedBlock(Point3d point)
        {
            foreach (var block in blockInfoList)
            {
                if (IsPointOnBlock(point, block))
                {
                    return block;
                }
            }
            return null;
        }

        // 新增：截图并调用AI识别
        private async Task<AiDetectionResult> CaptureAndDetectBlock(Point3d centerPoint)
        {
            try
            {
                // 设置截图范围（例如 100x100 世界单位）
                double captureWidth = 100.0;
                double captureHeight = 100.0;
                double dpi = 10.0; // 每个单位 10 像素，最终图像 1000x1000

                // 需要 Database 对象，可以从全局变量或传递进来
                // 假设您有一个成员变量 _db 或可以重新打开文件获取 Database
                // 这里演示如何从当前文件重新获取 Database（注意文件路径）
                using (Services serv = new Services())
                using (Database db = new Database(false, false))
                {
                    db.ReadDwgFile(file, FileShare.Read, false, null);
                    using (var bitmap = CaptureCadImage(db, centerPoint, captureWidth, captureHeight, dpi))
                    {
                        if (bitmap == null)
                        {
                            Console.WriteLine("截图失败");
                            return null;
                        }

// ===== Excerpt lines 1335-1610 =====
        void GetData(string inFile)
        {
            // 清空列表
            blockInfoList.Clear();
            polylineInfoList.Clear();
            lineInfoList.Clear();
            hatchInfoList.Clear();
            textInfoList.Clear();
            listBox1.DataSource = null;
            listBox1.Items.Clear();
            ClearBlockDetails();

            using (Services serv = new Services())
            {
                using (Database db = new Database(false, false))
                {
                    db.ReadDwgFile(inFile, FileShare.ReadWrite, false, null);

                    // 1. 第一步：先收集所有 ID
                    List<ObjectId> allEntityIds = new List<ObjectId>();
                    ObjectId modelSpaceId;

                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                        modelSpaceId = ms.ObjectId;

                        foreach (ObjectId id in ms)
                        {
                            allEntityIds.Add(id);
                        }
                        tr.Commit();
                    }

                    Console.WriteLine($"共扫描到 {allEntityIds.Count} 个实体，开始分批处理...");

                    // 2. 第二步：分批处理
                    int batchSize = 500;
                    int totalProcessed = 0;
                    int blockCount = 0;

                    Transaction trBatch = db.TransactionManager.StartTransaction();
                    BlockTableRecord recordBatch = (BlockTableRecord)trBatch.GetObject(modelSpaceId, OpenMode.ForWrite);

                    try
                    {
                        foreach (ObjectId id in allEntityIds)
                        {
                            try
                            {
                                var entity = trBatch.GetObject(id, OpenMode.ForRead, false, true) as Entity;
                                if (entity == null) continue;

                                var colorYellow = Teigha.Colors.Color.FromRgb(255, 255, 0);
                                var colorCyan = Teigha.Colors.Color.FromRgb(0, 255, 255);

                                // --- 逻辑处理 ---
                                if (entity is BlockReference bitem)
                                {
                                    if (bitem.Bounds.HasValue)
                                    {
                                        Extents3d bounds = bitem.Bounds.Value;

                                        // --- 宽高修正逻辑 ---
                                        double width = bounds.MaxPoint.X - bounds.MinPoint.X;
                                        double height = bounds.MaxPoint.Y - bounds.MinPoint.Y;

                                        // 强制给一个默认值 (如果宽高过小)
                                        double minSize = 10.0;
                                        if (width < 0.1) width = minSize;
                                        if (height < 0.1) height = minSize;

                                        // 重新校正 Min/Max 点，以 Position 为中心向外扩展
                                        double halfW = width / 2.0;
                                        double halfH = height / 2.0;

                                        double minX = bitem.Position.X - halfW;
                                        double maxX = bitem.Position.X + halfW;
                                        double minY = bitem.Position.Y - halfH;
                                        double maxY = bitem.Position.Y + halfH;

                                        // 使用修正后的数据创建 BlockInfo
                                        BlockReferenceInfo blockInfo = new BlockReferenceInfo(
                                            bitem.Name,
                                            bitem.Name + "_" + (++blockCount),
                                            bitem.ObjectId,
                                            bitem.Position,
                                            width,
                                            height,
                                            maxX, maxY,
                                            minX, minY
                                        );
                                        blockInfoList.Add(blockInfo);

                                        // 构造一个新的 Extents3d 用于画框，确保画出来的框也是大的
                                        Extents3d drawBounds = new Extents3d(
                                            new Point3d(minX, minY, 0),
                                            new Point3d(maxX, maxY, 0)
                                        );

                                        // 画框 (使用修正后的范围)
                                        DrawBoundingBoxOptimized(recordBatch, trBatch, drawBounds, bitem.Name, colorYellow);
                                    }
                                }
                                else if (entity is Teigha.DatabaseServices.Polyline polyline)
                                {
                                    var points = new List<Point3d>();
                                    for (int i = 0; i < polyline.NumberOfVertices; i++)
                                        points.Add(polyline.GetPoint3dAt(i));

                                    polylineInfoList.Add(new PolyLineInfo("POLYLINE", polyline.ObjectId, points));

                                    if (polyline.Bounds.HasValue)
                                    {
                                        // 线段只画框
                                        DrawBoundingBoxOptimized(recordBatch, trBatch, polyline.Bounds.Value, null, colorCyan);
                                    }
                                }
                                else if (entity is Line line)
                                {
                                    lineInfoList.Add(new LineInfo("LINE", line.ObjectId, line.StartPoint, line.EndPoint));

                                    if (line.Bounds.HasValue)
                                    {
                                        DrawBoundingBoxOptimized(recordBatch, trBatch, line.Bounds.Value, null, colorCyan);
                                    }
                                }
                                // 新增：识别Hatch（填充的圆）
                                else if (entity is Hatch hatch)
                                {
                                    try
                                    {
                                        // 尝试获取Hatch的边界信息
                                        if (hatch.Bounds.HasValue)
                                        {
                                            Extents3d bounds = hatch.Bounds.Value;
                                            Point3d center = new Point3d(
                                                (bounds.MinPoint.X + bounds.MaxPoint.X) / 2,
                                                (bounds.MinPoint.Y + bounds.MaxPoint.Y) / 2,
                                                0
                                            );
                                            double radius = Math.Max(
                                                bounds.MaxPoint.X - bounds.MinPoint.X,
                                                bounds.MaxPoint.Y - bounds.MinPoint.Y
                                            ) / 2;

                                            hatchInfoList.Add(new HatchInfo("HATCH", hatch.ObjectId, center, radius));

                                            // 将Hatch视为特殊类型的块
                                            BlockReferenceInfo hatchBlock = new BlockReferenceInfo(
                                                "HATCH_CIRCLE",
                                                $"HATCH_{hatch.Handle}",
                                                hatch.ObjectId,
                                                center,
                                                radius * 2,
                                                radius * 2,
                                                bounds.MaxPoint.X, bounds.MaxPoint.Y,
                                                bounds.MinPoint.X, bounds.MinPoint.Y
                                            );
                                            blockInfoList.Add(hatchBlock);

                                            Console.WriteLine($"识别到HATCH圆: 中心({center.X:F2}, {center.Y:F2}), 半径{radius:F2}");
                                        }
                                    }
                                    catch { }
                                }
                            }

                            catch { /* 忽略单个实体错误 */ }

                            totalProcessed++;

                            // --- 分批提交 ---
                            if (totalProcessed % batchSize == 0)
                            {
                                trBatch.Commit();
                                trBatch.Dispose();

                                trBatch = db.TransactionManager.StartTransaction();
                                recordBatch = (BlockTableRecord)trBatch.GetObject(modelSpaceId, OpenMode.ForWrite);
                                Console.WriteLine($"已处理 {totalProcessed} / {allEntityIds.Count} ...");
                            }
                        }

                        trBatch.Commit(); // 提交剩余
                    }
                    finally
                    {
                        trBatch.Dispose();
                    }

                    // 3. 保存
                    string saveDir = @"D:\360MoveData\Users\宋硕宇\Desktop\SAMAToXML 12.1\save";
                    if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);
                    string outputPath = Path.Combine(saveDir, Path.GetFileNameWithoutExtension(inFile) + "_optimized.dwg");

                    db.SaveAs(outputPath, DwgVersion.AC1024);
                    Console.WriteLine("完成！");
                }
            }
        }


        void AnalyzeData()
        {
            Console.WriteLine($"扫描到的直线数量: {lineInfoList.Count}");
            foreach (LineInfo info in lineInfoList)
            {
                Console.WriteLine($"Entity Type: {info.EntityType}");
                Console.WriteLine($"Entity ID: {info.EntityId}");
                Console.WriteLine($"Start Point: {info.StartPoint}");
                Console.WriteLine($"End Point: {info.EndPoint}");
                Console.WriteLine();
            }
            Console.WriteLine($"扫描到的多段线数量: {polylineInfoList.Count}");
            foreach (PolyLineInfo info in polylineInfoList)
            {
                Console.WriteLine($"Entity Type: {info.EntityType}");
                Console.WriteLine($"Entity ID: {info.EntityId}");
                foreach (Point3d point in info.Points)
                {
                    Console.WriteLine($"Points: {point}");
                }
                Console.WriteLine();

            }
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"扫描到的块的数量: {blockInfoList.Count}");
            foreach (BlockReferenceInfo info in blockInfoList)
            {
               
                Console.WriteLine($"Type: {info.Type}");
                Console.WriteLine($"Name: {info.Name}");
                Console.WriteLine($"ObjectId: {info.ObjectId}");
                Console.WriteLine($"Position: {info.Position}");
                Console.WriteLine($"Width: {info.Width}");
                Console.WriteLine($"Height: {info.Height}");
                Console.WriteLine($"Bounds: [{info.MinX:F2}, {info.MinY:F2}] - [{info.MaxX:F2}, {info.MaxY:F2}]");
                Console.WriteLine();
            }
            Console.WriteLine($"扫描到的填充图案数量: {hatchInfoList.Count}");
            foreach (HatchInfo info in hatchInfoList)
            {
                Console.WriteLine($"Type: {info.HatchType}");
                Console.WriteLine($"ObjectId: {info.HatchId}");
                Console.WriteLine($"Center: {info.HatchCenter}");
                Console.WriteLine($"Radius: {info.HatchRadius}");
                Console.WriteLine();
            }

            Console.WriteLine($"扫描到的文字数量: {textInfoList.Count}");
            foreach (TextInfo info in textInfoList)
            {
                Console.WriteLine($"Entity Type: {info.EntityType}");
                Console.WriteLine($"Entity ID: {info.EntityId}");
                Console.WriteLine($"Position: {info.Position}");
                Console.WriteLine($"Text Content: {info.TextContent}");
                Console.WriteLine();
            }
        }

        //判断点是否在块上
        static bool IsPointOnBlock(Point3d point, BlockReferenceInfo blockInfo)
        {
            // 使用动态容差：块宽高的10%或最小5单位
            double toleranceX = Math.Max(blockInfo.Width * 0.15, 5.0);
            double toleranceY = Math.Max(blockInfo.Height * 0.15, 5.0);

            // 扩展判定区域
            bool insideX = point.X >= (blockInfo.MinX - toleranceX) &&
                          point.X <= (blockInfo.MaxX + toleranceX);
            bool insideY = point.Y >= (blockInfo.MinY - toleranceY) &&
                          point.Y <= (blockInfo.MaxY + toleranceY);

            // 调试输出（如果发现特定连接连不上，取消注释这行来查看距离）

// ===== Excerpt lines 1780-1885 =====
            Console.WriteLine($"成功建立连接的块的数量: {visited.Count}");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBlockDetails(listBox1.SelectedItem as BlockReferenceInfo);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 from2 = new Form2();
            from2.ShowDialog();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 from4 = new Form4();
            from4.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string codesource = "c.txt";
            stfunblocks = ParseBlocks(codesource);
            GetSourceData("Sample.dwg");
            Form3 from3 = new Form3();
            from3.stList = stfunblocks;
            from3.ShowDialog();
            

        }

        // 添加UpdateProgressBar方法
        private void UpdateProgressBar(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        // 添加配置AI服务的UI方法
        private void btnConfigureAI_Click(object sender, EventArgs e)
        {
            using (var configForm = new Form())
            {
                configForm.Text = "AI模型配置";
                configForm.Size = new Size(400, 200);

                var lblUrl = new Label { Text = "AI服务URL:", Location = new Point(20, 20), Size = new Size(100, 25) };
                var txtUrl = new TextBox { Text = _aiModelUrl, Location = new Point(120, 20), Size = new Size(250, 25) };

                var lblSize = new Label { Text = "截图尺寸:", Location = new Point(20, 60), Size = new Size(100, 25) };
                var numSize = new NumericUpDown
                {
                    Value = (decimal)_captureSize,
                    Minimum = 10,
                    Maximum = 500,
                    Location = new Point(120, 60),
                    Size = new Size(100, 25)
                };

                var btnSave = new Button { Text = "保存", Location = new Point(150, 100), Size = new Size(80, 30) };

                btnSave.Click += (s, ev) =>
                {
                    _aiModelUrl = txtUrl.Text;
                    _captureSize = (double)numSize.Value;
                    configForm.DialogResult = DialogResult.OK;
                };

                configForm.Controls.AddRange(new Control[] { lblUrl, txtUrl, lblSize, numSize, btnSave });

                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("AI配置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // 1. 添加数据管理类，避免静态变量
        public class DataManager
        {
            public List<SourceBlock> SourceBlocks { get; set; } = new List<SourceBlock>();
            public Dictionary<ObjectId, LineStruct> ConnectionsP { get; set; } = new Dictionary<ObjectId, LineStruct>();
            public Dictionary<ObjectId, BlockStruct> ConnectionsB { get; set; } = new Dictionary<ObjectId, BlockStruct>();
            // 添加其他数据集合...
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            UpdateBlockDetails(listBox1.SelectedItem as BlockReferenceInfo);
        }



        // 或者添加一个方法打开Form5
