using Microsoft.AspNetCore.Mvc;
using Orders.Api.Models.Dtos;
using Orders.Api.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/student")]
public sealed class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public ActionResult<StudentInfo> GetStudentInfo()
    {
        var studentInfo = _studentService.GetStudentInfo();
        return Ok(new { studentInfo });
    }
}
