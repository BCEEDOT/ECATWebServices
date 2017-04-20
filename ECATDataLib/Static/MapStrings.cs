using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Data.Static
{
    public static class MpStrings
    {
    }

    public static class MpSpItemScore
    {
        public const int Hea = 6,
            Heu = 5,
            Ea = 4,
            Eu = 3,
            Nd = 2,
            Ieu = 1,
            Iea = 0;
    }

    public static class MpSpResultScore
    {
        public const int He = 90,
            Aae = 80,
            E = 60,
            Bae = 40,
            Ie = 20;
    }

    public static class MpInstituteRoleId
    {
        public const string HqAdmin = "ECAT_01_HQ",
            Designer = "ECAT_02_Designer",
            CourseAdmin = "ECAT_03_CourseAdmin",
            Faculty = "ECAT_04_Faculty",
            Student = "ECAT_05_Student",
            External = "ECAT_06_External",
            RefOnly = "ECAT_07_RefOnly",
            Undefined = "Undefined";
    }

    public static class MpInstituteRoleName
    {
        public const string HqAdmin = "BCEE: ECAT 01 HQ",
            Designer = "BCEE: ECAT 02 Designer",
            CourseAdmin = "BCEE: ECAT 03 Course Admin",
            Facilitator = "BCEE: ECAT 04 Faculty",
            Student = "BCEE: ECAT 05 Student",
            External = "BCEE: ECAT 06 External",
            RefOnly = "BCEE: ECAT 07 Reference Only";
    }


    public static class MpRoleTransform
    {
        public static RoleMap InstituteRoleToEnum(string instituteRole)
        {
            switch (instituteRole)
            {
                case MpInstituteRoleId.HqAdmin:
                    return RoleMap.SysAdmin;
                case MpInstituteRoleId.Designer:
                    return RoleMap.Designer;
                case MpInstituteRoleId.CourseAdmin:
                    return RoleMap.Faculty;
                case MpInstituteRoleId.Faculty:
                    return RoleMap.Faculty;
                case MpInstituteRoleId.Student:
                    return RoleMap.Student;
                case MpInstituteRoleId.External:
                    return RoleMap.External;
                case MpInstituteRoleId.RefOnly:
                    return RoleMap.RefOnly;
                default:
                    return RoleMap.Unknown;
            }
        }

        public static string RoleNameToId(string roleName)
        {
            switch (roleName)
            {
                case MpInstituteRoleName.HqAdmin:
                    return MpInstituteRoleId.HqAdmin;
                case MpInstituteRoleName.CourseAdmin:
                    return MpInstituteRoleId.CourseAdmin;
                case MpInstituteRoleName.Designer:
                    return MpInstituteRoleId.Designer;
                case MpInstituteRoleName.Facilitator:
                    return MpInstituteRoleId.Faculty;
                case MpInstituteRoleName.Student:
                    return MpInstituteRoleId.Student;
                case MpInstituteRoleName.External:
                    return MpInstituteRoleId.External;
                case MpInstituteRoleName.RefOnly:
                    return MpInstituteRoleName.RefOnly;

                default:
                    return MpInstituteRoleId.Undefined;
            }
        }

        public static string BbWsRoleToEcat(string roleId)
        {
            switch (roleId)
            {
                case "BCEE_Course_Admin":
                case "BCEE_Flt_Instructor":
                case "P":
                    return MpInstituteRoleId.Faculty;
                case "S":
                    return MpInstituteRoleId.Student;
                default:
                    return MpInstituteRoleId.Undefined;
            }
        }
    }

    public static class MpGroupCategory
    {
        public const string Wg1 = "BC1",
              Wg2 = "BC2",
              Wg3 = "BC3",
              Wg4 = "BC4",
              None = "None";
    }


    public static class MpCommentFlag
    {
        public const string Pos = "Positive",
            Neg = "Negative",
            Neut = "Neutral",
            Appr = "Appropriate",
            Inappr = "Inappropriate";
    }

    public static class MpCourseRole
    {
        public const string Student = "Student",
            Faculty = "Faculty",
            CrseAdmin = "Course Admin",
            SysAdmin = "System Admin";
    }

    public static class MpEdLevel
    {
        public const string Cla = "Chief Leadership Academy",
              Sncoa = "Senior NCO Academy",
              Ncoa = "NCO Academy",
              Als = "Airman Leadership",
              Epmeic = "EPMEIC",
              None = "None";
    }

    public static class MpAssessResult
    {
        public const string He = "Highly Effectiveness",
            Aae = "Above Avg Effectiveness",
            E = "Effective",
            Bae = "Below Avg Effectiveness",
            Ie = "Ineffective";
    }

    public static class MpAffiliation
    {
        public const string Usaf = "Air Force",
            Usa = "Army",
            Uscg = "Coast Guard",
            Usn = "Navy",
            Usmc = "Marines",
            Fn = "Foreign National",
            None = "Unaffiliated",
            Unk = "Unknown";
    }

    public static class MpGender
    {
        public const string Unk = "Unknown",
            Other = "Other",
            Male = "Male",
            Female = "Female";
    }

    public static class MpComponent
    {
        public const string Active = "Active Duty",
               Reserve = "Reserves",
               Guard = "National Guard",
               None = "Unaffiliated",
               Unk = "Unknown";
    }

    public static class MpPaygrade
    {
        public const string E1 = "E1",
             E2 = "E2",
             E3 = "E3",
             E4 = "E4",
             E5 = "E5",
             E6 = "E6",
             E7 = "E7",
             E8 = "E8",
             E9 = "E9",
             Civ = "Civilian",
             Fn = "Foreign National",
             Unk = "Unknown";
    }

    public static class MpSpStatus
    {
        public const string Unk = "Uknown",
            Open = "Open",
            UnderReview = "Under Review",
            Published = "Published",
            Arch = "ArchiveE";
    }

    public static class MpSpItemResponse
    {
        public const string
            Iea = "IEA",
            Ieu = "IEU",
            Nd = "ND",
            Eu = "EU",
            Ea = "EA",
            Heu = "HEU",
            Hea = "HEA";
    }

    public static class MpAcadCat
    {
        public const string AFSNCOA = "_888_1",
            BCEE = "_206_1",
            CLC = "_892_1",
            ECAT = "_897_1",
            ECATILE = "_907_1",
            ECATALE = "_908_1",
            EPMEIC = "_894_1",
            NCOA = "_889_1",
            Keesler = "_890_1",
            Sheppard = "_893_1",
            Tyndall = "_891_1",
            Elmendorf = "_903_1",
            Hickam = "_904_1",
            Kadena = "_905_1",
            Kapaun = "_906_1",
            Lackland = "_900_1",
            Peterson = "_901_1",
            McGheeTyson = "_902_1";
    }

    public static class MpEntityError
    {
        public const string CrseNotOpen = "Course Not Open",
            WgNotOpen = "Wg Not Open";
    }
    public static class MpToken
    {
        public static string[] TokenMethods => new[] {
            "Context.WS:login",
            "Context.WS:loginTool",
            "Context.WS:emulateUser",
            "Context.WS:extendSessionLife",
            "Context.WS:getMemberships",
            "Context.WS:getMyMemberships",
            "Context.WS:initialize",
            "Course.WS:changeCourseBatchUid",
            "Course.WS:changeCourseCategoryBatchUid",
            "Course.WS:changeCourseDataSourceId",
            "Course.WS:changeOrgBatchUid",
            "Course.WS:changeOrgCategoryBatchUid",
            "Course.WS:changeOrgDataSourceId",
            "Course.WS:createCourse",
            "Course.WS:createOrg",
            "Course.WS:getAvailableGroupTools",
            "Course.WS:getCartridge",
            "Course.WS:getCategories",
            "Course.WS:getClassifications",
            "Course.WS:getCourse",
            "Course.WS:getCourseCategoryMembership",
            "Course.WS:getGroup",
            "Course.WS:getOrg",
            "Course.WS:getOrgCategoryMembership",
            "Course.WS:getServerVersion",
            "Course.WS:getStaffInfo",
            "Course.WS:initializeCourse",
            "Course.WS:saveCartridge",
            "Course.WS:saveCourse",
            "Course.WS:saveCourseCategory",
            "Course.WS:saveGroup",
            "Course.WS:saveOrgCategory",
            "Course.WS:saveOrgCategoryMembership",
            "Course.WS:saveStaffInfo",
            "Course.WS:setCourseBannerImage",
            "Course.WS:updateCourse",
            "Course.WS:updateOrg",
            "Announcement.WS:getCourseAnnouncements",
            "Announcement.WS:getOrgAnnouncements",
            "Announcement.WS:getSystemAnnouncements",
            "Announcement.WS:initializeAnnouncementWS",
            "Announcement.WS:createCourseAnnouncements",
            "Announcement.WS:createOrgAnnouncements",
            "Announcement.WS:createSystemAnnouncements",
            "Announcement.WS:updateCourseAnnouncements",
            "Announcement.WS:updateOrgAnnouncements",
            "Announcement.WS:updateSystemAnnouncements",
            "Announcement.WS:getServerVersion",
            "Gradebook.WS:getAttempts",
            "Gradebook.WS:getGradebookColumns",
            "Gradebook.WS:getGradebookTypes",
            "Gradebook.WS:getGrades",
            "Gradebook.WS:getGradingSchemas",
            "Gradebook.WS:getRequiredEntitlements",
            "Gradebook.WS:getServerVersion",
            "Gradebook.WS:initializeGradebookWS",
            "Gradebook.WS:saveAttempts",
            "Gradebook.WS:saveColumns",
            "Gradebook.WS:saveGradebookTypes",
            "Gradebook.WS:saveGrades",
            "Gradebook.WS:saveGradingSchemas",
            "CourseMembership.WS:getCourseMembership",
            "CourseMembership.WS:getCourseRoles",
            "CourseMembership.WS:getGroupMembership",
            "CourseMembership.WS:getServerVersion",
            "CourseMembership.WS:initializeCourseMembershipWS",
            "Content.WS:addContentFile",
            "Content.WS:updateContentFileLinkName",
            "Content.WS:getContentFiles",
            "Content.WS:getFilteredContent",
            "Content.WS:getReviewStatusByCourseId",
            "Content.WS:getFilteredCourseStatus",
            "Content.WS:getLinksByReferrerType",
            "Content.WS:getLinksByReferredToType",
            "Content.WS:getTOCsByCourseId",
            "Content.WS:initializeContentWS",
            "Content.WS:initializeVersion2ContentWS",
            "Content.WS:getRequiredEntitlements",
            "Content.WS:saveContent",
            "Content.WS:saveCourseTOC",
            "Content.WS:saveContentsReviewed",
            "NotificationDistributorOperations.WS:getServerVersion",
            "NotificationDistributorOperations.WS:initializeNotificationDistributorOperationsWS",
            "NotificationDistributorOperations.WS:setDistributorAvailabilityForUser",
            "NotificationDistributorOperations.WS:registerDistributorResults",
            "NotificationDistributorOperations.WS:",
            "User.WS:getServerVersion",
            "User.WS:initializeUserWS",
            "User.WS:saveUser",
            "User.WS:saveObserverAssociation",
            "User.WS:saveAddressBookEntry",
            "User.WS:getUser",
            "User.WS:getAddressBookEntry",
            "User.WS:getObservee",
            "User.WS:changeUserBatchUid",
            "User.WS:changeUserDataSourceId",
            "User.WS:getSystemRoles",
            "User.WS:getInstitutionRoles",
            "User.WS:getUserInstitutionRoles",
            "Calendar.WS:initializeCalendarWS",
            "Calendar.WS:createPersonalCalendarItem",
            "Calendar.WS:createCourseCalendarItem",
            "Calendar.WS:createInstitutionCalendarItem",
            "Calendar.WS:updatePersonalCalendarItem",
            "Calendar.WS:updateCourseCalendarItem",
            "Calendar.WS:updateInstitutionCalendarItem",
            "Calendar.WS:savePersonalCalendarItem",
            "Calendar.WS:saveCourseCalendarItem",
            "Calendar.WS:saveInstitutionCalendarItem",
            "Calendar.WS:getCalendarItem",
            "Calendar.WS:canUpdatePersonalCalendarItem",
            "Calendar.WS:canUpdateCourseCalendarItem",
            "Calendar.WS:canUpdateInstitutionCalendarItem",
            "Util.WS:initializeUtilWS",
            "Util.WS:getRequiredEntitlements",
            "Util.WS:checkEntitlement",
            "Util.WS:saveSetting",
            "Util.WS:loadSetting",
            "Util.WS:deleteSetting",
            "Util.WS:getDataSources"
        };
    }
}
