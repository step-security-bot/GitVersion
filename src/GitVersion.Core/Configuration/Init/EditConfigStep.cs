using GitVersion.Configurations.Init.BuildServer;
using GitVersion.Configurations.Init.SetConfig;
using GitVersion.Configurations.Init.Wizard;
using GitVersion.Logging;

namespace GitVersion.Configurations.Init;

public class EditConfigStep : ConfigInitWizardStep
{
    public EditConfigStep(IConsole console, IFileSystem fileSystem, ILog log, IConfigInitStepFactory stepFactory) : base(console, fileSystem, log, stepFactory)
    {
    }

    protected override StepResult HandleResult(string? result, Queue<ConfigInitWizardStep> steps, Model.Configurations.Configuration configuration, string workingDirectory)
    {
        switch (result)
        {
            case "0":
                return StepResult.SaveAndExit();
            case "1":
                return StepResult.ExitWithoutSaving();

            case "2":
                steps.Enqueue(this.StepFactory.CreateStep<PickBranchingStrategyStep>());
                return StepResult.Ok();

            case "3":
                steps.Enqueue(this.StepFactory.CreateStep<SetNextVersion>());
                return StepResult.Ok();

            case "4":
                steps.Enqueue(this.StepFactory.CreateStep<ConfigureBranches>());
                return StepResult.Ok();
            case "5":
                var editConfigStep = this.StepFactory.CreateStep<EditConfigStep>();
                steps.Enqueue(this.StepFactory.CreateStep<GlobalModeSetting>().WithData(editConfigStep, false));
                return StepResult.Ok();
            case "6":
                steps.Enqueue(this.StepFactory.CreateStep<AssemblyVersioningSchemeSetting>());
                return StepResult.Ok();
            case "7":
                steps.Enqueue(this.StepFactory.CreateStep<SetupBuildScripts>());
                return StepResult.Ok();
        }
        return StepResult.InvalidResponseSelected();
    }

    protected override string GetPrompt(Model.Configurations.Configuration configuration, string workingDirectory) => $@"Which would you like to change?

0) Save changes and exit
1) Exit without saving

2) Run getting started wizard

3) Set next version number
4) Branch specific configuration
5) Branch Increment mode (per commit/after tag) (Current: {configuration.VersioningMode})
6) Assembly versioning scheme (Current: {configuration.AssemblyVersioningScheme})
7) Setup build scripts";

    protected override string? DefaultResult => null;
}
