namespace GraphicsBackend.Notifications;

public class SocketMessage
{
    public SocketMessage(SocketMessageType type, ActionTaken action, object message)
    {
        Type = type.ToString();
        Action = action.ToString();
        Message = message;
    }

    public string Type { get; set; }
    public string Action { get; set; }
    public object Message { get; set; }
}
