namespace Shared.DTOs
{
    public class TodoReadDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public string? Description { get; set; }

        public TodoReadDto(int id, int ownerId, string title, bool isCompleted, string? description)
        {
            Id = id;
            OwnerId = ownerId;
            Title = title;
            IsCompleted = isCompleted;
            Description = description;
        }
    }
}
