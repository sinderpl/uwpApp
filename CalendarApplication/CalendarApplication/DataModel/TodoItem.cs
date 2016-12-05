using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CalendarApplication
{
    public class TodoItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "appointmentID")]
        public string appointmentID{ get; set; }

        [JsonProperty(PropertyName = "appointmentName")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "appointmentTime")]
        public String appointmentTime { get; set; }

        [JsonProperty(PropertyName = "appointmentTimeEnd")]
        public String appointmentTimeEnd { get; set; }

        [JsonProperty(PropertyName = "appointmentDate")]
        public String appointmentDate { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
}
