# Results Summary

This file summarizes the detector and system-level results reported in the manuscript source included in this workspace. It is intended to mirror the paper-facing numbers rather than raw training logs.

## Proposed detector

- Model name in the manuscript: `FCR-DETR`
- Role in the full system: supplementary detector for unresolved local regions during engineering drawing parsing
- Reported detector result: `mAP50 = 97.7%`
- Reported efficiency gain over strong baselines: `18.7%` fewer parameters and `28.4%` lower computational cost

## Ablation study

| ID | FasterNet | CGRFPN | MPDIoU | mAP50 (%) | Params (M) | GFLOPs |
| --- | --- | --- | --- | ---: | ---: | ---: |
| E1 | No | No | No | 82.6 | 19.8 | 57.0 |
| E2 | No | No | Yes | 85.2 | 19.8 | 57.0 |
| E3 | No | Yes | No | 92.2 | 19.2 | 48.2 |
| E4 | Yes | No | No | 91.8 | 16.8 | 49.5 |
| E5 | Yes | Yes | Yes | 97.7 | 16.1 | 40.8 |

Interpretation in the manuscript:

- `MPDIoU` improves localization without increasing complexity.
- `CGRFPN` gives the largest single-module accuracy improvement and also reduces cost.
- `FasterNet` improves efficiency while keeping strong detection performance.
- The full `FCR-DETR` combination gives the best balance of accuracy and efficiency.

## Comparison with real-time detectors

| Model | Backbone | Neck | P | R | mAP50 | Params (M) | GFLOPs |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |
| YOLOv8-N | - | - | 82.6 | 97.2 | 95.1 | 3.2 | 8.7 |
| YOLOv8-S | - | - | 84.1 | 91.6 | 94.8 | 11.1 | 25.5 |
| YOLOv8-M | - | - | 81.4 | 97.1 | 94.3 | 25.8 | 78.7 |
| YOLOv8-L | - | - | 85.8 | 97.3 | 97.6 | 43.7 | 165.2 |
| YOLOv8-X | - | - | 87.2 | 96.8 | 97.5 | 68.2 | 257.8 |
| YOLOv12-S | - | - | 78.1 | 98.4 | 94.1 | 2.56 | 6.3 |
| YOLOv10-S | - | - | 81.3 | 86.3 | 94.6 | 7.2 | 21.4 |
| YOLOv10-M | - | - | 87.0 | 90.8 | 96.8 | 15.3 | 58.9 |
| YOLOv10-L | - | - | 85.6 | 87.6 | 93.1 | 24.3 | 120.0 |
| RT-DETR-R18 | - | - | 81.3 | 84.5 | 92.1 | 19.9 | 57.0 |
| RT-DETR-R50 | - | - | 83.7 | 86.2 | 93.8 | 37.8 | 108.5 |
| D-FINE | - | - | 70.7 | 97.1 | 96.2 | 30.7 | 91.7 |
| RF-DETR | - | - | 70.8 | 77.0 | 96.3 | 27.1 | 38.9 |
| LW-DETR-S | - | - | 66.2 | 71.6 | 95.0 | 14.5 | 39.5 |
| LW-DETR-M | - | - | 66.8 | 75.3 | 93.8 | 28.2 | 65.6 |
| Ours | FasterNet | CGRFPN | 94.5 | 90.7 | 97.7 | 16.1 | 40.8 |

Highlights emphasized by the manuscript:

- `FCR-DETR` exceeds `RT-DETR-R50` by `3.9` points in `mAP50`.
- `FCR-DETR` slightly exceeds `YOLOv8-L` in `mAP50` while using far fewer parameters and FLOPs.
- `FCR-DETR` reaches the best reported precision in the comparison table.

## Detection on real engineering drawings

| Test sheet | Devices | Detected | Correct | Accuracy | Recall |
| --- | ---: | ---: | ---: | ---: | ---: |
| T1 | 45 | 42 | 40 | 95.23% | 88.89% |
| T2 | 123 | 118 | 116 | 98.31% | 94.31% |
| T3 | 145 | 142 | 141 | 99.26% | 97.24% |
| T4 | 78 | 74 | 73 | 98.65% | 93.59% |

## End-to-end code generation evaluation

| Method | Parsing Accuracy (%) | Connection Accuracy (%) | Generation Accuracy (%) | Time (min) | Time Reduction (%) |
| --- | ---: | ---: | ---: | ---: | ---: |
| Manual design (MD) | - | - | - | 38.0 | - |
| Baseline pipeline (BP) | 91.4 | 89.5 | 87.0 | 1.3 | 96.6 |
| Hybrid method (HM) | 96.3 | 94.2 | 92.7 | 4.2 | 88.9 |

## Included figures

- `qualitative_examples/FCR_model.jpg`: architecture figure used in the manuscript for `FCR-DETR`
- `qualitative_examples/detection_results.jpg`: qualitative detection example from the manuscript

## Source note

These values were aligned to the manuscript source file:

- `From Pixels to Code Recursive Graphical Parsing and Image-Based Code Generation Method/manuscript.tex`
