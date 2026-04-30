# SAMA-to-XML Workflow

The engineering application follows this high-level flow:

1. Load a DWG drawing.
2. Extract blocks, lines, polylines, hatch objects, and text candidates.
3. Build block-line connection relations and derive simplified topology.
4. Present detected blocks in the UI and expose block attributes on selection.
5. Convert selected topology or detected blocks into code-generation input objects.
6. Generate structured output code through the `CodeGeneration/` helpers.

## Included source

- `../core/Form1_core_excerpt.cs`
- `../core/SumStruct.cs`
- `../core/CodeGeneration/`

## Missing asset

- No existing GUI screenshot source file was found for `gui_screenshot.png`
