namespace ChatClient.Models
{
    public class MessageModel
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{Sender}: {Text}";
        }
    }
}
