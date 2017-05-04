using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ecat.Business.Repositories;
using Ecat.Data.Models.Student;
using Ecat.Data.Models.Faculty;
using Ecat.Data.Models.School;
using Ecat.Data.Models.User;
using Ecat.Data.Contexts;
using Ecat.Data.Static;

//using Ecat.LmsAdmin.Mod;
//using Ecat.Shared.Core.Logic;
//using Ecat.Shared.Core.ModelLibrary.Learner;
//using Ecat.Shared.Core.ModelLibrary.School;
//using Ecat.Shared.Core.ModelLibrary.User;
//using Ecat.Shared.Core.Utility;
//using Ecat.Shared.DbMgr.BbWs.BbCourse;
//using Ecat.Shared.DbMgr.BbWs.BbCourseMem;
//using Ecat.Shared.DbMgr.BbWs.BbUsers;
//using Ecat.Shared.DbMgr.BbWs.BbContext;
//using Ecat.Shared.DbMgr.Context;

namespace Ecat.DevOps
{
    internal class Develop
    {
        static void Main(string[] args)
        {
            var userSelectOptions = 0;

            do
            {
                userSelectOptions = ShowMenu();
                switch (userSelectOptions)
                {
                    case 1:
                        var count = DbOperations.LoadStaticDbData();
                        Console.WriteLine($"Finished loading {count} static records into the database");
                        break;
                    case 2:
                        DbOperations.LoadStudentsIntoGroups();
                        break;
                    case 3:
                        DbOperations.DoAssessments();
                        break;
                    case 4:
                        break;
                    case 5:
                        DbOperations.LoadIST();
                        break;
                    //case 6:
                    //    DbOperations.ListBbCategoryId();
                    //    break;
                }
            } while (userSelectOptions != 9);
        }


        internal static int ShowMenu()
        {

            Console.WriteLine("Developer Menu");
            Console.WriteLine("=======================");
            Console.WriteLine("1. Load Static Data (Must be an empty Db)");
            Console.WriteLine("2. Load/Reload Group Members");
            Console.WriteLine("3. Load/Reload Assess/Comments");
            Console.WriteLine("4. Load/Reload Fac Assess/Comments");
            Console.WriteLine("5. Load IST Members and Assessments");
            Console.WriteLine("6. Get Bb Category Id");
            Console.WriteLine("7. Load Ecat Academy");
            Console.WriteLine("8. Get Bb Workgroup By Course Id");
            //Console.WriteLine("9. Get Bb Students By Wg Id");
            Console.WriteLine("9. Exit");
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }
    }

    public static class DbOperations
    {
        private static readonly Random Rand = new Random();

        static DbOperations()
        {

        }


        public static int LoadStaticDbData()
        {
            var count = File.ReadAllLines("DbDevSeed.sql").Length;
            var seedFile = File.ReadAllText("DbDevSeed.sql");

            using (var ctx = new EcatContext())
            {
                ctx.Database.ExecuteSqlCommand(seedFile);
            }
            return count;
        }

        public static void LoadStudentsIntoGroups()
        {

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            using (var ctx = new EcatContext())
            {
                Console.WriteLine("Clearing Databases....");
                ctx.Database.ExecuteSqlCommand("delete from dbo.SpResponse");
                ctx.Database.ExecuteSqlCommand("delete from dbo.StudSpCommentFlag");
                ctx.Database.ExecuteSqlCommand("delete from dbo.StudSpComment");
                ctx.Database.ExecuteSqlCommand("delete from dbo.StratResponse");
                ctx.Database.ExecuteSqlCommand("delete from dbo.SpResult");
                ctx.Database.ExecuteSqlCommand("delete from dbo.FacSpCommentFlag");
                ctx.Database.ExecuteSqlCommand("delete from dbo.FacSpComment");
                ctx.Database.ExecuteSqlCommand("delete from dbo.FacSpResponse");
                ctx.Database.ExecuteSqlCommand("delete from dbo.FacStratResponse");
                ctx.Database.ExecuteSqlCommand("delete from dbo.StratResult");
                ctx.Database.ExecuteSqlCommand("delete from dbo.CrseStudentInGroup");

                Console.WriteLine("Done! Assigning Students");
                var studCrses = ctx.StudentInCourses.GroupBy(sic => sic.CourseId).ToList();
                var courses = ctx.Courses.Include(c => c.WorkGroups).ToList();
                const string insertPreamble =
                    "INSERT INTO CrseStudentInGroup (CourseId,WorkGroupId,StudentId,IsDeleted,HasAcknowledged,ModifiedDate) values ";
                var sb = new StringBuilder(insertPreamble);
                var insertCount = 1;
                var insertGroups = new List<string>();
                foreach (var crse in studCrses)
                {
                    var currentCourse = courses.Single(c => c.Id == crse.Key);
                    var studIds = crse.Select(stud => stud.StudentPersonId)
                        .Distinct()
                        .ToList();
                    Console.WriteLine(
                        $"There are {studIds.Count()} Students in Course {currentCourse.Name} [ID: {crse.Key}]");
                    Console.WriteLine(
                        $"There are {currentCourse.WorkGroups.GroupBy(wg => wg.MpCategory).Count()} WorkGroups Categories in Course {currentCourse.Name} [ID: {crse.Key}]");

                    foreach (var wgCat in currentCourse.WorkGroups.GroupBy(wg => wg.MpCategory))
                    {
                        var numOfGroups = wgCat.Count();
                        Console.WriteLine(
                            $"There are {numOfGroups} WorkGroups in Wg Category {wgCat.Key} in Course {currentCourse.Name} [ID: {crse.Key}]");

                        var numberPerGroup = studIds.Count/numOfGroups;
                        var studentMix = studIds.OrderBy(id => Rand.Next(studIds.Min(), studIds.Max() + 1)).ToList();
                        var groupMembers = new List<CrseStudentInGroup>();

                        foreach (var grp in wgCat)
                        {
                            var currentGroupMembers = new List<CrseStudentInGroup>();

                            for (var i = 0; i < numberPerGroup; i++)
                            {
                                var currentStudentId = studentMix.First();
                                var gm = new CrseStudentInGroup
                                {
                                    CourseId = crse.Key,
                                    WorkGroupId = grp.WorkGroupId,
                                    StudentId = currentStudentId,
                                    IsDeleted = false,
                                    HasAcknowledged = false,
                                    ModifiedDate = DateTime.Now.ToUniversalTime()
                                };
                                currentGroupMembers.Add(gm);
                                sb.Append(
                                    $"({gm.CourseId},{gm.WorkGroupId},{gm.StudentId},'{gm.IsDeleted}','{gm.HasAcknowledged}','{gm.ModifiedDate}'),");
                                insertCount += 1;
                                studentMix.Remove(currentStudentId);

                                if (insertCount != 1000) continue;

                                sb.Length--;
                                sb.Append(";");
                                insertGroups.Add(sb.ToString());
                                sb.Clear();
                                sb.Append(insertPreamble);
                                insertCount = 1;
                            }
                            Console.WriteLine(
                                $"Loaded {currentGroupMembers.Count} in Group {grp.DefaultName} ID:[{grp.WorkGroupId}]");
                            groupMembers.AddRange(currentGroupMembers);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Loaded {groupMembers.Count} in All WorkGroups in Category {wgCat.Key}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Loaded All WorkGroups in Course");
                    Console.WriteLine($"Completed Member Record Creation for Course ID:[{crse.Key}]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine("Writing insert packages to the database....");
                sb.Length--;
                sb.Append(";");
                insertGroups.Add(sb.ToString());

                foreach (var command in insertGroups)
                {
                    ctx.Database.ExecuteSqlCommand(command);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Writing insert packages to the database....");
            }

        }

        public static void DoAssessments()
        {

            using (var ctx = new EcatContext())
            {
                var courses = ctx.Courses.Where(crse => crse.WorkGroups.Any()).ToList();
                Console.WriteLine("Which course should I assess?");
                Console.WriteLine();
                Console.WriteLine("Courses Loaded In Database");
                Console.WriteLine("============================");
                foreach (var course in courses)
                {
                    Console.WriteLine($"{course.Id}: {course.Name} [{course.ClassNumber}]");
                }
                var result = Console.ReadLine();
                var courseId = Convert.ToInt32(result);
                Console.WriteLine($"Which Group Category in course {courseId} to load?");
                Console.WriteLine("Courses Categories");
                Console.WriteLine("==================");
                Console.WriteLine("1: ALL");
                var i = 2;

                var wgCats =
                    ctx.WorkGroups.Where(wg => wg.CourseId == courseId).Select(wg => wg.MpCategory).Distinct().ToArray();

                foreach (var wg in wgCats)
                {
                    Console.WriteLine($"{i}: {wg}");
                    i += 1;
                }
                Console.WriteLine("0: Cancel");
                var groupResponse = Console.ReadLine();
                var groupSelect = Convert.ToInt32(groupResponse);
                var groupTypes = new List<string>();

                switch (groupSelect)
                {
                    case 1:
                        groupTypes.Add(MpGroupCategory.Wg1);
                        groupTypes.Add(MpGroupCategory.Wg2);
                        groupTypes.Add(MpGroupCategory.Wg3);
                        groupTypes.Add(MpGroupCategory.Wg4);
                        break;
                    case 2:
                        groupTypes.Add(wgCats[0]);
                        break;
                    case 3:
                        groupTypes.Add(wgCats[1]);
                        break;
                    case 4:
                        groupTypes.Add(wgCats[2]);
                        break;
                    case 5:
                        groupTypes.Add(wgCats[3]);
                        break;
                    default:
                        return;
                }

                var studIds = string.Join(",",
                    ctx.StudentInGroups.Where(
                        sig => sig.CourseId == courseId && groupTypes.Contains(sig.WorkGroup.MpCategory))
                        .Select(sig => sig.StudentId));

                Console.WriteLine("Clearing databases");

                ctx.Database.ExecuteSqlCommand($"DELETE dbo.StudSpCommentFlag WHERE AuthorPersonId IN ({studIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE dbo.StudSpComment WHERE AuthorPersonId IN ({studIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE dbo.StratResult WHERE StudentId IN ({studIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE dbo.SpResult WHERE StudentId IN ({studIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE dbo.StratResponse WHERE AssessorPersonId IN ({studIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE dbo.SpResponse WHERE AssessorPersonId IN ({studIds})");

                var groupsToLoad =
                    ctx.StudentInGroups.Where(g => g.CourseId == courseId && groupTypes.Contains(g.WorkGroup.MpCategory))
                        .GroupBy(g => g.WorkGroup)
                        .ToList();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Black;

                Console.WriteLine($"Loading {groupsToLoad.ToList().Count} in {groupsToLoad.Count} WorkGroups");

                const string spResponsePreamble =
                    "INSERT INTO SpResponse(AssessorPersonId,AssesseePersonId,CourseId,WorkGroupId,InventoryItemId,ItemModelScore,ModifiedById,ModifiedDate,ItemResponse) values ";
                const string spCommentResponsePreamble =
                    "INSERT INTO StudSpComment(CourseId,WorkGroupId,AuthorPersonId,RecipientPersonId,RequestAnonymity,CommentText,CreatedDate,ModifiedDate,ModifiedById) values ";
                const string spCommentFlagPreamble =
                    "INSERT INTO StudSpCommentFlag (CourseId,WorkGroupId,AuthorPersonId,RecipientPersonId,Author) values ";
                const string stratResponsePreamble =
                    "INSERT INTO StratResponse (CourseId,WorkGroupId,AssessorPersonId,AssesseePersonId,StratPosition,ModifiedDate,ModifiedById) values ";
                var spResponseSb = new StringBuilder(spResponsePreamble);
                var spCommentResponseSb = new StringBuilder(spCommentResponsePreamble);
                var spCommentFlagResponseSb = new StringBuilder(spCommentFlagPreamble);
                var stratResponseSb = new StringBuilder(stratResponsePreamble);

                var numOfSpRecords = 1;
                var numOfOtherRecords = 1;
                var insertGroups = new List<string>();
                foreach (var wg in groupsToLoad)
                {
                    var groupMembers = wg.ToList();
                    var inventoryList =
                        ctx.SpInventories.Where(inv => inv.InstrumentId == wg.Key.AssignedSpInstrId)
                            .Select(inv => inv.Id)
                            .ToList();
                    foreach (var member in groupMembers)
                    {
                        var peers = groupMembers.ToList();
                        var stratsNeed = new List<int>();

                        for (var j = 1; j <= peers.Count; j++)
                        {
                            stratsNeed.Add(j);
                        }

                        var strats = stratsNeed.OrderBy(s => Rand.Next()).ToList();

                        foreach (var peer in peers)
                        {
                            var isSelf = peer.StudentId == member.StudentId;

                            foreach (var spResponse in inventoryList.Select(item => new SpResponse
                            {
                                AssessorPersonId = member.StudentId,
                                AssesseePersonId = peer.StudentId,
                                CourseId = wg.Key.CourseId,
                                WorkGroupId = wg.Key.WorkGroupId,
                                InventoryItemId = item,
                                ItemModelScore = Rand.Next(0, 7),
                                ModifiedById = member.StudentId,
                                ModifiedDate = DateTime.Now.ToUniversalTime()
                            }))
                            {
                                switch (spResponse.ItemModelScore)
                                {
                                    case 0:
                                        spResponse.MpItemResponse = MpSpItemResponse.Iea;
                                        break;
                                    case 1:
                                        spResponse.MpItemResponse = MpSpItemResponse.Ieu;
                                        break;
                                    case 2:
                                        spResponse.MpItemResponse = MpSpItemResponse.Nd;
                                        break;
                                    case 3:
                                        spResponse.MpItemResponse = MpSpItemResponse.Ea;
                                        break;
                                    case 4:
                                        spResponse.MpItemResponse = MpSpItemResponse.Eu;
                                        break;
                                    case 5:
                                        spResponse.MpItemResponse = MpSpItemResponse.Heu;
                                        break;
                                    case 6:
                                        spResponse.MpItemResponse = MpSpItemResponse.Hea;
                                        break;
                                }
                                spResponseSb.Append(
                                    $"({spResponse.AssessorPersonId}, {spResponse.AssesseePersonId}, {spResponse.CourseId},{spResponse.WorkGroupId}, {spResponse.InventoryItemId},'{spResponse.ItemModelScore}',{spResponse.ModifiedById},'{spResponse.ModifiedDate}','{spResponse.MpItemResponse}'),");

                                numOfSpRecords += 1;

                                if (numOfSpRecords < 1000) continue;

                                spResponseSb.Length--;
                                spResponseSb.Append(";");
                                insertGroups.Add(spResponseSb.ToString());
                                spResponseSb.Clear();
                                spResponseSb.Append(spResponsePreamble);
                            }

                                var spComment = new StudSpComment
                                {
                                    CourseId = wg.Key.CourseId,
                                    AuthorPersonId = member.StudentId,
                                    RecipientPersonId = peer.StudentId,
                                    WorkGroupId = wg.Key.WorkGroupId,
                                    RequestAnonymity = Rand.Next(0, 2) == 1,
                                    CommentText = CommentList[Rand.Next(1, 21)],
                                    CreatedDate = DateTime.Now.ToUniversalTime(),
                                    ModifiedDate = DateTime.Now.ToUniversalTime(),
                                    ModifiedById = member.StudentId
                                };

                                spCommentResponseSb.Append(
                                    $"({spComment.CourseId},{spComment.WorkGroupId},{spComment.AuthorPersonId},{spComment.RecipientPersonId},'{spComment.RequestAnonymity}','{spComment.CommentText}','{spComment.CreatedDate}','{spComment.ModifiedDate}',{spComment.ModifiedById}),");

                                //Console.WriteLine($"Student {spComment.AuthorPersonId} ==> Recipient {spComment.RecipientPersonId} in Course {wg.Key.CourseId} WorkGroup {wg.Key.Id}");
                                var spCommentFlag = new StudSpCommentFlag
                                {
                                    MpAuthor = FlagList[Rand.Next(1, 4)],
                                    CourseId = wg.Key.CourseId,
                                    AuthorPersonId = member.StudentId,
                                    RecipientPersonId = peer.StudentId,
                                    WorkGroupId = wg.Key.WorkGroupId
                                };

                                spCommentFlagResponseSb.Append(
                                    $"({spCommentFlag.CourseId},{spCommentFlag.WorkGroupId},{spCommentFlag.AuthorPersonId},{spCommentFlag.RecipientPersonId},'{spCommentFlag.MpAuthor}'),");


                                var peerStrat = strats.First();
                            var stratResponse = new StratResponse
                            {
                                AssesseePersonId = peer.StudentId,
                                AssessorPersonId = member.StudentId,
                                CourseId = wg.Key.CourseId,
                                WorkGroupId = wg.Key.WorkGroupId,
                                StratPosition = peerStrat,
                                ModifiedDate = DateTime.Now.ToUniversalTime(),
                                ModifiedById = member.StudentId
                            };

                            stratResponseSb.Append(
                                $"({stratResponse.CourseId},{stratResponse.WorkGroupId},{stratResponse.AssessorPersonId},{stratResponse.AssesseePersonId},'{stratResponse.StratPosition}','{stratResponse.ModifiedDate}',{stratResponse.ModifiedById}),");


                            strats.Remove(peerStrat);
                            numOfOtherRecords += 1;

                            if (numOfOtherRecords < 1000) continue;

                            spCommentResponseSb.Length--;
                            spCommentResponseSb.Append(";");
                            insertGroups.Add(spCommentResponseSb.ToString());
                            spCommentFlagResponseSb.Length--;
                            spCommentFlagResponseSb.Append(";");
                            insertGroups.Add(spCommentFlagResponseSb.ToString());
                            stratResponseSb.Length--;
                            stratResponseSb.Append(";");
                            insertGroups.Add(stratResponseSb.ToString());

                            spCommentResponseSb.Clear();
                            spCommentFlagResponseSb.Clear();
                            stratResponseSb.Clear();

                            spCommentResponseSb.Append(spCommentResponsePreamble);
                            spCommentFlagResponseSb.Append(spCommentFlagPreamble);
                            stratResponseSb.Append(stratResponsePreamble);
                        }
                        var peerStringList = peers.Select(p => p.StudentId);
                        Console.WriteLine(
                            $"{member.StudentId} has completed assessment on [{string.Join(",", peerStringList)}]");

                    }

                    Console.WriteLine(
                        $"{wg.Key.DefaultName} has completed {wg.Key.GroupMembers.Count*wg.Key.GroupMembers.Count*inventoryList.Count} assessments with comments and stratification");
                    Console.WriteLine("Writing to database, standby....");

                    using (var cmd = new SqlCommand())
                    {
                        using (var conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ecatlocaldev;Integrated Security=True;Connect Timeout=30;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                        {
                            //assign connection string and open connection
                            cmd.Connection = conn;
                            cmd.Connection.Open();
                            foreach (var command in insertGroups)
                            {
                                cmd.CommandText = command;
                                //Console.WriteLine(cmd.CommandText);
                                cmd.ExecuteNonQuery();
                            }
                            cmd.Connection.Close();
                            insertGroups.Clear();
                        }
                    }
                }

                Console.WriteLine($"Done...completed {groupsToLoad.ToList().Count} members assessments.");
            }
        }

        //public static async void ListBbCategoryId()
        //{
        //    Console.WriteLine("Grabbing Bb Categories via web services");

        //    var ctx = new EcatContext();
        //    var bbWs = new BbWsCnet();
        //    var adminMod = new CourseOps(ctx, bbWs);

        //    var catList = await adminMod.GetBbCategories();

        //     Console.WriteLine("Success...here are the categories");

        //    foreach (var cat in catList)
        //    {
        //        Console.WriteLine($"-- {cat.id}: {cat.title} [Parent: {cat.parentId}]");
        //    }
            

        //}

        public static void LoadIST()
        {
            using (var ctx = new EcatContext())
            {
                var courses = ctx.Courses.Where(crse => crse.WorkGroups.Any()).ToList();
                Console.WriteLine("Select a course");
                Console.WriteLine();
                Console.WriteLine("Courses Loaded In Database");
                Console.WriteLine("============================");
                foreach (var course in courses)
                {
                    Console.WriteLine($"{course.Id}: {course.Name} [{course.ClassNumber}]");
                }

                var result = Console.ReadLine();
                var courseId = Convert.ToInt32(result);
                var groupNums = new List<string>();
                for (var i = 1; i < 18; i++) {
                    if (i < 10) { groupNums.Add("0" + i.ToString()); }
                    else { groupNums.Add(i.ToString()); }
                }

                var students = ctx.People.Where(p => p.PersonId < 171).ToList();
                var studentIds = new StringBuilder();
                foreach (var stud in students) { studentIds.Append($" {stud.PersonId},"); }
                studentIds.Length--;

                var groups = ctx.WorkGroups.Where(g => g.CourseId == courseId && g.MpCategory == MpGroupCategory.Wg1 && groupNums.Contains(g.GroupNumber)).OrderBy(g => g.GroupNumber).ToList();
                var groupIds = new StringBuilder();
                foreach (var group in groups) { groupIds.Append($" {group.WorkGroupId},"); };
                groupIds.Length--;

                Console.WriteLine($"Deleting Group Data for {groups[0].MpCategory}: {groups[0].DefaultName} through {groups[groups.Count - 1].DefaultName}");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.StudSpCommentFlag WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.StudSpComment WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.StratResult WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.SpResult WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.StratResponse WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.SpResponse WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.FacSpResponse WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.FacStratResponse WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.CrseStudentInGroup WHERE CourseId = {courseId} AND WorkGroupId IN ({groupIds})");
                ctx.Database.ExecuteSqlCommand($"DELETE FROM dbo.StudentInCourse WHERE CourseId = {courseId} AND StudentPersonId IN ({studentIds})");

                Console.WriteLine("Done. Adding Students to Course and Groups.");
                const string sicInsertPreamble =
                    "INSERT INTO StudentInCourse (StudentPersonId,CourseId,IsDeleted) VALUES ";
                var sicSB = new StringBuilder(sicInsertPreamble);
                const string csigInsertPreamble =
                    "INSERT INTO CrseStudentInGroup (CourseId,WorkGroupId,StudentId,IsDeleted,HasAcknowledged,ModifiedDate) VALUES ";
                var csigSB = new StringBuilder(csigInsertPreamble);
                var insertList = new List<string>();

                var csigCount = 0;
                var grpInd = 0;
                foreach (var stud in students) {
                    var sic = new StudentInCourse
                    {
                        StudentPersonId = stud.PersonId,
                        CourseId = courseId,
                        IsDeleted = false,
                    };
                    sicSB.Append($"({sic.StudentPersonId},{sic.CourseId},'{sic.IsDeleted}');");
                    insertList.Add(sicSB.ToString());
                    sicSB.Clear();
                    sicSB.Append(sicInsertPreamble);

                    csigCount++;
                    if (csigCount > 10)
                    {
                        csigCount = 1;
                        grpInd++;
                    }

                    var csig = new CrseStudentInGroup
                    {
                        StudentId = stud.PersonId,
                        CourseId = courseId,
                        WorkGroupId = groups[grpInd].WorkGroupId,
                        IsDeleted = false,
                        HasAcknowledged = false,
                        ModifiedDate = DateTime.Now.ToUniversalTime()
                    };

                    csigSB.Append($"({csig.CourseId},{csig.WorkGroupId},{csig.StudentId},'{csig.IsDeleted}','{csig.HasAcknowledged}','{csig.ModifiedDate}');");
                    insertList.Add(csigSB.ToString());
                    csigSB.Clear();
                    csigSB.Append(csigInsertPreamble);
                }

                foreach (var command in insertList)
                {
                    ctx.Database.ExecuteSqlCommand(command);
                }

                Console.WriteLine("Done. Adding Assessment/Strat/Comment Data.");

                insertList.Clear();
                groups = null;
                groups = ctx.WorkGroups
                    .Where(g => g.CourseId == courseId && g.MpCategory == MpGroupCategory.Wg1 && groupNums.Contains(g.GroupNumber)).OrderBy(g => g.GroupNumber)
                    .Include(g => g.GroupMembers)
                    .Include(g => g.AssignedSpInstr)
                    .Include(g => g.AssignedSpInstr.InventoryCollection)
                    .ToList();
                //var inventories = ctx.SpInventories.Where(inv => inv.InstrumentId == groups[0].AssignedSpInstrId).ToList();
                var inventories = groups[0].AssignedSpInstr.InventoryCollection;

                const string assessPreamble = "INSERT INTO dbo.SpResponse (AssessorPersonId, AssesseePersonId, WorkGroupId, CourseId, InventoryItemId, ItemResponse, ItemModelScore, ModifiedDate) VALUES ";
                var assessInsert = new StringBuilder(assessPreamble);
                const string stratPreamble = "INSERT INTO dbo.StratResponse (AssessorPersonId, AssesseePersonId, WorkGroupId, CourseId, StratPosition, ModifiedDate) VALUES ";
                var stratInsert = new StringBuilder(stratPreamble);
                const string commentPreamble = "INSERT INTO dbo.StudSpComment (AuthorPersonId, RecipientPersonId, WorkGroupId, CourseId, RequestAnonymity, CommentText, CreatedDate, ModifiedById, ModifiedDate) VALUES ";
                var commentInsert = new StringBuilder(commentPreamble);
                const string commentFlagPreamble = "INSERT INTO dbo.StudSpCommentFlag (AuthorPersonId, RecipientPersonId, WorkGroupId, CourseId, Author) VALUES ";
                var commentFlagInsert = new StringBuilder(commentFlagPreamble);

                foreach (var group in groups) {
                    var peers = group.GroupMembers.ToList();
                    foreach (var gm in group.GroupMembers) {
                        var peersToStrat = new List<int>();
                        foreach (var peer in peers)
                        {
                            peersToStrat.Add(peer.StudentId);
                            foreach (var inv in inventories)
                            {
                                var response = new SpResponse
                                {
                                    AssessorPersonId = gm.StudentId,
                                    AssesseePersonId = peer.StudentId,
                                    WorkGroupId = group.WorkGroupId,
                                    CourseId = group.CourseId,
                                    InventoryItemId = inv.Id,
                                    ItemModelScore = Rand.Next(0, 7),
                                    ModifiedDate = DateTime.Now.ToUniversalTime()
                                };
                                switch (response.ItemModelScore)
                                {
                                    case 0: response.MpItemResponse = MpSpItemResponse.Iea; break;
                                    case 1: response.MpItemResponse = MpSpItemResponse.Ieu; break;
                                    case 2: response.MpItemResponse = MpSpItemResponse.Nd; break;
                                    case 3: response.MpItemResponse = MpSpItemResponse.Ea; break;
                                    case 4: response.MpItemResponse = MpSpItemResponse.Eu; break;
                                    case 5: response.MpItemResponse = MpSpItemResponse.Heu; break;
                                    case 6: response.MpItemResponse = MpSpItemResponse.Hea; break;
                                }

                                assessInsert.Append($"({response.AssessorPersonId}, {response.AssesseePersonId}, {response.WorkGroupId},{response.CourseId},{response.InventoryItemId},'{response.MpItemResponse}',{response.ItemModelScore},'{response.ModifiedDate}');");
                                insertList.Add(assessInsert.ToString());
                                assessInsert.Clear();
                                assessInsert.Append(assessPreamble);
                            }

                            if (peer.StudentId != gm.StudentId)// && (Rand.Next(0, 2) == 1))
                            {
                                var comment = new StudSpComment
                                {
                                    AuthorPersonId = gm.StudentId,
                                    RecipientPersonId = peer.StudentId,
                                    WorkGroupId = group.WorkGroupId,
                                    CourseId = group.CourseId,
                                    RequestAnonymity = Rand.Next(0, 2) == 1,
                                    CommentText = CommentList[Rand.Next(1, 21)],
                                    CreatedDate = DateTime.Now.ToUniversalTime(),
                                    ModifiedDate = DateTime.Now.ToUniversalTime(),
                                    ModifiedById = gm.StudentId
                                };
                                commentInsert.Append($"({comment.AuthorPersonId},{comment.RecipientPersonId},{comment.WorkGroupId},{comment.CourseId},'{comment.RequestAnonymity}','{comment.CommentText}','{comment.CreatedDate}',{comment.ModifiedById},'{comment.ModifiedDate}');");
                                insertList.Add(commentInsert.ToString());
                                commentInsert.Clear();
                                commentInsert.Append(commentPreamble);

                                var commentFlag = new StudSpCommentFlag
                                {
                                    AuthorPersonId = gm.StudentId,
                                    RecipientPersonId = peer.StudentId,
                                    CourseId = group.CourseId,
                                    WorkGroupId = group.WorkGroupId,
                                    MpAuthor = FlagList[Rand.Next(1, 4)]
                                };
                                commentFlagInsert.Append($"({commentFlag.AuthorPersonId},{commentFlag.RecipientPersonId},{commentFlag.WorkGroupId},{commentFlag.CourseId},'{commentFlag.MpAuthor}');");
                                insertList.Add(commentFlagInsert.ToString());
                                commentFlagInsert.Clear();
                                commentFlagInsert.Append(commentFlagPreamble);
                            }
                        }

                        for (var i = 1; i < (peers.Count + 1); i++)
                        {
                            var peerIndex = Rand.Next(0, peersToStrat.Count);
                            var strat = new StratResponse
                            {
                                AssessorPersonId = gm.StudentId,
                                AssesseePersonId = peersToStrat[peerIndex],
                                WorkGroupId = group.WorkGroupId,
                                CourseId = group.CourseId,
                                StratPosition = i,
                                ModifiedDate = DateTime.Now.ToUniversalTime()
                            };

                            stratInsert.Append($"({strat.AssessorPersonId}, {strat.AssesseePersonId}, {strat.WorkGroupId},{strat.CourseId},{strat.StratPosition},'{strat.ModifiedDate}');");
                            insertList.Add(stratInsert.ToString());
                            stratInsert.Clear();
                            stratInsert.Append(stratPreamble);
                            peersToStrat.RemoveAt(peerIndex);
                        }


                        using (var cmd = new SqlCommand())
                        {
                            using (var conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ecatlocaldev;Integrated Security=True;Connect Timeout=30;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                            //using (var conn = new SqlConnection("Data Source=tcp:epme-tech-intergration.database.windows.net,1433;Initial Catalog=ecat;User ID=epmeTechAdmin;Password=P)O(i8u7p0o9I*U&;Integrated Security=False;Connect Timeout=60;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                            {
                                //assign connection string and open connection
                                cmd.Connection = conn;
                                cmd.Connection.Open();
                                foreach (var command in insertList)
                                {
                                    cmd.CommandText = command;
                                    //Console.WriteLine(cmd.CommandText);
                                    cmd.ExecuteNonQuery();
                                }
                                cmd.Connection.Close();
                                insertList.Clear();
                            }
                        }

                        //foreach (var command in insertList)
                        //{
                        //    ctx.Database.ExecuteSqlCommand(command);
                        //}

                        insertList.Clear();
                    }
                    Console.WriteLine($"{group.MpCategory}-{group.DefaultName} Done.");
                }
            }
            Console.WriteLine("IST Load Complete.");
        }

       // public static async void LoadEcatAcademy()
      //  {

        //    var bbWsCnet = new BbWsCnet();

        //    var courseClient = await bbWsCnet.GetCourseClient();

        //    var courseFilter = new CourseFilter
        //    {
        //        filterType = (int)CourseFilterType.LoadByCatId,
        //        filterTypeSpecified = true,
        //        categoryIds = new[] {StaticAcademy.Ecat.BbCategoryId}
        //    };

        //    var crseQuery = await courseClient.getCourseAsync(courseFilter);

        //    using (var ctx = new EcatContext())
        //    {

        //        var existingCourse = await
        //            ctx.Courses.FirstOrDefaultAsync(crse => crse.AcademyId == StaticAcademy.Ecat.Id);

        //        if (existingCourse == null)
        //        {

        //            existingCourse = new Course
        //            {
        //                AcademyId = StaticAcademy.Ecat.Id,
        //                Name = "Not Assigned",
        //                ClassNumber = "16-Test",
        //                GradReportPublished = false,
        //                StartDate = DateTime.Now,
        //                GradDate = DateTime.Now.AddDays(1),
        //            };
        //        }

        //        if (existingCourse.Id == 0)
        //        {
        //            Console.WriteLine($"Ecat Test was not loaded in the database");
        //            var bbEcatCourse = crseQuery.@return[0];
        //            existingCourse.BbCourseId = bbEcatCourse.id;
        //            Console.WriteLine($"{crseQuery.@return.Length} Bb Ecat Course was pulled selected Course with dbId: {bbEcatCourse.id} / dbCourseId: {bbEcatCourse.courseId}");

        //            //var saveResult = ctx.SaveChanges();
        //            Console.WriteLine($"Course save result{ctx.Database.Connection}");
                    
        //            ctx.Database.ExecuteSqlCommand($"INSERT INTO Course(BbCourseId,AcademyId,Name,ClassNumber,GradReportPublished,StartDate,GradDate) values ('{existingCourse.BbCourseId}','{existingCourse.AcademyId}','{existingCourse.Name}', '{existingCourse.ClassNumber}', '{existingCourse.GradReportPublished}','{existingCourse.StartDate.ToUniversalTime()}' ,'{existingCourse.GradDate.ToUniversalTime()}')");
        //        }

        //        Console.WriteLine($"Getting Workgroups from Bb");

        //        var groupFilter = new GroupFilter
        //        {
        //            filterTypeSpecified = true,
        //            filterType = 2
        //        };

        //        var groupQuery = await courseClient.getGroupAsync(existingCourse.BbCourseId, groupFilter);

        //        var bbGroups = groupQuery.@return;

        //        if (bbGroups == null || bbGroups.Length == 0) return;

        //        bbGroups = bbGroups.Where(gl => gl.title.StartsWith("BC") || gl.title == "Instructor Cadre").ToArray();

        //        var existingWorkGroups = await ctx.WorkGroups.Where(wg => wg.CourseId == existingCourse.Id).ToListAsync();

        //        var wgList = bbGroups
        //                .Where(gl => !existingWorkGroups
        //                .Select(wg => wg.BbGroupId)
        //                .Contains(gl.id)).Select(grp =>
        //                {
        //                    Console.WriteLine(
        //                        $"Retrived the following groups ID: {grp.id} Title: {grp.title} With Description: {grp.description}");
        //                    var bo = grp.title.Split('-');
        //                    string category;
        //                    var modelId = 0;
        //                    var assignedInstr = 0;
        //                    switch (bo[0])
        //                    {
        //                        case MpGroupCategory.Wg1:
        //                            category = MpGroupCategory.Wg1;
        //                            modelId = 4;
        //                            assignedInstr = 8;
        //                            break;
        //                        case MpGroupCategory.Wg2:
        //                            category = MpGroupCategory.Wg2;
        //                            modelId = 6;
        //                            assignedInstr = 10;
        //                            break;
        //                        case MpGroupCategory.Wg3:
        //                            category = MpGroupCategory.Wg3;
        //                            modelId = 7;
        //                            assignedInstr = 8;
        //                            break;
        //                        default:
        //                            category = MpGroupCategory.None;
        //                            break;
        //                    }
        //                    return new WorkGroup
        //                    {
        //                        CourseId = existingCourse.Id,
        //                        MpCategory = category,
        //                        WgModelId = modelId,
        //                        AssignedSpInstrId = assignedInstr,
        //                        DefaultName = bo[2],
        //                        GroupNumber = bo[1].Substring(1),
        //                        IsPrimary = bo[0] == MpGroupCategory.Wg1,
        //                        BbGroupId = grp.id,
        //                        ModifiedDate = DateTime.Now.ToUniversalTime(),
        //                        MpSpStatus = MpSpStatus.Open
        //                    };
        //                }).ToList();

        //        foreach (var wg in wgList)
        //        {
        //            Console.WriteLine(
        //                $"Created the following wg {wg.BbGroupId}");
        //            ctx.Database.ExecuteSqlCommand($"INSERT INTO WorkGroup(CourseId,Category,WgModelId,AssignedSpInstrId,DefaultName,GroupNumber,IsPrimary,BbGroupId,ModifiedDate,SpStatus) values ('{wg.CourseId}','{wg.MpCategory}',{wg.WgModelId}, '{wg.AssignedSpInstrId}', '{wg.DefaultName}','{wg.GroupNumber}' ,'{wg.IsPrimary}','{wg.BbGroupId}','{wg.ModifiedDate}','{wg.MpSpStatus}')");
        //        }


        //        var grpMemberFilter = new MembershipFilter
        //        {
        //            filterType = (int)GrpMemFilterType.LoadByGrpId,
        //            filterTypeSpecified = true,
        //            groupIds = bbGroups.Select(grp => grp.id).Distinct().ToArray()
        //        };

        //        var crseMemberFilter = new MembershipFilter
        //        {
        //            filterType = (int)GrpMemFilterType.LoadByCourseId,
        //            filterTypeSpecified = true,
        //            courseIds = new []{existingCourse.BbCourseId}
        //        };

        //        var courseMemClient = await bbWsCnet.GetMemClient();

        //        var grpMemQuery =
        //            await courseMemClient.getGroupMembershipAsync(existingCourse.BbCourseId, grpMemberFilter);

        //        var crseMemQuery =
        //            await courseMemClient.getCourseMembershipAsync(existingCourse.BbCourseId, crseMemberFilter);

        //        var grpMems = grpMemQuery.@return;

        //        var crseMems = crseMemQuery.@return;

        //        var userFilter = new UserFilter
        //        {
        //            filterTypeSpecified = true,
        //            filterType = (int)UserFilterType.UseByGroupIdWithAvailability,
        //            groupId = bbGroups.Select(grp => grp.id).Distinct().ToArray()
        //        };

        //        var userClient = await bbWsCnet.GetUserClient();

        //        var userQuery = await userClient.getUserAsync(userFilter);

        //        //var ecatCourseRoster = ctx.People
        //        //    .Where(person => crseMems.Select(cm => cm.id).Contains(person.BbUserId))
        //        //    .Include(person => person.Student)
        //        //    .Include(person => person.Faculty);

        //        var bbCourseUsers = userQuery.@return;

        //        var bbFacultyGroup = bbGroups.First(bg => bg.title == "Instructor Cadre");

        //        var bbFaculty = grpMems.Select(gm => gm.groupId == bbFacultyGroup.id);
        //        var i = 1;
        //        foreach (var bcu in bbCourseUsers)
        //        {
        //            var person = new Person
        //            {
        //                BbUserId = bcu.id,
        //                FirstName = bcu.extendedInfo.givenName,
        //                LastName = bcu.extendedInfo.familyName,
        //                MpGender = MpGender.Unk,
        //                MpAffiliation = MpAffiliation.Usaf,
        //                MpPaygrade = MpPaygrade.E7,
        //                MpComponent = MpComponent.Active,
        //                Email = bcu.extendedInfo.emailAddress,
        //                RegistrationComplete = true,
        //                MpInstituteRole =
        //                    bcu.extendedInfo.familyName.ToLower().Contains("instructor")
        //                        ? MpInstituteRoleId.Faculty
        //                        : MpInstituteRoleId.Student
        //            };

        //            if (person.MpInstituteRole == MpInstituteRoleId.Faculty)
        //            {
        //                person.Faculty = new ProfileFaculty
        //                {
        //                    AcademyId = StaticAcademy.Ecat.Id,
        //                    IsCourseAdmin = false,
        //                    IsReportViewer = false
        //                };

        //            }
        //            else
        //            {
        //                person.Student = new ProfileStudent
        //                {
        //                    PersonId = 
        //                };
        //            }



        //            ctx.Database.ExecuteSqlCommand($"INSERT INTO Person(CourseId,Category,WgModelId,AssignedSpInstrId,DefaultName,GroupNumber,IsPrimary,BbGroupId,ModifiedDate,SpStatus) values ('{wg.CourseId}','{wg.MpCategory}',{wg.WgModelId}, '{wg.AssignedSpInstrId}', '{wg.DefaultName}','{wg.GroupNumber}' ,'{wg.IsPrimary}','{wg.BbGroupId}','{wg.ModifiedDate}','{wg.MpSpStatus}')");
        //        }

        //    }
        //}

        private static Dictionary<int, string> FlagList => new Dictionary<int, string>
        {
            {1, MpCommentFlag.Neg },
            {2, MpCommentFlag.Neut },
            {3, MpCommentFlag.Pos},
            {4, MpCommentFlag.Appr},
            {5, MpCommentFlag.Inappr}
        };

        private static Dictionary<int, string> CommentList => new Dictionary<int, string>
        {
            {1, "<p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat wisi, condimentum sed, commodo vitae, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. Donec non enim in turpis pulvinar facilisis. Ut felis. Praesent dapibus, neque id cursus faucibus, tortor neque egestas augue, eu vulputate magna eros eu erat. Aliquam erat volutpat. Nam dui mi, tincidunt quis, accumsan porttitor, facilisis luctus, metus</p>" },
            {2,"<p>Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.</p>" },
            {3,"<p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo.</p>"},
            {4,"<p>Most of its text is made up from sections 1.10.32–3 of Ciceros De finibus bonorum et malorum (On the Boundaries of Goods and Evils; finibus may also be translated as purposes). Neque porro quisquam est qui do<strong>lorem ipsum</strong> quia <strong>dolor sit amet, consectetur, adipisci</strong> v<strong>elit</strong> is the first known version (Neither is there anyone who loves grief itself since it is grief and thus wants to obtain it). It was found by Richard McClintock, a philologist, director of publications at Hampden-Sydney College in Virginia; he searched for citings of consectetur in classical Latin literature, a term of remarkably low frequency in that literary corpus.</p>"},
            {5,"In a professional context it often happens that private or corporate clients corder a publication to be made and presented with the actual content still not being ready. Think of a news blog thats filled with content hourly on the day of going live. However, reviewers tend to be distracted by comprehensible content, say, a random text copied from a newspaper or the internet. The are likely to focus on the text, disregarding the layout and its elements. Besides, random text risks to be unintendedly humorous or offensive, an unacceptable risk in corporate environments. Lorem ipsum and its many variants have been employed since the early 1960ies, and quite likely since the sixteenth century."},
            {6,"<p>In mea rebum delectus, cum ex homero albucius sapientem, ne eum sanctus argumentum. Vim case disputando consectetuer ei, ipsum saperet partiendo ei est, eruditi perfecto nam at. No saepe similique quo, inermis assentior complectitur ut usu, dolore postulant voluptatum has at. Eius solum democritum in eum, illud sensibus consequuntur vis ne? Te adhuc audiam duo? Sint eloquentiam usu ei, eos legere placerat iracundia at!</p><p>Qui posse efficiendi no, no sale simul homero eos, eos ut suscipit reformidans? Ad sea utamur vivendo. Utinam graecis expetendis vim no? Ullum offendit sed ei? Essent equidem nam an. Vel eius constituto ne, vix at aliquip detracto!</p>Forensibus percipitur ex est, eu putent propriae concludaturque vel, ne has tibique efficiendi! At semper accumsan pri, nam eu quando tritani iuvaret? Nemore bonorum saperet cu sit! Suas sanctus ne duo? No corrumpit mnesarchum complectitur cum, ei eam aperiam fierent, est summo saperet ex. Et mea quas adipisci."},
            {7,"Lorem ipsum dolor sit amet, et modus falli veniam ius! Labore patrioque mel no, laudem salutatus elaboraret id cum, an wisi feugiat eos. Eu pri epicuri voluptatum, eos doming voluptua an. Vim dico corrumpit definitionem in.Et usu malorum dissentiet liberavisse, no per nonumy partem. Ea sale vulputate eam, quis legimus placerat ad cum, simul nonumy usu in. His ne viris tacimates. Et eum eirmod blandit. Augue postea pertinacia eu vis, ea quo dico sint eruditi.Etiam eloquentiam conclusionemque vel no, quo prima singulis liberavisse et, eam ad tincidunt intellegam reformidans. In mea novum menandri prodesset, mel an tale esse ornatus. Melius denique mea ne. In delenit phaedrum voluptaria eos, te civibus dissentias vix! Ea pro ornatus expetenda, pro ne esse possim debitis?"},
            {8,"But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful.Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure? On the other hand, we denounce with righteous indignation and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee"},
            {9,"<p>Li Europan lingues es membres del sam familie. Lor separat existentie es un myth. Por scientie, musica, sport etc, litot Europa usa li sam vocabular. Li lingues differe solmen in li grammatica, li pronunciation e li plu commun vocabules. Omnicos directe al desirabilite de un nov lingua franca: On refusa continuar payar custosi traductores. At solmen va esser necessi far uniform grammatica, pronunciation e plu sommun paroles.</p><p>Ma quande lingues coalesce, li grammatica del resultant lingue es plu simplic e regulari quam ti del coalescent lingues. Li nov lingua franca va esser plu simplic e regulari quam li existent Europan lingues. It va esser tam simplic quam Occidental in fact, it va esser Occidental. A un Angleso it va semblar un simplificat Angles, quam un skeptic Cambridge amico dit me que Occidental es. Li Europan lingues es membres del sam familie. Lor separat existentie es un myth. Por scientie, musica, sport etc, litot Europa usa li sam vocabular. Li lingues differe solmen in li grammatica, li pronunciation e li plu commun vocabules. Omnicos directe al desirabilite de un nov lingua franca: On refusa continuar payar custosi traductores. At solmen va esser necessi far uniform grammatica, pronunciation e plu sommun paroles.</p>"},
            {10,"<p>The quick, brown fox jumps over a lazy dog. DJs flock by when MTV ax quiz prog. Junk MTV quiz graced by fox whelps. Bawds jog, flick quartz, vex nymphs. Waltz, bad nymph, for quick jigs vex! Fox nymphs grab quick-jived waltz. Brick quiz whangs jumpy veldt fox. Bright vixens jump; dozy fowl quack. Quick wafting zephyrs vex bold Jim. Quick zephyrs blow, vexing daft Jim.</p><p>Sex-charged fop blew my junk TV quiz. How quickly daft jumping zebras vex. Two driven jocks help fax my big quiz. Quick, Baz, get my woven flax jodhpurs! Now fax quiz Jack!  my brave ghost pled. Five quacking zephyrs jolt my wax bed. Flummoxed by job, kvetching W. zaps Iraq. Cozy sphinx waves quart jug of bad milk. A very bad quack might jinx zippy fowls. Few quips galvanized the mock jury box. Quick brown dogs jump over the lazy fox. The jay, pig, fox, zebra, and my wolves quack! Blowzy red vixens fight for a quick jump. Joaquin Phoenix was gazed by MTV for luck. A wizards job is to vex chumps quickly in fog. Watch Jeopardy! , Alex Trebeks fun TV quiz game. Woven silk pyjamas exchanged for blue quartz. Brawny gods just</p>"},
            {11,"<p>One morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armour-like back, and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment.</p><p>His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he looked. Whats happened to me?  he thought. It wasnt a dream. His room, a proper human room although a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had recently cut out of an illustrated magazine and housed in a nice, gilded frame. It showed a lady fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole of her lower arm towards the viewer. Gregor then turned to look out the window at the dull weather. Drops</p>" },
            {12,"<p>Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere</p>" },
            {13,"<p>Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure? On the other hand, we denounce with righteous indignation and dislike men who are so beguiled and demoralized by the charms of pleasure of the moment, so blinded by desire, that they cannot foresee</p>" },
            {14,"<p>If several languages coalesce, the grammar of the resulting language is more simple and regular than that of the individual languages. The new common language will be more simple and regular than the existing European languages. It will be as simple as Occidental; in fact, it will be Occidental. To an English person, it will seem like simplified English, as a skeptical Cambridge friend of mine told me what Occidental is. The European languages are members of the same family. Their separate existence is a myth. For science, music, sport, etc, Europe uses the same vocabulary. The languages only differ in their grammar, their pronunciation and their most common words. Everyone realizes why a new common language would be desirable: one could refuse to pay expensive translators. To</p>" },
            {15,"<p>Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts. Separated they live in Bookmarksgrove right at the coast of the Semantics, a large language ocean. A small river named Duden flows by their place and supplies it with the necessary regelialia. It is a paradisematic country, in which roasted parts of sentences fly into your mouth.</p>" },
            {16,"<p>Even the all-powerful Pointing has no control about the blind texts it is an almost unorthographic life One day however a small line of blind text by the name of Lorem Ipsum decided to leave for the far World of Grammar. The Big Oxmox advised her not to do so, because there were thousands of bad Commas, wild Question Marks and devious Semikoli, but the Little Blind Text didn’t listen. She packed her seven versalia, put her initial into the belt and made herself on the way. When she reached the first hills of the Italic Mountains, she had a last view back on the skyline of her hometown Bookmarksgrove, the headline of Alphabet Village and the subline of her own road, the Line Lane. Pityful a rethoric question ran over her cheek, then</p>" },
            {17,"<p>Separated they live in Bookmarksgrove right at the coast of the Semantics, a large language ocean. A small river named Duden flows by their place and supplies it with the necessary regelialia. It is a paradisematic country, in which roasted parts of sentences fly into your mouth.</p><p>Even the all-powerful Pointing has no control about the blind texts it is an almost unorthographic life One day however a small line of blind text by the name of Lorem Ipsum decided to leave for the far World of Grammar. The Big Oxmox advised her not to do so, because there were thousands of bad Commas!,</p><p> wild Question Marks and devious Semikoli, but the Little Blind Text didn’t listen. She packed her seven versalia, put her initial into the belt and made herself on the way. When she reached the first hills of the Italic Mountains, she had a last view back on the skyline of her hometown Bookmarksgrove, the headline of Alphabet Village and the subline of her own road, the Line Lane. Pityful a rethoric question ran over her cheek, then</p>" },
            {18,"<p>I should be incapable of drawing a single stroke at the present moment; and yet I feel that I never was a greater artist than now. When, while the lovely valley teems with vapour around me, and the meridian sun strikes the upper surface of the impenetrable foliage of my trees, and but a few stray gleams steal into the inner sanctuary.</p><p>I throw myself down among the tall grass by the trickling stream; and, as I lie close to the earth, a thousand unknown plants are noticed by me: when I hear the buzz of the little world among the stalks, and grow familiar with the countless indescribable forms of the insects and flies, then I feel the presence of the Almighty, who formed us in his own image, and the breath</p>" },
            {19,"<p>O my friend -- but it is too much for my strength -- I sink under the weight of the splendour of these visions! A wonderful serenity has taken possession of my entire soul, like these sweet mornings of spring which I enjoy with my whole heart. I am alone, and feel the charm of existence in this spot, which was created for the bliss of souls like mine.</p><p>I am so happy, my dear friend, so absorbed in the exquisite sense of mere tranquil existence, that I neglect my talents. I should be incapable of drawing a single stroke at the present moment; and yet I feel that I never was a greater artist than now.</p>" },
            {20,"<p>When, while the lovely valley teems with vapour around me, and the meridian sun strikes the upper surface of the impenetrable foliage of my trees, and but a few stray gleams steal into the inner sanctuary, I throw myself down among the tall grass by the trickling stream; and, as I lie close to the earth, a thousand unknown plants are noticed by me: when I hear the buzz of the little world among the stalks, and grow familiar with the countless indescribable forms of the insects and flies, then I feel the presence of the Almighty, who formed us in his own image, and the breath of that universal love which bears and sustains us, as it floats around us in an eternity of bliss; and then, my friend, when darkness overspreads my eyes, and heaven and earth seem to dwell in my soul and absorb its power, like the form of a beloved mistress, then I often think with longing, Oh, would I could describe these conceptions, could impress upon paper all that is living so full and warm within me, that it might be the mirror of my soul, as my soul is the mirror of the infinite God!</p>" }
        };
    }

}
