namespace DevQuestions.Contracts;

public record CreateQuestionDto(
    string Title,
    string Body,
    Guid UserId,
    Guid[] TagIds);