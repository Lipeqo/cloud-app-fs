namespace CloudBackend.DTOs;

public class TaskUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
