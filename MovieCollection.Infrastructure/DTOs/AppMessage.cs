namespace MovieCollection.Infrastructure.DTOs
{
    public class AppMessage
    {
        public AppMessage(string code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}
