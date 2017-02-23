﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Clients;
using Xunit;

namespace Sdl.Community.GroupShareKit.Tests.Integration.Clients
{
    public class ProjectClientTests
    {

        [Fact]
        public async Task GetProjectByName()
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();

            var projectRequest = new ProjectsRequest("/", true, 7) {Filter = {ProjectName = "Andrea"}};
            var result =
                await
                    groupShareClient.Project.GetProjects(projectRequest);

            Assert.True(result.Items[0].Name=="Andrea");
        }

        [Fact]
        public async Task SortProjectsByName()
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();
            var sortParameters = new SortParameters()
            {
                Property = SortParameters.PropertyOption.ProjectName,
                Direction = SortParameters.DirectionOption.DESC
            };
            var projectRequest = new ProjectsRequest(sortParameters);
            
            var sortedProjects = await groupShareClient.Project.GetProjects(projectRequest);
            Assert.True(sortedProjects.Items[0].Name == "Test");
        }

        [Fact]
        public async Task GetAllProjects()
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();

            var projects = await groupShareClient.Project.GetAllProjects();

            Assert.True(projects.Count>0);

        }
        [Theory]
        [InlineData("c1f47d9c-a9dd-4069-b636-3405d4fb98a8")]
        public async Task GetProjectById(string projectId)
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();

            var actualProject = await groupShareClient.Project.Get(projectId);

            Assert.Equal(actualProject.ProjectId, projectId);
        }

        [Theory]
        [InlineData("SDL Community Developers")]
        public async Task GetProjectsForOrganization(string organizationName)
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();
            var projects =  groupShareClient.Project.GetProjectsForOrganization(organizationName);

            foreach (var project in projects)
            {
                Assert.Equal(project.OrganizationName, organizationName);
            }
        }

        [Fact]
        public async Task CreateProject()
        {
            var groupShareClient = await Helper.GetAuthenticatedClient();
            var rawData =
                File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\ProjectPackage.sdlppx"));

            var projectId =
                await groupShareClient.Project.CreateProject(new CreateProjectRequest("ProjectPackage.sdlppx",
                    "5bdb10b8-e3a9-41ae-9e66-c154347b8d17", rawData));

            Assert.True(!string.IsNullOrEmpty(projectId));

           // await groupShareClient.Project.DeleteProject(projectId);
        }



    }
}
