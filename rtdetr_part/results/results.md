# RT-DETR训练结果

FASTER-RCNN

![image-20250211111520189](https://mzy777.oss-cn-hangzhou.aliyuncs.com/img/image-20250211111520189.png)



>```
>==============================
>Use size divisor set input shape from (640, 640) to (800, 800)
>==============================
>Compute type: dataloader: load a picture from the dataset
>Input shape: (800, 800)
>Flops: 0.134T
>Params: 41.379M
>==============================
>!!!Please be cautious if you use the results in papers. You may need to check if all ops are supported and verify that the flops computation is correct.
>```
>
>```
>root@autodl-container-9b4c40bda6-ace5d050:~/mmdetection#  python tools/analysis_tools/benchmark.py /root/mmdetection/configs/faster_rcnn/fast-rcnn_r50_fpn_1x_PIDV5.py --checkpoint /root/mmdetection/work_dirs/fast-rcnn_r50_fpn_1x_PIDV5/epoch_12.pth --task inference --fuse-conv-bn
>02/11 11:20:41 - mmengine - INFO - before build: 
>02/11 11:20:41 - mmengine - INFO - (GB) mem_used: 49.91 | uss: 0.21 | pss: 0.30 | total_proc: 1
>Loads checkpoint by local backend from path: /root/mmdetection/work_dirs/fast-rcnn_r50_fpn_1x_PIDV5/epoch_12.pth
>loading annotations into memory...
>Done (t=0.00s)
>creating index...
>index created!
>02/11 11:20:47 - mmengine - INFO - after build: 
>02/11 11:20:47 - mmengine - INFO - (GB) mem_used: 54.43 | uss: 4.72 | pss: 5.15 | total_proc: 1
>02/11 11:20:47 - mmengine - INFO - ============== Done ==================
>```
>
>

# dtmdet

> ```
> DONE (t=0.05s).
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area=   all | maxDets=100 ] = 0.519
>  Average Precision  (AP) @[ IoU=0.50      | area=   all | maxDets=100 ] = 0.869
>  Average Precision  (AP) @[ IoU=0.75      | area=   all | maxDets=100 ] = 0.527
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area= small | maxDets=100 ] = -1.000
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area=medium | maxDets=100 ] = 0.537
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area= large | maxDets=100 ] = 0.517
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets=  1 ] = 0.520
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets= 10 ] = 0.627
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets=100 ] = 0.644
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area= small | maxDets=100 ] = -1.000
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=medium | maxDets=100 ] = 0.647
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area= large | maxDets=100 ] = 0.639
> 02/11 13:42:13 - mmengine - INFO - bbox_mAP_copypaste: 0.519 0.869 0.527 -1.000 0.537 0.517
> 02/11 13:42:13 - mmengine - INFO - Epoch(val) [100][3/3]    coco/bbox_mAP: 0.5190  coco/bbox_mAP_50: 0.8690  coco/bbox_mAP_75: 0.5270  coco/bbox_mAP_s: -1.0000  coco/bbox_mAP_m: 0.5370  coco/bbox_mAP_l: 0.5170  data_time: 0.0444  time: 0.2966
> ```
>
> ```
> Compute type: dataloader: load a picture from the dataset
> Input shape: (640, 640)
> Flops: 80.121G
> Params: 52.316M
> ==============================
> ```
>
> ```
> 02/11 14:12:01 - mmengine - INFO - before build: 
> 02/11 14:12:01 - mmengine - INFO - (GB) mem_used: 43.28 | uss: 0.40 | pss: 0.40 | total_proc: 1
> Loads checkpoint by local backend from path: /root/mmdetection/work_dirs/rtmdet_l_swin_b_4xb32-100e_PID/epoch_100.pth
> loading annotations into memory...
> Done (t=0.00s)
> creating index...
> index created!
> 02/11 14:12:07 - mmengine - INFO - after build: 
> 02/11 14:12:07 - mmengine - INFO - (GB) mem_used: 47.89 | uss: 5.73 | pss: 5.74 | total_proc: 1
> ```
>
> 

# gfl

> ```
> Average Precision  (AP) @[ IoU=0.50:0.95 | area=   all | maxDets=100 ] = 0.556
>  Average Precision  (AP) @[ IoU=0.50      | area=   all | maxDets=1000 ] = 0.962
>  Average Precision  (AP) @[ IoU=0.75      | area=   all | maxDets=1000 ] = 0.629
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area= small | maxDets=1000 ] = -1.000
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area=medium | maxDets=1000 ] = 0.552
>  Average Precision  (AP) @[ IoU=0.50:0.95 | area= large | maxDets=1000 ] = 0.686
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets=100 ] = 0.644
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets=300 ] = 0.644
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=   all | maxDets=1000 ] = 0.644
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area= small | maxDets=1000 ] = -1.000
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area=medium | maxDets=1000 ] = 0.639
>  Average Recall     (AR) @[ IoU=0.50:0.95 | area= large | maxDets=1000 ] = 0.733
> 02/11 19:31:16 - mmengine - INFO - bbox_mAP_copypaste: 0.556 0.962 0.629 -1.000 0.552 0.686
> 02/11 19:31:16 - mmengine - INFO - Epoch(val) [12][3/3]    coco/bbox_mAP: 0.5560  coco/bbox_mAP_50: 0.9620  coco/bbox_mAP_75: 0.6290  coco/bbox_mAP_s: -1.0000  coco/bbox_mAP_m: 0.5520  coco/bbox_mAP_l: 0.6860  data_time: 0.0446  time: 0.2311
> ```
>
> ```
> Input shape: (800, 800)
> Flops: 0.13T
> Params: 32.441M
> ```
>
> ```
> 02/11 19:35:29 - mmengine - INFO - after build: 
> 02/11 19:35:29 - mmengine - INFO - (GB) mem_used: 46.89 | uss: 4.76 | pss: 5.07 | total_proc: 1
> 02/11 19:35:29 - mmengine - INFO - ============== Done ==================
> ```
>
> 
