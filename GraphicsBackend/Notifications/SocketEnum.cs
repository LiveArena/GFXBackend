namespace GraphicsBackend.Notifications
{
    public enum SocketEnum
    {
        ready,
        cued,
        coming,
        onair,
        cuedoff,
        going
    }

    public enum SocketMessageType
    {
        None = 0,
        Project = 1,
        Graphic = 2,
        Theme = 3,
    }

    public enum ActionTaken
    {
        None = 0,
        ReadSingle = 1,
        ReadList = 2,
        Created = 3,
        Updated = 4,
        Deleted = 5
    }
}
