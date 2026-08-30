namespace ShortVideoCutter.Exceptions;

public class VideoCutterException : Exception
{
    public override string Message => $"{GetType().Name}:{base.Message}";

    public VideoCutterException(string message):base(message) { }
}

public class VideoCutterDIException : VideoCutterException
{
    public VideoCutterDIException(string message) : base(message) { }
}

public class VideoCutterModuleException : VideoCutterException
{
    public VideoCutterModuleException(string message) : base(message) { }
}