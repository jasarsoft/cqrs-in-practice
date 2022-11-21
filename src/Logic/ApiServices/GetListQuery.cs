using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;


namespace Logic.AppServices
{
    //public sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
    //{
    //    private readonly UnitOfWork _unitOfWork;

    //    public GetListQueryHandler(UnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //    }

    //    public List<StudentDto> Handle(GetListQuery query)
    //    {
    //        return new StudentRepository(_unitOfWork)
    //            .GetList(query.EnrolledIn, query.NumberOfCourses)
    //            .Select(x => ConvertToDto(x))
    //            .ToList();
    //    }

    //    private StudentDto ConvertToDto(Student student)
    //    {
    //        return new StudentDto
    //        {
    //            Id = student.Id,
    //            Name = student.Name,
    //            Email = student.Email,
    //            Course1 = student.FirstEnrollment?.Course?.Name,
    //            Course1Grade = student.FirstEnrollment?.Grade.ToString(),
    //            Course1Credits = student.FirstEnrollment?.Course?.Credits,
    //            Course2 = student.SecondEnrollment?.Course?.Name,
    //            Course2Grade = student.SecondEnrollment?.Grade.ToString(),
    //            Course2Credits = student.SecondEnrollment?.Course?.Credits,
    //        };
    //    }
    //}

    public sealed class GetListQuery : IQuery<List<StudentDto>>
    {
        public string EnrolledIn { get; }
        public int? NumberOfCourses { get; }

        public GetListQuery(string enrolledIn, int? numberOfCourses)
        {
            EnrolledIn = enrolledIn;
            NumberOfCourses = numberOfCourses;
        }

        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<StudentDto>>
        {
            private readonly string _connectionString;

            public GetListQueryHandler(string connectionString)
            {
                _connectionString = connectionString;
            }

            public List<StudentDto> Handle(GetListQuery query)
            {
                string sql = @"
                    SELECT s.StudentID Id, s.Name, s.Email,
	                    s.FirstCourseName Course1, s.FirstCourseCredits Course1Credits, s.FirstCourseGrade Course1Grade,
	                    s.SecondCourseName Course2, s.SecondCourseCredits Course2Credits, s.SecondCourseGrade Course2Grade
                    FROM dbo.Student s
                    WHERE (s.FirstCourseName = @Course
		                    OR s.SecondCourseName = @Course
		                    OR @Course IS NULL)
                        AND (s.NumberOfEnrollments = @Number
                            OR @Number IS NULL)
                    ORDER BY s.StudentID ASC";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    List<StudentDto> students = connection
                        .Query<StudentDto>(sql, new
                        {
                            Course = query.EnrolledIn,
                            Number = query.NumberOfCourses
                        })
                        .ToList();

                    return students;
                }
            }
        }
    }
}
