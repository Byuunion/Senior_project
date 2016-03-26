using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetMeet_Native_Portable
{
    public interface Updatable
    {
        //this just tells the poster that the classes which implement this should be able to be sent to the server
        string GetName();
    }
}
