using FluentAssertions;
using ShortVideoCutter.Models;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter.Tests;

public class MapperTest
{
    [Fact]
    public async Task FullTest()
    {
        var textFiles = new List<string>();
        var momentFiles = new List<string>();

        var data = """
            ru1                          
            en1                          
            url1                         
            1 2:00 4:00    !                
            3 5:00 6:00    ID1PART1         
            3 5:00 6:00    ID2PART1         
            4 12:00 14:00   ID2PART3       
            6 20:00 40:00   ID2PART2       
            6 20:00 40:00   ID2PART2GLOB1                                             
            ---
            ru2
            en2
            url2
            4 2:00 4:00    !
            7 5:00 6:00    ID1PART1
            3 5:00 6:00    ! ID2PART1
            5 12:00 14:00   ID2PART1GLOB1
            6 20:00 40:00   ID1PART2
            12 20:00 40:00   ID4PART2  
        """;
        var saveDirectory = @"sd";
        var expectedSeasons = AssertData.GetCorrectData(saveDirectory);

        var mockedClicker = MockFactory.CreateClicker();
        var mockedModuleIO = MockFactory.CreateModuleIO(textFiles);
        var mapper = new Mapper(mockedModuleIO);
        var mockedFFMpegModule = MockFactory.CreateFFMpegModule(momentFiles);
        var downloader = new Downloader(mockedModuleIO);
        var converter = new ConverterVideoProcessor(mockedModuleIO, mockedFFMpegModule);

        var seasons = mapper.Init(data, saveDirectory);
        await mockedClicker.InitDownloadLinks(seasons);
        converter.Processed(seasons, saveDirectory);

        seasons.Count.Should().Be(expectedSeasons.Count);
        for (int i = 0; i < seasons.Count; i++)
        {
            AssertData.AssertSeason(seasons[i], expectedSeasons[i]);
        }

        textFiles.Count.Should().Be(5); //3 ! + 12 20:00 40:00   ID4PART2 - invalid sequence + 3 5:00 6:00    ! ID2PART1 - sequence with invalid
        textFiles.Count(x => x.Contains("WrongSeq")).Should().Be(2);
        textFiles.Count(x => x.Contains("moments")).Should().Be(3);

        momentFiles.Count.Should().Be(11); // 1 just correct moment + 3 mergeed + 7 for merge
        momentFiles.Count(x => !x.Contains("SplitMomentWithId", StringComparison.CurrentCultureIgnoreCase)).Should().Be(1);
        momentFiles.Count(x => x.Contains("Final", StringComparison.CurrentCultureIgnoreCase)).Should().Be(3);
        momentFiles.Count(x =>
            x.Contains("SplitMomentWithId", StringComparison.CurrentCultureIgnoreCase) &&
            !x.Contains("Final", StringComparison.CurrentCultureIgnoreCase)).Should().Be(7);
    }

    [Theory]
    [InlineData("ID2PART1GLOB1", MomentStatus.Part)]
    [InlineData("ID2PART1", MomentStatus.Part)]
    [InlineData("ID2P1", MomentStatus.Simple)]
    [InlineData("#fsdvsv", MomentStatus.Simple)]
    [InlineData("!ID2PART1", MomentStatus.Invalid)]
    [InlineData("!", MomentStatus.Invalid)]
    public void TestParseStatus(string note, MomentStatus Expectedstatus)
    {
        var moment = new Moment(new TimeSpan(1, 1, 1), new TimeSpan(1, 1, 1), note);
        moment.GetStatus().Should().Be(Expectedstatus);
    }

    [Theory]
    [InlineData("ID10PART11GLOB31", 10, 11, 31)]
    [InlineData("ID1PART12", 1, 12, null)]
    public void TestParseMergeData(string note, int ExpectedId, int ExpectedPart, int? ExpectedGlobalId)
    {
        var moment = new Moment(new TimeSpan(1, 1, 1), new TimeSpan(1, 1, 1), note);
        moment.GetStatus().Should().Be(MomentStatus.Part);
        var mergeData = moment.IsPartMoment().data;
        mergeData.Should().NotBeNull();
        mergeData.Id.Should().Be(ExpectedId);
        mergeData.Part.Should().Be(ExpectedPart);
        mergeData.GlobalId.Should().Be(ExpectedGlobalId);
    }
}
