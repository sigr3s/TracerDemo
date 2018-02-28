

namespace TracerDemo.Model
{
    public class TeamTracerPlayer
    {
        public string TracerPlayerId { get; set; }
        public TracerPlayer TracerPlayer { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }

        public InGameRole Position {get; set;}
    }


    public enum InGameRole{
        top, 
        jungle,
        mid,
        adc,
        suppot
    }
}