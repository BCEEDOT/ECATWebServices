using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.User;
using Ecat.Data.Models.Designer;

namespace Ecat.Data.Models.Cognitive
{
    public class CogEtmpreResult
    {
        public int PersonId { get; set; }
        public int InstrumentId { get; set; }
        public int Attempt { get; set; }
        public int Creator { get; set; }
        public int Advancer { get; set; }
        public int Refiner { get; set; }
        public int Executor { get; set; }

        public Person Person { get; set; }
        public CogInstrument Instrument { get; set; }
    }
}
