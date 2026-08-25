namespace EngineeringModel.BuildingBlocks;

public sealed class BusinessRuleViolationException(string message) : Exception(message);
