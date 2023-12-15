using CommandLine;

namespace FP.GitHubActions.DemoApp.Client.Options;

public class ServiceOptions
{
    [Option('u',"url",  Required = true, HelpText = "Url of the demo app service")]
    public Uri? ServiceUrl { get; set;  }
    

}