# Method Overview

This companion package reflects two connected parts of the full work:

1. A DWG/SAMA engineering tool that extracts graphical blocks, lines, and topology information from control logic drawings.
2. An RT-DETR-based symbol detection branch used to study symbol recognition on PID/SAMA-style images.

## RT-DETR part

- Baseline configuration: `rtdetr-r18-baseline.yaml`
- Selected improved configuration: `rtdetr-Faster.yaml`
- Key code modules: `head.py`, `block.py`, `conv.py`

## Engineering part

- Main workflow excerpt: `Form1_core_excerpt.cs`
- Shared data structures: `SumStruct.cs`
- Code generation utilities: `CodeGeneration/`

## Included figures

- `system_pipeline.png`: copied from an existing RT-DETR structure figure in the workspace

## Missing figures

- No existing GUI screenshot source file was found for `ui_demo.png`
