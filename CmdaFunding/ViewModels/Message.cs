
namespace CmdaFunding.ViewModels
{
    public class Message
    {
        public string message { get; set; }

        public static implicit operator Message(string v)
        {
            throw new NotImplementedException();
        }
    }
}
