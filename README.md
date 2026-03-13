# PrintTool

Sends a screenshot of the active monitor directly to the default printer — no dialogs, no clicks.

Designed for use with Siri Operator Workplace (SCADA/HMI), where the built-in print function is unavailable.

## How it works

1. Detects which monitor the cursor is on
2. Captures the full screen using GDI BitBlt (supports hardware-accelerated content)
3. Scales the image to fit the page with correct margins
4. Prints to the default printer automatically

## Usage

Bind `PrintTool.exe` to a keyboard shortcut or button. When triggered, it captures and prints the screen the cursor is currently on — no interaction required.

## Requirements

- Windows
- .NET 10 (Windows)
- A configured default printer

## Build

```
dotnet publish -c Release -r win-x64 --self-contained
```
