using Games.Domain.Types;

namespace Games.Domain.Errors;

public sealed record NoPathFound(HexCoordinate From, HexCoordinate To);