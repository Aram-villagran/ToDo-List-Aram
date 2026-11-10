namespace todoExample.domain.DTOs
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Enabled { get; set; }
    }
}