IF object_id(N'[dbo].[FK_dbo.CogResponse_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogResponse] DROP CONSTRAINT [FK_dbo.CogResponse_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.CogResponse_dbo.CogInventory_CogInventoryId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogResponse] DROP CONSTRAINT [FK_dbo.CogResponse_dbo.CogInventory_CogInventoryId]
IF object_id(N'[dbo].[FK_dbo.CogEtmpreResult_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEtmpreResult] DROP CONSTRAINT [FK_dbo.CogEtmpreResult_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.CogEtmpreResult_dbo.CogInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEtmpreResult] DROP CONSTRAINT [FK_dbo.CogEtmpreResult_dbo.CogInstrument_InstrumentId]
IF object_id(N'[dbo].[FK_dbo.CogEsalbResult_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEsalbResult] DROP CONSTRAINT [FK_dbo.CogEsalbResult_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.CogEsalbResult_dbo.CogInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEsalbResult] DROP CONSTRAINT [FK_dbo.CogEsalbResult_dbo.CogInstrument_InstrumentId]
IF object_id(N'[dbo].[FK_dbo.CogEcpeResult_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEcpeResult] DROP CONSTRAINT [FK_dbo.CogEcpeResult_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.CogEcpeResult_dbo.CogInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEcpeResult] DROP CONSTRAINT [FK_dbo.CogEcpeResult_dbo.CogInstrument_InstrumentId]
IF object_id(N'[dbo].[FK_dbo.CogEcmspeResult_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEcmspeResult] DROP CONSTRAINT [FK_dbo.CogEcmspeResult_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.ProfileStudent_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ProfileStudent] DROP CONSTRAINT [FK_dbo.ProfileStudent_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.Security_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Security] DROP CONSTRAINT [FK_dbo.Security_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.RoadRunner_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[RoadRunner] DROP CONSTRAINT [FK_dbo.RoadRunner_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.ProfileFaculty_dbo.Person_PersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[ProfileFaculty] DROP CONSTRAINT [FK_dbo.ProfileFaculty_dbo.Person_PersonId]
IF object_id(N'[dbo].[FK_dbo.FacultyInCourse_dbo.ProfileFaculty_FacultyPersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacultyInCourse] DROP CONSTRAINT [FK_dbo.FacultyInCourse_dbo.ProfileFaculty_FacultyPersonId]
IF object_id(N'[dbo].[FK_dbo.FacultyInCourse_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacultyInCourse] DROP CONSTRAINT [FK_dbo.FacultyInCourse_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.SpResponse_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResponse] DROP CONSTRAINT [FK_dbo.SpResponse_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.SpResponse_dbo.SpInventory_InventoryItemId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResponse] DROP CONSTRAINT [FK_dbo.SpResponse_dbo.SpInventory_InventoryItemId]
IF object_id(N'[dbo].[FK_dbo.SpResponse_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResponse] DROP CONSTRAINT [FK_dbo.SpResponse_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResponse] DROP CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResponse] DROP CONSTRAINT [FK_dbo.SpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.CrseStudentInGroup_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CrseStudentInGroup] DROP CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.CrseStudentInGroup_dbo.ProfileStudent_StudentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CrseStudentInGroup] DROP CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.ProfileStudent_StudentId]
IF object_id(N'[dbo].[FK_dbo.CrseStudentInGroup_dbo.StudentInCourse_StudentId_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CrseStudentInGroup] DROP CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.StudentInCourse_StudentId_CourseId]
IF object_id(N'[dbo].[FK_dbo.StudentInCourse_dbo.ProfileStudent_StudentPersonId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentInCourse] DROP CONSTRAINT [FK_dbo.StudentInCourse_dbo.ProfileStudent_StudentPersonId]
IF object_id(N'[dbo].[FK_dbo.StudentInCourse_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudentInCourse] DROP CONSTRAINT [FK_dbo.StudentInCourse_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.StratResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResult] DROP CONSTRAINT [FK_dbo.StratResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.SpResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResult] DROP CONSTRAINT [FK_dbo.SpResult_dbo.CrseStudentInGroup_StudentId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacStratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacStratResponse] DROP CONSTRAINT [FK_dbo.FacStratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.CrseStudentInGroup_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CrseStudentInGroup] DROP CONSTRAINT [FK_dbo.CrseStudentInGroup_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.StudSpComment_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudSpComment] DROP CONSTRAINT [FK_dbo.StudSpComment_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.WorkGroup_dbo.WorkGroupModel_WgModelId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkGroup] DROP CONSTRAINT [FK_dbo.WorkGroup_dbo.WorkGroupModel_WgModelId]
IF object_id(N'[dbo].[FK_dbo.WorkGroupModel_dbo.SpInstrument_AssignedSpInstrId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkGroupModel] DROP CONSTRAINT [FK_dbo.WorkGroupModel_dbo.SpInstrument_AssignedSpInstrId]
IF object_id(N'[dbo].[FK_dbo.StratResult_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResult] DROP CONSTRAINT [FK_dbo.StratResult_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.StratResult_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResult] DROP CONSTRAINT [FK_dbo.StratResult_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.StratResponse_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResponse] DROP CONSTRAINT [FK_dbo.StratResponse_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.SpResult_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResult] DROP CONSTRAINT [FK_dbo.SpResult_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.SpResult_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResult] DROP CONSTRAINT [FK_dbo.SpResult_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.SpResult_dbo.SpInstrument_AssignedInstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpResult] DROP CONSTRAINT [FK_dbo.SpResult_dbo.SpInstrument_AssignedInstrumentId]
IF object_id(N'[dbo].[FK_dbo.FacStratResponse_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacStratResponse] DROP CONSTRAINT [FK_dbo.FacStratResponse_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacStratResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacStratResponse] DROP CONSTRAINT [FK_dbo.FacStratResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId]
IF object_id(N'[dbo].[FK_dbo.FacSpResponse_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpResponse] DROP CONSTRAINT [FK_dbo.FacSpResponse_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacSpResponse_dbo.SpInventory_InventoryItemId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpResponse] DROP CONSTRAINT [FK_dbo.FacSpResponse_dbo.SpInventory_InventoryItemId]
IF object_id(N'[dbo].[FK_dbo.FacSpResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpResponse] DROP CONSTRAINT [FK_dbo.FacSpResponse_dbo.FacultyInCourse_FacultyPersonId_CourseId]
IF object_id(N'[dbo].[FK_dbo.FacSpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpResponse] DROP CONSTRAINT [FK_dbo.FacSpResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacSpComment_dbo.WorkGroup_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpComment] DROP CONSTRAINT [FK_dbo.FacSpComment_dbo.WorkGroup_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpComment] DROP CONSTRAINT [FK_dbo.FacSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacSpCommentFlag_dbo.FacSpComment_RecipientPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpCommentFlag] DROP CONSTRAINT [FK_dbo.FacSpCommentFlag_dbo.FacSpComment_RecipientPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.FacSpComment_dbo.FacultyInCourse_FacultyPersonId_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpComment] DROP CONSTRAINT [FK_dbo.FacSpComment_dbo.FacultyInCourse_FacultyPersonId_CourseId]
IF object_id(N'[dbo].[FK_dbo.FacSpComment_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[FacSpComment] DROP CONSTRAINT [FK_dbo.FacSpComment_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.WorkGroup_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkGroup] DROP CONSTRAINT [FK_dbo.WorkGroup_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.SpInventory_dbo.SpInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[SpInventory] DROP CONSTRAINT [FK_dbo.SpInventory_dbo.SpInstrument_InstrumentId]
IF object_id(N'[dbo].[FK_dbo.WorkGroup_dbo.SpInstrument_AssignedSpInstrId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkGroup] DROP CONSTRAINT [FK_dbo.WorkGroup_dbo.SpInstrument_AssignedSpInstrId]
IF object_id(N'[dbo].[FK_dbo.StudSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudSpComment] DROP CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_RecipientPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.StudSpCommentFlag_dbo.StudSpComment_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudSpCommentFlag] DROP CONSTRAINT [FK_dbo.StudSpCommentFlag_dbo.StudSpComment_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.StudSpComment_dbo.Course_CourseId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudSpComment] DROP CONSTRAINT [FK_dbo.StudSpComment_dbo.Course_CourseId]
IF object_id(N'[dbo].[FK_dbo.StudSpComment_dbo.CrseStudentInGroup_AuthorPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StudSpComment] DROP CONSTRAINT [FK_dbo.StudSpComment_dbo.CrseStudentInGroup_AuthorPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResponse] DROP CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssessorPersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[StratResponse] DROP CONSTRAINT [FK_dbo.StratResponse_dbo.CrseStudentInGroup_AssesseePersonId_CourseId_WorkGroupId]
IF object_id(N'[dbo].[FK_dbo.CogEcmspeResult_dbo.CogInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogEcmspeResult] DROP CONSTRAINT [FK_dbo.CogEcmspeResult_dbo.CogInstrument_InstrumentId]
IF object_id(N'[dbo].[FK_dbo.CogInventory_dbo.CogInstrument_InstrumentId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[CogInventory] DROP CONSTRAINT [FK_dbo.CogInventory_dbo.CogInstrument_InstrumentId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[CogResponse]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[CogResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CogInventoryId' AND object_id = object_id(N'[dbo].[CogResponse]', N'U'))
    DROP INDEX [IX_CogInventoryId] ON [dbo].[CogResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[CogEtmpreResult]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[CogEtmpreResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[CogEtmpreResult]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[CogEtmpreResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[CogEsalbResult]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[CogEsalbResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[CogEsalbResult]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[CogEsalbResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[CogEcpeResult]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[CogEcpeResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[CogEcpeResult]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[CogEcpeResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[Security]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[Security]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[RoadRunner]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[RoadRunner]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[ProfileStudent]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[ProfileStudent]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[StudentInCourse]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[StudentInCourse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_StudentPersonId' AND object_id = object_id(N'[dbo].[StudentInCourse]', N'U'))
    DROP INDEX [IX_StudentPersonId] ON [dbo].[StudentInCourse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssignedSpInstrId' AND object_id = object_id(N'[dbo].[WorkGroupModel]', N'U'))
    DROP INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroupModel]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_WorkGroupId' AND object_id = object_id(N'[dbo].[StratResult]', N'U'))
    DROP INDEX [IX_WorkGroupId] ON [dbo].[StratResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[StratResult]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[StratResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_StudentId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StratResult]', N'U'))
    DROP INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[StratResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssignedInstrumentId' AND object_id = object_id(N'[dbo].[SpResult]', N'U'))
    DROP INDEX [IX_AssignedInstrumentId] ON [dbo].[SpResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_WorkGroupId' AND object_id = object_id(N'[dbo].[SpResult]', N'U'))
    DROP INDEX [IX_WorkGroupId] ON [dbo].[SpResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[SpResult]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[SpResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_StudentId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[SpResult]', N'U'))
    DROP INDEX [IX_StudentId_CourseId_WorkGroupId] ON [dbo].[SpResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_WorkGroupId' AND object_id = object_id(N'[dbo].[FacStratResponse]', N'U'))
    DROP INDEX [IX_WorkGroupId] ON [dbo].[FacStratResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FacultyPersonId_CourseId' AND object_id = object_id(N'[dbo].[FacStratResponse]', N'U'))
    DROP INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacStratResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssesseePersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[FacStratResponse]', N'U'))
    DROP INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacStratResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InventoryItemId' AND object_id = object_id(N'[dbo].[FacSpResponse]', N'U'))
    DROP INDEX [IX_InventoryItemId] ON [dbo].[FacSpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FacultyPersonId_CourseId' AND object_id = object_id(N'[dbo].[FacSpResponse]', N'U'))
    DROP INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssesseePersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[FacSpResponse]', N'U'))
    DROP INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[FacSpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_RecipientPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[FacSpCommentFlag]', N'U'))
    DROP INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpCommentFlag]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FacultyPersonId_CourseId' AND object_id = object_id(N'[dbo].[FacSpComment]', N'U'))
    DROP INDEX [IX_FacultyPersonId_CourseId] ON [dbo].[FacSpComment]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[FacSpComment]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[FacSpComment]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_RecipientPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[FacSpComment]', N'U'))
    DROP INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[FacSpComment]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[SpInventory]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[SpInventory]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssignedSpInstrId' AND object_id = object_id(N'[dbo].[WorkGroup]', N'U'))
    DROP INDEX [IX_AssignedSpInstrId] ON [dbo].[WorkGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_WgModelId' AND object_id = object_id(N'[dbo].[WorkGroup]', N'U'))
    DROP INDEX [IX_WgModelId] ON [dbo].[WorkGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_UniqueCourseGroup' AND object_id = object_id(N'[dbo].[WorkGroup]', N'U'))
    DROP INDEX [IX_UniqueCourseGroup] ON [dbo].[WorkGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StudSpCommentFlag]', N'U'))
    DROP INDEX [IX_AuthorPersonId_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpCommentFlag]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_RecipientPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StudSpComment]', N'U'))
    DROP INDEX [IX_RecipientPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AuthorPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StudSpComment]', N'U'))
    DROP INDEX [IX_AuthorPersonId_CourseId_WorkGroupId] ON [dbo].[StudSpComment]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssesseePersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StratResponse]', N'U'))
    DROP INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssessorPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[StratResponse]', N'U'))
    DROP INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[StratResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_WorkGroupId' AND object_id = object_id(N'[dbo].[CrseStudentInGroup]', N'U'))
    DROP INDEX [IX_WorkGroupId] ON [dbo].[CrseStudentInGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[CrseStudentInGroup]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[CrseStudentInGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_StudentId_CourseId' AND object_id = object_id(N'[dbo].[CrseStudentInGroup]', N'U'))
    DROP INDEX [IX_StudentId_CourseId] ON [dbo].[CrseStudentInGroup]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InventoryItemId' AND object_id = object_id(N'[dbo].[SpResponse]', N'U'))
    DROP INDEX [IX_InventoryItemId] ON [dbo].[SpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssesseePersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[SpResponse]', N'U'))
    DROP INDEX [IX_AssesseePersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssessorPersonId_CourseId_WorkGroupId' AND object_id = object_id(N'[dbo].[SpResponse]', N'U'))
    DROP INDEX [IX_AssessorPersonId_CourseId_WorkGroupId] ON [dbo].[SpResponse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_UniqueBbCourseId' AND object_id = object_id(N'[dbo].[Course]', N'U'))
    DROP INDEX [IX_UniqueBbCourseId] ON [dbo].[Course]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CourseId' AND object_id = object_id(N'[dbo].[FacultyInCourse]', N'U'))
    DROP INDEX [IX_CourseId] ON [dbo].[FacultyInCourse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_FacultyPersonId' AND object_id = object_id(N'[dbo].[FacultyInCourse]', N'U'))
    DROP INDEX [IX_FacultyPersonId] ON [dbo].[FacultyInCourse]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[ProfileFaculty]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[ProfileFaculty]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_UniqueEmailAddress' AND object_id = object_id(N'[dbo].[Person]', N'U'))
    DROP INDEX [IX_UniqueEmailAddress] ON [dbo].[Person]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[CogInventory]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[CogInventory]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_InstrumentId' AND object_id = object_id(N'[dbo].[CogEcmspeResult]', N'U'))
    DROP INDEX [IX_InstrumentId] ON [dbo].[CogEcmspeResult]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_PersonId' AND object_id = object_id(N'[dbo].[CogEcmspeResult]', N'U'))
    DROP INDEX [IX_PersonId] ON [dbo].[CogEcmspeResult]
DROP TABLE [dbo].[CogResponse]
DROP TABLE [dbo].[CogEtmpreResult]
DROP TABLE [dbo].[CogEsalbResult]
DROP TABLE [dbo].[CogEcpeResult]
DROP TABLE [dbo].[Security]
DROP TABLE [dbo].[RoadRunner]
DROP TABLE [dbo].[ProfileStudent]
DROP TABLE [dbo].[StudentInCourse]
DROP TABLE [dbo].[WorkGroupModel]
DROP TABLE [dbo].[StratResult]
DROP TABLE [dbo].[SpResult]
DROP TABLE [dbo].[FacStratResponse]
DROP TABLE [dbo].[FacSpResponse]
DROP TABLE [dbo].[FacSpCommentFlag]
DROP TABLE [dbo].[FacSpComment]
DROP TABLE [dbo].[SpInventory]
DROP TABLE [dbo].[SpInstrument]
DROP TABLE [dbo].[WorkGroup]
DROP TABLE [dbo].[StudSpCommentFlag]
DROP TABLE [dbo].[StudSpComment]
DROP TABLE [dbo].[StratResponse]
DROP TABLE [dbo].[CrseStudentInGroup]
DROP TABLE [dbo].[SpResponse]
DROP TABLE [dbo].[Course]
DROP TABLE [dbo].[FacultyInCourse]
DROP TABLE [dbo].[ProfileFaculty]
DROP TABLE [dbo].[Person]
DROP TABLE [dbo].[CogInventory]
DROP TABLE [dbo].[CogInstrument]
DROP TABLE [dbo].[CogEcmspeResult]
DROP TABLE [dbo].[__MigrationHistory]
