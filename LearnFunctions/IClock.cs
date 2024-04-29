using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnFunctions
{
    public interface IClock
    {
        DateTime GetNow();
    }

    public class SystemClock : IClock
    {
        public DateTime GetNow() => DateTime.Now;
    }
}
