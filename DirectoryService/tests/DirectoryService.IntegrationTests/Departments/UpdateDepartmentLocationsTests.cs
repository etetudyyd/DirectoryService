using DirectoryService.IntegrationTests.Infrastructure;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentLocationsTests : DirectoryBaseTests
{
    protected UpdateDepartmentLocationsTests(DirectoryTestWebFactory factory)
        : base(factory)
    {
    }
    
    [Fact]
    public async Task UpdateDepartmentLocations_With_Valid_Data_Should_Succeed()
    {
        
    }
    
    [Fact]
    public async Task UpdateDepartmentLocations_With_Invalid_Data_Should_Failed()
    {
        
    }
    
    [Fact]
    public async Task UpdateDepartmentLocations_With_Department_Doesnt_Exist_Should_Failed()
    {
        
    }
    
    [Fact]
    public async Task UpdateDepartmentLocations_With_Location_Doesnt_Exist_Should_Failed()
    {
        
    }
    
    [Fact]
    public async Task UpdateDepartmentLocations_With_Location_Already_Exists_Should_Failed()
    {
        
    }
}