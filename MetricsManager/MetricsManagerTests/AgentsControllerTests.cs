using System;
using System.Collections.Generic;
using MetricsManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.Responses.DTO;
using MetricsManager.Responses;
using AutoFixture;

namespace MetricsManagerTests
{
    public class AgentsControllerTests
    {
        private readonly Mock<IAgentsRepository> _repositoryAgent;
        private readonly Mock<ILogger<AgentsController>> _loggerMock;
        private readonly AgentsController _controller;

        public AgentsControllerTests()
        {
            _repositoryAgent = new Mock<IAgentsRepository>();
            _loggerMock = new Mock<ILogger<AgentsController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<AgentInfo, AgentDto>());
            var mapper = config.CreateMapper();

            _controller = new AgentsController(_repositoryAgent.Object, mapper, _loggerMock.Object);
        }

        [Fact]
        public void ShouldCall_RegisterAgent_ReturnOk()
        {
            _repositoryAgent.Setup(repository => repository.RegisterAgent(It.IsAny<AgentInfo>())).Verifiable();

            var result = _controller.RegisterAgent(new AgentInfo{ AgentId = 3, AgentAddress = "http://localhost:5005" });

            _repositoryAgent.Verify(repository => repository.RegisterAgent(It.IsAny<AgentInfo>()), Times.AtMostOnce());
        }

        [Fact]
        public void ShouldCall_GetAgentList_ReturnOk()
        {
            var fixture = new Fixture { RepeatCount = 5 };
            var checkList = fixture.Create<List<AgentInfo>>();

            _repositoryAgent.Setup(repository => repository.GetAgentList()).Returns(checkList);
            
            var result = (OkObjectResult)_controller.GetAgentList();

            var action = (AgentListApiResponse)result.Value;

            var actionResult = action.Agents;

            _repositoryAgent.Verify(repository => repository.GetAgentList(), Times.AtMostOnce());

            Assert.Equal(checkList[0].AgentId, actionResult[0].AgentId);
        }

        [Fact]
        public void ShouldCall_GetAgentAddressFromId_ReturnOk()
        {
            var fixture = new Fixture { RepeatCount = 3 };
            var check = fixture.Create<string>();

            _repositoryAgent.Setup(repository => repository.GetAgentAddressFromId(It.IsAny<int>())).Returns(check);

            var result = (OkObjectResult)_controller.GetAgentAddressFromId(3);

            var action = (string)result.Value;

            _repositoryAgent.Verify(repository => repository.GetAgentAddressFromId(It.IsAny<int>()), Times.AtMostOnce());

            Assert.Equal(check, action);
        }
    }
}
