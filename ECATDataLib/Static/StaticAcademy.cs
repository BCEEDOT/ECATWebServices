using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecat.Data.Models.School;

namespace Ecat.Data.Static
{
    //TODO: Update CanvasAcctIds
    public static class StaticAcademy
    {

        public static Dictionary<string, Academy> AcadLookupById => new Dictionary<string, Academy>
        {
            {Afsncoa.Id, Afsncoa},
            {Keesler.Id, Keesler },
            {Ecat.Id, Ecat},
            {Ecatncoa.Id, Ecatncoa},
            {Ecatsncoa.Id, Ecatsncoa},
            {Peterson.Id, Peterson},
            {Kapaun.Id, Kapaun},
            {Kadena.Id, Kadena},
            {McGheeTyson.Id, McGheeTyson},
            {Lackland.Id, Lackland},
            {Tyndall.Id, Tyndall},
            {Sheppard.Id, Sheppard},
            {Elmendorf.Id, Elmendorf},
            {Hickam.Id, Hickam}
        };

        public static Dictionary<string, Academy> AcadLookupByCat => new Dictionary<string, Academy>
        {
            {Afsncoa.BbCategoryId, Afsncoa},
            {Keesler.BbCategoryId, Keesler },
            {Ecat.BbCategoryId, Ecat },
            {Ecatncoa.BbCategoryId, Ecatncoa},
            {Ecatsncoa.BbCategoryId, Ecatsncoa},
            {Peterson.BbCategoryId, Peterson},
            {Kapaun.BbCategoryId, Kapaun},
            {Kadena.BbCategoryId, Kadena},
            {McGheeTyson.BbCategoryId, McGheeTyson},
            {Lackland.BbCategoryId, Lackland},
            {Tyndall.BbCategoryId, Tyndall},
            {Sheppard.BbCategoryId, Sheppard},
            {Elmendorf.BbCategoryId, Elmendorf},
            {Hickam.BbCategoryId, Hickam}
        };

        public static Academy Afsncoa
         => new Academy
         {
             Id = "AFSNCOA",
             LongName = "Air Force Senior Noncommissioned Academy",
             ShortName = "AFSNCOA",
             MpEdLevel = MpEdLevel.Sncoa,
             Base = AcademyBase.Gunter,
             BbCategoryId = MpAcadCat.AFSNCOA,
             ParentBbCategoryId = MpAcadCat.BCEE,
             CanvasAcctId = 1
         };

        public static Academy Ecat
       => new Academy
       {
           Id = "ECAT",
           LongName = "ECAT Developer Academy",
           ShortName = "ECAT",
           MpEdLevel = MpEdLevel.None,
           Base = AcademyBase.Gunter,
           BbCategoryId = MpAcadCat.ECAT,
           ParentBbCategoryId = MpAcadCat.BCEE,
           CanvasAcctId = 1
       };

        public static Academy Ecatncoa
       => new Academy
       {
           Id = "ECAT ILE",
           LongName = "ECAT Developer NCOA",
           ShortName = "ECAT ILE",
           MpEdLevel = MpEdLevel.Ncoa,
           Base = AcademyBase.Gunter,
           BbCategoryId = MpAcadCat.ECATILE,
           ParentBbCategoryId = MpAcadCat.NCOA,
           CanvasAcctId = 1
       };

        public static Academy Ecatsncoa
       => new Academy
       {
           Id = "ECAT ALE",
           LongName = "ECAT Developer SNCOA",
           ShortName = "ECAT ALE",
           MpEdLevel = MpEdLevel.Sncoa,
           Base = AcademyBase.Gunter,
           BbCategoryId = MpAcadCat.ECATALE,
           ParentBbCategoryId = MpAcadCat.BCEE,
           CanvasAcctId = 1
       };

        public static Academy Keesler
        => new Academy
        {
            Id = "KENCOA",
            LongName = "Keesler Noncommissioned Officer Academy",
            ShortName = "Mathies NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Gunter,
            BbCategoryId = MpAcadCat.Keesler,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Peterson
        => new Academy
        {
            Id = "PENCOA",
            LongName = "Peterson Noncommissioned Officer Academy",
            ShortName = "Vosler NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Peterson,
            BbCategoryId = MpAcadCat.Peterson,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Kapaun
        => new Academy
        {
            Id = "KINCOA",
            LongName = "Kapaun Noncommissioned Officer Academy",
            ShortName = "Kisling NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Kapaun,
            BbCategoryId = MpAcadCat.Kapaun,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Kadena
        => new Academy
        {
            Id = "KANCOA",
            LongName = "Kadena Noncommissioned Officer Academy",
            ShortName = "Kadena NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Kadena,
            BbCategoryId = MpAcadCat.Kadena,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Hickam
        => new Academy
        {
            Id = "HINCOA",
            LongName = "Hickam Noncommissioned Officer Academy",
            ShortName = "Hickam NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Hickam,
            BbCategoryId = MpAcadCat.Hickam,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Elmendorf
        => new Academy
        {
            Id = "ELNCOA",
            LongName = "Elmendorf Noncommissioned Officer Academy",
            ShortName = "Elmendorf NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Elmendorf,
            BbCategoryId = MpAcadCat.Elmendorf,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy McGheeTyson
        => new Academy
        {
            Id = "MTNCOA",
            LongName = "McGhee Tyson Noncommissioned Officer Academy",
            ShortName = "McGhee Tyson NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.McGheeTyson,
            BbCategoryId = MpAcadCat.McGheeTyson,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Sheppard
        => new Academy
        {
            Id = "SHNCOA",
            LongName = "Sheppard Noncommissioned Officer Academy",
            ShortName = "Sheppard NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Sheppard,
            BbCategoryId = MpAcadCat.Sheppard,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Tyndall
        => new Academy
        {
            Id = "TYNCOA",
            LongName = "Tyndall Noncommissioned Officer Academy",
            ShortName = "Tyndall NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Tyndall,
            BbCategoryId = MpAcadCat.Tyndall,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };

        public static Academy Lackland
        => new Academy
        {
            Id = "LANCOA",
            LongName = "Lackland Noncommissioned Officer Academy",
            ShortName = "Lackland NCOA",
            MpEdLevel = MpEdLevel.Ncoa,
            Base = AcademyBase.Lackland,
            BbCategoryId = MpAcadCat.Lackland,
            ParentBbCategoryId = MpAcadCat.NCOA,
            CanvasAcctId = 1
        };
    }
}
