using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtaitePlayer.Classes.Utils
{
    public class YesOrNoDialogInfo
    {
        // Dailog title
        public string title { get; set; }
        // Dailog message
        public string message { get; set; }
        // Dailog button 1 text
        public string button1Title { get; set; }
        // Dailog button 2 text
        public string button2Title { get; set; }
        // Dailog button 1 event
        public Action button1Event { get; set; }
        // Dailog button 2 event
        public Action button2Event { get; set; }
    }
}
