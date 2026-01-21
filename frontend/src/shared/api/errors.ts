export type ApiError = {
  messages: ErrorMessage[];
  type: ErrorType;
};

export type ErrorMessage = {
  code: string;
  message: string;
  invalidField?: string | null;
};

export type ErrorType =
  | "VALIDATION"
  | "NOT_FOUND"
  | "CONFLICT"
  | "FAILURE"
  | "AUTHENTICATION"
  | "AUTHORIZATION";

export class EnvelopeError extends Error {
  public readonly apiError: ApiError;
  public readonly type: ErrorType;

  constructor(apiError: ApiError) {
    super(apiError.messages[0].message);

    this.name = "EnvelopeError";
    this.apiError = apiError;
    this.type = apiError.type;

    Object.setPrototypeOf(this, EnvelopeError.prototype);
  }

  get messages(): ErrorMessage[] {
    return this.apiError.messages;
  }

  get firstMessage(): string {
    return this.apiError.messages[0].message;
  }

  getAllMessages(): string {
    return this.apiError.messages.map((m) => m.message).join("; ");
  }
}
