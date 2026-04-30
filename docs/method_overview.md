# Method Overview

This companion package reflects two connected parts of the full work:

1. A DWG/SAMA engineering tool that extracts graphical blocks, lines, and topology information from control logic drawings.
2. A supplementary detector named `FCR-DETR` for unresolved local regions in engineering drawings.

## FCR-DETR part

- Baseline configuration: `rtdetr-r18-baseline.yaml`
- Selected improved configuration: `rtdetr-FCR.yaml`
- Key code modules: `head.py`, `block.py`, `conv.py`
- Core manuscript claims:
  - `mAP50 = 97.7%`
  - `18.7%` parameter reduction versus strong baselines mentioned in the paper
  - `28.4%` computational-cost reduction mentioned in the paper

## Engineering part

- Main workflow excerpt: `Form1_core_excerpt.cs`
- Shared data structures: `SumStruct.cs`
- Code generation utilities: `CodeGeneration/`

## Included figures

- `system_pipeline.jpg`: overall process framework diagram from the manuscript source
- `ui_demo.png`: main interface figure from the manuscript source
