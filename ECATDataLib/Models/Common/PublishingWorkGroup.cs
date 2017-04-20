using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.Student;

namespace Ecat.Data.Models.Common
{
    public class PubWg
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int CountInventory { get; set; }
        public int? InstrumentId { get; set; }
        public decimal WgSpTopStrat { get; set; }
        public decimal WgFacTopStrat { get; set; }
        public decimal StratDivisor { get; set; }
        public IEnumerable<PubWgMember> PubWgMembers { get; set; }
    }

    public class PubWgMember
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public int SpResponseTotalScore { get; set; }
        public int FacStratPosition { get; set; }
        public int SelfStratPosition { get; set; }
        public bool HasSpResult { get; set; }
        public bool HasStratResult { get; set; }
        public int CountSpResponses { get; set; }
        public StratResult StratResult { get; set; }
        public PubWgBreakOut BreakOut { get; set; }
        public IEnumerable<int> PeersDidNotAssessMe { get; set; }
        public IEnumerable<int> PeersIdidNotAssess { get; set; }
        public IEnumerable<int> PeersDidNotStratMe { get; set; }
        public IEnumerable<int> PeersIdidNotStrat { get; set; }
        public IEnumerable<PubStratResponse> PubStratResponses { get; set; }
        public IEnumerable<PubWgStratTable> StratTable { get; set; }
    }

    public class PubStratResponse
    {
        public int AssesseeId { get; set; }
        public int StratPosition { get; set; }
    }

    public class PubWgStratTable
    {
        public int Position { get; set; }
        public int Count { get; set; }
    }

    public class PubWgBreakOut
    {
        public int NotDisplayed { get; set; }
        public int IneffA { get; set; }
        public int IneffU { get; set; }
        public int EffA { get; set; }
        public int EffU { get; set; }
        public int HighEffU { get; set; }
        public int HighEffA { get; set; }
    }
}
