using Microsoft.Extensions.Configuration;
using Orders.Api.Models.Dtos;

namespace Orders.Api.Services;

public sealed class StudentService : IStudentService
{
    private readonly StudentInfo _studentInfo;

    public StudentService(IConfiguration configuration)
    {
        _studentInfo = configuration.GetSection("Student").Get<StudentInfo>()
            ?? throw new InvalidOperationException("The Student configuration section is missing.");
    }

    public StudentInfo GetStudentInfo()
    {
        return _studentInfo;
    }
}
