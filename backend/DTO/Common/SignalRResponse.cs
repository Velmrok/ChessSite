

namespace backend.DTO.Common;

public record SignalRResponse<T>(
     string Type,
     string CorrelationId,
     T? Data,
     SignalRError? Error
);
public record SignalRResponse(
    string Type,
    string CorrelationId,
    SignalRError? Error
);