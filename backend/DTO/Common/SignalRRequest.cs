

namespace backend.DTO.Common;

public record SignalRRequest<T>(
     string Type,
     string CorrelationId,
     T? Payload
);
public record SignalRRequest(
     string Type,
     string CorrelationId
);