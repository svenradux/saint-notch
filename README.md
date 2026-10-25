# Saint Notch — Concept C (Saint Signature)

Windows Desktop companion adhering to the Saint Signature design language.

## Design Concept C Features
- **S0 (Continuous Idle Pill)**: Top-center continuous presence (~140 × 38 DIP), deep obsidian glass, silver fleur-de-lis (⚜) vector emblem with gentle breathing glow (~3.6s cycle).
- **S1 (Horizontal Compact Media)**: Compact media card (~380 × 56 DIP) displaying vinyl album thumbnail, track metadata ("Saint Signature"), and transport controls.
- **S2 (Expanded Downward Panel)**: Detailed interaction panel (~620 × 290 DIP) with large vinyl artwork, scrub bar, full transport, volume slider, and subtle watermark fleur-de-lis.
- **Window Mechanics**: Top-center screen anchored (`Top = 0`), per-pixel non-client transparency (clicks pass through outside the notch), topmost floating window, Per-Monitor V2 DPI awareness.
- **Demo State Switcher**: Interactive switcher bar + keyboard shortcuts (`0`, `1`, `2`, `Space` for play/pause, `Tab` to cycle, `Esc` to collapse/exit, `Ctrl+Q` to quit).

## Building & Running

Requirements: .NET 10 Desktop SDK (or .NET 8/9/10 Windows Desktop)

```powershell
dotnet build SaintNotch.csproj
dotnet run --project SaintNotch.csproj
```
