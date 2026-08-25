namespace EngineeringModel.BuildingBlocks;

public sealed class ResourceNotFoundException(string message) : Exception(message);
