using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Exceptions;
using Sdl.Community.GroupShareKit.Helpers;
using Sdl.Community.GroupShareKit.Http;
using Sdl.Community.GroupShareKit.Models.Response;

namespace Sdl.Community.GroupShareKit.Clients
{
    /// <summary>
    /// A client for GroupShare's ProjectServer API.
    /// </summary>
    /// <remarks>
    /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">ProjectServer API documentation</a> for more details.
    /// </remarks>
    public class ProjectClient : ApiClient, IProjectClient
    {
        public ProjectClient(IApiConnection apiConnection) : base(apiConnection)
        {
        }

        /// <summary>
        /// Gets all <see cref="Project"/>s for the organization.
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Project"/>s.</returns>
        public Task<Project> GetProjects(ProjectsRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            return ApiConnection.Get<Project>(ApiUrls.GetAllProjects(), request.ToParametersDictionary());
        }

        public Task<Project> GetAllProjects()
        {
            return ApiConnection.Get<Project>(ApiUrls.GetAllProjects(),null);
        }

        public List<ProjectDetails> GetProjectsForOrganization(string organizationName)
        {
            var allProjects =  ApiConnection.Get<Project>(ApiUrls.GetAllProjects(), null);
            return allProjects.Result.Items.Where(o => o.OrganizationName == organizationName).ToList();         
        }

        /// <summary>
        /// Gets all <see cref="File"/>s for the project.
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Project"/>s.</returns>
        public Task<IReadOnlyList<File>> GetAllFilesForProject(string projectId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectId");

            return ApiConnection.GetAll<File>(ApiUrls.ProjectFiles(projectId));
        }

        /// <summary>
        /// Gets all <see cref="Phase"/>s for the organization.
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Phase"/>s.</returns>
        public Task<IReadOnlyList<Phase>> GetAllPhasesForProject(string projectId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectId");
            return ApiConnection.GetAll<Phase>(ApiUrls.ProjectPhases(projectId));

        }

        public Task<IReadOnlyList<PhasesWithAssignees>> GetPhasesWithAssignees(string projectId, int phaseId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectId");
            Ensure.ArgumentNotNullOrEmptyString(phaseId.ToString(), "phaseId");
            return ApiConnection.GetAll<PhasesWithAssignees>(ApiUrls.ProjectPhasesWithAssignees(projectId, phaseId));

        }


        /// <summary>
        /// Change the project phases
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Phase"/>s.</returns>
        public Task<string> ChangePhases(string projectId, ChangePhaseRequest request)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectid");
            Ensure.ArgumentNotNull(request, "request");

            return ApiConnection.Post<string>(ApiUrls.ChangePhases(projectId), request, "application/json");
        }

        /// <summary>
        /// Change the project assignments
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Phase"/>s.</returns>
        public Task<string> ChangeAssignments(string projectId, ChangeAssignmentRequest request)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectid");
            Ensure.ArgumentNotNull(request, "request");

            return ApiConnection.Post<string>(ApiUrls.ChangeAssignments(projectId), request, "application/json");
        }

        /// <summary>
        /// Create project
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Phase"/>s.</returns>
        public async Task<string> CreateProject(CreateProjectRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            var projectUri = await ApiConnection.Post<string>(ApiUrls.GetAllProjects(), request, "application/json");
            var projectId = projectUri.Split('/').Last();

            await UploadFilesForProject(projectId, request.RawData);
            return projectId;
        }

        /// <summary>
        /// Delete project
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Project"/>s.</returns>
        public Task DeleteProject(string projectId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId,"projectId");

            return ApiConnection.Delete(ApiUrls.Project(projectId));
        }

        /// <summary>
        /// Get project
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Project"/>s.</returns>
        public Task<ProjectDetails> Get(string projectId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectId");

            return ApiConnection.Get<ProjectDetails>(ApiUrls.Project(projectId), null);
        }

        /// <summary>
        /// get the publishing status of a server project.
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="PublishingStatus"/>s.</returns>
        public async Task<PublishingStatus> PublishingStatus(string projectId)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId,"projectId");
            return await ApiConnection.Get<PublishingStatus>(ApiUrls.PublishingStatus(projectId),null);
        }



        /// <summary>
        ///Downloads the files with the specific language ids
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="byte[]"/>s.</returns>
        public async Task<byte[]> DownloadFiles(string projectId, List<string> languageFileIds)
        {
            Ensure.ArgumentNotEmpty(languageFileIds, "languageFileIds");

            return await ApiConnection.Get<byte[]>(ApiUrls.DownloadFiles(projectId, LanguageIdQuery(languageFileIds)),null);
        }

        /// <summary>
        /// Helper method to create  query
        /// </summary>
        /// <param name="languageFileIds"></param>
        /// <returns></returns>
        public string LanguageIdQuery(List<string> languageFileIds)
        {
            var query = string.Empty;
            foreach (var id in languageFileIds)
            {
                query = query + "languageFileIds=" + id + "&";
            }

            return query;
        }

        public string FileIdQuery(List<string> languageFileIds)
        {
            var query = string.Empty;
            foreach (var id in languageFileIds)
            {
                query = query + "fileId=" + id + "&";
            }

            return query;
        }
        /// <summary>
        ///Downloads the files with the specific type and language code
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="byte[]"/>s.</returns>
        public async Task<byte[]> DownloadFile(FileDownloadRequest downloadRequest)
        {
            if (downloadRequest.Type != null)
            {
                return
                 await 
                     ApiConnection.Get<byte[]>(
                         ApiUrls.DownloadFile(downloadRequest.ProjectId, Enum.GetName(typeof(FileDownloadRequest.Types),downloadRequest.Type)),
                         null);
            }

                return await ApiConnection.Get<byte[]>(ApiUrls.DownloadFile(downloadRequest.ProjectId, "all"), null);

        }
        
        public async Task<IReadOnlyList<UserAssignments>> GetProjectsAssignments()
        {
           
            return await ApiConnection.GetAll<UserAssignments>(ApiUrls.GetProjectsAssignments(), null);
        }

        public async Task<IReadOnlyList<ProjectAssignment>> GetProjectAssignmentById(string projectId, List<string> fileIdsList)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId,"projectId");
            Ensure.ArgumentNotNull(fileIdsList,"fileIdsList");

            return
                await
                    ApiConnection.GetAll<ProjectAssignment>(
                        ApiUrls.GetProjectAssignmentById(projectId, FileIdQuery(fileIdsList)), null);
        }

        public async Task<string> UploadFilesForProject(string projectId,byte[] rawData)
        {
            Ensure.ArgumentNotNullOrEmptyString(projectId, "projectId");
            var byteContent = new ByteArrayContent(rawData);
            byteContent.Headers.Add("Content-Type", "application/zip");
            var multipartContent = new MultipartFormDataContent
            {
                {byteContent,"file", "testZip"}
            };
            return await ApiConnection.Post<string>(ApiUrls.UploadFilesForProject(projectId), multipartContent, "application/zip");
        }

        public async Task<string> ChangeProjectStatus(ChangeStatusRequest statusRequest)
        {

             return await ApiConnection.Put<string>(ApiUrls.ChangeProjectStatus(statusRequest.ProjectId, Enum.GetName(typeof(ChangeStatusRequest.ProjectStatus), statusRequest.Status)),statusRequest);
           
        }

        public async Task<string> ChangeProjectStatusDetach(ChangeStatusRequest statusRequest)
        {
            return await ApiConnection.Put<string>(ApiUrls.ChangeProjectStatus(statusRequest), statusRequest);
        }

        public async Task ChangeProjectStatusDeleteDetach(string projectId, bool deleteTms)
        {
            await ApiConnection.Delete(ApiUrls.ProjectStatusDeleteDetach(projectId, deleteTms));
        }

        public async Task PublishPackage(CreateProjectRequest projectRequest)
        {
            Ensure.ArgumentNotNull(projectRequest, "request");
            var projectId = await CreateProjectForPublishingPackage(projectRequest);

            var byteContent = new ByteArrayContent(projectRequest.RawData);
            byteContent.Headers.Add("Content-Type", "application/json");
            var multipartContent = new MultipartFormDataContent
            {
                {byteContent,projectRequest.Name, projectRequest.Name}
            };
            await ApiConnection.Post<string>(ApiUrls.PublishProjectPackage(projectId), multipartContent, "application/json");

        }
        private async Task<string> CreateProjectForPublishingPackage(CreateProjectRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            var projectUri = await ApiConnection.Post<string>(ApiUrls.GetAllProjects(), request, "application/json");
            return  projectUri.Split('/').Last();
        }
    }
}