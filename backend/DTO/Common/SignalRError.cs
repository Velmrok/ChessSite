

namespace backend.DTO.Common;

public record SignalRError(
    string Title,
     string Message,
     object? Details 
);