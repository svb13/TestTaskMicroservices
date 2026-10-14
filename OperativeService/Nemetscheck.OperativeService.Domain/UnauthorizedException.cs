namespace Nemetscheck.OperativeService.Domain;

public class UnauthorizedException(string message) : Exception(message);
