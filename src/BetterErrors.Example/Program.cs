using BetterErrors;

GetTextCommaSeparated()
    .Switch(
        txtArr => txtArr.ToList().ForEach(x => Console.WriteLine(x)),
        err => Console.WriteLine($"err: {err.Message}")
    );

Result<string> ReadFile()
{
    try
    {
        return File.ReadAllText("file that cant exist");
    }
    catch (FileNotFoundException ex)
    {
        return new FileNotFoundError(ex.Message);
    }
}

Result<string[]> GetTextCommaSeparated() => ReadFile().Map<string[]>(txt => 
{
    var strArr = txt.Split(",");
    if (strArr.Length < 1)
    {
        return strArr;
    }
    return new Error("No commas found in the file", "NoComma");
});

public record FileNotFoundError(string Message) : Error(Message, "FileNotFound");
