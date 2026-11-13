using CSharpFunctionalExtensions;
using Shared.SharedKernel;

namespace DirectoryService.MediaProcessing;

public class VideoProcess
{

    public VideoProcess(Guid videoAssetId)
    {
        Id = Guid.NewGuid();
        VideoAssetId = videoAssetId;
        Status = ProcessingStatus.IN_PROGRESS;
        ProcessingPercentage = 0;
        CreatedAt = DateTime.UtcNow;

        InitializeSteps();
    }

    private VideoProcess() { }

    private readonly List<ProcessingStep> _steps = [];

    public Guid Id { get; private set; }

    public Guid VideoAssetId { get; private set; }

    public ProcessingStatus Status { get; private set; }

    public int ProcessingPercentage { get; private set; }

    public string? ErrorMessage { get; private set; }

    public bool IsCriticalError { get; private set; }

    public int RetryCount { get; private set; }

    public DateTime NextRetryAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime CompletedAt { get; private set; }

    public IReadOnlyList<ProcessingStep> Steps => _steps.AsReadOnly();

    public ProcessingStep? CurrentStep => _steps.FirstOrDefault(s => s.Status == StepStatus.IN_PROGRESS);

    private void InitializeSteps()
    {
        int order = 1;
        foreach ((StepType stepType, int weight) in _stepWeights)
        {
            _steps.Add(new ProcessingStep(stepType, order++, weight));
        }
    }

    public Result<ProcessingStep?, Error> ProcessNextStep(ProcessingStep step)
    {
        if (Status == ProcessingStatus.IN_PROGRESS)
        {
            return Error.Failure(
                "processing.invalid.status",
                $"Cannot process step {step.Id}");
        }

        ProcessingStep? currentStep = CurrentStep;
        if (currentStep is null)
            return currentStep;

        ProcessingStep? nextStep = _steps
            .OrderBy(s => s.Order)
            .FirstOrDefault(s => s.Status == StepStatus.PENDING);

        if (nextStep is null)
        {
            Complete();
            return Result.Success<ProcessingStep?, Error>(null);
        }

        UnitResult<Error> startResult = nextStep.Start();
        if (startResult.IsFailure)
            return startResult.Error;

        return nextStep;
    }

    // 22:25
    
    private UnitResult<Error> Complete()
    {
        if(Status != ProcessingStatus.IN_PROGRESS)
           return Error.Validation("step.error.required", $"Can only fail step with IN_PROGRESS status, current: {Status}");

        bool allStepsCompleted = _steps.All(s => s.Status == StepStatus.COMPLETED);
        if (!allStepsCompleted)
            return Error.Validation("process.incomplete.steps", "Cannot complete processing when not all steps are completed");

        Status = ProcessingStatus.COMPLETED;
        CompletedAt = DateTime.UtcNow;
        ProcessingPercentage = 100;

        return UnitResult.Success<Error>();

    }

    private static readonly Dictionary<StepType, int> _stepWeights = new()
    {
        { StepType.INITIALIZE, 0 },
        { StepType.EXTRACT_METADATA, 10 },
        { StepType.GENERATE_HLS, 60 },
        { StepType.UPLOAD_HLS, 15 },
        { StepType.GENERATE_PREVIEW, 10 },
        { StepType.CLEANUP, 5 },
    };
}

public enum ProcessingStatus
{
    IN_PROGRESS,
    COMPLETED,
    FAILED
}