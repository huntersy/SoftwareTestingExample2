using System.Text.Json;using NUnit.Framework;
namespace SoftwareTestingExample.Utilities;
public static class TestLogger
{
 public static void Log(string eventName,string level="Information",object? properties=null,Exception? exception=null){var t=TestContext.CurrentContext.Test;TestContext.Progress.WriteLine(JsonSerializer.Serialize(new{timestamp=DateTimeOffset.UtcNow,level,eventName,test=new{className=t.ClassName,name=t.Name},properties,exception=exception is null?null:new{type=exception.GetType().FullName,exception.Message,exception.StackTrace}}));}
}