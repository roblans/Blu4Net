![](https://dev.azure.com/roblans/Blu4Net/_apis/build/status/Blu4Net)

# Blu4Net
Blu4Net is a .NET library that interfaces with BluOS players (Bluesound, NAD). It uses an event-driven (RX), non-blocking model (Async/Await) that makes it lightweight and efficient.

Supported Targets:

- .NET Standard: 2.0
- .NET Standard: 2.1

## Features

### Basic operations

- Play
- Pause
- Stop
- Skip
- Previous
- Back
- Shuffle (on | off)
- Repeat (off | one | all)
- Volume (percentage)
- Position (elapsed time + length of current track) 

### Media information

- Titles
- Image (artwork)
- Service icon

### Presets

- Fetch presets
- Load preset (number)

### Queue

- Info (name + length)
- Fetch songs (paged)

### Music sources (Library, TuneIn, ...)

- Fetch sources
- Browse hierarchy

### Notifications (Observable - ReactiveX)
- State changes (Playing, Paused, Streaming, ...)
- Mode changes (Shuffle, Repeat)
- Volume changes
- Position changes (note: not realtime)
- Media changes
- Preset changes
- Queue changes
