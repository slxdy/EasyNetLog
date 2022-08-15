# EasyNetLog
EasyNetLog is a simple logging solution for any .NET project.

## Usage
To create a new logger instance, use the main constructor of `EasyNetLogger`.<br>
The constructor has 4 parameters:
- `logFormat` Allows you to set a post-processor for logs.
- `includeConsoleStream` Will force the logger to write logs to a console.
- `files` An array of file paths that the logger will write to.
- `customLogStreams` An array of TextWriter streams that the logger will write to.

## Formatting
EasyNetLog has a simple XML formatting system that allows you to add some colors to your logs.<br>
The color formatter supports known color names and HEX color codes.<br>
Example:
```cs
"<color=red>I'm red!</color>"
```
Sometimes you have to log user input, but don't want it to be formatted.<br>
You can ignore text formats using `<ignore>`.<br>
Example:
```cs
"<ignore><color=red>I'm not red!</color></ignore>"
```

## EasyNetLog Example Usage
```cs
var logger = new EasyNetLogger((x) => $"[<color=magenta>{DateTime.Now:HH:mm:ss.fff}</color>] <color=gray>{x}</color>", true, new string[]
{
    @"test.log"
}, null);

for (; ; )
{
    logger.Log("A cool <color=#e63d6d>fuity</color> <color=#94ff2a>log</color> :)");
    Thread.Sleep(1000);
}
```
![image](https://user-images.githubusercontent.com/61495410/184698757-8573c633-9c81-4541-bfc2-52afbfdf891f.png)
