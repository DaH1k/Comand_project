namespace ChatServer.Models;

public class MessageDto
{
    public string Type { get; set; }
    public string Sender { get; set; }
    public string Text { get; set; }
}