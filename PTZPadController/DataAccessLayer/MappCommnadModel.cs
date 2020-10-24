using PTZPadController.BusinessLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PTZPadController.DataAccessLayer
{
    public class MappCommnadModel
    {
        public GamePadHandlerCommands PTZCommand { get; set; }
        public HIDGamePadCommands[] GamePadCommand { get; set; }
    }
}