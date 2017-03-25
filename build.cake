var mainTemplate = @"
## Try Web Programming

{{links}}
";

var readmeTemplate = @"
## {{fileName}}

```fsharp
{{source}}
```
";

var haskellTemplate = @"
## {{fileName}}

```haskell
{{source}}
```
";

Task("Build-Readme").Does(() => {
    var links = new List<string>();
    
    var files = new System.IO.DirectoryInfo("./")
        .GetFiles("*.*s*", System.IO.SearchOption.AllDirectories)
        .GroupBy(x => x.Directory.FullName)
        .Select(x => x.FirstOrDefault())
        .Where(x => !x.FullName.Contains("node_module"))
        .Where(x => x.Extension == ".fsx" || x.Extension == ".csx" || x.Extension == ".hs" || x.Extension == ".ts")
        .ToList();

    files.ForEach(file => {
        Console.WriteLine(file.FullName);
        var text = file.FullName.EndsWith(".hs") ? haskellTemplate : readmeTemplate; 
        var source = System.IO.File.ReadAllText(file.FullName);
        var name = file.Name;
        var newText = text
            .Replace("{{fileName}}", name)
            .Replace("{{source}}", source);

        var mdFile = System.IO.Path.Combine(file.Directory.FullName, "README.md");
        System.IO.File.WriteAllText(mdFile, newText);

        var dirName = file.Directory.Name;
        var parent = file.Directory.Parent.Name;
        var link = $"- [{parent} ⌁ {dirName}]({parent}/{dirName})";
        links.Add(link);
    });

    var mainText = mainTemplate.Replace("{{links}}", String.Join("\n", links));
    System.IO.File.WriteAllText("README.md", mainText);
});

var target = Argument("target", "default");
RunTarget(target);