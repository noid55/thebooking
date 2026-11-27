using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using thebooking.Controllers;
using thebooking.DAL;
using thebooking.Models;
using thebooking.ViewModels;

namespace thebooking.Tests.Controllers;

// Unit tests for RoomController (same structure as ItemControllerTests in the demo)
public class RoomControllerTests
{
    private readonly IRoomRepository _mockRoomRepository = Substitute.For<IRoomRepository>();
    private readonly IBuildingRepository _mockBuildingRepository = Substitute.For<IBuildingRepository>();
    private readonly IEquipmentRepository _mockEquipmentRepository = Substitute.For<IEquipmentRepository>();
    private readonly ILogger<RoomController> _mockLogger = Substitute.For<ILogger<RoomController>>();

    private RoomController CreateController()
    {
        // The building and equipment lists are used for the dropdowns and checkboxes in the views
        _mockBuildingRepository.GetAll().Returns(new List<Building>());
        _mockEquipmentRepository.GetAll().Returns(new List<Equipment>());
        return new RoomController(_mockRoomRepository, _mockBuildingRepository, _mockEquipmentRepository, _mockLogger);
    }

    private static Room CreateTestRoom()
    {
        return new Room { Name = "Grupperom 601", Capacity = 6, Description = "New test room", BuildingId = 1 };
    }

    // ---------- Positive tests ----------

    [Fact]
    public async Task Table_WhenRoomsExist_ReturnsViewResultWithRooms()
    {
        // Arrange: the search without any filters returns two rooms
        var roomList = new List<Room>
        {
            new Room { RoomId = 1, Name = "Grupperom 101", Capacity = 4, BuildingId = 1 },
            new Room { RoomId = 2, Name = "Grupperom 102", Capacity = 6, BuildingId = 1 }
        };
        _mockRoomRepository.Search(null, null, Arg.Any<List<int>>()).Returns(roomList);
        var controller = CreateController();

        // Act
        var result = await controller.Table(null, null, new List<int>());

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsAssignableFrom<RoomSearchViewModel>(viewResult.ViewData.Model);
        Assert.Equal(2, viewModel.Rooms.Count());
        Assert.Equal(roomList, viewModel.Rooms);
    }

    [Fact]
    public async Task Create_WhenRoomIsValid_RedirectsToTable()
    {
        // Arrange: no other room in the building has the same name (the mock returns false by default)
        var room = CreateTestRoom();
        var controller = CreateController();

        // Act
        var result = await controller.Create(room, new List<int> { 1, 2 });

        // Assert: the room and its equipment are saved, and the user is sent to the room list
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Table", redirectResult.ActionName);
        await _mockRoomRepository.Received(1).Create(room);
        await _mockRoomRepository.Received(1).UpdateEquipment(room.RoomId, Arg.Any<List<int>>());
    }

    // ---------- Negative tests ----------

    [Fact]
    public async Task Create_WhenRepositoryThrows_ReturnsViewWithRoom()
    {
        // Arrange: simulate a database error when the room is saved
        var room = CreateTestRoom();
        _mockRoomRepository.Create(room).Returns(Task.FromException(new Exception("Database error")));
        var controller = CreateController();

        // Act
        var result = await controller.Create(room, new List<int>());

        // Assert: the app does not crash; the form is shown again with the user's input and an error
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsAssignableFrom<RoomFormViewModel>(viewResult.ViewData.Model);
        Assert.Equal(room, viewModel.Room);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_WhenNameAlreadyExistsInBuilding_ReturnsViewWithNameError()
    {
        // Arrange: the building already has a room with the same name
        var room = CreateTestRoom();
        _mockRoomRepository.NameExistsInBuilding(room.Name, room.BuildingId).Returns(true);
        var controller = CreateController();

        // Act
        var result = await controller.Create(room, new List<int>());

        // Assert: an error is shown on the name field, and the room is not saved
        Assert.IsType<ViewResult>(result);
        Assert.True(controller.ModelState.ContainsKey("Room.Name"));
        await _mockRoomRepository.DidNotReceive().Create(Arg.Any<Room>());
    }
}