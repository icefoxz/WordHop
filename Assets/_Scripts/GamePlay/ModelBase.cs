public class ModelBase
{
    protected void SendEvent(string eventName,params object[] data)
    {
        // 通过事件中心发送事件
        Game.MessagingManager.SendParams(eventName, data);
    }
}