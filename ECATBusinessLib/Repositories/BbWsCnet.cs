using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ecat.Business.Utilities;
using Ecat.Business.BbWs.BbContext;
using Ecat.Business.BbWs.BbCourse;
using Ecat.Business.BbWs.BbCourseMembership;
using Ecat.Business.BbWs.BbGradebook;
using Ecat.Business.BbWs.BbUser;

namespace Ecat.Data.Contexts
{
    public class BbWsCnet
    {
        private const string BaseUrl = "https://barnescenter.blackboard.com:443/webapps/ws/services";
        private const string VendorId = "EPME";
        private const string ProgramId = "Ecat";
        private const string Secret = "p0i38e382ur";
        private const string DefaultSession = "nosession";
        private int _tryCount = 0;
        private readonly BasicHttpsBinding _defaultBinding;
        private ContextWSPortTypeClient _bbCtx;
        private CourseMembershipWSPortTypeClient _memClient;
        private CourseWSPortTypeClient _crseClient;
        private UserWSPortTypeClient _userClient;
        private GradebookWSPortTypeClient _gradeClient;
        private string _sessionKey;

        public BbWsCnet()
        {
            _defaultBinding = new BasicHttpsBinding { MaxReceivedMessageSize = 2147483647 };
            _sessionKey = "nosession";
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<CourseVO[]> BbCourses(CourseFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _crseClient = null;
            }

            if (_crseClient == null) _crseClient = await GetCourseClient();
            var crseVo = await _crseClient.getCourseAsync(filter);
            _tryCount = 0;
            return crseVo.@return;
        }

        public async Task<CategoryVO[]> BbCourseCategories(CategoryFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _crseClient = null;
            }

            if (_crseClient == null) _crseClient = await GetCourseClient();
            var crseVo = await _crseClient.getCategoriesAsync(filter);
            _tryCount = 0;
            return crseVo.@return;
        }

        public async Task<UserVO[]> BbCourseUsers(UserFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _userClient = null;
            }

            if (_userClient == null) _userClient = await GetUserClient();
            var userVo = await _userClient.getUserAsync(filter);
            _tryCount = 0;
            return userVo.@return;
        }

        public async Task<CourseMembershipVO[]> BbCourseMembership(string courseId, MembershipFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _memClient = null;
            }

            if (_memClient == null) _memClient = await GetMemClient();
            var crseMemVo = await _memClient.getCourseMembershipAsync(courseId, filter);
            _tryCount = 0;
            return crseMemVo.@return;
        }

        public async Task<GroupMembershipVO[]> BbGroupMembership(string courseId, MembershipFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _memClient = null;
            }

            if (_memClient == null) _memClient = await GetMemClient();
            var gmVo = await _memClient.getGroupMembershipAsync(courseId, filter);
            _tryCount = 0;
            return gmVo.@return;
        }

        public async Task<GroupVO[]> BbWorkGroups(string courseId, GroupFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _crseClient = null;
            }

            if (_crseClient == null) _crseClient = await GetCourseClient();
            var wkGrpVo = await _crseClient.getGroupAsync(courseId, filter);
            _tryCount = 0;
            return wkGrpVo.@return;
        }

        public async Task<ColumnVO[]> BbColumns(string courseId, ColumnFilter filter)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _gradeClient = null;
            }

            if (_gradeClient == null) _gradeClient = await GetGradebookClient();
            var colVo = await _gradeClient.getGradebookColumnsAsync(courseId, filter);
            _tryCount = 0;
            return colVo.@return;
        }

        public async Task<saveGradesResponse> SaveGrades(string courseId, ScoreVO[] grades)
        {
            _tryCount += 1;

            if (_tryCount == 3)
            {
                await ResetConnection();
                _gradeClient = null;
            }

            if (_gradeClient == null) _gradeClient = await GetGradebookClient();
            var saveResp = await _gradeClient.saveGradesAsync(courseId, grades, false);
            _tryCount = 0;
            return saveResp;
        }

        private async Task Activate()
        {
            if (_sessionKey != DefaultSession)
            {
                await Task.FromResult(0);
                return;
            }

            var endpoint = new EndpointAddress($"{BaseUrl}/Context.WS");
            var context = new ContextWSPortTypeClient(_defaultBinding, endpoint);
            context.Endpoint.Behaviors.Add(
                new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", _sessionKey))));


            var returnedSessionKey = await context.initializeAsync();

            _bbCtx = new ContextWSPortTypeClient(_defaultBinding, endpoint);

            _bbCtx.Endpoint.Behaviors.Add(
                    new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", returnedSessionKey.@return))));

            try
            {
                var loginResults = await _bbCtx.loginToolAsync(Secret, VendorId, ProgramId, null, 120);

                var success = loginResults.@return;

                if (!success)
                {
                    throw new HttpRequestException();
                }

                _sessionKey = returnedSessionKey.@return;
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                throw new HttpRequestException();
            }

        }

        private async Task<CourseWSPortTypeClient> GetCourseClient()
        {
            await Activate();
            var endpoint = new EndpointAddress($"{BaseUrl}/Course.WS");
            var course = new CourseWSPortTypeClient(_defaultBinding, endpoint);
            course.Endpoint.Behaviors.Add(
                new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", _sessionKey))));
            await course.initializeCourseWSAsync(false);
            return course;
        }

        private async Task<CourseMembershipWSPortTypeClient> GetMemClient()
        {
            await Activate();
            var endpoint = new EndpointAddress($"{BaseUrl}/CourseMembership.WS");
            var mem = new CourseMembershipWSPortTypeClient(_defaultBinding, endpoint);
            mem.Endpoint.Behaviors.Add(
                new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", _sessionKey))));
            await mem.initializeCourseMembershipWSAsync(false);
            return mem;
        }

        private async Task<UserWSPortTypeClient> GetUserClient()
        {
            await Activate();
            var endpoint = new EndpointAddress($"{BaseUrl}/User.WS");
            var user = new UserWSPortTypeClient(_defaultBinding, endpoint);
            user.Endpoint.Behaviors.Add(
                new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", _sessionKey))));
            await user.initializeUserWSAsync(false);
            return user;
        }

        private async Task<GradebookWSPortTypeClient> GetGradebookClient()
        {
            await Activate();
            var endpoint = new EndpointAddress($"{BaseUrl}/Gradebook.WS");
            var gradebook = new GradebookWSPortTypeClient(_defaultBinding, endpoint);
            gradebook.Endpoint.Behaviors.Add(
                new WsSecurityBehavior(new MessageInspector(new SecurityHeader("session", _sessionKey))));
            await gradebook.initializeGradebookWSAsync(false);
            return gradebook;
        }

        private async Task ResetConnection()
        {
            if (_bbCtx == null) return;

            await _bbCtx.logoutAsync();
            _sessionKey = DefaultSession;
        }
    }
}
