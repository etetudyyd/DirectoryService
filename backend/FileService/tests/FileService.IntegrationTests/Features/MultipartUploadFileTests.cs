using DirectoryService.Infrastructure;

namespace DirectoryService.Features;

public class MultipartUploadFileTests : FileServiceBaseClass
{
    private readonly FileServiceTestsWebFactory _factory;

    public MultipartUploadFileTests(FileServiceTestsWebFactory factory)
        : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MultipartUpload_FullCycle_PersistsMediaFile()
    {
        //arrange
        //act
        //assert
    }
}