namespace DevQuestions.Contracts;

public record AddAnswerDto(
    Guid UserId,
    string Text,
    string Body,
    Guid[] TagIds);